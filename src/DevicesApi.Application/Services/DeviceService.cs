using DevicesApi.Application.DTOs;
using DevicesApi.Application.Interfaces;
using DevicesApi.Domain.Entities;
using DevicesApi.Domain.Enums;
using DevicesApi.Domain.Exceptions;
using DevicesApi.Domain.Interfaces;

namespace DevicesApi.Application.Services;

public sealed class DeviceService(IDeviceRepository repository) : IDeviceService
{
    public async Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, CancellationToken ct = default)
    {
        var device = new Device(request.Name, request.Brand, request.State);
        await repository.AddAsync(device, ct);
        return ToResponse(device);
    }

    public async Task<DeviceResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var device = await repository.GetByIdAsync(id, ct)
            ?? throw new DeviceNotFoundException(id);
        return ToResponse(device);
    }

    public async Task<IReadOnlyList<DeviceResponse>> GetAllAsync(string? brand, DeviceState? state, CancellationToken ct = default)
    {
        var devices = await repository.GetAllAsync(brand, state, ct);
        return devices.Select(ToResponse).ToList();
    }

    public async Task<DeviceResponse> UpdateAsync(Guid id, UpdateDeviceRequest request, CancellationToken ct = default)
    {
        var device = await repository.GetByIdAsync(id, ct)
            ?? throw new DeviceNotFoundException(id);

        device.Update(request.Name, request.Brand, request.State);
        await repository.UpdateAsync(device, ct);
        return ToResponse(device);
    }

    public async Task<DeviceResponse> PatchAsync(Guid id, PatchDeviceRequest request, CancellationToken ct = default)
    {
        var device = await repository.GetByIdAsync(id, ct)
            ?? throw new DeviceNotFoundException(id);

        device.Update(request.Name, request.Brand, request.State);
        await repository.UpdateAsync(device, ct);
        return ToResponse(device);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var device = await repository.GetByIdAsync(id, ct)
            ?? throw new DeviceNotFoundException(id);

        if (!device.CanBeDeleted())
            throw new DeviceDeletionNotAllowedException(id);

        await repository.DeleteAsync(device, ct);
    }

    private static DeviceResponse ToResponse(Device d) =>
        new(d.Id, d.Name, d.Brand, d.State, d.CreationTime);
}
