using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Repositories;
using FantasyChas_Backend.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyChas_Backend_Tests.ActiveStoryTests
{
    [TestClass]
    public class ActiveStoryServiceTests
    {
        private Mock<IActiveStoryRepository> _mockRepository;
        private IActiveStoryService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IActiveStoryRepository>();
            _service = new ActiveStoryService(_mockRepository.Object);
        }

        [TestMethod]
        public async Task CreateStoryAsync_ValidInput_ReturnsStoryId()
        {
            // Arrange
            var user = new IdentityUser { Id = "user123" };
            var storyDto = new ActiveStoryDto { Name = "Test Story", BasePrompt = "Test Prompt" };
            var newStory = new ActiveStory { Id = 1, User = user, Name = storyDto.Name, BasePrompt = storyDto.BasePrompt };

            _mockRepository
                .Setup(repo => repo.AddStoryAsync(It.IsAny<ActiveStory>()))
                .Returns(Task.CompletedTask)
                .Callback<ActiveStory>(story =>
                {
                    story.Id = newStory.Id;
                    story.User = newStory.User;
                    story.Name = newStory.Name;
                    story.BasePrompt = newStory.BasePrompt;
                });

            // Act
            var result = await _service.CreateStoryAsync(user, storyDto);

            // Assert
            Assert.AreEqual(1, result);
            _mockRepository.Verify(repo => repo.AddStoryAsync(It.Is<ActiveStory>(s =>
                s.User == user && s.Name == storyDto.Name && s.BasePrompt == storyDto.BasePrompt && s.Id == newStory.Id)), Times.Once);
        }


        [TestMethod]
        public async Task CreateStoryAsync_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var user = new IdentityUser { Id = "user123" };
            var storyDto = new ActiveStoryDto { Name = "Test Story", BasePrompt = "Test Prompt" };

            _mockRepository
                .Setup(repo => repo.AddStoryAsync(It.IsAny<ActiveStory>()))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _service.CreateStoryAsync(user, storyDto));
            Assert.AreEqual("Failed to create Story", exception.Message);
        }
    }
}
