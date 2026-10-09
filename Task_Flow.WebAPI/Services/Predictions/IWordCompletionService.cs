using Task_Flow.WebAPI.Services.Results;

namespace Task_Flow.WebAPI.Services.Predictions
{
    /// <summary>
    /// Yarımçıq yazılmış sözü tamamlamaq üçün təklif verir.
    /// </summary>
    public interface IWordCompletionService
    {
        Task<ServiceResult<object>> CompleteAsync(string prompt);
    }
}
