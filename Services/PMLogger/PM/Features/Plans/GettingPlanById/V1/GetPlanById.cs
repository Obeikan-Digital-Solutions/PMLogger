using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;

namespace PMLogger.PM.Features.Plans.GettingPlanById.V1
{
    public record GetPlanById(int Id) : IQuery<GetPlanByIdResult>;

    public record PlanMachineDto(
        int Id,
        string Area,
        string MachineName,
        string ChecklistType,
        string AssignedTo,
        string Priority,
        string Shift,
        int[] ScheduledDays,
        string Status);

    public record GetPlanByIdResult(
        int Id,
        string Name,
        int Month,
        int Year,
        string Status,
        int MachineCount,
        List<PlanMachineDto> Machines);

    public class GetPlanByIdValidator : AbstractValidator<GetPlanById>
    {
        public GetPlanByIdValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Valid Id is required!");
        }
    }

    public class GetPlanByIdHandler : IQueryHandler<GetPlanById, GetPlanByIdResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetPlanByIdHandler> _logger;

        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public GetPlanByIdHandler(ApplicationDbContext context, ILogger<GetPlanByIdHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<GetPlanByIdResult> Handle(GetPlanById request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("QueryHandler {Query}, Request {Request}", nameof(GetPlanById), request);
            Guard.Against.Null(request, nameof(request));

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null) throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            var companyId = company.Id;

            var initialPlan = await _context.PmPlans.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == companyId, cancellationToken);

            if (initialPlan == null)
            {
                throw new Exception("PM plan not found");
            }

            var allRows = await _context.PmPlans.AsNoTracking()
                .Where(p => p.PlanName == initialPlan.PlanName && p.CompanyId == initialPlan.CompanyId)
                .OrderBy(p => p.MachineName == "MONTHLY_PLAN_HEADER" ? 0 : 1)
                .ThenBy(p => p.AreaName)
                .ThenBy(p => p.MachineName)
                .ThenBy(p => p.ChecklistType)
                .ToListAsync(cancellationToken);

            var machines = new List<PlanMachineDto>();
            var machineKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rawStatus = "Draft";
            var month = 0;
            var year = 0;

            foreach (var row in allRows)
            {
                var isHeader = string.Equals(row.MachineName, "MONTHLY_PLAN_HEADER", StringComparison.OrdinalIgnoreCase);
                
                if (isHeader)
                {
                    var date = row.ScheduledDate ?? row.BaselineDate ?? row.CreatedAt;
                    month = date.Month;
                    year = date.Year;
                }

                var candidateStatus = !string.IsNullOrEmpty(row.PlanStatus) ? row.PlanStatus : (!string.IsNullOrEmpty(row.Status) ? row.Status : "Draft");
                if (isHeader || string.IsNullOrWhiteSpace(rawStatus) || rawStatus == "Draft")
                {
                    rawStatus = candidateStatus;
                }

                if (string.IsNullOrWhiteSpace(row.MachineName) || isHeader)
                {
                    continue;
                }

                machineKeys.Add($"{row.AreaName}|||{row.MachineName}");

                var scheduledDate = row.ScheduledDate ?? row.BaselineDate ?? row.CreatedAt;
                var scheduledDays = new[] { scheduledDate.Day };

                machines.Add(new PlanMachineDto(
                    row.Id,
                    row.AreaName,
                    row.MachineName,
                    row.ChecklistType ?? string.Empty,
                    row.AssignedTechnician ?? string.Empty,
                    row.Priority ?? "Medium",
                    row.Shift ?? "Morning",
                    scheduledDays,
                    NormalizeWorkflowStatusForApi(candidateStatus, row.ExecutionSubmitted, row.OperationApprovedBy, row.SupervisorApprovedBy)
                ));
            }

            return new GetPlanByIdResult(
                initialPlan.Id,
                initialPlan.PlanName,
                month,
                year,
                NormalizeApiStatus(rawStatus),
                machineKeys.Count,
                machines
            );
        }

        private static string NormalizeApiStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return "Draft";
            var normalized = status.Trim().ToLowerInvariant();
            return normalized switch
            {
                "draft" => "Draft",
                "planning" => "Planning",
                "pending execution" => "Pending Execution",
                "pending_execution" => "Pending Execution",
                "awaiting approval" => "Awaiting Approval",
                "awaiting_approval" => "Awaiting Approval",
                "completed" => "Completed",
                _ => status
            };
        }

        private static string NormalizeWorkflowStatusForApi(string? status, bool executionSubmitted, string? operationApprovedBy, string? supervisorApprovedBy)
        {
            var s = status?.Trim().ToLowerInvariant();
            if (s == "completed") return "Completed";

            if (executionSubmitted || !string.IsNullOrWhiteSpace(operationApprovedBy) || !string.IsNullOrWhiteSpace(supervisorApprovedBy))
            {
                return "Awaiting Approval";
            }

            return "Pending Execution";
        }
    }
}

