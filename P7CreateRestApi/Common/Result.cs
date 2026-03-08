namespace FindexiumAPI.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; } = default!;
        public string ErrorMessage { get; set; } = string.Empty;


        public static Result<T> Ok(T data)
        {
            return new Result<T> { IsSuccess = true, Data = data };
        }
        public static Result<T> Fail(string errorMessage)
        {
            return new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
