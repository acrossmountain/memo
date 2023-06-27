using Memo.Events;
using Memo.Services;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace Memo.Extensions
{
    public static class Dialog
    {
        /// <summary>
        /// 询问窗口
        /// </summary>
        /// <param name="dialogHost"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="dialogHostName"></param>
        /// <returns></returns>
        public static async Task<IDialogResult> Question(this IDialogHostService dialogHost,
            string title, string content, string dialogHostName = "Root")
        {
            DialogParameters parameters = new DialogParameters();
            parameters.Add("title", title);
            parameters.Add("content", content);
            parameters.Add("dialogHostName", dialogHostName);

            return await dialogHost.ShowDialog("MessageBoxView", parameters, dialogHostName);
        }

        public static void Update(this IEventAggregator aggregator, LoadingModel loadingModel)
        {
            aggregator.GetEvent<LoadingEvent>().Publish(loadingModel);
        }

        public static void Register(this IEventAggregator aggregator, Action<LoadingModel> action)
        {
            aggregator.GetEvent<LoadingEvent>().Subscribe(action);
        }

        public static void SubscribeMessage(this IEventAggregator aggregator, Action<string> action)
        {
            aggregator.GetEvent<MessageEvent>().Subscribe(action);
        }

        public static void PublishMessage(this IEventAggregator aggregator, string message)
        {
            aggregator.GetEvent<MessageEvent>().Publish(message);
        }
    }
}
