using System.Linq;
using System.Runtime.CompilerServices;
using AbyssRiaCarBot.Controllers;
using AbyssRiaCarBot.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace AbyssRiaCarBot;

public class AbyssRia
{
   TelegramBotClient botClient = new TelegramBotClient("5868430035:AAGO_CVGWRCRKRY_2slLrSiPu8_NPseYvF8");
   CancellationToken cancellationToken = new CancellationToken();
   ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
   public async Task Start()
   {
      botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
      var botMe = await botClient.GetMeAsync();
      Console.WriteLine($"Бот {botMe.Username} почав роботу");
      Console.ReadKey();
   }
   private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
   {
      var ErrorMessage = exception switch
      {
         ApiRequestException apiRequestException => $"Помилка в телеграм бот АПІ:\n{apiRequestException.ErrorCode}" +
                                                    $"\n{apiRequestException.Message}",
         _ => exception.ToString()
      };
      Console.WriteLine(ErrorMessage);
      return Task.CompletedTask;
   }
   private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update,
      CancellationToken cancellationToken)
   {
      if (update.Type == UpdateType.Message && update?.Message?.Text != null)
      {
         await HandlerMessageAsync(botClient, update.Message);
      }
   }
   ReplyKeyboardMarkup paramsKeyboard = new(new[]
      {

         new KeyboardButton[] { "🧽Марка та модель", "🧼Модель" },
         new KeyboardButton[] { "🧼Ціна у $ від", "🧽Ціна у $ до" },
         new KeyboardButton[] { "🧽Рік випуску від", "🧼Рік випуску до" },
         new KeyboardButton[] { "🚀До меню" }
      })
      { ResizeKeyboard = true };

   ReplyKeyboardMarkup menuKeyboard = new(new[]
      {
         new KeyboardButton[] { "🖥Розпочати пошук" },
         new KeyboardButton[] { "🚘Марка", "🏎Модель" },
         new KeyboardButton[] { "💸Ціна у $ від", "💰Ціна у $ до" },
         new KeyboardButton[] { "📅Рік випуску від", "🗓Рік випуску до" },
         new KeyboardButton[] { "🏯На головну" },
         new KeyboardButton[] { "🧹Очистити" }
      })
      { ResizeKeyboard = true };

   ReplyKeyboardMarkup nextKeyboard = new(new[]
      {
         new KeyboardButton[] { "⬅️", "💓", "➡️️" },
         new KeyboardButton[] { "🤖Youtube огляд", "🛠СТО у місті" },
         new KeyboardButton[] { "🏯На головну", "🌌Порівняти" }
      })
      { ResizeKeyboard = true };

   ReplyKeyboardMarkup favKeyboard = new(new[]
      {
         new KeyboardButton[] { "◀️", "🗑️", "▶️" },
         new KeyboardButton[] { "🏯На головну" }
      })
      { ResizeKeyboard = true };
   GeneralClient generalClient = new GeneralClient();
   GetIds idList = new GetIds();
   private GetIds withoutPIdList = new GetIds();
   private List<Mark> marksList = new List<Mark>();
   ReplyKeyboardMarkup startKeyBoard =
      new(new[] { new KeyboardButton[] { "⚡Швидкий пошук","🕵️‍♂️Детальний пошук", "💌Обрані оголошення" } })
         { ResizeKeyboard = true };

   private int year = DateTime.Now.Year;
   private Dictionary<long, CarParams> userParams = new Dictionary<long, CarParams>();
   private Dictionary<long, string> searchRequest = new Dictionary<long, string>();
   private Dictionary<long, string> userStage = new Dictionary<long, string>();
   private async Task HandlerMessageAsync(ITelegramBotClient botClient, Message message)
   {
      if (!userParams.ContainsKey(message.Chat.Id))
      {
         userParams.Add(message.Chat.Id, new CarParams());
      }
      CarParams userParameters = userParams[message.Chat.Id];
      
      if (message.Text == "/start")
      {
         try
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Виберіть пункт меню\n\n'⚡Швидкий пошук' - пошук без параметрів\n\n'🕵️‍♂️Детальний пошук' - пошук за параметрами\n\n'💌Обрані оголошення' - ваші обрані оголошення", replyMarkup: startKeyBoard);
            userParameters.mark = "";
            userParameters.model = "";
            userParameters.startPrice = 0;
            userParameters.endPrice = 0;
            userParameters.startYear = 0;
            userParameters.endYear = 0;
            userParameters.i = 0;
            userParameters.k = 0;
            searchRequest[message.Chat.Id] = "";
            userStage[message.Chat.Id] = "";
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: startKeyBoard);
         }
      }
      else if (message.Text == "🏯На головну")
      {
         try
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Ви на головній сторінці", replyMarkup: startKeyBoard);
            userParameters.mark = "";
            userParameters.model = "";
            userParameters.startPrice = 0;
            userParameters.endPrice = 0;
            userParameters.startYear = 0;
            userParameters.endYear = 0;
            userParameters.i = 0;
            userParameters.k = 0;
            searchRequest[message.Chat.Id] = "";
            userStage[message.Chat.Id] = "";
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: startKeyBoard);
         }
      }
      else if (message.Text == "⚡Швидкий пошук")
      {
         try
         {
            searchRequest[message.Chat.Id] = "Search";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Розпочнемо пошук");
            userParameters.idsList = await generalClient.GetIds();
            var announcment = await generalClient.GetInfo(userParameters.idsList.result.search_result.ids[userParameters.i]);
            string output =
               $"Id оголошення для порівняння: {announcment.autoData.autoId}\nМарка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
            await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
               caption: output, replyMarkup: nextKeyboard);
            userParameters.YTMark = announcment.markName;
            userParameters.YTModel = announcment.modelName;
            userParameters.YTYear = announcment.autoData.year;
            userParameters.GMLocation = announcment.locationCityName;
            userParameters.id = announcment.autoData.autoId;
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: startKeyBoard);
         }
      }
      else if (message.Text == "🕵️‍♂️Детальний пошук")
      {
         try
         {
            searchRequest[message.Chat.Id] = "Search";
            await botClient.SendTextMessageAsync(message.Chat.Id, "Для пошуку по параметрам заповніть всі параметри і натисніть '🖥Розпочати пошук'",
               replyMarkup: menuKeyboard);
            await botClient.SendTextMessageAsync(message.Chat.Id,
               $"Ваші параметри зараз:\n\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}");
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: startKeyBoard);
         }
      }
      else if (message.Text == "💌Обрані оголошення")
      {
         try
         {
            userParameters.favIds = await generalClient.GetDBIds(message.Chat.Id);
            if (userParameters.favIds.Count == 0)
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "У вас ще немає оголошень у обраному", replyMarkup: startKeyBoard);
            }
            else
            {
               var announcment = await generalClient.GetInfo(userParameters.favIds[userParameters.k]);
               string output =
                  $"Марка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
               await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
                  caption: output, replyMarkup: favKeyboard);
               userParameters.id = announcment.autoData.autoId;
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: startKeyBoard);
         }
      }
      else if (message.Text == "◀️")
      {
         try
         {
            if (userParameters.k > 0)
            {
               userParameters.k--;
               var announcment = await generalClient.GetInfo(userParameters.favIds[userParameters.k]);
               string output =
                  $"Марка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
               await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
                  caption: output, replyMarkup: favKeyboard);
               userParameters.id = announcment.autoData.autoId;
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Позаду немає інших оголошень");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "▶️")
      {
         try
         {
            if (userParameters.k + 1 < userParameters.favIds.Count)
            {
               userParameters.k++;
               var announcment = await generalClient.GetInfo(userParameters.favIds[userParameters.k]);
               string output =
                  $"Марка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
               await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
                  caption: output, replyMarkup: favKeyboard);
               userParameters.id = announcment.autoData.autoId;
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Ваші обрані оголошення закінчились");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "🗑️")
      {
         try
         {
            await generalClient.DeleteIdBD(Convert.ToString(userParameters.id), message.Chat.Id);
            userParameters.favIds = await generalClient.GetDBIds(message.Chat.Id);
            userParameters.k = userParameters.k-1;
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Оголошення з id {userParameters.id} успішно видалене із обраного");
            
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: startKeyBoard);
         }
      }
      else if (message.Text == "🌌Порівняти")
      {
         try
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть id другого оголошення для порівняння");
            userStage[message.Chat.Id] = "Compare";
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "🚘Марка")
      {
         try
         {
            userParameters.model = "";
            marksList = await generalClient.GetMarks();
            var keyboardButtons = marksList.Select(x => new[]
            {
               new KeyboardButton(x.Name)
            }).ToArray();
            var marksKeyboard = new ReplyKeyboardMarkup(keyboardButtons) { ResizeKeyboard = true };
            await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть марку", replyMarkup: marksKeyboard);
            userStage[message.Chat.Id] = "Marka";
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "🏎Модель")
      {
         try
         {
            userParameters.markValue = await MarkValue2(marksList, userParameters.mark);
            userParameters.modelsList = await generalClient.GetModels(userParameters.markValue);
            var keyboardButtonsM = userParameters.modelsList.Select(x => new[] { new KeyboardButton(x.name) }).ToArray();
            var modelsKeyboard = new ReplyKeyboardMarkup(keyboardButtonsM) { ResizeKeyboard = true };
            await botClient.SendTextMessageAsync(message.Chat.Id, "Оберіть модель", replyMarkup: modelsKeyboard);
            userStage[message.Chat.Id] = "Model";
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Спочатку вкажіть марку!");
         }
      }
      else if (message.Text == "💸Ціна у $ від")
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Вкажіть стартову ціну у $");
         userStage[message.Chat.Id] = "PriceFrom";
      }
      else if (message.Text == "💰Ціна у $ до")
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Вкажіть кінцеву ціну у $");
         userStage[message.Chat.Id] = "PriceTo";
      }
      else if (message.Text == "📅Рік випуску від")
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Вкажіть початковий рік\nМінімальне значення: 1900\nМаксимальне значення: 2023");
         userStage[message.Chat.Id] = "StartYear";
      }
      else if (message.Text == "🗓Рік випуску до")
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Вкажіть кінцевий рік\nМінімальне значення: 1900\nМаксимальне значення: 2023");
         userStage[message.Chat.Id] = "EndYear";
      }
      else if (message.Text == "🚀До меню")
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Ви повернулися до меню", replyMarkup: menuKeyboard);
      }
      else if (message.Text == "🧹Очистити")
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Вкажіть параметр, який хочете очистити",
            replyMarkup: paramsKeyboard);
         userStage[message.Chat.Id] = "Cleaning";
      }
      else if (message.Text == "🖥Розпочати пошук")
      {
         try
         {
            if(userParameters.mark != "" && userParameters.model != "" && userParameters.startPrice > 0 && userParameters.endPrice > 0 && userParameters.startPrice<=userParameters.endPrice && userParameters.startYear <= userParameters.endYear && userParameters.startYear != 0 && userParameters.endYear != 0)
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Розпочнемо пошук");
               userParameters.idsList = await generalClient.GetIds(userParameters.markValue, userParameters.modelValue, userParameters.startPrice, userParameters.endPrice, userParameters.startYear, userParameters.endYear);
               if (userParameters.idsList.result.search_result.ids.Count == 0)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Наразі немає оголошень за такими параметрами");
               }
               else if (userParameters.idsList.result.search_result.ids.Count != 0)
               {
                  var announcment = await generalClient.GetInfo(userParameters.idsList.result.search_result.ids[userParameters.i]);
                  string output =
                     $"Id оголошення для порівняння: {announcment.autoData.autoId}\nМарка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
                  await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
                     caption: output, replyMarkup: nextKeyboard);
                  userParameters.YTMark = announcment.markName;
                  userParameters.YTModel = announcment.modelName;
                  userParameters.YTYear = announcment.autoData.year;
                  userParameters.GMLocation = announcment.locationCityName;
                  userParameters.id = announcment.autoData.autoId;
               }
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Обов'язково заповніть всі параметри та перевірте їх правильність!", replyMarkup: menuKeyboard);
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "➡️️")
      {
         try
         {
            if (userParameters.i + 1 < userParameters.idsList.result.search_result.ids.Count)
            {
               userParameters.YTMark = "";
               userParameters.YTModel = "";
               userParameters.YTYear = 0;
               userParameters.i++;
               var announcment = await generalClient.GetInfo(userParameters.idsList.result.search_result.ids[userParameters.i]);
               string output =
                  $"Id оголошення для порівняння: {announcment.autoData.autoId}\nМарка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
               await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
                  caption: output, replyMarkup: nextKeyboard);
               userParameters.YTMark = announcment.markName;
               userParameters.YTModel = announcment.modelName;
               userParameters.YTYear = announcment.autoData.year;
               userParameters.GMLocation = announcment.locationCityName;
               userParameters.id = announcment.autoData.autoId;
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Оголошення закінчились");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "⬅️")
      {
         try
         {
            if (userParameters.i > 0)
            {
               userParameters.YTMark = "";
               userParameters.YTModel = "";
               userParameters.YTYear = 0;
               userParameters.i--;
               var announcment = await generalClient.GetInfo(userParameters.idsList.result.search_result.ids[userParameters.i]);
               string output =
                  $"Id оголошення для порівняння: {announcment.autoData.autoId}\nМарка: {announcment.markName}\nМодель: {announcment.modelName}\nПробіг: {announcment.autoData.race}\nТип палива: {announcment.autoData.fuelName}\nКоробка передач: {announcment.autoData.gearboxName}\nРік: {announcment.autoData.year}\nЛокація: {announcment.locationCityName}\nЦіна в $: {announcment.USD}\nЦіна в ₴: {announcment.UAH}\nДетальніше:\nhttps://auto.ria.com{announcment.linkToView}";
               await botClient.SendPhotoAsync(message.Chat.Id, InputFile.FromUri(announcment.photoData.seoLinkF),
                  caption: output, replyMarkup: nextKeyboard);
               userParameters.YTMark = announcment.markName;
               userParameters.YTModel = announcment.modelName;
               userParameters.YTYear = announcment.autoData.year;
               userParameters.GMLocation = announcment.locationCityName;
               userParameters.id = announcment.autoData.autoId;
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Позаду немає інших оголошень");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "🤖Youtube огляд")
      {
         try
         {
            userParameters.youtubeVideo = await generalClient.GetVideo($"{userParameters.YTMark} {userParameters.YTModel} {userParameters.YTYear} Review");
            if (userParameters.youtubeVideo.items.Count == 0)
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "На жаль, доступних відео зараз немає");
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id,
                  $"Якщо дане відео не відповідає темі, це означає що по цьому авто немає доступних оглядів\n\nВідеоогляд на дане авто:\nhttps://www.youtube.com/watch?v={userParameters.youtubeVideo.items[userParameters.j].id.videoId}&ab_channel={userParameters.youtubeVideo.items[userParameters.j].snippet.channelTitle.Replace(" ","")}");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
         
      }
      else if (message.Text == "🛠СТО у місті")
      {
         try
         {
            userParameters.carsRepair = await generalClient.GetPlaces(userParameters.GMLocation);
            List<string> topAutoRepair = new List<string>();

            for (int i = 0; i < userParameters.carsRepair.Results.Count && i < 100; i++)
            {
               if (userParameters.carsRepair.Results[i].Rating >= 4.5)
               {
                  topAutoRepair.Add(userParameters.carsRepair.Results[i].Name);
               }
            }
            if (topAutoRepair.Count == 0)
            {
               await botClient.SendTextMessageAsync(message.Chat.Id,
                  "На даний момент СТО у місті з рейтингом вище 4.5 не знайдено");
            }
            else
            {
               string allAutoRepair = string.Join("\n", topAutoRepair);
               await botClient.SendTextMessageAsync(message.Chat.Id,
                  $"Топ СТО у місті {userParameters.GMLocation} з рейтингом 4,5 і вище:\n{allAutoRepair}");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (message.Text == "💓")
      {
         try
         {
            userParameters.favIds = await generalClient.GetDBIds(message.Chat.Id);
            if (userParameters.favIds.Contains(userParameters.id.ToString()))
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Це оголошення вже є у обраному");
            }

            if (userParameters.id == 0)
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Неможливо додати");
            }
            else
            {
               await generalClient.AddId(userParameters.id.ToString(), message.Chat.Id);
               await botClient.SendTextMessageAsync(message.Chat.Id, $"Оголошення {userParameters.id} успішно додано у обране");
            }
         }
         catch
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception);
         }
      }
      else if (searchRequest[message.Chat.Id] == "Search")
      {
         if (userStage[message.Chat.Id] == "Compare")
         {
            try
            {
               if (Convert.ToInt32(message.Text) == userParameters.id)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Введіть коректне id");
                  userStage[message.Chat.Id] = "Compare";
               }
               else if (Convert.ToInt32(message.Text) != userParameters.id)
               {
                  List<string> compare = new List<string>();
               string id1 = message.Text;
               var infoOfFirst = await generalClient.GetInfo(userParameters.id.ToString());
               var infoOfSecond = await generalClient.GetInfo(id1);
               if (infoOfFirst.autoData.raceInt > infoOfSecond.autoData.raceInt)
               {
                  compare.Add(
                     $"Пробіг першого авто {infoOfFirst.autoData.race} > за пробіг другого {infoOfSecond.autoData.race}");
               }
               if (infoOfFirst.autoData.raceInt < infoOfSecond.autoData.raceInt)
               {
                  compare.Add(
                     $"Пробіг першого авто {infoOfFirst.autoData.race} < за пробіг другого {infoOfSecond.autoData.race}");
               }
               if (infoOfFirst.autoData.raceInt == infoOfSecond.autoData.raceInt)
               {
                  compare.Add($"Пробіг однаковий");
               }
               if (infoOfFirst.autoData.year > infoOfSecond.autoData.year)
               {
                  compare.Add(
                     $"Рік випуску першого авто {infoOfFirst.autoData.year} > за рік випуску другого {infoOfSecond.autoData.year}");
               }
               if (infoOfFirst.autoData.year < infoOfSecond.autoData.year)
               {
                  compare.Add(
                     $"Рік випуску першого авто {infoOfFirst.autoData.year} < за рік випуску другого {infoOfSecond.autoData.year}");
               }
               if (infoOfFirst.autoData.year == infoOfSecond.autoData.year)
               {
                  compare.Add($"Роки випуску однакові");
               }
               if (infoOfFirst.USD > infoOfSecond.USD)
               {
                  compare.Add($"Ціна першого авто у $: {infoOfFirst.USD} > за ціну другого авто у $: {infoOfSecond.USD}");
               }
               if (infoOfFirst.USD < infoOfSecond.USD)
               {
                  compare.Add($"Ціна першого авто у $ {infoOfFirst.USD} < за ціну другого авто у $ {infoOfSecond.USD}");
               }
               if (infoOfFirst.USD == infoOfSecond.USD)
               {
                  compare.Add($"Ціни однакові");
               }
               string car1Stats = $"Марка: {infoOfFirst.markName}\nМодель: {infoOfFirst.modelName}\nТип палива: {infoOfFirst.autoData.fuelName}";
               string car2Stats = $"Марка: {infoOfSecond.markName}\nМодель: {infoOfSecond.modelName}\nТип палива: {infoOfSecond.autoData.fuelName}";
               string allCompares = string.Join("\n\n", compare);
               await botClient.SendTextMessageAsync(message.Chat.Id,
                  $"Перше авто:\n{car1Stats}\n\nДруге авто:\n{car2Stats}\n\n{allCompares}");
               userStage[message.Chat.Id] = "";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: nextKeyboard);
               userStage[message.Chat.Id] = "";
            }
            
         }
         else if (userStage[message.Chat.Id] == "Cleaning")
         {
            if (message.Text == "🧽Марка та модель")
            {
               try
               {
                  userParameters.mark = "";
                  userParameters.markValue = 0;
                  userParameters.model = "";
                  userParameters.modelValue = 0;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Марка та модель успішно очищені\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: paramsKeyboard);
                  userStage[message.Chat.Id] = "Cleaning";
               }
               catch
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
            }
            else if (message.Text == "🧼Модель")
            {
               try
               {
                  userParameters.model = "";
                  userParameters.modelValue = 0;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Модель успішно очищена\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: paramsKeyboard);

                  userStage[message.Chat.Id] = "Cleaning";
               }
               catch
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
            }

            else if (message.Text == "🧼Ціна у $ від")
            {
               try
               {
                  userParameters.startPrice = 0;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Ціна у $ від успішно очищена\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: paramsKeyboard);
                  userStage[message.Chat.Id] = "Cleaning";
               }
               catch
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
            }

            else if (message.Text == "🧽Ціна у $ до")
            {
               try
               {
                  userParameters.endPrice = 0;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Ціна у $ до успішно очищена\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: paramsKeyboard);
                  userStage[message.Chat.Id] = "Cleaning";
               }
               catch
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
            }

            else if (message.Text == "🧽Рік випуску від")
            {
               try
               {
                  userParameters.startYear = 0;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Рік випуску від успішно очищен\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: paramsKeyboard);
                  userStage[message.Chat.Id] = "Cleaning";
               }
               catch
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
            }
            else if (message.Text == "🧼Рік випуску до")
            {
               try
               {
                  userParameters.endYear = 0;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Рік випуску до успішно очищен\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: paramsKeyboard);
                  userStage[message.Chat.Id] = "Cleaning";
               }
               catch
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
            }
            else
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, "Введен невірний параметр",
                  replyMarkup: paramsKeyboard);
               userStage[message.Chat.Id] = "Cleaning";
            }
         }

         else if (userStage[message.Chat.Id] == "Marka")
         {
            try
            {
               if (marksList.Any(m => m.Name == message.Text))
               {
                  userParameters.mark = message.Text;
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Ви обрали марку {userParameters.mark}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
               else
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Невірно введена марка");
                  userStage[message.Chat.Id] = "Marka";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
               userStage[message.Chat.Id] = "";
            }
         }

         else if (userStage[message.Chat.Id] == "Model")
         {
            try
            {
               if (userParameters.markValue == 0)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Спочатку введіть марку!");
                  userStage[message.Chat.Id] = "Marka";
               }
               if (userParameters.modelsList.Any(m => m.name == message.Text))
               {
                  userParameters.model = message.Text;
                  userParameters.modelValue = await ModelValue(userParameters.modelsList);
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     $"Ви обрали модель {userParameters.model}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                     replyMarkup: menuKeyboard);
                  userStage[message.Chat.Id] = "";
               }
               else
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Невірно введена модель!");
                  userStage[message.Chat.Id] = "Model";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
               userStage[message.Chat.Id] = "";
            }
         }
         else if (userStage[message.Chat.Id] == "PriceFrom")
         {
            try
            {
               if (Convert.ToInt32(message.Text) > 0)
               {
                  if (userParameters.endPrice == 0)
                  {
                     userParameters.startPrice = Convert.ToInt32(message.Text);
                     userParameters.endPrice = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали ціну у $ від {userParameters.startPrice}\n\nІдентичну ціну було додано у параметр кінцевої ціни, для зміни натисніть '💰Ціна у $ до' або очистіть параметр за допомогою функції '🧹Очистити'\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if (Convert.ToInt32(message.Text) <= userParameters.endPrice)
                  {
                     userParameters.startPrice = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали ціну у $ від {userParameters.startPrice}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if (Convert.ToInt32(message.Text) > userParameters.endPrice)
                  {
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Ціна не може бути більшою за кінцеву!\nВведіть параметр ще раз");
                     userStage[message.Chat.Id] = "PriceFrom";
                  }
               }
               else if (Convert.ToInt32(message.Text) <= 0)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Ціна не може бути мінусовою та дорівнювати нулю!\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "PriceFrom";
               }
               else
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Помилка, перевірте правильність параметрів");
                  userStage[message.Chat.Id] = "PriceFrom";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
               userStage[message.Chat.Id] = "";
            }
         }
         else if (userStage[message.Chat.Id] == "PriceTo")
         {
            try
            {
               if (Convert.ToInt32(message.Text) > 0 && Convert.ToInt32(message.Text) >= userParameters.startPrice)
               {
                  if (userParameters.startPrice == 0)
                  {
                     userParameters.endPrice = Convert.ToInt32(message.Text);
                     userParameters.startPrice = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали ціну у $ до {userParameters.endPrice}\n\nІдентичну ціну було додано у параметр 'Ціна у $ від', для зміни натисніть '💸Ціна у $ від' або очистіть параметр за допомогою функції '🧹Очистити'\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if(Convert.ToInt32(message.Text) >= userParameters.startPrice)
                  {
                     userParameters.endPrice = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали ціну у $ до {userParameters.endPrice}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if (Convert.ToInt32(message.Text) < userParameters.startPrice)
                  {
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Ціна не може бути меншою за початкову!\nВведіть параметр ще раз");
                     userStage[message.Chat.Id] = "PriceTo";
                  }
               }
               else if (Convert.ToInt32(message.Text) < userParameters.startPrice)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Кінцева ціна не може бути меншою за початкову!\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "PriceTo";
               }
               else if (Convert.ToInt32(message.Text) <= 0)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Ціна не може бути мінусовою та дорівнювати нулю!\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "PriceTo";
               }
               else
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Помилка, перевірте правильність параметрів");
                  userStage[message.Chat.Id] = "PriceTo";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
               userStage[message.Chat.Id] = "";
            }
         }
         else if (userStage[message.Chat.Id] == "StartYear")
         {
            try
            {
               if (Convert.ToInt32(message.Text) >= 1900 && Convert.ToInt32(message.Text) <= 2023 && Convert.ToInt32(message.Text) > 0)
               {
                  if (userParameters.endYear == 0)
                  {
                     userParameters.startYear = Convert.ToInt32(message.Text);
                     userParameters.endYear = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали рік випуску від {userParameters.startYear}\n\nІдентичний рік було додано у параметр 'Рік випуску до', для зміни натисніть '🗓Рік випуску до' або очистіть параметр за допомогою функції '🧹Очистити'\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if(Convert.ToInt32(message.Text) <= userParameters.endYear)
                  {
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали рік випуску від {userParameters.startYear}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if (Convert.ToInt32(message.Text) > userParameters.endYear)
                  {
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Початковий рік не може бути більший за кінцевий!\nВведіть параметр ще раз");
                     userStage[message.Chat.Id] = "StartYear";
                  }
               }
               else if (Convert.ToInt32(message.Text) < 1900)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Рік не може бути меншим за 1900\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "StartYear";
               }
               else if (Convert.ToInt32(message.Text) > 2023)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     "Рік не може бути більшим за 2023\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "StartYear";
               }
               else if (Convert.ToInt32(message.Text) < 0)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Рік не може бути мінусовим!\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "StartYear";
               }
               else
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Помилка, перевірте правильність параметрів");
                  userStage[message.Chat.Id] = "StartYear";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
               userStage[message.Chat.Id] = "";
            }
         }
// else
         // { 
         //    userParameters.startYear = Convert.ToInt32(message.Text);
         //    await botClient.SendTextMessageAsync(message.Chat.Id,
         //       $"Ви обрали рік випуску від {userParameters.startPrice}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
         //       replyMarkup: menuKeyboard);
         //    userStage[message.Chat.Id] = "";
         // }
         else if (userStage[message.Chat.Id] == "EndYear")
         {
            try
            {
               //userParameters.endYear = Convert.ToInt32(message.Text);
               if (Convert.ToInt32(message.Text) >= 1900 && Convert.ToInt32(message.Text) <= 2023 && Convert.ToInt32(message.Text) > 0 && Convert.ToInt32(message.Text) >= userParameters.startYear)
               {
                  if (userParameters.startYear == 0)
                  {
                     userParameters.endYear = Convert.ToInt32(message.Text);
                     userParameters.startYear = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали рік випуску до {userParameters.endYear}\n\nІдентичний рік було додано у параметр 'Рік випуску від', для зміни натисніть '📅Рік випуску від' або очистіть параметр за допомогою функції '🧹Очистити'\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if (Convert.ToInt32(message.Text) >= userParameters.startYear)
                  {
                     userParameters.endYear = Convert.ToInt32(message.Text);
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"Ви обрали рік випуску до {userParameters.endYear}\n\nОбрані параметри:\nМарка {userParameters.mark}\nМодель {userParameters.model}\nПочаткова ціна {userParameters.startPrice}\nКінцева ціна {userParameters.endPrice}\nРік випуску від {userParameters.startYear}\nРік випуску до {userParameters.endYear}",
                        replyMarkup: menuKeyboard);
                     userStage[message.Chat.Id] = "";
                  }
                  else if (Convert.ToInt32(message.Text) < userParameters.startYear)
                  {
                     await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Кінцевий рік не може бути меншим за початковий!\nВведіть параметр ще раз");
                  }
               }
               else if (Convert.ToInt32(message.Text) < 1900)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Рік не може бути меншим за 1900\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "EndYear";
               }
               else if (Convert.ToInt32(message.Text) > 2023)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id,
                     "Рік не може бути більшим за 2023\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "EndYear";
               }

               else if (Convert.ToInt32(message.Text) <= 0)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Рік не може бути мінусовим та рівним 0!\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "EndYear";
               }
               else if (Convert.ToInt32(message.Text) < userParameters.startYear)
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Рік не може бути меншим за початковий!\nВведіть параметр ще раз");
                  userStage[message.Chat.Id] = "EndYear";
               }
               else
               {
                  await botClient.SendTextMessageAsync(message.Chat.Id, "Помилка, перевірте правильність параметрів");
                  userStage[message.Chat.Id] = "EndYear";
               }
            }
            catch
            {
               await botClient.SendTextMessageAsync(message.Chat.Id, userParameters.exception, replyMarkup: menuKeyboard);
               userStage[message.Chat.Id] = "";
            }
         }
         else
         {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Приїхала помилка");
         }
      }
      else
      {
         await botClient.SendTextMessageAsync(message.Chat.Id, "Приїхала помилка");
      }

      async Task<int> MarkValue2(List<Mark> marksList1, string mark)
      {
         foreach (var markInList in marksList1)
         {
            if (markInList.Name == mark)
            {
               return markInList.Value;
            }
         }

         throw new ArgumentException("Mark not found");
      }

      async Task<int> MarkValue(string markName, List<Mark> marksList)
      {
         foreach (var mark in marksList)
         {
            if (mark.Name.Equals(markName, StringComparison.OrdinalIgnoreCase))
            {
               return mark.Value;
            }
         }
         throw new ArgumentException("Mark not found");
      }

      async Task<int> ModelValue(List<Model> modelsList1)
      {
         foreach (var modelsInList in modelsList1)
         {
            if (modelsInList.name == userParameters.model)
            {
               return modelsInList.value;
            }
         }
         throw new ArgumentException("Model not found");
      }
   }
}

