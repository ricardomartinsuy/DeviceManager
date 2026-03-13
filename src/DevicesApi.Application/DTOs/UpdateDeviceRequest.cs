using DevicesApi.Domain.Enums;

namespace DevicesApi.Application.DTOs;

public sealed record UpdateDeviceRequest(
    string Name,
    string Brand,
    DeviceState State
);
