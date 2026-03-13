namespace DevicesApi.Domain.Exceptions;
public sealed class DeviceNotFoundException(Guid id)
    : Exception($"Device with ID '{id}' was not found.");
