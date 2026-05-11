using Avalonia.Controls;
using MobileVersion.ViewModels.Admin;

namespace MobileVersion.Views.Admin;

public partial class DeletePosts : UserControl
{
    public DeletePosts()
    {
        InitializeComponent();
    }
    private void ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv && DataContext is DeletePostsVM vm)
        {
            // Trigger when user is within 50px of the bottom
            if (sv.Offset.Y >= sv.Extent.Height - sv.Viewport.Height - 50)
            {
                vm.LoadMoreCommand.Execute(null);
            }
        }
    }
}