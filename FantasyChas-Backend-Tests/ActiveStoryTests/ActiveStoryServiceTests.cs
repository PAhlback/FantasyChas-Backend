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
    //[TestClass]
    //public class ActiveStoryServiceTests
    //{
    //    private Mock<IActiveStoryRepository> _mockRepository;
    //    private IActiveStoryService _service;

    //    [TestInitialize]
    //    public void Setup()
    //    {
    //        _mockRepository = new Mock<IActiveStoryRepository>();
    //        _service = new ActiveStoryService(_mockRepository.Object);
    //    }

    //    [TestMethod]
    //    public async Task CreateStoryAsync_Success()
    //    {
    //        // Arrange
    //        var user = new IdentityUser { Id = "testUserId" };
    //        var storyDto = new ActiveStoryDto { Name = "Test Story", BasePrompt = "Test Prompt"};
    //        var newStory = new ActiveStory { Id = 1, User = user, Name = storyDto.Name, BasePrompt = storyDto.BasePrompt };

    //        _mockRepository.Setup(repo => repo.AddStoryAsync(It.IsAny<ActiveStory>()));
    //            //.Callback<ActiveStory>(story => story.Id = 1)
    //            //.Returns(Task.CompletedTask);

    //        // Act
    //        var result = await _service.CreateStoryAsync(user, storyDto);

    //        // Assert
    //        Assert.AreEqual(1, result);
    //        _mockRepository.Verify(repo => repo.AddStoryAsync(It.IsAny<ActiveStory>()), Times.Once);
    //    }


    //    [TestMethod]
    //    public async Task CreateStoryAsync_Failure()
    //    {
    //        // Arrange
    //        var user = new IdentityUser { Id = "testUserId" };
    //        var storyDto = new ActiveStoryDto { Name = "Test Story", BasePrompt = "Test Prompt"};

    //        _mockRepository.Setup(repo => repo.AddStoryAsync(It.IsAny<ActiveStory>())).ThrowsAsync(new Exception("Database failure"));

    //        // Act & Assert
    //        await Assert.ThrowsExceptionAsync<Exception>(() => _service.CreateStoryAsync(user, storyDto));
    //    }
    //}
}
