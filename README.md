# ShittyAneksTGBot
Random aneks from baneks.site


# Как работает сей бот
он отвечает только если в сообщении пингуется бот.
* пустой пинг - получить случайный анек с сайта baneks.site
* пинг со словами типа 'кулдаун', 'ждать' и прочая хуйня (там буквально список слов блядь поиграйся не тупой) - получить информацию о кулдауне, либо же поставить его (тогда ещё число введи)
* ~~пинг со словами типа 'помоги', 'хелп' - помощь, ну а как ты ещё щас эту всю хуйню читаешь. там тоже список слов есть, можешь поиграться, а можешь и в очке поиграться, мне похуй, честно.~~ Неактуально. Теперь это тут. Когда ты успел это пропустить? Ещё на этапе разработки первой версии бота. А вырезать мне это лень. Соси короче.
# Вчом прикол
Сам принцип бота заключается в том, что анек выдаётся на одного пользователя, затем пользователь отправляется в кулдаун на несколько часов. Именно часов, не минут, не секунд, а именно часов. Как минимум ты прождёшь 1 час. Ограничение можешь поставить сам себе сколько хочешь, хоть блядь через год.\
Ты меня спросишь, а нахуя это надо, если можно самому зайти на сайт и посмотреть, не ожидая 1 час реального времени? Ну иди смотри блядь, сам листай, я тебя не держу, только бот выдаёт тебе анеки у которых не меньше %сколько-то, смотреть в коде% лайков. Удачи тебе найти столько анеков в быстрые сроки.

Исходный код бота приведён здесь: ты прямо на этой странице, привет\
Какие-то отдельные пожелания разрабу? Да пошёл ты нахуй!\
Хохлы? Пидорасы.


```
static readonly string HELP = "Как работает сей бот: он отвечает только если в сообщении пингуется бот.\n" +
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
                            "Хохлы? Пидорасы.";
```

# А мне его как собрать?
Достаточно схватить бесплатно файл Program.cs, из nuget пакетов тебе нужны только HtmlAgilityPack, Telegram.Bot и Telegram.Bot.Extensions.Polling.
