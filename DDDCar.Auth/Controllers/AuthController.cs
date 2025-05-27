using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDDCar.Auth.Controllers;

[ApiController]
[Route("api/check")]
public class AuthController : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "TestRole")]
    public async Task<IActionResult> Check() => Ok("Hello World!");
}