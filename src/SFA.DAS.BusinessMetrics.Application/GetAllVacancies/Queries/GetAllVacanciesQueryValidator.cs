using FluentValidation;

namespace SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries
{
    public class GetAllVacanciesQueryValidator : AbstractValidator<GetAllVacanciesQuery>
    {
        public GetAllVacanciesQueryValidator()
        {
            RuleFor(exp => exp.StartDate)
                .NotEmpty()
                .NotNull()
                .WithMessage("Start date could not be null or empty")
                .LessThanOrEqualTo(exp => DateTime.UtcNow);

            RuleFor(exp => exp.EndDate)
                .NotEmpty()
                .NotNull()
                .WithMessage("End date could not be null or empty")
                .LessThanOrEqualTo(exp => DateTime.UtcNow);

            RuleFor(exp => exp.EndDate)
                .GreaterThanOrEqualTo(exp => exp.StartDate)
                .WithMessage("End date must be greater than start date");
        }
    }
}