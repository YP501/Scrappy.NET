using System.Text.Json.Nodes;
using Discord;
using Discord.Interactions;

namespace Bot.Commands.Fun;

public class DadJokeCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DadJokeCommand(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SlashCommand("dadjoke", "Responds with a dad-flavored joke")]
    public async Task ExecuteAsync()
    {
        await DeferAsync();

        var httpClient = _httpClientFactory.CreateClient();
        
        // Add header so API knows we want JSON and not the entire HTML webpage lol
        httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

        string jsonResponse = await httpClient.GetStringAsync("https://icanhazdadjoke.com/");
        var jsonNode = JsonNode.Parse(jsonResponse);
        string dadJoke = jsonNode?["joke"]?.GetValue<string>() ?? "Can't think of a joke right now sorry -dad";
        
        await FollowupAsync(dadJoke);
    }
}