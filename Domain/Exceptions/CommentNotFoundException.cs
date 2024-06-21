namespace Domain.Exceptions;

public class CommentNotFoundException(string message) : Exception(message);