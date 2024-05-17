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
        Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject);
        Task AddChatHistory(string message, int chatId, int characterId);
    }

    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IOpenAiService _openAiService;
        private readonly ICharacterService _characterService;

        private readonly int _maxTokensAllowed = 16000;

        public ChatService(IChatRepository chatRepository, ICharacterRepository characterRepository, IOpenAiService openAiService, ICharacterService characterService)
        {
            _chatRepository = chatRepository;
            _characterRepository = characterRepository;
            _openAiService = openAiService;
            _characterService = characterService;
        }

        public async Task AddChatHistory(string message, int chatId, int characterId)
        {
            try
            {
                Character? character = new Character();

                if (characterId > 0)
                {
                    character = await _characterRepository.GetCharacterByIdAsync(characterId);
                }

                Chat? chat = await _chatRepository.GetChatByIdAsync(chatId);

                var historyLine = new ChatHistory
                {
                    Character = character,
                    Chat = chat,
                    Message = message,
                    Timestamp = DateTime.Now
                };

                await _chatRepository.SaveChatHistoryMessageInDatabase(historyLine);
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject)
        {
            try
            {
                // Skicka in karaktärsid till repo för att hämta history och senaste ChatSummary
                List<ChatHistory>? history = await _chatRepository.GetChatHistory(chatPromptObject.CharacterId);
                Chat? chat = await _chatRepository.GetChat(chatPromptObject.CharacterId);

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
                await AddChatHistory(chatPromptObject.Message, chat.Id, chatPromptObject.CharacterId);
                await AddChatHistory(result.Message, chat.Id, 0);


                if (response.Usage.TotalTokens > _maxTokensAllowed)
                {
                    // Lägg till metod för att skapa ett nytt ChattObjekt och länka den med ActiveStory.
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
