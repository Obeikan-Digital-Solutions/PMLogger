using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Web;
using BuildingBlocks.SanedTenants;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PMLogger.Data;
using PMLogger.PM.Models;

namespace PMLogger.PM.Features.Plans.UpdatingPlanStatus.V1
{
    public record UpdatePlanStatus(int Id, string Status) : ICommand<UpdatePlanStatusResult>;

    public record UpdatePlanStatusResult(bool Success);

    public class UpdatePlanStatusValidator : AbstractValidator<UpdatePlanStatus>
    {
        public UpdatePlanStatusValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Status).NotEmpty();
        }
    }

    public class UpdatePlanStatusHandler : ICommandHandler<UpdatePlanStatus, UpdatePlanStatusResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdatePlanStatusHandler> _logger;
        private readonly ICurrentTenantAndUserProvider _tenantProvider;

        public UpdatePlanStatusHandler(ApplicationDbContext context, ILogger<UpdatePlanStatusHandler> logger, ICurrentTenantAndUserProvider tenantProvider)
        {
            _context = context;
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        public async Task<UpdatePlanStatusResult> Handle(UpdatePlanStatus request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CommandHandler {Command}, Request {Request}", nameof(UpdatePlanStatus), request);
            Guard.Against.Null(request, nameof(request));

            var tenantIdentifier = _tenantProvider.GetCurrentTenantIdentifier();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.CompanyCode == tenantIdentifier, cancellationToken);
            if (company == null) throw new Exception($"Unable to resolve company for tenant: {tenantIdentifier}");
            var companyId = company.Id;

            var plan = await _context.PmPlans
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CompanyId == companyId, cancellationToken);

            if (plan == null)
            {
                throw new Exception("PM plan not found");
            }

            plan.PlanStatus = request.Status;
            plan.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new UpdatePlanStatusResult(true);
        }
    }
}

