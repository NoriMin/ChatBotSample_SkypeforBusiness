using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ChatBotSample_SkypeforBiz.Models;

namespace ChatBotSample_SkypeforBiz.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static string convert = "";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(HelloMessage);

            return Task.CompletedTask;
        }

        private async Task HelloMessage(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("こんにちは！私はPoC Botです。どのようなご用件でしょうか？ ");

            await MenuMessage(context);
        }

        private async Task MenuMessage(IDialogContext context)
        {
            int i;

            string[] array;
            array = new string[2] { "社内手続きに関する問い合わせ", "終了" };


            string resultMessage = "以下から選択してください(番号で入力)\n";

            for (i = 0; i < 2; i++)
            {
                resultMessage = resultMessage + (i + 1).ToString() + ". " + array[i] + "\n";
            }


            await context.PostAsync(resultMessage);

            context.Wait(SelectDialog);

        }

        private async Task SelectDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            convert = await ZenkakuConvert.Convert(message.Text);

            if (convert == "1")
            {
                context.Call(new FAQDialog(), QnaResumeAfterDialog);
            }
            else if (convert == "2")
            {
                await context.PostAsync("ご利用ありがとうございました。最後にアンケートをお願いできますか？");
                context.Call(new EnqueteDialog(), EnqueteResumeAfterDialog);
            }

        }

        private async Task QnaResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            await FeedbackMessage(context);
        }

        private async Task FeedbackMessage(IDialogContext context)
        {
            int i;

            string[] array;
            array = new string[2] { "はい", "いいえ" };

            string resultMessage = "解決しましたか？(番号で入力)\n";

            for (i = 0; i < 2; i++)
            {
                resultMessage = resultMessage + (i + 1).ToString() + ". " + array[i] + "\n";
            }

            await context.PostAsync(resultMessage);
            context.Wait(FeedbackDialog);
        }


        private async Task FeedbackDialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var feedbackMenu = await result;

            convert = await ZenkakuConvert.Convert(feedbackMenu.Text);

            if (convert == "1")
            {
                await context.PostAsync("ご利用ありがとうございました。");
                await MenuMessage(context);
            }
            else if (convert == "2")
            {
                await context.PostAsync("どのような回答をご希望でしたか？");
                context.Wait(InputMessage);
            }

        }

        private async Task InputMessage(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* Use below code when you want to trace logs partly */
            /* Then delete code of Line.27 in Global.assax.cs */

            //var activity = context.Activity as Microsoft.Bot.Connector.Activity;
            //Trace.TraceInformation($"{activity.Text}");

            await context.PostAsync("フィードバックありがとうございます。今後の精度改善の参考にさせて頂きます。");

            await MenuMessage(context);
        }

        private async Task EnqueteResumeAfterDialog(IDialogContext context, IAwaitable<string> result)
        {
            await context.PostAsync($"ご協力、ありがとうございました。");

            await MenuMessage(context);
        }
    }
}