using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;


namespace ChatBot
{
    class Program()
    {
        static string PATH = "..//..//..//members.txt";
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static string connectionString = "Data Source=..//..//..//Users_DB.db";

        //DeleteWebhookRequest _deleteWebhookRequest;
        //https://t.me/TeSt222288bot?start=ref01

        public static ReplyKeyboardMarkup menu = new(new[]
        {
            new KeyboardButton[] { "Ejemplos matemáticos" },
            new KeyboardButton[] { "INVITAR A AMIGOS" },
            new KeyboardButton[] { "MI PERFIL/BALANCE" },
        })
        {
            ResizeKeyboard = true
        };

        public static ReplyKeyboardMarkup checksubscribe = new(new[]
                    {
                        new KeyboardButton[] { "COMPROBAR SUSCRIPCIÓN" },
                    })
        {
            ResizeKeyboard = true
        };

        static async Task Main()
        {

            //System.Data.SQLite

            var connection = new SQLiteConnection(connectionString);
            connection.Open();


            _botClient = new TelegramBotClient("1872154697:AAGUxJZjUloMjrjd5Qprw2ldJjfb2aqtysQ");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.ChatMember,
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
                if (update.Message == null) { return; }
                var message = update.Message;
                var messageText = update.Message.Text;
                var chatId = message.Chat.Id;
                var GroupChatId = "#-1002054923410";
                var callback = update.CallbackQuery;
                bool Tim = false;
                string nameUsers;


                if (update.Message.Type == MessageType.ChatMemberLeft)
                {
                    //string[] textFromFile = System.IO.File.ReadAllLines(PATH);
                    //var Ids = new List<string>(textFromFile);
                    //Ids.Remove(update.Message.LeftChatMember.Id.ToString());

                    //System.IO.File.Delete(PATH);
                    //System.IO.File.WriteAllLines(PATH, textFromFile);
                }

                if (update.Message.Type == MessageType.ChatMembersAdded)
                {
                    //string[] a = new string[] { update.Message.NewChatMembers[0].Id.ToString() };
                    //System.IO.File.AppendAllLines(PATH, a);

                    SQLiteCommand command = new SQLiteCommand();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO Users (Id, Money) VALUES ('{update.Message.NewChatMembers[0].Id}', 0)";
                    command.ExecuteNonQuery();
                }

                if (messageText != null)
                {

                    if (messageText.Contains("/START"))
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                        new KeyboardButton[] { "COMENZAR A GANAR" },
                    })
                        {
                            ResizeKeyboard = true
                        };
                        Message sendMessage = await botClient.SendTextMessageAsync(
                             chatId: chatId,
                             text: "PRIMERO VAMOS A CONOCERNOS - EN ESTE POST OS VOY A CONTAR UN\r\nPOCO SOBRE NOSOTROS\r\nSomos el equipo de desarrollo de inteligencia artificial \"MATEMATICAS.IO\".\r\nNuestra inteligencia artificial se especializará en resolver ejemplos y\r\necuaciones matemáticas complejas, y esperamos que sea muy útil para\r\ntodos en el campo de las matemáticas y la ingeniería, ya que sustituirá el\r\ntrabajo humano por cálculos automáticos de máquinas.\r\nLa inteligencia artificial se auto-desarrolla constantemente - todos esos\r\nejemplos que resolverás en este bot irán a la base de nuestra red neuronal y\r\nel bot será entrenado y desarrollado\r\nESTÁS LISTO PARA EMPEZAR A GANAR AHORA MISMO?",
                             replyMarkup: replyKeyboardMarkup,
                             cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("COMENZAR A GANAR")) //готов
                    {


                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Para empezar,suscríbase a nuestro canal de noticias",
                            replyMarkup: checksubscribe,
                            cancellationToken: cancellationToken
                            );
                    }

                    if (messageText.Contains("COMPROBAR SUSCRIPCIÓN")) //проверка пиписки
                    {
                        //var subscribe = await botClient.GetChatMemberAsync(chatId: "@dasdasdasdasdasqweqw", userId: update.Message.From.Id);

                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM Users WHERE Id = {update.Message.From.Id}";
                        SQLiteDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            //https://t.me/dasdasdasdasdasqweqw
                            Message sendMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "НА ВАШ БАЛАНС\r\nНАЧИСЛЕН ПОДАРОК -\r\n500MXN",
                               cancellationToken: cancellationToken);
                            Thread.Sleep(1500);

                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "MUY BIEN, PUEDES EMPEZAR.\r\nHAY DOS TIPOS DE INGRESOS\r\n1 - RESOLVIENDO EJEMPLOS DE MATEMÁTICAS ( 50MXN POR CADA EJEMPLO RESUELTO )\r\n2 - INVITAR A TUS AMIGOS A ESTE BOT ( 500 MXM POR CADA AMIGO INVITADO )",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);

                        }
                        else
                        {
                            Message sendMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Вы не подписаны",
                                replyMarkup: checksubscribe,
                                cancellationToken: cancellationToken);


                        }
                    }

                    if (messageText.Contains("/help"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Use /menu to call menu",
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("/menu"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "MENU",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("Ejemplos matemáticos"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "MENU",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("INVITAR A AMIGOS"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "MENU",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("MI PERFIL/BALANCE"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "MENU",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }
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