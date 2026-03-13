using DevicesApi.Domain.Enums;

namespace DevicesApi.Application.DTOs;

public sealed record CreateDeviceRequest(
    string Name,
    string Brand,
    DeviceState State
);
