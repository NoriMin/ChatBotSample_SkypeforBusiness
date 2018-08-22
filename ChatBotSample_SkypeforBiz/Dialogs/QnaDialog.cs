using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Newtonsoft.Json;
using ChatBotSample_SkypeforBiz.Models;


namespace ChatBotSample_SkypeforBiz.Dialogs
{
    [Serializable]
    public class FAQDialog : IDialog<object>
    {
        public static string json = "";
        public static string convert = "";

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("どのようなことでお困りでしょうか？文章で質問を入力してください。");
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;

            json = await CustomQnAMaker.GetResultAsync(message.Text);


            if (json != "failure")
            {
                var result = JsonConvert.DeserializeObject<QnAMakerResults>(json);
                var result2 = JsonConvert.DeserializeObject<QnAMakerResult>(json);

                if (result2.Score == 0)
                {
                    await context.PostAsync("質問に対する回答が見つかりませんでした。");
                }

                await ShowQuestions(context, result);
            }

        }

        private async Task ShowQuestions(IDialogContext context, QnAMakerResults result)
        {
            int i;
            string resultMessage = "以下から選択してください(番号で入力)\n";


            for (i = 0; i < result.Answers.Count; i++)
            {
                resultMessage = resultMessage + (i + 1).ToString() + ". " + result.Answers[i].Questions[0] + "\n";
            }
            resultMessage = resultMessage + (i + 1).ToString() + ". 上記のどれでもない\n";
            await context.PostAsync(resultMessage);

            context.Wait(ShowAnswer);

        }

        private async Task ShowAnswer(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var num = await item;
            var result = JsonConvert.DeserializeObject<QnAMakerResults>(json);

            convert = await ZenkakuConvert.Convert(num.Text);

            if (Int32.Parse(convert) >= 1 && Int32.Parse(convert) <= result.Answers.Count)
            {
                await context.PostAsync(result.Answers[Int32.Parse(convert) - 1].Answer.ToString());
                context.Done<object>(null);
            }
            else if (Int32.Parse(convert) == result.Answers.Count + 1)
            {
                await context.PostAsync("お役に立てず申し訳ございません。。");
                context.Done<object>(null);
            }
            else
            {
                await ShowQuestions(context, result);
            }

        }

    }
}