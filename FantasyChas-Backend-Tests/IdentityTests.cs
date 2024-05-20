using FantasyChas_Backend.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FantasyChas_Backend_Tests
{
    [TestClass]
    public class IdentityTests
    {
        //[TestMethod]
        //public void PostUserCredentials_RegisterUser()
        //{
        //    // Arrange
        //    DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase("PostUserCredentials_RegisterUser")
        //        .Options;

        //var mockHandler = new Mock<HttpMessageHandler>();

        //mockHandler
        //    .Protected()
        //        .Setup<Task<HttpResponseMessage>>(
        //            "SendAsync",
        //            ItExpr.IsAny<HttpRequestMessage>(),
        //            ItExpr.IsAny<CancellationToken>()
        //        )
        //        .ReturnsAsync(new HttpResponseMessage()
        //{
        //    StatusCode = HttpStatusCode.OK
        //        });

        //    using (ApplicationDbContext context = new ApplicationDbContext(options))
        //    {
        //        string email = "test@test.com";
        //        string password = "testPassword";

        //        HttpClient mockClient = new HttpClient(mockHandler.Object);

                
        //    }
        //}

        [TestMethod]
        public async Task PostUserCredentials_RegisterUserChatGPTVersion() 
        {
            var options = TestHelper.GetInMemoryDbContextOptions();
            using (var context = new TestDbContext(options))
            {
                var userManager = TestHelper.GetUserManager(context);
                var user = new IdentityUser { UserName = "testuser", Email = "testuser@example.com" };
                var result = await userManager.CreateAsync(user, "Test@123");

                Assert.IsTrue(result.Succeeded);
                Assert.IsNotNull(await userManager.FindByNameAsync("testuser"));
            }
        }
    }
}
