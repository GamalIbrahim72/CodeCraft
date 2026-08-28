using CodeCraft.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodeCraft.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected IActionResult SuccessResponse<T>(T? data = default, string message = "Success")
    {
        return Ok(new ApiResponse<T?>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }
}
