using Microsoft.AspNetCore.Mvc;

namespace blending2._0_web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {

                dynamic result = new
                {
                    Code = 200,
                    Message = "The Notification api is healthy Ivan Sanchez",
                    Data = new
                    {
                        Status = "Ok",
                        DateTime = DateTime.Now
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    Code = 500,
                    Message = "Error interno del servidor",
                    Details = ex.Message
                };
                return await Task.FromResult(new JsonResult(result));
            }
        }
    }
}