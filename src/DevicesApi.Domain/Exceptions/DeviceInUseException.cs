namespace DevicesApi.Domain.Exceptions;

public sealed class DeviceInUseException(Guid id)
    : Exception($"Device '{id}' is currently in use. Name and Brand cannot be modified.");
