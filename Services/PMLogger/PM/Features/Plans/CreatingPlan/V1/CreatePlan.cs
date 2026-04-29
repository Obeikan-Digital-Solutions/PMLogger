using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;

namespace PMLogger.PM.Features.Plans.CreatingPlan.V1
{
    public record CreatePlan(
        int Month,
        int Year,
        int[] MachineIds,
        string? Status = "Draft",
        string? Priority = "Medium",
        string? Shift = "Morning",
        string? AssignedTechnician = null,
        string? Notes = null) : ICommand<CreatePlanResult>;

    public record CreatePlanResult(int Id, string Name);

    public class CreatePlanValidator : AbstractValidator<CreatePlan>
    {
        public CreatePlanValidator()
        {
            RuleFor(x => x.Month).InclusiveBetween(1, 12);
            RuleFor(x => x.Year).InclusiveBetween(2000, 2100);
        }
    }

    public class CreatePlanHandler : ICommandHandler<CreatePlan, CreatePlanResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreatePlanHandler> _logger;
        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public CreatePlanHandler(ApplicationDbContext context, ILogger<CreatePlanHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<CreatePlanResult> Handle(CreatePlan request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CommandHandler {Command}, Request {Request}", nameof(CreatePlan), request);
            Guard.Against.Null(request, nameof(request));

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null)
            {
                throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            }
            var companyId = company.Id;

            var planName = BuildPlanName(request.Month, request.Year);

            if (await _context.PmPlans.AsNoTracking().AnyAsync(p => p.PlanName == planName && p.CompanyId == companyId, cancellationToken))
            {
                throw new Exception($"Plan \"{planName}\" already exists.");
            }

            var selectedMachines = await _context.PmMachines
                .Include(m => m.PmArea)
                .Where(m => request.MachineIds.Contains(m.Id) && m.CompanyId == companyId)
                .ToListAsync(cancellationToken);

            var scheduledDate = new DateTime(request.Year, request.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var normalizedPlanStatus = NormalizeStoredStatus(request.Status);
            var updatedBy = _tenantProvider.GetCurrentUserId() ?? "system";

            // Create Header Row
            var headerRow = new PmPlan
            {
                PlanName = planName,
                MachineName = "MONTHLY_PLAN_HEADER",
                AreaName = "MONTHLY_PLAN",
                ChecklistType = "Header",
                AssignedTechnician = "System",
                ScheduledDate = scheduledDate,
                Shift = "All",
                Priority = "Medium",
                Status = "Planning",
                PlanStatus = normalizedPlanStatus,
                Notes = request.Notes,
                CompanyId = companyId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PmPlans.Add(headerRow);

            foreach (var machine in selectedMachines)
            {
                var checklistTypes = await _context.PmChecklistItems
                    .Where(i => i.MachineId == machine.Id && i.CompanyId == companyId)
                    .Select(i => i.PmChecklistType.Name)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (checklistTypes.Count == 0)
                {
                    checklistTypes.Add(string.Empty);
                }

                foreach (var checklistType in checklistTypes)
                {
                    var planRow = new PmPlan
                    {
                        PlanName = planName,
                        MachineName = machine.Name,
                        AreaName = machine.PmArea.Name,
                        ChecklistType = checklistType,
                        AssignedTechnician = request.AssignedTechnician,
                        ScheduledDate = scheduledDate,
                        Shift = request.Shift ?? "Morning",
                        Priority = request.Priority ?? "Medium",
                        Status = "Pending Execution",
                        PlanStatus = normalizedPlanStatus,
                        Notes = request.Notes,
                        CompanyId = companyId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.PmPlans.Add(planRow);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new CreatePlanResult(headerRow.Id, planName);
        }

        private static string BuildPlanName(int month, int year)
        {
            var monthName = new DateTime(2000, month, 1).ToString("MMMM");
            return $"{monthName} {year} PM Plan";
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

