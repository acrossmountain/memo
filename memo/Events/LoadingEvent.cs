using Prism.Events;

namespace Memo.Events
{
    public class LoadingModel
    {
        public bool IsOpen { get; set; }
    }

    public class LoadingEvent : PubSubEvent<LoadingModel>
    {

    }
}
