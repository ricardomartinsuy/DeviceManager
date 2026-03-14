using DevicesApi.Domain.Entities;
using DevicesApi.Domain.Enums;
using DevicesApi.Domain.Exceptions;
using FluentAssertions;

namespace DevicesApi.Tests.Unit.Domain;

public sealed class DeviceTests
{
    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var device = new Device("Laptop", "Dell", DeviceState.Available);

        device.Id.Should().NotBeEmpty();
        device.Name.Should().Be("Laptop");
        device.Brand.Should().Be("Dell");
        device.State.Should().Be(DeviceState.Available);
        device.CreationTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    // ── Update ───────────────────────────────────────────────────────────────

    [Fact]
    public void Update_WhenAvailable_ShouldChangeNameAndBrand()
    {
        var device = new Device("Old Name", "Old Brand", DeviceState.Available);

        device.Update("New Name", "New Brand", DeviceState.Inactive);

        device.Name.Should().Be("New Name");
        device.Brand.Should().Be("New Brand");
        device.State.Should().Be(DeviceState.Inactive);
    }

    [Fact]
    public void Update_WhenInUse_ChangingName_ShouldThrowDeviceInUseException()
    {
        var device = new Device("Laptop", "Dell", DeviceState.InUse);

        var act = () => device.Update("New Name", null, null);

        act.Should().Throw<DeviceInUseException>();
    }

    [Fact]
    public void Update_WhenInUse_ChangingBrand_ShouldThrowDeviceInUseException()
    {
        var device = new Device("Laptop", "Dell", DeviceState.InUse);

        var act = () => device.Update(null, "New Brand", null);

        act.Should().Throw<DeviceInUseException>();
    }

    [Fact]
    public void Update_WhenInUse_ChangingOnlyState_ShouldSucceed()
    {
        var device = new Device("Laptop", "Dell", DeviceState.InUse);

        device.Update(null, null, DeviceState.Available);

        device.State.Should().Be(DeviceState.Available);
        device.Name.Should().Be("Laptop");
        device.Brand.Should().Be("Dell");
    }

    [Fact]
    public void Update_ShouldNeverModifyCreationTime()
    {
        var device = new Device("Laptop", "Dell", DeviceState.Available);
        var originalTime = device.CreationTime;

        device.Update("New Name", "New Brand", DeviceState.Inactive);

        device.CreationTime.Should().Be(originalTime);
    }

    [Fact]
    public void Update_WithSameNameWhenInUse_ShouldSucceed()
    {
        var device = new Device("Laptop", "Dell", DeviceState.InUse);

        // Same name/brand = not "changing", so no exception
        device.Update("Laptop", "Dell", DeviceState.Available);

        device.State.Should().Be(DeviceState.Available);
    }

    // ── CanBeDeleted ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData(DeviceState.Available, true)]
    [InlineData(DeviceState.Inactive, true)]
    [InlineData(DeviceState.InUse, false)]
    public void CanBeDeleted_ShouldReturnExpectedResult(DeviceState state, bool expected)
    {
        var device = new Device("Laptop", "Dell", state);

        device.CanBeDeleted().Should().Be(expected);
    }
}
