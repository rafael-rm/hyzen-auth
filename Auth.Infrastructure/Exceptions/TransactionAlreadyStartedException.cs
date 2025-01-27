namespace Auth.Infrastructure.Exceptions;

public class TransactionAlreadyStartedException(Guid guid) : Exception($"Transaction with guid {guid} already started in this context.")
{
}