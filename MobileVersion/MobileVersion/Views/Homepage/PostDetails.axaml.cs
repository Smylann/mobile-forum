using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PostVM = MobileVersion.ViewModels.Homepage.PostVM;

namespace MobileVersion.Views.Homepage;

public partial class PostDetails : UserControl
{
    public PostDetails()
    {
        InitializeComponent();
    }
    private void ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv && DataContext is PostVM vm)
        {
            // Trigger when user is within 50px of the bottom
            if (sv.Offset.Y >= sv.Extent.Height - sv.Viewport.Height - 50)
            {
                vm.LoadMoreCommand.Execute(null);
            }
        }
    }
    private async void ToTop_OnClick(object? sender, RoutedEventArgs e)
    {
        // Give layout time to update
        await Task.Delay(10);
    
        CommentScroller.Offset = new Vector(0, 0);
        var commentBorder = this.FindControl<Border>("CommentBorder");
        commentBorder?.BringIntoView();
    }
}