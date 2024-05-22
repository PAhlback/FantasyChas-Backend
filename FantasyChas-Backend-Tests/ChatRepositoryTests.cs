using FantasyChas_Backend.Data;
using FantasyChas_Backend.Models;
using FantasyChas_Backend.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyChas_Backend_Tests
{
    [TestClass]
    public class ChatRepositoryTests
    {
        [TestMethod]
        public void ChatRepo_SaveNewChatToDatabase()
        {
            // Arrange
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ChatRepo_SaveNewChatToDatabase")
                .Options;

            using (ApplicationDbContext context = new ApplicationDbContext(options))
            {
                ChatRepository chatRepository = new ChatRepository(context);
                // Act?
                ActiveStory story = new ActiveStory() 
                {
                    Name = "test-story",
                    BasePrompt = "test-base-prompt"
                };
                context.ActiveStories.Add(story);

                Chat chat = new Chat()
                {
                    ChatSummary = story.BasePrompt
                };
                chatRepository.AddChatAsync(chat);

                // Assert
                Assert.AreEqual("test-base-prompt", context.Chats.SingleOrDefault().ChatSummary);
                Assert.AreEqual(1, context.Chats.Count());
            }
        }

        //[TestMethod]
        //public async void ChatRepo_GetChatsFromDatabase()
        //{
        //    // Arrange
        //    DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase("ChatRepo_GetChatsFromDatabase")
        //        .Options;

        //    using (ApplicationDbContext context = new ApplicationDbContext(options))
        //    {
        //        ChatRepository chatRepository = new ChatRepository(context);
        //        // Act?
        //        ActiveStory story = new ActiveStory()
        //        {
        //            Name = "test-story",
        //            BasePrompt = "test-base-prompt"
        //        };
        //        context.ActiveStories.Add(story);
        //        context.SaveChanges();

        //        Character character = new Character()
        //        {
        //            Name = "test-name",
        //            Backstory = "test-backstory",
        //            Gender = "test-gender",
        //            Profession = "test-prof",
        //            Species = "test-species"
        //        };
        //        context.Characters.Add(character);

        //        for(int i = 0; i < 5; i++)
        //        {
        //            Chat chat = new Chat() 
        //            {
        //                ChatSummary = i.ToString()
        //            };
        //            context.Chats.Add(chat);
        //            context.SaveChanges();
        //        }

        //        Chat chatFromDb = await chatRepository.GetChatAsync(1);

        //        // Assert
        //        Assert.AreEqual("0", chatFromDb.ChatSummary);
        //        Assert.AreEqual(5, context.Chats.Count());
        //    }
        //}
    }
}
