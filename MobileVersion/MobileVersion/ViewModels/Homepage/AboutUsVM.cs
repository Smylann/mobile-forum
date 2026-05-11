using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MobileVersion.Messages;

namespace MobileVersion.ViewModels.Homepage
{
    public partial class AboutUsVM : ViewModelBase
    {
        public AboutUsVM()
        {
        }

        [RelayCommand]
        private void Close() => WeakReferenceMessenger.Default.Send(new GoBackMessage());
    }
}
