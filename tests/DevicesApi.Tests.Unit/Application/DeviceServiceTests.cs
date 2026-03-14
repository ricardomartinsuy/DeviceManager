using DevicesApi.Application.DTOs;
using DevicesApi.Application.Services;
using DevicesApi.Domain.Entities;
using DevicesApi.Domain.Enums;
using DevicesApi.Domain.Exceptions;
using DevicesApi.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace DevicesApi.Tests.Unit.Application;

public sealed class DeviceServiceTests
{
    private readonly IDeviceRepository _repository;
    private readonly DeviceService _sut;

    public DeviceServiceTests()
    {
        _repository = Substitute.For<IDeviceRepository>();
        _sut = new DeviceService(_repository);
    }

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ShouldAddDeviceAndReturnResponse()
    {
        var request = new CreateDeviceRequest("Laptop", "Dell", DeviceState.Available);

        var result = await _sut.CreateAsync(request);

        await _repository.Received(1).AddAsync(
            Arg.Is<Device>(d => d.Name == "Laptop" && d.Brand == "Dell"),
            Arg.Any<CancellationToken>());

        result.Name.Should().Be("Laptop");
        result.Brand.Should().Be("Dell");
        result.State.Should().Be(DeviceState.Available);
        result.Id.Should().NotBeEmpty();
    }

    // ── GetByIdAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WhenDeviceExists_ShouldReturnResponse()
    {
        var device = new Device("Laptop", "Dell", DeviceState.Available);
        _repository.GetByIdAsync(device.Id, Arg.Any<CancellationToken>()).Returns(device);

        var result = await _sut.GetByIdAsync(device.Id);

        result.Id.Should().Be(device.Id);
        result.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetByIdAsync_WhenDeviceNotFound_ShouldThrowDeviceNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _sut.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<DeviceNotFoundException>();
    }

    // ── GetAllAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedList()
    {
        var devices = new List<Device>
        {
            new("Laptop", "Dell", DeviceState.Available),
            new("Phone", "Samsung", DeviceState.InUse)
        };
        _repository.GetAllAsync(null, null, Arg.Any<CancellationToken>()).Returns(devices);

        var result = await _sut.GetAllAsync(null, null);

        result.Should().HaveCount(2);
    }

    // ── UpdateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WhenDeviceExists_ShouldUpdateAndReturn()
    {
        var device = new Device("Old", "OldBrand", DeviceState.Available);
        _repository.GetByIdAsync(device.Id, Arg.Any<CancellationToken>()).Returns(device);

        var request = new UpdateDeviceRequest("New", "NewBrand", DeviceState.Inactive);
        var result = await _sut.UpdateAsync(device.Id, request);

        result.Name.Should().Be("New");
        result.Brand.Should().Be("NewBrand");
        result.State.Should().Be(DeviceState.Inactive);
        await _repository.Received(1).UpdateAsync(device, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateAsync_WhenDeviceNotFound_ShouldThrowDeviceNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _sut.UpdateAsync(
            Guid.NewGuid(), new UpdateDeviceRequest("N", "B", DeviceState.Available));

        await act.Should().ThrowAsync<DeviceNotFoundException>();
    }

    // ── PatchAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task PatchAsync_WithPartialFields_ShouldOnlyUpdateProvided()
    {
        var device = new Device("Original", "Brand", DeviceState.Available);
        _repository.GetByIdAsync(device.Id, Arg.Any<CancellationToken>()).Returns(device);

        var result = await _sut.PatchAsync(device.Id, new PatchDeviceRequest(null, null, DeviceState.Inactive));

        result.State.Should().Be(DeviceState.Inactive);
        result.Name.Should().Be("Original");
        result.Brand.Should().Be("Brand");
    }

    // ── DeleteAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenDeviceIsAvailable_ShouldDelete()
    {
        var device = new Device("Laptop", "Dell", DeviceState.Available);
        _repository.GetByIdAsync(device.Id, Arg.Any<CancellationToken>()).Returns(device);

        await _sut.DeleteAsync(device.Id);

        await _repository.Received(1).DeleteAsync(device, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenDeviceIsInUse_ShouldThrowDeviceDeletionNotAllowedException()
    {
        var device = new Device("Laptop", "Dell", DeviceState.InUse);
        _repository.GetByIdAsync(device.Id, Arg.Any<CancellationToken>()).Returns(device);

        var act = async () => await _sut.DeleteAsync(device.Id);

        await act.Should().ThrowAsync<DeviceDeletionNotAllowedException>();
        await _repository.DidNotReceive().DeleteAsync(Arg.Any<Device>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenDeviceNotFound_ShouldThrowDeviceNotFoundException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<DeviceNotFoundException>();
    }
}
