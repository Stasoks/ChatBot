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
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;

        private static string connectionString = "Data Source=..//..//..//Users_DB.db";
        private static string Link = "https://t.me/+0gfb7z3CK3c5ODQ0";
        public  static string managerLink = "@tasikkk";
        public  static string BotId = "TeSt222288bot";

        static Dictionary<long, string> equations = new Dictionary<long, string>();
        static Random rnd = new Random();

        public static ReplyKeyboardMarkup menu = new(new[]
        {
            new KeyboardButton[] { "Ejemplos matemáticos" },
            new KeyboardButton[] { "Invitar a amigos" },
            new KeyboardButton[] { "Mi perfil/balance" },
            new KeyboardButton[] { "Retiros" }
        })
        {
            ResizeKeyboard = true
        };
        public static ReplyKeyboardMarkup primers = new(new[]
        {
            new KeyboardButton[] { "Ejemplos matemáticos" },
            new KeyboardButton[] { "Menú general" },
        })
        {
            ResizeKeyboard = true
        };

        public static ReplyKeyboardMarkup checksubscribe = new(new[]
        {
            new KeyboardButton[] { "Comprobar suscripción" },
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

                int answer = 0;

                if (update.Type == UpdateType.ChatMember)
                {
                    if (update.ChatMember != null)
                    {
                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"INSERT INTO Users (Id, Money, Friends, Cases, StartTime) VALUES ('{update.ChatMember.NewChatMember.User.Id}', 0, 0, 0, datetime('now'))";
                        command.ExecuteNonQuery();
                    }
                }

                if (messageText != null)
                {
                    if(int.TryParse(messageText, out answer)) 
                    {
                        string equation = "";
                        if(equations.TryGetValue(message.From.Id, out equation))
                        {
                            if((int.Parse(equation.Substring(0, 3)) + int.Parse(equation.Substring(6, 3))) == answer)
                            {
                                SQLiteCommand command = new SQLiteCommand();
                                command.Connection = connection;
                                command.CommandText = $"UPDATE Users SET Money = Money + 50, Cases = Cases + 1 WHERE Id = {message.From.Id};";  
                                command.ExecuteNonQuery();
                                equations.Remove(message.From.Id);

                                Message sendMessage = await botClient.SendTextMessageAsync(
                             chatId: chatId,
                             text: "¡La respuesta es correcta!\r\a tu saldo\r\acreditado + 50 MXN",
                             replyMarkup: primers);
                            }

                            else
                            {
                                Message sendMessage = await botClient.SendTextMessageAsync(
                                 chatId: chatId,
                                 text: "Error! intentar de\r\nnuevo",
                                 replyMarkup: primers);
                                Thread.Sleep(2000);
                                GenerateEquation(botClient, update, message, chatId);
                            }
                        }
                    }


                    if (messageText.Contains("/start"))
                    {
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                        new KeyboardButton[] { "Comenzar a ganar" },
                    })
                        {
                            ResizeKeyboard = true
                        };
                        long id;
                        if (messageText.Length > 15 && long.TryParse(messageText.Substring(7), out id))
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
                             text: "Primero vamos a conocernos - en este post os voy a contar un\r\npoco sobre nosotros\r\nsomos el equipo de desarrollo de inteligencia artificial \"matematicas.io\".\r\nnuestra inteligencia artificial se especializará en resolver ejemplos y\r\necuaciones matemáticas complejas, y esperamos que sea muy útil para\r\ntodos en el campo de las matemáticas y la ingeniería, ya que sustituirá el\r\ntrabajo humano por cálculos automáticos de máquinas.\r\nla inteligencia artificial se auto-desarrolla constantemente - todos esos\r\nejemplos que resolverás en este bot irán a la base de nuestra red neuronal y\r\nel bot será entrenado y desarrollado\r\nestás listo para empezar a ganar ahora mismo?",
                             replyMarkup: replyKeyboardMarkup,
                             cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("Comenzar a ganar")) //готов
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Para empezar,suscríbase a nuestro canal de noticias",
                            replyMarkup: checksubscribe,
                            cancellationToken: cancellationToken
                            );
                    }

                    if (messageText.Contains("Comprobar suscripción")) //проверка пиписки
                    {
                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM Users WHERE Id = {update.Message.From.Id}";
                        SQLiteDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            //https://t.me/dasdasdasdasdasqweqw
                            Message sendMessage = await botClient.SendTextMessageAsync(
                               chatId: chatId,
                               text: "En tu saldo\r\ncarga regalo - 500MXN",
                               cancellationToken: cancellationToken);
                            Thread.Sleep(1500);

                            await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Muy bien, puedes empezar.\r\nhay dos tipos de ingresos\r\n1 - Resolviendo ejemplos de matemáticas (50MXN por cada ejemplo resuelto)\r\n2 - Invitar a tus amigos a este bot (500 MXN por cada amigo invitado)",
                                replyMarkup: menu,
                                cancellationToken: cancellationToken);
                        }

                        else
                        {
                            Message sendMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"Para empezar,\r\nsuscríbase a nuestro\r\ncanal de noticias\r\n {Link}",
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

                    if (messageText.Contains("Menú general"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Menu",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("Invitar a amigos"))
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Reenvía este mensaje a tus amigos y por cada amigo que se una a través\r\nde este enlace recibirás 500 MXN, https://t.me/{BotId}?start={message.From.Id}",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("Mi perfil/balance"))
                    {
                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"SELECT Money, Friends, Cases, (julianday('now') - julianday(StartTime))*24 FROM Users WHERE Id = {update.Message.From.Id}";
                        SQLiteDataReader reader = command.ExecuteReader();
                        string Money = "";
                        string Friends = "";
                        string Cases = "";
                        float Hours = 0;

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Money   = reader.GetValue(0).ToString();
                                Friends = reader.GetValue(1).ToString();
                                Cases   = reader.GetValue(2).ToString();
                                Hours = float.Parse(reader.GetValue(3).ToString());
                            }
                        }

                        Message sendMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                            text: $"Tu número es - {update.Message.From.Id}\r\n{Math.Round(Hours, 1)} Horas que llevas\r\ntrabajando con\r\nnosotros\r\nnúmero de ejemplos\r\nresueltos - {Cases}\r\nNúmero de amigos\r\ninvitados - {Friends}\r\nTu saldo - {Money}",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken); 
                    }
                    if (messageText.Contains("Retiros"))
                    {
                        SQLiteCommand command = new SQLiteCommand();
                        command.Connection = connection;
                        command.CommandText = $"SELECT Money FROM Users WHERE Id = {update.Message.From.Id}";
                        SQLiteDataReader reader = command.ExecuteReader();
                        long Money = 0;

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Money = (long)reader.GetValue(0);

                            }
                        }

                        if (Money >= 15000)
                        {
                            Message sendMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: $"Póngase en contacto con su gerente financiero personal - {managerLink}",
                           replyMarkup: menu,
                           cancellationToken: cancellationToken);
                        }
                        else
                        {
                            Message sendMessage = await botClient.SendTextMessageAsync(
                           chatId: chatId,
                           text: "Mínimo cantidad a retiro 15.000",
                           replyMarkup: menu,
                           cancellationToken: cancellationToken);

                        }
                    }

                    if (messageText.Contains("Ejemplos matemáticos"))
                    {
                        GenerateEquation(botClient, update, message, chatId);
                    } 
                }
            }

            async Task GenerateEquation( ITelegramBotClient botClient, Update update, Message message, ChatId chatId )
            {
                int onefour = rnd.Next(100, 1000);
                int eighteight = rnd.Next(100, 1000);

                if (equations.ContainsKey(message.From.Id))
                {
                    equations[message.From.Id] = $"{onefour} + {eighteight}";
                }
                else
                {
                    equations.Add(message.From.Id, $"{onefour} + {eighteight}");
                }


                Message sendMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Your primero {onefour} + {eighteight}",
                    replyMarkup: primers
                    );
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