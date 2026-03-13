namespace DevicesApi.Domain.Exceptions;
public sealed class DeviceDeletionNotAllowedException(Guid id)
    : Exception($"Device '{id}' is currently in use and cannot be deleted.");
