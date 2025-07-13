namespace TaskFlow.Domain.Exceptions;

public class NotFoundException(string resourceType, string resourceIdentifier)
    : Exception($"Resource '{resourceType}' with id '{resourceIdentifier}' was not found.")
{
}
