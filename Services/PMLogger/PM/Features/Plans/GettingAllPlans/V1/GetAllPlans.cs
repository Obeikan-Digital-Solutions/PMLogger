using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;

namespace PMLogger.PM.Features.Plans.GettingAllPlans.V1
{
    public record GetAllPlans(string? PlanName = null, string? Status = null, int? Limit = null) : IQuery<GetAllPlansResult>;

    public record PlanSummaryDto(
        int Id,
        string Name,
        string Status,
        int Month,
        int Year,
        int MachineCount);

    public record GetAllPlansResult(List<PlanSummaryDto> Items);

    public class GetAllPlansValidator : AbstractValidator<GetAllPlans>
    {
        public GetAllPlansValidator()
        {
        }
    }

    public class GetAllPlansHandler : IQueryHandler<GetAllPlans, GetAllPlansResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllPlansHandler> _logger;

        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public GetAllPlansHandler(ApplicationDbContext context, ILogger<GetAllPlansHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<GetAllPlansResult> Handle(GetAllPlans request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("QueryHandler {Query}, Request {Request}", nameof(GetAllPlans), request);
            Guard.Against.Null(request, nameof(request));

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null) throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            var companyId = company.Id;

            var query = _context.PmPlans.AsNoTracking().Where(p => p.CompanyId == companyId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.PlanName))
            {
                query = query.Where(p => p.PlanName.Contains(request.PlanName));
            }

            // Grouping logic to match LoadPlanSummariesAsync
            var groupedPlans = await query
                .GroupBy(p => p.PlanName)
                .Select(g => new
                {
                    Id = g.Min(p => p.Id),
                    Name = g.Key,
                    // Use PlanStatus if available, else Status, else 'Draft'
                    RawStatus = g.Max(p => !string.IsNullOrEmpty(p.PlanStatus) ? p.PlanStatus : (!string.IsNullOrEmpty(p.Status) ? p.Status : "Draft")),
                    // Extract month/year from ScheduledDate or BaselineDate or CreatedAt
                    Date = g.Max(p => p.ScheduledDate ?? p.BaselineDate ?? p.CreatedAt),
                    MachineCount = g.Count(p => !string.IsNullOrEmpty(p.MachineName) && p.MachineName != "MONTHLY_PLAN_HEADER")
                })
                .ToListAsync(cancellationToken);

            var items = groupedPlans
                .Select(p => new PlanSummaryDto(
                    p.Id,
                    p.Name,
                    NormalizeApiStatus(p.RawStatus),
                    p.Date.Month,
                    p.Date.Year,
                    p.MachineCount
                ))
                .Where(p => string.IsNullOrWhiteSpace(request.Status) || p.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ThenBy(p => p.Name)
                .ToList();

            if (request.Limit.HasValue)
            {
                items = items.Take(request.Limit.Value).ToList();
            }

            return new GetAllPlansResult(items);
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
    }
}

