using DevicesApi.Domain.Enums;

namespace DevicesApi.Application.DTOs;

public sealed record PatchDeviceRequest(
    string? Name,
    string? Brand,
    DeviceState? State
);
