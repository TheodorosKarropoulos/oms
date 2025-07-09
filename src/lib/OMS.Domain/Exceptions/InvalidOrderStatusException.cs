namespace OMS.Domain.Exceptions;

public sealed class InvalidOrderStatusException(string message) : Exception(message);