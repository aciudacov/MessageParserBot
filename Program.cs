using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var keepRunning = true;
int? offset = null;
var bot = new TelegramBotClient(config.GetValue<string>("TelegramBotToken")!);

Console.WriteLine("Bot started. Press Ctrl+C to stop");

Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("\nStopping bot...");
    e.Cancel = true;
    keepRunning = false;
};

while (keepRunning)
{
    try
    {
        var dateTime = DateTime.UtcNow;
        Console.WriteLine("Checking messages");
        var updates = await bot.GetUpdates(offset: offset, allowedUpdates: [UpdateType.ChannelPost]);
        foreach (var update in updates)
        {
            if (update.ChannelPost != null && update.ChannelPost.Chat.Id == -1002196781912 && update.ChannelPost.Type == MessageType.Text && update.ChannelPost.Text!.Contains('#'))
            {
                Console.WriteLine($"Sending message to channel: {update.ChannelPost.Text}");
                await bot.SendMessage(chatId: -1002439713369, text: update.ChannelPost.Text, parseMode: ParseMode.Html);
                offset = update.Id + 1;
            }
            if (update.ChannelPost != null && update.ChannelPost.Chat.Id == -1002196781912 && update.ChannelPost.Type == MessageType.Text && update.ChannelPost.Text!.Contains("autostop", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"Sending message to channel: {update.ChannelPost.Text}");
                await bot.SendMessage(chatId: -1002388684326, text: update.ChannelPost.Text, parseMode: ParseMode.Html);
                offset = update.Id + 1;
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex);
    }
    finally
    {
        Thread.Sleep(5000);
    }
}

Console.WriteLine("Bot stopped");