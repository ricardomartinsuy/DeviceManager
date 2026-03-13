using DevicesApi.Application.DTOs;
using DevicesApi.Domain.Enums;

namespace DevicesApi.Application.Interfaces;

public interface IDeviceService
{
    Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, CancellationToken ct = default);
    Task<DeviceResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<DeviceResponse>> GetAllAsync(string? brand, DeviceState? state, CancellationToken ct = default);
    Task<DeviceResponse> UpdateAsync(Guid id, UpdateDeviceRequest request, CancellationToken ct = default);
    Task<DeviceResponse> PatchAsync(Guid id, PatchDeviceRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
