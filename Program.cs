using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;
using static System.Net.WebRequestMethods;


namespace ChatBot
{
    class Program()
    {
        static string PATH = "..//..//..//members.txt";
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        private static string connectionString = "Data Source=..//..//..//Users_DB.db";

        //DeleteWebhookRequest _deleteWebhookRequest;
        //https://t.me/TeSt222288bot?start=1213213454

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
                var message = update.Message;
                var messageText = "";
                long chatId = 0;
                if (message != null) { 
                    messageText = update.Message.Text; 
                    chatId = message.Chat.Id;
                }
                var callback = update.CallbackQuery;


                if (update.Type == UpdateType.ChatMember)
                {
                    if (update.ChatMember != null)
                    {
                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"INSERT INTO Users (Id, Money, Friends, Cases) VALUES ('{update.ChatMember.NewChatMember.User.Id}', 0, 0, 0)";
                        command.ExecuteNonQuery();
                    }
                }

                if (messageText != null)
                {

                    if (messageText.Contains("/start"))
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                        new KeyboardButton[] { "COMENZAR A GANAR" },
                    })
                        {
                            ResizeKeyboard = true
                        };
                        long id;
                        if (long.TryParse(messageText.Substring(7), out id))
                        {
                            if (message.From.Id != id)
                            {

                                SQLiteCommand command = new SQLiteCommand();
                                command.Connection = connection;
                                command.CommandText = $"UPDATE Users SET Money = Money + 500, Friends = Friends + 1 WHERE Id = {id};";
                                command.ExecuteNonQuery();
                            }
                        }

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
                            text: $"REENVÍA ESTE MENSAJE A TUS AMIGOS Y POR CADA AMIGO QUE SE UNA A TRAVÉS\r\nDE ESTE ENLACE RECIBIRÁS 500 MXN, https://t.me/TeSt222288bot?start={message.From.Id}",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("MI PERFIL/BALANCE"))
                    {
                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"SELECT Money, Friends, Cases  FROM Users WHERE Id = {update.Message.From.Id}";
                        SQLiteDataReader reader = command.ExecuteReader();
                        string Money = "";

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Money = reader.GetValue(0).ToString();
                            }
                        }

                        Message sendMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                            text: "TU NÚMERO ES - update.Message.From.Id\r\nHORAS QUE LLEVAS\r\nTRABAJANDO CON\r\nNOSOTROS\r\nNÚMERO DE EJEMPLOS\r\nRESUELTOS\r\nNÚMERO DE AMIGOS\r\nINVITADOS\r\nTU SALDO",
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