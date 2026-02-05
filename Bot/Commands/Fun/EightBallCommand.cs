using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Discord;
using Discord.Interactions;

namespace Bot.Commands.Fun;

public class EightballCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EightballCommand(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SlashCommand("8ball", "Ask the all-knowing 8ball a question")]
    public async Task ExecuteAsync([Summary("question", "What do you want to ask the 8ball?")] string question)
    {
        await DeferAsync();
        
        // Make API request because I don't want to write 100 possible responses and random picking logic lol
        var httpClient = _httpClientFactory.CreateClient();
        string encodedQuestion = Uri.EscapeDataString(question);
        string jsonResponse = await httpClient.GetStringAsync($"https://eightballapi.com/api/biased/?question={encodedQuestion}");

        var jsonNode = JsonNode.Parse(jsonResponse);
        string answer = jsonNode?["reading"]?.GetValue<string>() ?? "The 8ball is silent.";
        
        // Construct reply embed
        var embed = new EmbedBuilder()
            .WithTitle("The magic all-knowing 8ball")
            .WithDescription($"❓ Question: {question}\n🎱 Answer: {answer}")
            .WithColor(Color.Purple)
            .Build();

        // await ModifyOriginalResponseAsync(props => props.Embed = embed);
        await FollowupAsync(embed: embed);
    }
}