using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using OpenAI_API.Chat;
using System.Diagnostics.CodeAnalysis;

namespace FantasyChas_Backend.Services
{
    public interface IChatService
    {
        Task AddChatHistoryAsync(string message, int chatId, int characterId);
        Task AddChatObjectWithSummaryAsync(int activeStoryId, List<ChatMessage> previousChatHistory, string previousSummary);
        Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject);
        Task AddFirstChatObjectToActiveStoryAsync(int activeStoryId);
    }

    public class ChatService : IChatService
    {
        private readonly IActiveStoryService _activeStoryService;
        private readonly IChatRepository _chatRepository;
        private readonly ICharacterService _characterService;
        private readonly ICharacterRepository _characterRepository;
        private readonly IOpenAiService _openAiService;
        
        private readonly int _maxTokensAllowed = 16000;

        public ChatService(IChatRepository chatRepository, 
            ICharacterRepository characterRepository, 
            IOpenAiService openAiService, 
            ICharacterService characterService,
            IActiveStoryService activeStoryService
            )
        {
            _chatRepository = chatRepository;
            _characterRepository = characterRepository;
            _openAiService = openAiService;
            _characterService = characterService;
            _activeStoryService = activeStoryService;
        }

        public async Task AddChatHistoryAsync(string message, int chatId, int characterId)
        {
            try
            {
                string stringCharacterId = characterId.ToString();

                Character? character = new Character();

                if (stringCharacterId != "0")
                {
                    int actualCharacterId = int.Parse(stringCharacterId);
                    character = await _characterRepository.GetCharacterByIdAsync(actualCharacterId);
                }

                Chat? chat = await _chatRepository.GetChatByIdAsync(chatId);

                var historyLine = new ChatHistory
                {
                    Chat = chat,
                    Message = message,
                    Timestamp = DateTime.Now
                };

                if(character.Name != null)
                {
                    historyLine.Character = character;
                }

                await _chatRepository.SaveChatHistoryMessageInDatabaseAsync(historyLine);
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task AddChatObjectWithSummaryAsync(int activeStoryId, List<ChatMessage> previousChatHistory, string previousSummary)
        {
            try
            {
                // Här bygger vi ett nytt systemmeddelande, samt lägger in föregående summary. Dessa kommer direkt från metoden 
                // som bygger anropet till ChatGPT. 
                // Vi tar bort den första posten ur previousChatMessages i och med att det är systemprompten från föregående metod.
                previousChatHistory.RemoveAt(0);
                var messages = new List<ChatMessage>
                {
                    new ChatMessage(ChatMessageRole.System, "Skapa en sammanfattning av den här historien."),
                    new ChatMessage(ChatMessageRole.User, previousSummary)
                };

                // AddRange lägger till alla(?) items från listan vi skickar in.
                messages.AddRange(previousChatHistory);

                var response = await _openAiService.GetChatGPTResultAsync(messages);

                string? resultNewSummary = response.Choices[0].Message.TextContent;

                if(resultNewSummary == null)
                {
                    throw new Exception("Failed to create new chat object. Result was null.");
                }

                await _chatRepository.AddChatAsync(new Chat()
                {
                    // Behöver hämta hela activeStory - räcker inte bara med id.
                    ActiveStory = await _activeStoryService.GetActiveStoryByIdAsync(activeStoryId),
                    ChatSummary = resultNewSummary
                });

                // return behövs egentligen inte
                return;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddFirstChatObjectToActiveStoryAsync(int activeStoryId)
        {
            try
            {
                ActiveStory story = await _activeStoryService.GetActiveStoryByIdAsync(activeStoryId);
                Chat newChat = new Chat
                {
                    ChatSummary = story.BasePrompt,
                    ActiveStory = story
                };

                await _chatRepository.AddChatAsync(newChat);
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject)
        {
            try
            {
                // Skicka in karaktärsid till repo för att hämta history och senaste ChatSummary
                List<ChatHistory>? history = await _chatRepository.GetChatHistoryAsync(chatPromptObject.CharacterId);
                Chat? chat = await _chatRepository.GetChatAsync(chatPromptObject.CharacterId);

                // ToDo: Ta bort ID och ImageURL innan vi skickar vidare till ChatGPT
                Character? character = await _characterRepository.GetCharacterByIdAsync(chatPromptObject.CharacterId);
                CharacterViewModel? characterViewModel = await _characterService.ConvertCharacterToViewModelAsync(character);
                string? serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(characterViewModel);

                var messages = new List<ChatMessage>
                            {
                                new ChatMessage(ChatMessageRole.System, "Du är en dungeon master för spelet Dungeons and Dragons. Du kommer att få en prompt med en historia som du ska fortsätta på och kommer att interagera med en karaktär som jag kommer att använda i den värld du bygger upp. Du får endast använda människa som ras för karaktärer som dyker upp. De måste ha klasser som passar in i den verkliga världen. Magi finns inte i detta scenario. Istället för att slå tärning för att avgöra hur saker går, räcker det att du bestämmer hur det går.\r\nHistoria: Du ska utgå från broschyren \"Om krisen eller kriget kommer\" när du fortsätter den här historian. Få med sådant som är viktigt att tänka på i ett riktigt överlevnadsscenario. Här är bakgrunden som du ska bygga vidare på: Det har varit oroliga tider i världen de senaste åren. Flertalet stora konflikter har blossat upp runtom i världen. Klimatkrisen har lett till att vädret är extremt ostabilt. Detta i kombination med interna stridigheter inom olika länders riksdagar har lett till en stor ekonomisk instabilitet i de allra flesta länder. Avsluta alltid dina svar med att fråga vad användaren vill göra härnäst."),
                                new ChatMessage(ChatMessageRole.User, $"Min karaktär: {serializedObject}. Skapa en start på en historia åt mig nu."),
                                //new ChatMessage(ChatMessageRole.Assistant, "Du väcks tidigt på morgonen av att sirener ljuder över området. Vad gör du?"),
                            };
                // Lägg sedan till historiken från ChatHistories
                foreach (var chatRow in history)
                {
                    if (chatRow.Character == null)
                    {
                        messages.Add(new ChatMessage(ChatMessageRole.Assistant, chatRow.Message));
                    }
                    else
                    {
                        messages.Add(new ChatMessage(ChatMessageRole.User, chatRow.Message));
                    }
                }
                // Lägg sedan till user query
                messages.Add(new ChatMessage(ChatMessageRole.User, chatPromptObject.Message));

                var response = await _openAiService.GetChatGPTResultAsync(messages);

                Console.WriteLine(response);

                StoryChatResponseViewModel result = new StoryChatResponseViewModel()
                {
                    Message = response.Choices[0].Message.TextContent
                };


                // spara i chathistory
                await AddChatHistoryAsync(chatPromptObject.Message, chat.Id, chatPromptObject.CharacterId);
                await AddChatHistoryAsync(result.Message, chat.Id, 0);


                if (response.Usage.TotalTokens > _maxTokensAllowed)
                {
                    // Lägg till metod för att skapa ett nytt ChattObjekt och länka den med ActiveStory.
                    // Kör man utan await borde den köras separat från resten (ingen väntetid för användaren).
                    await AddChatObjectWithSummaryAsync(character.ActiveStory.Id, messages, chat.ChatSummary);
                }

                return result;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
