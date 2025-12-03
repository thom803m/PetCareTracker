using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace PetCareTracker.Tests.Helpers
{
    public static class TestHelpers
    {
        public static ClaimsPrincipal CreateTestUser(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }
    }
}