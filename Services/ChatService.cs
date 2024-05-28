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
        Task AddFirstChatObjectToActiveStoryAsync(int activeStoryId);
        Task<List<ChatHistoryViewModel>> GetChatHistoryPaginatedAsync(int activeStoryId, int amountPerPage, int pageNumber);
        Task<StoryChatResponseViewModel> SendToChatServiceAsync(StoryChatPromptDto chatPromptObject);
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

        // return ViewModel
        public async Task<List<ChatHistoryViewModel>> GetChatHistoryPaginatedAsync(int activeStoryId, int amountPerPage, int pageNumber)
        {
            var result = await _chatRepository.GetPaginatedChatHistoryAsync(activeStoryId, amountPerPage, pageNumber);

            if(result is null)
            {
                throw new Exception("No more messages");
            }

            List<ChatHistoryViewModel> resultViewModelList = result
                .Select(result => new ChatHistoryViewModel()
                {
                    Id = result.Id,
                    Message = result.Message,
                    CharacterName = result.Character == null ? null : result.Character.Name,
                    Timestamp = result.Timestamp
                })
                .ToList();

            return resultViewModelList;
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
                                new ChatMessage(ChatMessageRole.System, "Du är Dungeon Master för spelet Stayin' Alive som är Dungeons & Masters inspirerad." +
                                "Historien som spelaren spelar, tillhör spelet Stayin' Alive kommer att ges till dig i ett prompt-meddelande." +
                                "En grundvisualisering för dig: Spelet är helt verklighetsbaserad och utspelar en kris-situation baserad på 'MSB's Broschyr om krisen eller kriget kommer.'" +
                                "Ditt jobb: Berätta vidare på histora-prompten som du kommer få in, med dom satta parametrarna i detta system-meddelande för dig. " +
                                "Parametrar: Spelaren har en karaktär som du kommer integrera med och du ska skapa och skräddarsy en upplevelse med för spelarens karaktär. " +
                                "Parametrar fortsättning: Skapa en beskrivande scen för spelaren baserad på tidigare information. Beskriv hur krisen påverkar karaktärens omedelbara situation" +
                                "Paramterar fortsättning: Presentera 3 verkliga valmöjligheter som ger en lärorik insikt för spelaren hur man ska ta sig i just den situationen om det skulle hända på riktigt." +
                                "Parametrar fortsättning: Tärnkastning existerar inte i detta spel, du ska skräddarsy en upplevelse på spelarens svar på dina genererade scenarion. " +
                                "Parametrar fortsättning: Du får endast använda människa som ras för karaktärer som dyker upp i olika scenarion." +
                                "Parametrar fortsättning: Spelaren kan möta på andra människor, som vill gott eller ont, det väljer du att beskriva men det ska passa pågående scenario. " +
                                "Parametrar fortsättning: Människorna som spelaren kommer stöta på under spelets gång kommer ha yrke istället för klasser.  " +
                                "Parametrar fortsättning: Magi existerar inte utan detta är totalt verklighetsbaserad utspelat i en kris-situation." +
                                "Parametrar fortsättning: Få med möjliga konsekvenser baserad på möjliga val och händelseförlopp och scenario som spelaren har framför sig." +
                                "Parametrar fortsättning: Varje val bör leda till olika fördelar eller konsekvenser och ytterligare utveckling av historien." +
                                "Parametrar fortsättning: Gör ALLTID en väl beskrivande story och en GEDIGEN Upplevelse för spelaren som spelar detta katastrof-scenario baserad text-spel" +
                                "Parametrar fortsättning: Avsluta alltid dina svar med att fråga vad användaren vill göra härnäst och uppmana till att skriva ett eget alternativ som du bygger på." +
                                "Parametrar fortsättning: Ta med allt i detta system-meddelande för att ge spelaren den bästa upplevelsen baserad på MSB's broschyr." +
                                "Parametrar fortsättning: Målet är att skapa en interaktiv och engagerande upplevelse där spelaren kan se världen och valen genom text och måste göra strategiska beslut för att överleva." +

                                "Bas-story: [Karaktärens namn och beskrivning] befinner sig i Sverige där samhället står inför en oväntad kris - " +
                                "Baserad på MSB's broschyr 'Om krisen eller kriget kommer'. " +
                                "Spelet börjar med att spelaren vaknar efter en kortare tupplur" +
                                "Välj en plats eller stad/stadsdel där Spelaren börjar, välj en plats som passar karaktären i helhet, " +
                                "Dygnstid: Välj utspelad start-tid mellan att det är morgon, mitt på dagen, kväll eller mitt i natten" +
                                "Välj och anpassa ett av följande kris-scenarier baserat på karaktärens bakgrund för bästa spel-upplevelse: naturkatastrofer, " +
                                "IT-attacker, terrorattentat, sabotage av infrastruktur, militära angrepp, brist på livsmedel och vatten, utbrott av livshotande-sjukdomar, pandemier eller världskrig." +
                                "Efter att tagit in all information om Bas-Story: Nu ska du skapa en startbeskrivning som är skräddarsydd efter allt du fått veta om alla krav satta för spelarens upplevelse. ") ,


  ///*Historia till frontend */   "\r\nHistoria: Du ska utgå från broschyren \"Om krisen eller kriget kommer\" när du fortsätter den här historian. " + /*------------*/
  ///*Avvaktar med att ta bort */ "Få med sådant som är viktigt att tänka på i ett riktigt överlevnadsscenario. " +
  ///*Tills det fungerar */       "Här är bakgrunden som du ska bygga vidare på: Det har varit oroliga tider i världen de senaste åren. " +
  ///*I frontend */               "Flertalet stora konflikter har blossat upp runtom i världen. Klimatkrisen har lett till att vädret är extremt ostabilt. " +
  //                              "Detta i kombination med interna stridigheter inom olika länders riksdagar har lett till en " +
  //                              "stor ekonomisk instabilitet i de allra flesta länder. "),

                                // Originalet:
                                //"Du är en dungeon master för spelet Dungeons and Dragons. Du kommer att få en prompt med en historia som du ska fortsätta på och kommer att interagera med en karaktär som jag kommer att använda i den värld du bygger upp. Du får endast använda människa som ras för karaktärer som dyker upp. De måste ha klasser som passar in i den verkliga världen. Magi finns inte i detta scenario. Istället för att slå tärning för att avgöra hur saker går, räcker det att du bestämmer hur det går.\r\nHistoria: Du ska utgå från broschyren "Om krisen eller kriget kommer" när du fortsätter den här historian. Få med sådant som är viktigt att tänka på i ett riktigt överlevnadsscenario. Här är bakgrunden som du ska bygga vidare på: Det har varit oroliga tider i världen de senaste åren. Flertalet stora konflikter har blossat upp runtom i världen. Klimatkrisen har lett till att vädret är extremt ostabilt. Detta i kombination med interna stridigheter inom olika länders riksdagar har lett till en stor ekonomisk instabilitet i de allra flesta länder. Avsluta alltid dina svar med att fråga vad användaren vill göra härnäst."),

                                new ChatMessage(ChatMessageRole.User, 
                                $"Min karaktär: {serializedObject}. Skapa en start på en historia åt mig nu."),
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
                    // Hittade en blogg som påstår att det är en dum idé att köra "fire and forget". Återkommer till detta.
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
