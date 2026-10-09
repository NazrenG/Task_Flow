namespace Task_Flow.WebAPI.Services.Results
{
    public enum ServiceResultStatus
    {
        Success,
        BadRequest,
        NotFound,
        Unauthorized,
        Failure
    }

    /// <summary>
    /// Service əməliyyatının nəticəsi. Controller bu nəticəni HTTP cavabına çevirir,
    /// beləliklə service HTTP-dən asılı olmur, controller isə biznes qərarı vermir.
    /// </summary>
    public class ServiceResult<T>
    {
        public ServiceResultStatus Status { get; }
        public T? Value { get; }
        public object? Error { get; }

        // Failure nəticəsi üçün HTTP status kodu (default 500)
        public int FailureStatusCode { get; }

        public bool IsSuccess => Status == ServiceResultStatus.Success;

        private ServiceResult(ServiceResultStatus status, T? value, object? error, int failureStatusCode = 500)
        {
            Status = status;
            Value = value;
            Error = error;
            FailureStatusCode = failureStatusCode;
        }

        public static ServiceResult<T> Success(T value) => new(ServiceResultStatus.Success, value, null);
        public static ServiceResult<T> BadRequest(object error) => new(ServiceResultStatus.BadRequest, default, error);
        public static ServiceResult<T> NotFound(object? error = null) => new(ServiceResultStatus.NotFound, default, error);
        public static ServiceResult<T> Unauthorized(object? error = null) => new(ServiceResultStatus.Unauthorized, default, error);
        public static ServiceResult<T> Failure(object error, int statusCode = 500) => new(ServiceResultStatus.Failure, default, error, statusCode);
    }
}
