using System.Security.Claims;
using EC_V2.Dtos.ProfileDtos;
using EC_V2.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EC_V2.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;

        }
        [HttpPost("customer")]
        public async Task<IActionResult> CreateCustomerProfile([FromBody] AddCustomerProfileDto dto)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");

            var result = await _profileService.CreateCustomerProfile(userId, dto);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);
        }
        [HttpPost("vendor")]
        public async Task<IActionResult> CreateVendorProfile([FromBody] AddVendorProfileDto dto)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");

            var result = await _profileService.CreateVendorProfile(userId, dto);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);

        }
        [HttpGet("customer")]
        public async Task<IActionResult> GetMyCustomerProfile()
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            var result = await _profileService.GetCustomerProfile(userId);
            if (result.Success)
                return Ok(result.Data);
            return NotFound(result.Error);
        }
        [HttpGet("vendor")]
        public async Task<IActionResult> GetMyVendorProfile()
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            var result = await _profileService.GetVendorProfile(userId);
            if (result.Success)
                return Ok(result.Data);
            return NotFound(result.Error);
        }
        [HttpPut("customer")]
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCustomerProfileDto dto)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            var result = await _profileService.UpdateCustomerProfile(userId, dto);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);
        }
        [HttpPut("vendor")]
        public async Task<IActionResult> UpdateVendorProfile([FromBody] UpdateVendorProfileDto dto)
        {
            var userId = GetUserId(); if (userId == null)
                return Unauthorized("User ID not found in token.");
            var result = await _profileService.UpdateVendorProfile(userId, dto);
            if (result.Success)
                return Ok(result.Data);
            return BadRequest(result.Error);
        }
    }
}