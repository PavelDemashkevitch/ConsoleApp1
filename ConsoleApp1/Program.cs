
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Data;

class Program
{
    // Введите сюда ваш токен
    private static readonly string Token = "5520749666:AAHHEywx6Tfa1X2INYXEut1JEUCMYJbVi14";
    private static readonly TelegramBotClient BotClient = new(Token);
    private static Task? taskMy;
    internal static async Task Main()
    {
        using CancellationTokenSource cts = new();

        // Настройка обработчиков
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        BotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await BotClient.GetMe();
        Console.WriteLine($"Бот {me.Username} запущен.");

        // Ожидаем завершения
        Console.ReadLine();

        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        Console.WriteLine($"Получено сообщение: {messageText}");

        if (messageText.Equals("/start", StringComparison.InvariantCultureIgnoreCase))
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: "Привет! Я ваш Telegram-бот.",
                cancellationToken: cancellationToken
            );

            taskMy ??= RunPeriodicAsync(() => Method(cancellationToken), TimeSpan.FromSeconds(10), cancellationToken);

        }
        else
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Вы написали: {messageText}",
                cancellationToken: cancellationToken
            );
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Произошла ошибка: {exception.Message}");
        return Task.CompletedTask;
    }

    private static async Task RunPeriodicAsync(Func<Task> task, TimeSpan interval, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await task();
            await Task.Delay(interval, cancellationToken);
        }
    }



    internal static async Task Method(CancellationToken cancellationToken)
    {
        // Ваш API-ключ
        string apiKey = "YourApiKeyToken";
        // Адрес аккаунта
        string address = "0x70F657164e5b75689b64B7fd1fA275F334f28e18";

        // Формируем URL для запроса
        string url = $"https://api.bscscan.com/api?module=account&action=balance&address={address}&apikey={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Отправляем запрос и получаем ответ
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Читаем содержимое ответа
                string responseBody = await response.Content.ReadAsStringAsync();

                // Выводим ответ
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка запроса: {e.Message}");
            }
        }

    }


}
