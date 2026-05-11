using Avalonia.Controls;
using OwnCommentsVM = MobileVersion.ViewModels.User.OwnCommentsVM;

namespace MobileVersion.Views.User;

public partial class OwnCommentsPage : UserControl
{
    public OwnCommentsPage()
    {
        InitializeComponent();
    }
    private void ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv && DataContext is OwnCommentsVM vm)
        {
            // Trigger when user is within 50px of the bottom
            if (sv.Offset.Y >= sv.Extent.Height - sv.Viewport.Height - 50)
            {
                vm.LoadMoreCommand.Execute(null);
            }
        }
    }
}