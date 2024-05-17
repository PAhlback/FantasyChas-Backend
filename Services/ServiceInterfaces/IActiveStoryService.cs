using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace FantasyChas_Backend.Services.ServiceInterfaces
{
    public interface IActiveStoryService
    {
        Task<int> CreateStoryAsync(IdentityUser user, ActiveStoryDto storyDto);
    }
}
