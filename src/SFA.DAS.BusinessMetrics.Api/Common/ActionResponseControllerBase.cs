using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;

namespace SFA.DAS.BusinessMetrics.Api.Common
{
    [ExcludeFromCodeCoverage]
    public abstract class ActionResponseControllerBase : ControllerBase
    {
        protected abstract string ControllerName { get; }

        protected IActionResult GetResponse<T>(ValidatedResponse<T> response) where T : class
        {
            if (response.IsValidResponse) return new OkObjectResult(response.Result);

            return new BadRequestObjectResult(FormatErrors(response.Errors));
        }
        
        private static List<ValidationError> FormatErrors(IEnumerable<ValidationFailure> errors)
        {
            return errors.Select(err => new ValidationError
            {
                PropertyName = err.PropertyName,
                ErrorMessage = err.ErrorMessage
            }).ToList();
        }
    }
}
