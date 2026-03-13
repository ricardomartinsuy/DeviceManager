using DevicesApi.Domain.Enums;
using DevicesApi.Domain.Exceptions;

namespace DevicesApi.Domain.Entities;

public sealed class Device
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public DeviceState State { get; private set; }
    public DateTime CreationTime { get; private set; }

    public Device(string name, string brand, DeviceState state)
    {
        Id = Guid.NewGuid();
        Name = name;
        Brand = brand;
        State = state;
        CreationTime = DateTime.UtcNow;
    }

    private Device() { }

    /// <summary>
    /// Updates device fields. Throws if device is in-use and name/brand change is attempted.
    /// CreationTime is never modified.
    /// </summary>
    public void Update(string? name, string? brand, DeviceState? state)
    {
        bool nameChanging = name is not null && name != Name;
        bool brandChanging = brand is not null && brand != Brand;

        if (State == DeviceState.InUse && (nameChanging || brandChanging))
            throw new DeviceInUseException(Id);

        if (name is not null) Name = name;
        if (brand is not null) Brand = brand;
        if (state is not null) State = state.Value;
    }

    /// <summary>
    /// Returns false if device is in-use, preventing deletion.
    /// </summary>
    public bool CanBeDeleted() => State != DeviceState.InUse;
}
