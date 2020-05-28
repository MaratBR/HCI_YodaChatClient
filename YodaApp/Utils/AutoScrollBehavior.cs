using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace YodaApp.Utils
{
    internal class AutoScrollBehavior : Behavior<ScrollViewer>
    {
        private bool enabled;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.ScrollChanged += Sv_ScrollChanged;
            enabled = AssociatedObject.VerticalOffset == AssociatedObject.ScrollableHeight;
        }

        private void Sv_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            enabled = AssociatedObject.VerticalOffset == AssociatedObject.ScrollableHeight;

            if (enabled)
            {
                AssociatedObject.ScrollToBottom();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.ScrollChanged -= Sv_ScrollChanged;
        }
    }
}