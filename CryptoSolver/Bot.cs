using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Crypto;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoSolver
{
    public class Bot
    {
        private static string Token { get; set; } = "";

        private static TelegramBotClient client;

        private static bool isAnyOtherMessage = true;

        private static Dictionary<string, Func<string, string>> dictMethods = new DictionaryMethods().dict;

        private static List<string> users = new List<string> { "123456789",
                                                               "987654321"};


        private static Dictionary<Telegram.Bot.Types.ChatId, List<string>> UsersPath = new Dictionary<Telegram.Bot.Types.ChatId, List<string>>();

        public Bot()
        {
            client = new TelegramBotClient(Token);
            client.StartReceiving();
            client.OnMessage += Client_OnMessage;
            Console.ReadLine();
            client.StopReceiving();
        }

        internal Task SendTextMessageAsync(object chatId, string v, object replyMarkup)
        {
            throw new NotImplementedException();
        }

        private static async void Client_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            if (!users.Contains(msg.Chat.Id.ToString()))
                return;
            if (!UsersPath.ContainsKey(msg.Chat.Id))
                UsersPath.Add(msg.Chat.Id, new List<string>());
            if (msg.Text != null)
            {
                // Уровень решений
                if (UsersPath[msg.Chat.Id].Count == 2)
                {
                    try
                    {
                        var data = msg.Text;
                        data = data.Replace(" ", "");
                        data = Regex.Replace(data, @"[A-Za-z]+", "");
                        UsersPath[msg.Chat.Id].Add(data);
                        var answer = dictMethods[UsersPath[msg.Chat.Id][1]](UsersPath[msg.Chat.Id][2]);
                        await client.SendTextMessageAsync(
                            chatId: msg.Chat.Id,
                            answer,
                            replyMarkup: GeneralMenu());
                        UsersPath[msg.Chat.Id] = new List<string>();
                    }
                    catch (FormatException)
                    {
                        await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Вы ввели неверные значения!",
                                replyMarkup: GeneralMenu());
                        UsersPath[msg.Chat.Id] = new List<string>();
                    }
                    catch (KeyNotFoundException)
                    {
                        await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Продолжишь тыкать и бот умрет!",
                                replyMarkup: GeneralMenu());
                        UsersPath[msg.Chat.Id] = new List<string>();
                    }
                    catch
                    {
                        await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Тебе меня не сломать!",
                                replyMarkup: GeneralMenu());
                        UsersPath[msg.Chat.Id] = new List<string>();
                    }
                }
                // Первый уровень
                if (UsersPath[msg.Chat.Id].Count == 1)
                {
                    try
                    {
                        isAnyOtherMessage = false;
                        UsersPath[msg.Chat.Id].Add(msg.Text);
                        var reply = new Second_Level(msg.Text);
                        if (reply.Key.Contains("http"))
                        {
                            await client.SendTextMessageAsync(
                                            chatId: msg.Chat.Id,
                                            reply.Key,
                                            replyMarkup: GeneralMenu());
                            UsersPath[msg.Chat.Id] = new List<string>();
                        }
                        else
                        {
                            await client.SendTextMessageAsync(
                                            chatId: msg.Chat.Id,
                                            reply.Key);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Тыкай по кнопочкам!",
                                replyMarkup: GeneralMenu());
                        UsersPath[msg.Chat.Id] = new List<string>();

                    }
                }

                if (UsersPath[msg.Chat.Id].Count == 0) //Начало работы
                {
                    isAnyOtherMessage = false;
                    switch (msg.Text)
                    {
                        case "Шифры":
                            UsersPath[msg.Chat.Id].Add(msg.Text);
                            var Ciphers = await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Выберите нужный шифр",
                                replyMarkup: GeneralCiphersMenu());
                            break;


                        case "Свойства":
                            UsersPath[msg.Chat.Id].Add(msg.Text);
                            var prop = await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Выберите нужную информацию",
                                replyMarkup: GeneralPropertiesMenu());
                            break;

                        case "Калькулятор":
                            UsersPath[msg.Chat.Id].Add(msg.Text);
                            var calc = await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                "Выберите нужный метод",
                                replyMarkup: GeneralMethodsMenu());
                            break;
                        default:
                            UsersPath[msg.Chat.Id] = new List<string>();
                            var answer = await client.SendTextMessageAsync(
                                 chatId: msg.Chat.Id,
                                 "Выберите нужное решение",
                                 replyMarkup: GeneralMenu());
                            break;
                    }
                }

                if (isAnyOtherMessage) // проверка в конце, если мы так и не нашли нужный ход
                {
                    UsersPath[msg.Chat.Id] = new List<string>();
                    var answer = await client.SendTextMessageAsync(
                         chatId: msg.Chat.Id,
                         "Выберите нужное решение",
                         replyMarkup: GeneralMenu());
                }
                isAnyOtherMessage = true;

            }
        }

        private static IReplyMarkup GeneralPropertiesMenu()
        {
            return new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                {
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Свойства RSA"}, new KeyboardButton {Text = "Свойства El Gamal"} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Свойства сравнений"}, new KeyboardButton {Text = "Малая теорема Ферма"} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Теорема Эйлера"} }
                }
            };
        }

        private static IReplyMarkup GeneralMethodsMenu()
        {
            return new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                {
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Разложение на множители"}, new KeyboardButton {Text = "Быстрое экспоненцирование" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "y = a^b mod p"}, new KeyboardButton {Text = "y = a*b mod p" }, new KeyboardButton {Text = "Решение сравнения"},},
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Минимальная циклическая группа"}, new KeyboardButton {Text = "Метод тривиальных(простых) делений" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Метод Ферма"}, new KeyboardButton {Text = "Тест Ферма" } , new KeyboardButton { Text = "НОД" }},
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Обратный элемент"}, new KeyboardButton { Text = "Решение системы сравнений" },  },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Размер хеша и вероятность повторения"}, new KeyboardButton {Text = "Функция Эйлера" } },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Вероятность некорректной шифровки" } }
                }
            };
        }

        private static IReplyMarkup GeneralCiphersMenu()
        {
            return new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                {
                    new List<KeyboardButton>{ new KeyboardButton {Text = "RSA"}, new KeyboardButton {Text = "El Gamal"} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "RSABank"} }
                }
            };
        }

        private static IReplyMarkup GeneralMenu()
        {
            return new ReplyKeyboardMarkup()
            {
                Keyboard = new List<List<KeyboardButton>>()
                {
                    new List<KeyboardButton>{ new KeyboardButton {Text = "Шифры" }, new KeyboardButton {Text ="Свойства" } },
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Калькулятор"} }
                }
            };
        }
    }
}
