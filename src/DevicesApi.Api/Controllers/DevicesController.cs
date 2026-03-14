using DevicesApi.Application.DTOs;
using DevicesApi.Application.Interfaces;
using DevicesApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevicesApi.Api.Controllers;

[ApiController]
[Route("api/devices")]
[Produces("application/json")]
public sealed class DevicesController(IDeviceService deviceService) : ControllerBase
{
    /// <summary>Creates a new device.</summary>
    /// <response code="201">Device created successfully.</response>
    /// <response code="400">Validation error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Returns all devices. Supports optional filtering by brand and state.</summary>
    /// <response code="200">Returns the list of devices.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DeviceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? brand,
        [FromQuery] DeviceState? state,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.GetAllAsync(brand, state, cancellationToken);
        return Ok(result);
    }

    /// <summary>Returns a single device by ID.</summary>
    /// <response code="200">Returns the device.</response>
    /// <response code="404">Device not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await deviceService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>Fully updates a device (name, brand and state are all required).</summary>
    /// <response code="200">Device updated successfully.</response>
    /// <response code="404">Device not found.</response>
    /// <response code="409">Device is in use — name and brand cannot be changed.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Partially updates a device. Only provided fields are changed.</summary>
    /// <response code="200">Device patched successfully.</response>
    /// <response code="404">Device not found.</response>
    /// <response code="409">Device is in use — name and brand cannot be changed.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Patch(
        Guid id,
        [FromBody] PatchDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.PatchAsync(id, request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Deletes a device. Devices in use cannot be deleted.</summary>
    /// <response code="204">Device deleted successfully.</response>
    /// <response code="404">Device not found.</response>
    /// <response code="409">Device is in use and cannot be deleted.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await deviceService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
