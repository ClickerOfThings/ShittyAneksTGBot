using System;
using System.Net;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using System.Threading;
using System.Collections.Generic;

// я всё это дерьмище потом задокументирую, главное не забыть

namespace shiddyAnecss
{
    class MainClass
    {
        public static List<CurrentUserCooldown> usersCooldown = new List<CurrentUserCooldown>();
        public static List<UserCooldownInfo> usersCooldownInfo = new List<UserCooldownInfo>();
        public static Telegram.Bot.TelegramBotClient baneksBot;
        public static void Main()
        {
            baneksBot =
            new TelegramBotClient("ЖОПА");
            ReceiverOptions opt = new ReceiverOptions()
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message }
            };
            baneksBot.StartReceiving(NewUpdateFunc, ErrorFunc, receiverOptions: opt);
            while (true) // Я кодер на monodevelop, иди нахуй
            {
                Thread.Sleep(1000);
                Console.ReadLine();
            }
            Console.WriteLine("Нажми энтер, чтобы убить и распотрошить нахуй этого бота.");
            Console.ReadLine();
        }

        static void ErrorFunc(ITelegramBotClient arg1, Exception arg2, System.Threading.CancellationToken arg3)
        {
            // Лол да кому нахуй сдались эти ошибки
        }

        static void NewUpdateFunc(ITelegramBotClient client, Telegram.Bot.Types.Update update, System.Threading.CancellationToken cancellationToken)
        {
            if (update.Message.Text is null)
                return;
            User user = update.Message.From;
            Chat chat = update.Message.Chat;
            User botUser = client.GetMeAsync().Result;
            if (!update.Message.Text.Contains("@" + botUser.Username))
                return;
            Console.WriteLine("Говно от " + user.Username);
            MessageType mesType = GetMessageType(update.Message.Text);
            switch (mesType)
            {
                case MessageType.Anek:
                    SendAnek(client, update.Message, user, chat);
                    break;
                case MessageType.Help:
                    SendHelp(client, update.Message, user, chat);
                    break;
                case MessageType.SetCooldown:
                    SetCooldown(client, update.Message, user, chat);
                    break;
                case MessageType.None:
                    return;
            }
        }

        public enum MessageType
        {
            Anek, SetCooldown, Help, None
        }

        public static List<string> SetCooldownTriggers = new List<string>()
        {
            "отдых", "кулдаун", "расслабон", "час", "часы", "перезарядка",
            "балдёж", "ждать"
        };
        public static List<string> HelpTriggers = new List<string>()
        {
            "помоги", "помощь", "хелп", "хелпани", "хелпа", "бля",
            "справочная", "справка", "я тупой обрыган и конченый хуесос"
        };

        const int MAX_MESSAGE_WORDS_COUNT = 3;

        static MessageType GetMessageType(string messageText)
        {
            string[] words = messageText.Split(' ');
            if (words.Length > MAX_MESSAGE_WORDS_COUNT)
                return MessageType.None;
            foreach (string _word in words)
            {
                string word = _word.ToLower();
                if (word.Contains('@')) // пинганули бота ёбана
                {
                    continue;
                }
                if (SetCooldownTriggers.Contains(word))
                    return MessageType.SetCooldown;
                if (HelpTriggers.Contains(word))
                    return MessageType.Help;
            }
            return MessageType.Anek;
        }

        static void SendAnek(ITelegramBotClient client, Message message, User user, Chat chat)
        {
            CurrentUserCooldown cooldown = IsUserInCooldown(user);
            if (cooldown != null)
            {
                client.SendTextMessageAsync(chat, "Соси, " + user.Username +
                    ", жди до " + cooldown.TimeCooledDown.AddHours(cooldown.CurrentCooldownHours));
                return;
            }
            client.SendTextMessageAsync(chat, ParseRandomAnecdote() + "\n\nТаймаут до " + DateTime.Now.AddHours(1));
            CooldownUser(user);
        }


        private static void SetCooldown(ITelegramBotClient client, Message message, User user, Chat chat)
        {
            string[] words = message.Text.Split(' ');
            int newCooldownTime = 0;
            foreach (string word in words)
            {
                if (int.TryParse(word, out newCooldownTime))
                    break;
            }
            if (newCooldownTime == 0)
            {
                int currentHourCooldown = GetCooldownInfoOnUser(user).HourCoolDown;
                CurrentUserCooldown userCooldown = usersCooldown.FirstOrDefault(x => x.User.Id == user.Id);
                string resultString = "Твой текущий кулдаун: " + currentHourCooldown + " час(ов)\n" +
                    "Сейчас у тебя кулдаун" +
                    (userCooldown != null ? (" до " + userCooldown.TimeCooledDown.AddHours(userCooldown.CurrentCooldownHours))
                                        : ("а нет, можешь схватить бесплатно анек"));
                client.SendTextMessageAsync(chat, resultString);
                return;
            }
            if (newCooldownTime < 0)
            {
                CooldownUser(user);
                client.SendTextMessageAsync(chat, "🤡");
                return;
            }
            EditUserInCooldownInfoList(user, newCooldownTime);
            CurrentUserCooldown cooldown = GetCooldownOnUser(user);
            client.SendTextMessageAsync(chat, "Без б, поставил тебе новый кулдаун, наслаждайся." +
            (cooldown != null ? ("............. если ты надеялся, что это как-то изменит твой " +
                                "текущий кулдаун, то ты далбаёб тупой хахахаххахахахаха")
                                : ""));
        }

        /*static readonly string HELP = "Как работает сей бот: он отвечает только если в сообщении пингуется бот.\n" +
                            "пустой пинг - получить случайный анек с сайта baneks.site\n" +
                            "пинг со словами типа 'кулдаун', 'ждать' и прочая хуйня (там " +
                            "буквально список слов блядь поиграйся не тупой) - получить " +
                            "информацию о кулдауне, либо же поставить его (тогда ещё число введи)\n" +
                            "пинг со словами типа 'помоги', 'хелп' - помощь, ну а как ты ещё щас " +
                            "эту всю хуйню читаешь. там тоже список слов есть, можешь поиграться, " +
                            "а можешь и в очке поиграться, мне похуй, честно.\n" +
                            "Сам принцип бота заключается в том, что анек выдаётся на одного пользователя, " +
                            "затем пользователь отправляется в кулдаун на несколько часов. Именно часов, не минут, " +
                            "не секунд, а именно часов. Как минимум ты прождёшь 1 час. Ограничение можешь поставить сам себе сколько " +
                            "хочешь, хоть блядь через год.\n" +
                            "Ты меня спросишь, а нахуя это надо, если можно самому зайти на сайт и посмотреть, не ожидая 1 час реального времени? " +
                            "Ну иди смотри блядь, сам листай, я тебя не держу, " +
                            "только бот выдаёт тебе анеки у которых не меньше " + MIN_LIKES_ON_ANEC + " лайков. " +
                            "Удачи тебе найти столько анеков в быстрые сроки.\n" +
                            "Исходный код бота приведён здесь: " + SOURCE_CODE + "\n" +
                            "Какие-то отдельные пожелания разрабу? Да пошёл ты нахуй!\n" +
                            "Хохлы? Пидорасы.";*/
        const string SOURCE_CODE = "Иди читай пока не разучился " +
        	"https://github.com/ClickerOfThings/ShittyAneksTGBot/blob/main/README.md";

        private static void SendHelp(ITelegramBotClient client, Message message, User user, Chat chat)
        {
            client.SendTextMessageAsync(chat, SOURCE_CODE);
        }

        /// <summary>
        /// Checks whether the user is in the cooldown list
        /// </summary>
        /// <returns>Cooldown object if the user is in the list or null if not</returns>
        static CurrentUserCooldown IsUserInCooldown(User userToCheck)
        {
            CurrentUserCooldown cooldown = usersCooldown.FirstOrDefault(x => x.User.Id == userToCheck.Id);
            if (cooldown != null)
            {
                if (cooldown.TimeCooledDown.AddHours(cooldown.CurrentCooldownHours) > DateTime.Now)
                    return cooldown;
                else
                {
                    usersCooldown.Remove(cooldown);
                    return null;
                }
            }
            else return null;
        }

        static void EditUserInCooldownInfoList(User userToEdit, int desiredHours)
        {
            usersCooldownInfo.First(x => x.User.Id == userToEdit.Id)
                .HourCoolDown = desiredHours;
        }

        static void CooldownUser(User user)
        {
            UserCooldownInfo cooldownInfo = GetCooldownInfoOnUser(user);
            usersCooldown.Add(new CurrentUserCooldown
            {
                User = user,
                CurrentCooldownHours = cooldownInfo.HourCoolDown,
                TimeCooledDown = DateTime.Now
            });
        }

        static UserCooldownInfo AddNewCooldownInfo(User user)
        {
            UserCooldownInfo returnCooldownInfo = new UserCooldownInfo
            {
                User = user,
                HourCoolDown = 1
            };
            usersCooldownInfo.Add(returnCooldownInfo);
            return returnCooldownInfo;
        }

        static UserCooldownInfo GetCooldownInfoOnUser(User user)
        {
            UserCooldownInfo cooldownInfo = usersCooldownInfo.FirstOrDefault(x => x.User.Id == user.Id);
            if (cooldownInfo == null)
            {
                UserCooldownInfo newCooldownInfo = AddNewCooldownInfo(user);
                cooldownInfo = newCooldownInfo;
            }
            return cooldownInfo;
        }

        static CurrentUserCooldown GetCooldownOnUser(User user)
        {
            return usersCooldown.FirstOrDefault(x => x.User.Id == user.Id);
        }

        const int MIN_LIKES_ON_ANEC = 700;
        static string ParseRandomAnecdote()
        {
            while (true)
            {
                WebRequest req = WebRequest.CreateHttp("https://baneks.site/random");
                HtmlDocument doc = new HtmlDocument();
                WebResponse resp = req.GetResponse();
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    doc.LoadHtml(reader.ReadToEnd());
                }
                Console.WriteLine("loaded");
                HtmlNode testNode = doc.DocumentNode;
                Console.WriteLine("docNode");
                HtmlNode likeCount = testNode.SelectSingleNode("//span[@itemprop=\"userInteractionCount\"]");
                if (int.Parse(likeCount.InnerText) < MIN_LIKES_ON_ANEC)
                {
                    Console.WriteLine("Лайков меньше " + MIN_LIKES_ON_ANEC + " (" + int.Parse(likeCount.InnerText) + "), продолжаем");
                    continue;
                }
                HtmlNode anek = testNode.SelectSingleNode("//section[@itemprop=\"description\"]");
                Console.WriteLine("selected");
                var returnText = anek.InnerHtml.Replace("<br>", "\n").Replace("<p>", "").Replace("</p>", "");
                Console.WriteLine("done");
                returnText += "\n" +
                    "-------------------------------------------\n" +
                    "Лучший комментарий:\n" + GetBestComment(resp.ResponseUri.ToString());
                //returnText += "\n\n" + resp.ResponseUri;
                return returnText;
            }
        }

        static string GetBestComment(string site)
        {
            int pageCounter = 1;
            int bestCommentLikes = 0;
            string bestCommentString = string.Empty;
            while (true)
            {
                WebRequest req = WebRequest.CreateHttp(site + "?p=" + pageCounter++);
                HtmlDocument doc = new HtmlDocument();
                WebResponse resp;
                try
                {
                    resp = req.GetResponse();
                }
                catch (System.Net.WebException)
                {
                    return bestCommentString;
                }
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    doc.LoadHtml(reader.ReadToEnd());
                }
                Console.WriteLine("loaded");
                HtmlNode testNode = doc.DocumentNode;
                HtmlNodeCollection commentsNodes = testNode.SelectNodes("//section[@itemprop=\"description\"]");
                HtmlNodeCollection commentLikesNodes = testNode.SelectNodes("//span[@itemprop=\"userInteractionCount\"]");
                for (int i = 1; i < commentsNodes.Count; i++)
                {
                    int currentLikes;
                    if (int.TryParse(commentLikesNodes[i].InnerText, out currentLikes) && currentLikes > bestCommentLikes)
                    {
                        bestCommentLikes = currentLikes;
                        bestCommentString = commentsNodes[i].InnerHtml.Replace("<br>", "\n").Replace("<p>", "").Replace("</p>", "");
                    }
                }
            }
        }
    }

    class UserCooldownInfo
    {
        public User User;
        public int HourCoolDown;
    }

    class CurrentUserCooldown
    {
        public User User;
        public DateTime TimeCooledDown;
        public int CurrentCooldownHours;
    }
}
