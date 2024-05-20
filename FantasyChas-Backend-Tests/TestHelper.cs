using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyChas_Backend_Tests
{
    public static class TestHelper
    {
        public static DbContextOptions<TestDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        public static UserManager<IdentityUser> GetUserManager(TestDbContext context)
        {
            var store = new UserStore<IdentityUser>(context);
            var options = new IdentityOptions();
            var idOptions = new OptionsWrapper<IdentityOptions>(options);
            var pwdHasher = new PasswordHasher<IdentityUser>();
            var userValidators = new List<IUserValidator<IdentityUser>> { new UserValidator<IdentityUser>() };
            var validators = new List<IPasswordValidator<IdentityUser>> { new PasswordValidator<IdentityUser>() };
            var lookupNormalizer = new UpperInvariantLookupNormalizer();
            var errors = new IdentityErrorDescriber();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IPasswordHasher<IdentityUser>>(pwdHasher);
            services.AddSingleton<ILookupNormalizer>(lookupNormalizer);
            services.AddSingleton<IdentityErrorDescriber>(errors);
            services.AddSingleton<IUserValidator<IdentityUser>>(new UserValidator<IdentityUser>());
            services.AddSingleton<IPasswordValidator<IdentityUser>>(new PasswordValidator<IdentityUser>());
            services.AddSingleton<UserManager<IdentityUser>>(sp => new UserManager<IdentityUser>(
                store, idOptions, pwdHasher, userValidators, validators, lookupNormalizer, errors, services.BuildServiceProvider(), null));
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetService<UserManager<IdentityUser>>();
        }
    }
}
