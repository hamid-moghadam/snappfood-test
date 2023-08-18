using Microsoft.AspNetCore.Mvc;

namespace Api.Helpers;

[ApiController]
[Route("api/v1")]
public abstract class ApiControllerBase : ControllerBase
{
}