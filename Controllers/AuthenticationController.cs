using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoboRecords.Models;

namespace RoboRecords.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private SignInManager<RoboUser> _signInManager;

        public AuthenticationController(SignInManager<RoboUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().Wait();
            return Ok("Succesfully signed out");
        }
    }
}