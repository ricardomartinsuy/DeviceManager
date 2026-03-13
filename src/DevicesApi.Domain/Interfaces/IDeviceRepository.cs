using DevicesApi.Domain.Entities;
using DevicesApi.Domain.Enums;

namespace DevicesApi.Domain.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Device>> GetAllAsync(string? brand, DeviceState? state, CancellationToken cancellationToken = default);
    Task AddAsync(Device device, CancellationToken cancellationToken = default);
    Task UpdateAsync(Device device, CancellationToken cancellationToken = default);
    Task DeleteAsync(Device device, CancellationToken cancellationToken = default);
}
