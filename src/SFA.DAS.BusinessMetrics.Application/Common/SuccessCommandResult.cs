using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.BusinessMetrics.Application.Common
{
    [ExcludeFromCodeCoverage]
    public record SuccessCommandResult(bool IsSuccess = true);
}
