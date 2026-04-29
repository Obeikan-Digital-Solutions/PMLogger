using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;

namespace PMLogger.PM.Features.Plans.UpdatingPlan.V1
{
    public record UpdatePlan(
        int Id,
        int Month,
        int Year,
        List<MachineUpdateDto> Machines,
        string? Notes = null) : ICommand<UpdatePlanResult>;

    public record UpdatePlanResult(bool Success);

    public record MachineUpdateDto(int Id, string MachineName, string Area, string ChecklistType, string AssignedTo, string Priority, string Shift, string Status);

    public class UpdatePlanValidator : AbstractValidator<UpdatePlan>
    {
        public UpdatePlanValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class UpdatePlanHandler : ICommandHandler<UpdatePlan, UpdatePlanResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdatePlanHandler> _logger;
        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public UpdatePlanHandler(ApplicationDbContext context, ILogger<UpdatePlanHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<UpdatePlanResult> Handle(UpdatePlan request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CommandHandler {Command}, Request {Request}", nameof(UpdatePlan), request);
            Guard.Against.Null(request, nameof(request));

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null) throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            var companyId = company.Id;

            var updatedBy = _tenantProvider.GetCurrentUserId() ?? "system";

            var headerRow = await _context.PmPlans
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == companyId, cancellationToken);

            if (headerRow == null)
            {
                throw new Exception("PM plan not found");
            }

            var planName = headerRow.PlanName;
            var planStatus = headerRow.PlanStatus;

            // Get existing rows for this plan
            var existingRows = await _context.PmPlans
                .Where(p => p.PlanName == planName && p.CompanyId == companyId && p.MachineName != "MONTHLY_PLAN_HEADER")
                .ToListAsync(cancellationToken);

            var existingRowDict = existingRows.ToDictionary(r => r.Id);
            var retainedIds = new HashSet<int>();

            // Extract month/year from header to build dates
            var headerDate = headerRow.ScheduledDate ?? headerRow.BaselineDate ?? headerRow.CreatedAt;
            var year = headerDate.Year;
            var month = headerDate.Month;

            foreach (var mReq in request.Machines)
            {
                var scheduledDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var normalizedStatus = NormalizeStoredStatus(mReq.Status);

                if (mReq.Id > 0 && existingRowDict.TryGetValue(mReq.Id, out var existingRow))
                {
                    // Update existing
                    existingRow.AreaName = mReq.Area;
                    existingRow.MachineName = mReq.MachineName;
                    existingRow.ChecklistType = mReq.ChecklistType;
                    existingRow.AssignedTechnician = mReq.AssignedTo;
                    existingRow.Priority = mReq.Priority;
                    existingRow.Shift = mReq.Shift;
                    existingRow.ScheduledDate = scheduledDate;
                    existingRow.Status = normalizedStatus;
                    existingRow.Notes = request.Notes;
                    existingRow.UpdatedAt = DateTime.UtcNow;

                    retainedIds.Add(existingRow.Id);
                }
                else
                {
                    // Insert new
                    var newRow = new PmPlan
                    {
                        PlanName = planName,
                        AreaName = mReq.Area,
                        MachineName = mReq.MachineName,
                        ChecklistType = mReq.ChecklistType,
                        AssignedTechnician = mReq.AssignedTo,
                        Priority = mReq.Priority,
                        Shift = mReq.Shift,
                        ScheduledDate = scheduledDate,
                        BaselineDate = scheduledDate,
                        Status = normalizedStatus,
                        PlanStatus = planStatus,
                        Notes = request.Notes,
                        CompanyId = companyId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.PmPlans.Add(newRow);
                }
            }

            // Delete removed rows
            var toDelete = existingRows.Where(r => !retainedIds.Contains(r.Id)).ToList();
            _context.PmPlans.RemoveRange(toDelete);

            headerRow.Notes = request.Notes;
            headerRow.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatePlanResult(true);
        }

        private static string NormalizeStoredStatus(string? status)
        {
            var s = status?.Trim().ToLowerInvariant();
            return s switch
            {
                "draft" => "draft",
                "planning" => "planning",
                "active" => "active",
                _ => "draft"
            };
        }
    }
}

