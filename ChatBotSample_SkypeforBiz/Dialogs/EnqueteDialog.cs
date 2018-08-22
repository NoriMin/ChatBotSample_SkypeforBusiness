using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace ChatBotSample_SkypeforBiz.Dialogs
{
    [Serializable]
    public class EnqueteDialog : IDialog<string>
    {

        public async Task StartAsync(IDialogContext context)
        {
            int i;

            string[] array;
            array = new string[5] { "大満足", "満足", "普通", "不満", "とても不満" };

            string resultMessage = "以下から選択してください(番号で入力)\n";

            for (i = 0; i < 5; i++)
            {
                resultMessage = resultMessage + (i + 1).ToString() + ". " + array[i] + "\n";
            }

            await context.PostAsync(resultMessage);

            context.Wait(SelectDialog);

        }

        private async Task SelectDialog(IDialogContext context, IAwaitable<object> result)
        {
            var selectedMenu = await result;
            context.Done(selectedMenu);
        }
    }
}