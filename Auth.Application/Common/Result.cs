namespace Auth.Application.Common;

public class Result
{
    public bool IsSuccess { get; set; }
    public ResultError Error { get; set; }
    
    protected Result(bool isSuccess, ResultError error)
    {
        if (isSuccess && error != ResultError.None)
        {
            throw new InvalidOperationException("Invalid operation for success result, error should be none.");
        }
        
        if (!isSuccess && error == ResultError.None)
        {
            throw new InvalidOperationException("Invalid operation for failure result, error should not be none.");
        }
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new(true, ResultError.None);
    public static Result Failure(ResultError error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, ResultError.None);
}