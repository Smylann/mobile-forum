using Avalonia.Controls;
using OwnPostsVM = MobileVersion.ViewModels.User.OwnPostsVM;

namespace MobileVersion.Views.User;

public partial class OwnPostsPage : UserControl
{
    public OwnPostsPage()
    {
        InitializeComponent();
    }

    private void ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv && DataContext is OwnPostsVM vm)
        {
            // Trigger when user is within 50px of the bottom
            if (sv.Offset.Y >= sv.Extent.Height - sv.Viewport.Height - 50)
            {
                vm.LoadMoreCommand.Execute(null);
            }
        }
    }
}