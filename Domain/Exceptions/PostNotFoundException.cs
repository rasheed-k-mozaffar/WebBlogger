namespace Domain.Exceptions;

public class PostNotFoundException(string message) : Exception(message);