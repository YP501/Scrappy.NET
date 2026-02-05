using System.Text.Json.Nodes;
using Discord.Interactions;

namespace Bot.Commands.Fun;

public class InsultCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public  InsultCommand(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SlashCommand("insult", "Get me to insult you (if you dare)")]
    public async Task ExecuteAsync()
    {
        await DeferAsync();
        
        var httpClient = _httpClientFactory.CreateClient();
        string jsonResponse = await httpClient.GetStringAsync("https://evilinsult.com/generate_insult.php?lang=en&type=json");
        
        var jsonNode = JsonNode.Parse(jsonResponse);
        string insult = jsonNode?["insult"]?.GetValue<string>() ?? "sorry i'm to scared to insult you";
        
        await FollowupAsync(insult);
    }
}