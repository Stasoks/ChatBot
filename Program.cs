using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Data.SQLite;


namespace ChatBot
{
    class Program()
    {
        private static ITelegramBotClient _botClient;
        private static ReceiverOptions _receiverOptions;
        static Dictionary<long, string> equations = new Dictionary<long, string>();
        static Random rnd = new Random();

        #region Messages
        private const string StartMessage = //first
            "Primero vamos a conocernos - en este post os voy a contar un\r\n" +
            "poco sobre nosotros  📲\r\n\r\n" +
            "Somos desarrolladores de inteligencia artificial \" matematicas.io \" \r\n" +
            "Se especializará en la resolución de ecuaciones matemáticas y ejemplos, será muy útil para todos en el campo de las matemáticas y la ingeniería, ya que sustituirá el trabajo humano por cálculos automáticos de máquinas 🔏\r\n\r\n" +
            "Todos los ejemplos que\r\nLos ejemplos que resolverás formarán la base de nuestra red neuronal, será entrenada y desarrollada \U0001f9e0\r\n\r\n" +
            "estás listo para empezar a ganar ahora mismo?";

        private const string PleaseSubscribe =
            $"<b>Para empezar, suscríbase a nuestro canal de noticias, donde a menudo compartimos información útil y sorteos</b> 🎁\r\n\r\n{NewsChannelLink}";

        private const string Instruction =
            "PARA EMPREZAR 🙏🏽\r\n\r\nHay dos tipos de ganancias 📲\r\n\r\n" +
            "1️⃣ - Resolviendo ejemplos matemáticos (50MXN por cada ejemplo resuelto)\r\n\r\n" +
            "2️⃣ - Invitando amigos a este bot (500MXN por cada amigo invitado) \r\n\r\n" +
            "Elige una opción de ingresos que te interese";
        #endregion

        #region Program сonstansts
        private const string stepBack = "..//..//..//"; //stepback = "" if release, = "..//..//..//" if debug
        private const string resourcesPath = $"{stepBack}resources//";
        private const string connectionString = $"Data Source={stepBack}Users_DB.db";
        private const string NewsChannelLink = "https://t.me/+0gfb7z3CK3c5ODQ0";
        private const string managerLink = "@Brouz39";
        private const string BotId = "TeSt222288bot";
        #endregion


        #region Keyboards
        public static ReplyKeyboardMarkup menu = new(new[]
        {
            new KeyboardButton[] { "ECUACIONES MATEMÁTICAS  (+ 50MXN)🔢" },
            new KeyboardButton[] { "INVITA A TUS AMIGOS (+500MXN) 📥" },
            new KeyboardButton[] { "BALANCE 🤑" },
            new KeyboardButton[] { "RETIRAR DINERO ❤️‍🔥" }
        })
        {
            ResizeKeyboard = true
        };
        public static ReplyKeyboardMarkup primers = new(new[]
        {
            new KeyboardButton[] { "ECUACIONES MATEMÁTICAS  (+ 50MXN)🔢" },
            new KeyboardButton[] { "INVITA A TUS AMIGOS (+500MXN) 📥" },
            new KeyboardButton[] { "MENU GENERAL 📑" },
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
        #endregion

        static async Task Main()
        {

            //System.Data.SQLite

            var connection = new SQLiteConnection(connectionString);
            connection.Open();


            _botClient = new TelegramBotClient("6857834562:AAGNWEM9FXMyIh-oddr4FDQZNmrgdfmyb60"); //1872154697:AAGUxJZjUloMjrjd5Qprw2ldJjfb2aqtysQ
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

                                SQLiteCommand command2 = new SQLiteCommand();
                                command2.Connection = connection;
                                command2.CommandText = $"SELECT Money FROM Users WHERE Id = {message.From.Id}";
                                SQLiteDataReader reader = command2.ExecuteReader();
                                long Money = 0;

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        Money = (long)reader.GetValue(0);
                                    }
                                }

                                await botClient.SendTextMessageAsync(
                                 parseMode: ParseMode.Html,
                                 chatId: chatId,
                                 text: $"La respuesta es correcta ✅\r\n\r\nSu beneficio: 💰<b>+ 50 MXN</b>💰\r\nSu equilibrio general: 💰 <b>{Money} MXN</b> 💰",
                                 replyMarkup: primers);

                                GenerateEquation(botClient, update, message, chatId);
                            }

                            else
                            {
                                Message sendMessage = await botClient.SendTextMessageAsync(
                                     chatId: chatId,
                                     parseMode: ParseMode.Html,
                                     text: "<b>Error</b> ❗️\r\n\r\nintentar de nuevo 👇🏾",
                                     replyMarkup: primers);
                                Thread.Sleep(1500);
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

                                await botClient.SendTextMessageAsync(
                                    chatId: id,
                                    parseMode: ParseMode.Html,
                                    text: "❗️Una persona se unirá a través de su enlace - 💰<b>+500 MXN</b> 💰 serán acreditados a su saldo",
                                    replyMarkup: replyKeyboardMarkup,
                                    cancellationToken: cancellationToken);
                            }
                        }

                        Message sendMessage = await botClient.SendTextMessageAsync(
                             chatId: chatId,
                             text: StartMessage,
                             replyMarkup: replyKeyboardMarkup,
                             cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("Comenzar a ganar")) //готов
                    {
                        Message sendMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            parseMode: ParseMode.Html,
                            text: PleaseSubscribe,
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
                               text: "500 MXN han sido abonados a tu saldo 💰",
                               cancellationToken: cancellationToken);

                            SQLiteCommand command2 = new SQLiteCommand();
                            command2.Connection = connection;
                            command2.CommandText = $"UPDATE Users SET Money = Money + 500 WHERE Id = {update.Message.From.Id};";
                            command2.ExecuteNonQuery();

                            Thread.Sleep(1500);

                            await using Stream stream = System.IO.File.OpenRead($"{resourcesPath}img1.png");
                            await botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream),
                                parseMode: ParseMode.Html,
                                replyMarkup: menu,
                                caption: Instruction);
                        }

                        else
                        {
                            Message sendMessage = await botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: $"❗️<b>Para empezar,suscríbase a nuestro canal de noticias</b> 👇🏾\r\n\r\n {NewsChannelLink}",
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
                            parseMode: ParseMode.Html,
                            text: "<b>Seleccione una de las opciones del menú</b> 📲",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("INVITA A TUS AMIGOS (+500MXN) 📥"))
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            parseMode: ParseMode.Html,
                            text: $"❗️<b>Reenvía este mensaje a tus amigos y por cada amigo que se una a través de este enlace, recibirás 500 MXN</b> ❗️\r\n\r\n⤵️⤵️⤵️⤵️⤵️⤵️⤵️⤵️⤵️",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                        Thread.Sleep(1500);
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Este es un bot de inteligencia artificial para cálculos matemáticos, ayúdalo a desarrollarse - realiza tareas sencillas y gana más de 16 000 MXN al día 📲\r\n\r\n\r\n 👉🏾 https://t.me/{BotId}?start={message.From.Id}  👈🏾",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken);
                    }

                    if (messageText.Contains("BALANCE 🤑"))
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
                            parseMode : ParseMode.Html,
                            text: $"❗️Tu número es - <b>{update.Message.From.Id}</b> ❗️\r\n\r\n\r\n🔢 Número de ejemplos\r\nresueltos - <b>{Friends}</b>\r\n\r\n📥 Número de amigos\r\ninvitados - 0\r\n\r\n💰Tu saldo - <b>{Money}</b>",
                            replyMarkup: menu,
                            cancellationToken: cancellationToken); 
                    }
                    if (messageText.Contains("RETIRAR DINERO ❤️‍🔥"))
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

                    if (messageText.Contains("ECUACIONES MATEMÁTICAS  (+ 50MXN)🔢"))
                    {
                        GenerateEquation(botClient, update, message, chatId);
                    } 
                }
            }

            async Task GenerateEquation( ITelegramBotClient botClient, Update update, Message message, ChatId chatId )
            {
                int first = rnd.Next(100, 1000);
                int second = rnd.Next(100, 1000);

                if (equations.ContainsKey(message.From.Id))
                {
                    equations[message.From.Id] = $"{first} + {second}";
                }
                else
                {
                    equations.Add(message.From.Id, $"{first} + {second}");
                }

                await botClient.SendTextMessageAsync(
                    chatId: chatId,                  
                    parseMode: ParseMode.Html,
                    text: $"Your primero <b>{first} + {second}</b>",
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