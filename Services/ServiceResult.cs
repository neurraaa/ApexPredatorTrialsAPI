namespace ApexPredatorTrialsAPI.Services
{
    public enum ServiceStatus { Ok, NotFound, Invalid }

    public class ServiceResult<T>
    {
        public ServiceStatus Status { get; init; }
        public string? Error { get; init; }
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data) => new() { Status = ServiceStatus.Ok, Data = data };
        public static ServiceResult<T> Invalid(string error) => new() { Status = ServiceStatus.Invalid, Error = error };
        public static ServiceResult<T> NotFound() => new() { Status = ServiceStatus.NotFound };
    }
}
