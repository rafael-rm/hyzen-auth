namespace Auth.Application.Common;

public class Result<TValue> : Result
{
    private readonly TValue _value;
    
    protected internal Result(TValue value, bool isSuccess, ResultError error) : base(isSuccess, error)
    {
        _value = value;
    }
    
    public TValue Value
    {
        get
        {
            if (!IsSuccess)
            {
                throw new InvalidOperationException("No value present. Check IsSuccess before accessing Value.");
            }
            
            return _value;
        }
    }
}