namespace TaskFlow.Domain.Exceptions;

public class InvalidInvitationException(string message = "The invitation is invalid or has expired.")
    : Exception(message);
