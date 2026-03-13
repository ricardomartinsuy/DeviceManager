using DevicesApi.Domain.Enums;

namespace DevicesApi.Application.DTOs;

public sealed record DeviceResponse(
    Guid Id,
    string Name,
    string Brand,
    DeviceState State,
    DateTime CreationTime
);
