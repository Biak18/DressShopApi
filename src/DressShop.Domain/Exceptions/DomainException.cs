namespace DressShop.Domain.Exceptions;

/// <summary>
/// Thrown when a domain invariant/business rule is violated (e.g. inside an
/// entity factory method or a value object constructor). The API layer's
/// global exception handler maps this to HTTP 400/422, not 500.
/// </summary>
public class DomainException(string message) : Exception(message);
