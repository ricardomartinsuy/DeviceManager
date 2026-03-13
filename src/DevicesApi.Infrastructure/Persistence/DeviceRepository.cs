using DevicesApi.Domain.Entities;
using DevicesApi.Domain.Enums;
using DevicesApi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DevicesApi.Infrastructure.Persistence;

public sealed class DeviceRepository(DevicesDbContext context) : IDeviceRepository
{
    public async Task<Device?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Devices.FindAsync([id], cancellationToken);

    public async Task<IReadOnlyList<Device>> GetAllAsync(
        string? brand,
        DeviceState? state,
        CancellationToken cancellationToken = default)
    {
        var query = context.Devices.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(d => d.Brand == brand);

        if (state is not null)
            query = query.Where(d => d.State == state);

        return await query
            .OrderBy(d => d.CreationTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Device device, CancellationToken cancellationToken = default)
    {
        await context.Devices.AddAsync(device, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Device device, CancellationToken cancellationToken = default)
    {
        context.Devices.Update(device);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Device device, CancellationToken cancellationToken = default)
    {
        context.Devices.Remove(device);
        await context.SaveChangesAsync(cancellationToken);
    }
}
