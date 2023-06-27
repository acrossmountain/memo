using Prism.Services.Dialogs;
using System.Threading.Tasks;

namespace Memo.Services
{
    public interface IDialogHostService : IDialogService
    {
        Task<IDialogResult> ShowDialog(string title, IDialogParameters dialogParameters, string dialogHostName = "Root");
    }
}
