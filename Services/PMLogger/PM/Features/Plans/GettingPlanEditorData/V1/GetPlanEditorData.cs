using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;

namespace PMLogger.PM.Features.Plans.GettingPlanEditorData.V1
{
    public record GetPlanEditorData(int? Id) : IQuery<GetPlanEditorDataResult>;

    public record GetPlanEditorDataResult(List<MachineDto> Machines, List<TechnicianDto> Technicians, PlanDto? Plan);

    public record MachineDto(int Id, string Name, string Area);
    public record TechnicianDto(string Id, string UserName, string FullName);
    public record PlanDto(int Id, string Name, string Status, string? Notes);

    public class GetPlanEditorDataValidator : AbstractValidator<GetPlanEditorData>
    {
        public GetPlanEditorDataValidator()
        {
        }
    }

    public class GetPlanEditorDataHandler : IQueryHandler<GetPlanEditorData, GetPlanEditorDataResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetPlanEditorDataHandler> _logger;
        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public GetPlanEditorDataHandler(ApplicationDbContext context, ILogger<GetPlanEditorDataHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<GetPlanEditorDataResult> Handle(GetPlanEditorData request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("QueryHandler {Query}, Request {Request}", nameof(GetPlanEditorData), request);

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null) throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            var companyId = company.Id;

            var machines = await _context.PmMachines
                .Where(m => m.CompanyId == companyId)
                .Select(m => new MachineDto(m.Id, m.Name, m.PmArea != null ? m.PmArea.Name : string.Empty))
                .ToListAsync(cancellationToken);

            var technicians = await _context.Users
                .Where(u => u.CompanyId == companyId)
                .Select(u => new TechnicianDto(u.Id.ToString(), u.Username, u.Username)) // Assume FullName is Username since it's missing in User model
                .ToListAsync(cancellationToken);

            PmPlan? plan = null;
            if (request.Id.HasValue)
            {
                plan = await _context.PmPlans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.Id.Value && p.CompanyId == companyId, cancellationToken);
            }

            PlanDto? planDto = null;
            if (plan != null)
            {
                var planStatus = plan.PlanStatus ?? plan.Status ?? "Draft";
                planDto = new PlanDto(plan.Id, plan.PlanName, planStatus, plan.Notes);
            }

            return new GetPlanEditorDataResult(machines, technicians, planDto);
        }
    }
}

