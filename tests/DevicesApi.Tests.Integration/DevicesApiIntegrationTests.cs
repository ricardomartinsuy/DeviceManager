using DevicesApi.Application.DTOs;
using DevicesApi.Domain.Enums;
using DevicesApi.Tests.Integration.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace DevicesApi.Tests.Integration;

public sealed class DevicesApiIntegrationTests(ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    // ── POST /api/devices ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateDevice_ShouldReturn201WithDevice()
    {
        var request = new CreateDeviceRequest("MacBook Pro", "Apple", DeviceState.Available);

        var response = await _client.PostAsJsonAsync("/api/devices", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<DeviceResponse>();
        body!.Name.Should().Be("MacBook Pro");
        body.Brand.Should().Be("Apple");
        body.State.Should().Be(DeviceState.Available);
        body.Id.Should().NotBeEmpty();
        response.Headers.Location.Should().NotBeNull();
    }

    // ── GET /api/devices ─────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ShouldReturn200WithList()
    {
        await _client.PostAsJsonAsync("/api/devices",
            new CreateDeviceRequest("Phone", "Samsung", DeviceState.Available));

        var response = await _client.GetAsync("/api/devices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<DeviceResponse>>();
        body.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetAll_FilterByBrand_ShouldReturnOnlyMatchingDevices()
    {
        await _client.PostAsJsonAsync("/api/devices",
            new CreateDeviceRequest("iPad", "Apple", DeviceState.Available));
        await _client.PostAsJsonAsync("/api/devices",
            new CreateDeviceRequest("Galaxy", "Samsung", DeviceState.Available));

        var response = await _client.GetAsync("/api/devices?brand=Apple");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<DeviceResponse>>();
        body.Should().OnlyContain(d => d.Brand == "Apple");
    }

    [Fact]
    public async Task GetAll_FilterByState_ShouldReturnOnlyMatchingDevices()
    {
        await _client.PostAsJsonAsync("/api/devices",
            new CreateDeviceRequest("ThinkPad", "Lenovo", DeviceState.InUse));
        await _client.PostAsJsonAsync("/api/devices",
            new CreateDeviceRequest("XPS", "Dell", DeviceState.Available));

        var response = await _client.GetAsync("/api/devices?state=1"); // 1 = InUse

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<DeviceResponse>>();
        body.Should().OnlyContain(d => d.State == DeviceState.InUse);
    }

    // ── GET /api/devices/{id} ────────────────────────────────────────────────

    [Fact]
    public async Task GetById_WhenExists_ShouldReturn200()
    {
        var created = await CreateDeviceAsync("Router", "Cisco", DeviceState.Available);

        var response = await _client.GetAsync($"/api/devices/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DeviceResponse>();
        body!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturn404()
    {
        var response = await _client.GetAsync($"/api/devices/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── PUT /api/devices/{id} ────────────────────────────────────────────────

    [Fact]
    public async Task Update_WhenAvailable_ShouldReturn200WithUpdatedDevice()
    {
        var created = await CreateDeviceAsync("Old", "OldBrand", DeviceState.Available);
        var updateRequest = new UpdateDeviceRequest("New", "NewBrand", DeviceState.Inactive);

        var response = await _client.PutAsJsonAsync($"/api/devices/{created.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DeviceResponse>();
        body!.Name.Should().Be("New");
        body.CreationTime.Should().Be(created.CreationTime); // CreationTime never changes
    }

    [Fact]
    public async Task Update_WhenInUse_ChangingNameOrBrand_ShouldReturn409()
    {
        var created = await CreateDeviceAsync("Server", "HP", DeviceState.InUse);
        var updateRequest = new UpdateDeviceRequest("New Server", "HP", DeviceState.InUse);

        var response = await _client.PutAsJsonAsync($"/api/devices/{created.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // ── PATCH /api/devices/{id} ──────────────────────────────────────────────

    [Fact]
    public async Task Patch_WhenInUse_ChangingOnlyState_ShouldReturn200()
    {
        var created = await CreateDeviceAsync("Printer", "Canon", DeviceState.InUse);
        var patchRequest = new PatchDeviceRequest(null, null, DeviceState.Available);

        var response = await _client.PatchAsJsonAsync($"/api/devices/{created.Id}", patchRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DeviceResponse>();
        body!.State.Should().Be(DeviceState.Available);
        body.Name.Should().Be("Printer"); // unchanged
    }

    // ── DELETE /api/devices/{id} ─────────────────────────────────────────────

    [Fact]
    public async Task Delete_WhenAvailable_ShouldReturn204()
    {
        var created = await CreateDeviceAsync("Monitor", "LG", DeviceState.Available);

        var response = await _client.DeleteAsync($"/api/devices/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Confirm it's gone
        var getResponse = await _client.GetAsync($"/api/devices/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenInUse_ShouldReturn409()
    {
        var created = await CreateDeviceAsync("Switch", "Cisco", DeviceState.InUse);

        var response = await _client.DeleteAsync($"/api/devices/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturn404()
    {
        var response = await _client.DeleteAsync($"/api/devices/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── Health Check ─────────────────────────────────────────────────────────

    [Fact]
    public async Task HealthCheck_ShouldReturn200()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<DeviceResponse> CreateDeviceAsync(string name, string brand, DeviceState state)
    {
        var response = await _client.PostAsJsonAsync("/api/devices",
            new CreateDeviceRequest(name, brand, state));
        return (await response.Content.ReadFromJsonAsync<DeviceResponse>())!;
    }
}
