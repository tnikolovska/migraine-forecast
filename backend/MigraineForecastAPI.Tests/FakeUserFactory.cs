using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MigraineForecastAPI.Tests
{
    public static class FakeUserFactory
    {
        public static ClaimsPrincipal AdminUser() => Create("Admin");
        public static ClaimsPrincipal NormalUser() => Create("User");

        private static ClaimsPrincipal Create(string role)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, "test"),
            new Claim(ClaimTypes.Role, role)
        }, "Fake"));
        }
    }
}
