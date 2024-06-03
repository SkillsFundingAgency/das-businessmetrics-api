using FluentValidation;

namespace SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries
{
    public class GetVacancyMetricsQueryValidator : AbstractValidator<GetVacancyMetricsQuery>
    {
        public const string VacancyReferenceEmpty = "You must include a vacancy reference.";
        public const string VacancyReferenceTooShort = "The vacancy reference must be atleast 10 characters or more.";

        public GetVacancyMetricsQueryValidator()
        {
            RuleFor(exp => exp.ServiceName)
                .NotEmpty()
                .NotNull()
                .WithMessage("Service name could not by null or empty");

            RuleFor(x => x.VacancyReference)
                .NotEmpty().WithMessage(VacancyReferenceEmpty)
                .NotNull()
                .MinimumLength(10).WithMessage(VacancyReferenceTooShort);

            RuleFor(exp => exp.StartDate)
                .NotEmpty()
                .NotNull()
                .WithMessage("Start date could not by null or empty")
                .LessThanOrEqualTo(exp => DateTime.UtcNow);

            RuleFor(exp => exp.EndDate)
                .NotEmpty()
                .NotNull()
                .WithMessage("End date could not by null or empty")
                .LessThanOrEqualTo(exp => DateTime.UtcNow);

            RuleFor(exp => exp.EndDate)
                .GreaterThanOrEqualTo(exp => exp.StartDate)
                .WithMessage("End date must be greater than start date");
        }
    }
}
