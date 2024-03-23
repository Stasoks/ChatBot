using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ChatBot
{
    class Program()
    {

        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;

        static async Task Main()
        {
            _botClient = new TelegramBotClient("1872154697:AAHIrgkCvmHzAvUkF_AKQ2QwAk06pZvW-Xo");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                },

                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();


            _botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, _receiverOptions, cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} is started!");

            await Task.Delay(-1);

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;
                var chatId = message.Chat.Id;

                if (messageText.Contains("/START"))
                {



                    Message sentmessage = await botClient.SendTextMessageAsync(
                         chatId: chatId,
                         text: "ДЛЯ НАЧАЛА ДАВАЙТЕ ПОЗНАКОМИМСЯ ПОБЛИЖЕ - В ЭТОМ ПОСТЕ Я\r\nРАССКАЖУ ВАМ НЕМНОГО О НАС.\r\nМы - команда разработчиков искусственного интеллекта\r\n\"MATEMATICAS.IO\".\r\nНаш искусственный интеллект будет специализироваться на решении\r\nсложных математических примеров и уравнений, и мы надеемся, что\r\nон будет очень полезен всем в области математики и инженерии, так\r\nкак заменит человеческий труд автоматическими машинными\r\nвычислениями.\r\nИскусственный интеллект постоянно саморазвивается - все те\r\nпримеры, которые вы будете решать в этом боте, пойдут в основу\r\nнашей нейронной сети, и бот будет обучаться и развиваться.\r\nГОТОВЫ ЛИ ВЫ НАЧАТЬ ЗАРАБАТЫВАТЬ ПРЯМО СЕЙЧАС?\r\nPRIMERO VAMOS A CONOCERNOS - EN ESTE POST OS VOY A CONTAR UN\r\nPOCO SOBRE NOSOTROS\r\nSomos el equipo de desarrollo de inteligencia artificial \"MATEMATICAS.IO\".\r\nNuestra inteligencia artificial se especializará en resolver ejemplos y\r\necuaciones matemáticas complejas, y esperamos que sea muy útil para\r\ntodos en el campo de las matemáticas y la ingeniería, ya que sustituirá el\r\ntrabajo humano por cálculos automáticos de máquinas.\r\nLa inteligencia artificial se auto-desarrolla constantemente - todos esos\r\nejemplos que resolverás en este bot irán a la base de nuestra red neuronal y\r\nel bot será entrenado y desarrollado\r\nESTÁS LISTO PARA EMPEZAR A GANAR AHORA MISMO?",
                         cancellationToken: cancellationToken);
                }
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }

        }
    }
}