using System.Text;
using System.Text.Json;
using FoodWasteReduction.Core.Enums;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FoodWasteReduction.Tests.Controllers.Web
{
    public abstract class ControllerTestBase
    {
        protected Mock<IHttpContextAccessor> HttpContextAccessor;
        protected DefaultHttpContext HttpContext;
        protected Mock<ISession> Session;

        protected ControllerTestBase()
        {
            Session = new Mock<ISession>();
            HttpContext = new DefaultHttpContext { Session = Session.Object };
            HttpContextAccessor = new Mock<IHttpContextAccessor>();
            HttpContextAccessor.Setup(x => x.HttpContext).Returns(HttpContext);
        }

        protected void SetupUserSession(
            string userId,
            string role,
            Location? location = null,
            City? studyCity = null,
            DateTime? dateOfBirth = null
        )
        {
            var userData = new
            {
                Id = userId,
                Role = role,
                Location = location.HasValue ? (int?)location.Value : null,
                StudyCity = studyCity.HasValue ? (int?)studyCity.Value : null,
                DateOfBirth = dateOfBirth?.ToString("O"),
            };

            var sessionData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(userData));
            Session.Setup(s => s.TryGetValue("UserData", out sessionData)).Returns(true);
        }
    }
}
