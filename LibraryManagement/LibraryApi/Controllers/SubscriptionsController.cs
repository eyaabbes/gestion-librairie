using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService) =>
        _subscriptionService = subscriptionService;

    [HttpGet]
    [Authorize(Policy = "Subscriptions.View")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _subscriptionService.GetAllAsync());
}
