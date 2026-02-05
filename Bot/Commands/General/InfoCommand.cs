using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Discord;
using Discord.Interactions;

namespace Bot.Commands.General;

public class InfoCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("info", "Display some statistics for this bot, for nerds")]
    public async Task ExecuteAsync()
    {
        // Get RAM usage
        var process = Process.GetCurrentProcess();
        double ramUsageMb = process.WorkingSet64 / 1024.0 / 1024.0;
        string ramUsageFormatted = ramUsageMb >= 1000
            ? $"{ramUsageMb / 1024.0:F2} GB"
            : $"{ramUsageMb:F1} MB";

        // Get other information things
        string osPlatform = RuntimeInformation.OSDescription;
        string runtimeVersion = RuntimeInformation.FrameworkDescription;
        int loadedUsersAmount = Context.Client.Guilds.Sum(g => g.Users.Count);
        int latency = Context.Client.Latency;

        // Build epic embed
        var embed = new EmbedBuilder()
            .WithTitle($"{Context.Client.CurrentUser.Username} statistics")
            .WithDescription("Follow development [here](https://github.com/YP501/Scrappy.NET)!")
            .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
            .WithColor(Color.Purple)
            // Fields
            // .AddField("Package info", Format.Code($"OS: {osPlatform}\nFramework: {runtimeVersion}", "txt"))
            .AddField("Runtime Info",
                $"""
                 >>> OS: {osPlatform}
                 Version:  {runtimeVersion}
                 """)
            .AddField("Client Info",
                $"""
                 >>> **RAM Usage:** {ramUsageFormatted}
                 **Latency:** {latency}ms
                 **Users Loaded:** {loadedUsersAmount}
                 """, inline: true)
            .AddField("Bot Info",
                $"""
                 >>> **Uptime:** {GetUptime()}
                 **Version:** {GetCleanBotVersion()}
                 **Developer:** <@513709333494628355>
                 """, inline: true)
            .Build();

        // Send embed
        await RespondAsync(embed: embed);
    }

    private string GetUptime()
    {
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }
    
    private static string GetCleanBotVersion()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "0.0.0-unknown";
        // Remove everything after the +, for example 1.0.0-alpha+516d2b55611b31844e5a893b3c6ec3b692bc2e03
        return version.Split('+')[0];
    }
}