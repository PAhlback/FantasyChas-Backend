using FantasyChas_Backend.Models;
using FantasyChas_Backend.Models.DTOs;
using FantasyChas_Backend.Models.ViewModels;
using FantasyChas_Backend.Repositories;
using FantasyChas_Backend.Services.ServiceInterfaces;
using OpenAI_API.Chat;

namespace FantasyChas_Backend.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IOpenAiService _openAiService;

        public ChatService(IChatRepository chatRepository, ICharacterRepository characterRepository, IOpenAiService openAiService)
        {
            _chatRepository = chatRepository;
            _characterRepository = characterRepository;
            _openAiService = openAiService;
        }

        public async Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject)
        {
            try
            {
                // Skicka in karaktärsid till repo för att hämta history
                List<ChatHistory>? history = await _chatRepository.GetChatHistory(chatPromptObject.CharacterId);
                string? summary = await _chatRepository.GetChatSummary(chatPromptObject.CharacterId);

                // Ta bort ID och ImageURL innan vi skickar vidare till ChatGPT
                CharacterViewModel? character = await _characterRepository.GetCharacterByIdAsync(chatPromptObject.CharacterId);
                string? serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(character);

                var messages = new List<ChatMessage>
                            {
                                new ChatMessage(ChatMessageRole.System, "Du är en dungeon master för spelet Dungeons and Dragons. Du kommer att få en prompt med en historia som du ska fortsätta på och kommer att interagera med en karaktär som jag kommer att använda i den värld du bygger upp. Du får endast använda människa som ras för karaktärer som dyker upp. De måste ha klasser som passar in i den verkliga världen. Magi finns inte i detta scenario. Istället för att slå tärning för att avgöra hur saker går, räcker det att du bestämmer hur det går.\r\nHistoria: Du ska utgå från broschyren \"Om krisen eller kriget kommer\" när du fortsätter den här historian. Få med sådant som är viktigt att tänka på i ett riktigt överlevnadsscenario. Här är bakgrunden som du ska bygga vidare på: Det har varit oroliga tider i världen de senaste åren. Flertalet stora konflikter har blossat upp runtom i världen. Klimatkrisen har lett till att vädret är extremt ostabilt. Detta i kombination med interna stridigheter inom olika länders riksdagar har lett till en stor ekonomisk instabilitet i de allra flesta länder."),
                                new ChatMessage(ChatMessageRole.User, $"Min karaktär: {serializedObject}"),
                                new ChatMessage(ChatMessageRole.Assistant, "Du väcks tidigt på morgonen av att sirener ljuder över området. Vad gör du?"),
                            };
                // - sen historiken från ChatHistories
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
                // - sen user query
                messages.Add(new ChatMessage(ChatMessageRole.User, chatPromptObject.Message));

                var response = await _openAiService.GetChatGPTResultAsync(messages);

                Console.WriteLine(response);

                StoryChatResponseViewModel result = new StoryChatResponseViewModel()
                {
                    Message = response.Choices[0].Message.TextContent
                };

                return result;
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
