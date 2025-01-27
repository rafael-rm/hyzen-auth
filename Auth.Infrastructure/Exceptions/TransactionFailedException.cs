namespace Auth.Infrastructure.Exceptions;

public class TransactionFailedException(Exception innerException) : Exception("Transaction failed.", innerException);