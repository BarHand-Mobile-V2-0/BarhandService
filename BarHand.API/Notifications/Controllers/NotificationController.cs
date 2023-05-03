using AutoMapper;
using BarHand.API.Suppliers.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using BarHand.API.Notifications.Domain.Models;
using BarHand.API.Notifications.Domain.Services;
using BarHand.API.Notifications.Resources;
using BarHand.API.Suppliers.Services;
using BarHand.API.Suppliers.Domain.Models;
using BarHand.API.Suppliers.Resources;
using BarHand.API.Shared.Extensions;
using BarHand.API.Inventory.Domain.Models;
using BarHand.API.Inventory.Resources;

namespace BarHand.API.Notifications.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class NotificationController : ControllerBase
{

    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public NotificationController(INotificationService supplierService, IMapper mapper)
    {
        _notificationService = supplierService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<NotificationResource>> GetAllAsync()
    {
        var notifications = await _notificationService.ListAsync();
        var resources = _mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationResource>>(notifications);
        return resources;
    }


    [HttpGet("{id}/{type}")]
    public async Task<IEnumerable<NotificationResource>> GetAlLById(long id, string type )
    {
        var notifications = await _notificationService.ListByTypeIdAsync(id, type);
        var resources = _mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationResource>>(notifications);
        return resources;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] SaveNotificationResource resource)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.GetErrorMessages());

        var product = _mapper.Map<SaveNotificationResource, Notification>(resource);

        var result = await _notificationService.SaveAsync(product);

        if (!result.Success)
            return BadRequest(result.Message);

        var supplierResource = _mapper.Map<Notification, NotificationResource>(result.Resource);
        return Created(nameof(PostAsync), supplierResource);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _notificationService.DeleteAsync(id);

        if (!result.Success)
            return BadRequest(result.Message);

        var productResource = _mapper.Map<Notification, NotificationResource>(result.Resource);

        return Ok(productResource);
    }


}

