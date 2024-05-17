using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FantasyChas_Backend.Services
{
    public interface IActiveStoryService
    {
        Task<int> CreateStoryAsync(IdentityUser user, ActiveStoryDto storyDto);
    }
    public class ActiveStoryService : IActiveStoryService
    {
        private readonly IActiveStoryRepository _activeStoryRepository;
        public ActiveStoryService(IActiveStoryRepository storyRepository)
        {
            _activeStoryRepository = storyRepository;
        }

        public async Task<int> CreateStoryAsync(IdentityUser user, ActiveStoryDto storyDto)
        {
            try
            {

                //// Add new ActiveStory
                ActiveStory newStory = new ActiveStory()
                {
                    User = user,
                    Name = storyDto.Name,
                    BasePrompt = storyDto.BasePrompt,
                };

                await _activeStoryRepository.AddStoryAsync(newStory);

                return newStory.Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create Story", ex);
            }
        }
    }
}
