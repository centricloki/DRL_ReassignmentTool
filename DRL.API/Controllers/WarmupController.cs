using DRL.Core.Interface;
using DRL.Model.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace DRL.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarmupController : ControllerBase
    {
        private readonly DRLNewContext _context;
        private readonly IAuthenticationService _authService;

        public WarmupController(DRLNewContext context, IAuthenticationService authService)
        {
            _context = context;
            _authService = authService;
        }
        /// <summary>
        /// Pre-warm the application (call this after deployment)
        /// </summary>
        [HttpGet("initialize")]
        [AllowAnonymous]
        public async Task<IActionResult> Initialize()
        {
            try
            {
                // Warm up database connection
                await _context.Database.CanConnectAsync();

                // Warm up EF Core model
                var count = await _context.UserMaster.CountAsync();

                // Warm up cache service
                //var cacheKey = "warmup_test";
                //_authService.Authenticate(null); // Just to trigger initialization

                return Ok(new
                {
                    success = true,
                    message = "Application warmed up successfully",
                    timestamp = System.DateTime.Now
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Warmup failed: {ex.Message}"
                });
            }
        }
    }
}