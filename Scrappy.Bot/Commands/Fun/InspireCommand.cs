using Discord;
using Discord.Interactions;

namespace Scrappy.Bot.Commands.Fun;

public class InspireCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public InspireCommand(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [SlashCommand("inspire", "Become wise with an inspirational quote")]
    public async Task ExecuteAsync()
    {
        await DeferAsync();

        var httpClient = _httpClientFactory.CreateClient();
        var imageLink = await httpClient.GetStringAsync("https://inspirobot.me/api?generate=true");
        if (string.IsNullOrEmpty(imageLink))
        {
            await FollowupAsync("Couldn't think of anything smart, sorry :p");
            return;
        }

        using var imageStream = await httpClient.GetStreamAsync(imageLink);
        var imageAttachment = new FileAttachment(imageStream, "wisdom_or_something.jpg");

        await FollowupWithFileAsync(imageAttachment);
    }
}