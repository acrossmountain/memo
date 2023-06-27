using System.Collections.ObjectModel;

namespace Memo.Models
{
    public class Summary
    {
        private int _todoCount;
        private int _todoCompletedCount;
        private string _todoCompletedRadio = "0%";
        private int _memoCount;

        public int MemoCount { get { return _memoCount; } set { _memoCount = value; } }
        public int TodoCount { get { return _todoCount; } set { _todoCount = value; } }
        public int TodoCompletedCount { get { return _todoCompletedCount; } set { _todoCompletedCount = value; } }
        public string TodoCompletedRadio { get { return _todoCompletedRadio; } set { _todoCompletedRadio = value; } }


        private ObservableCollection<TodoModel> _todos;
        public ObservableCollection<TodoModel> Todos
        {
            get { return _todos; }
            set { _todos = value; }
        }

        private ObservableCollection<MemoModel> _memos;
        public ObservableCollection<MemoModel> Memos
        {
            get { return _memos; }
            set { _memos = value; }
        }
    }
}
