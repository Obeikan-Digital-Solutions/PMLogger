using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;

namespace PMLogger.PM.Features.Plans.DeletingPlan.V1
{
    public record DeletePlan(int Id) : ICommand<DeletePlanResult>;

    public record DeletePlanResult(bool Success);

    public class DeletePlanValidator : AbstractValidator<DeletePlan>
    {
        public DeletePlanValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    public class DeletePlanHandler : ICommandHandler<DeletePlan, DeletePlanResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeletePlanHandler> _logger;
        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public DeletePlanHandler(ApplicationDbContext context, ILogger<DeletePlanHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<DeletePlanResult> Handle(DeletePlan request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CommandHandler {Command}, Request {Request}", nameof(DeletePlan), request);
            Guard.Against.Null(request, nameof(request));

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null) throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            var companyId = company.Id;

            var initialPlan = await _context.PmPlans
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == companyId, cancellationToken);

            if (initialPlan == null)
            {
                throw new Exception("PM plan not found");
            }

            var planName = initialPlan.PlanName;

            var rowsToDelete = await _context.PmPlans
                .Where(p => p.PlanName == planName && p.CompanyId == companyId)
                .ToListAsync(cancellationToken);

            _context.PmPlans.RemoveRange(rowsToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeletePlanResult(true);
        }
    }
}

