using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YodaApp.Behaviors
{
    public interface IParentCloseable
    {
        event EventHandler RequestCloseParent;
    }

    public class CloseWindowBehavior : Behavior<Window>
    {
        // https://stackoverflow.com/questions/16172462/close-window-from-viewmodel

        public IParentCloseable CloseTrigger
        {
            get { return (IParentCloseable)GetValue(CloseTriggerProperty); }
            set { SetValue(CloseTriggerProperty, value); }
        }

        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register("CloseTrigger", typeof(IParentCloseable), typeof(CloseWindowBehavior), new PropertyMetadata(null, CloseTriggerProperty_Changed));

        private static void CloseTriggerProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CloseWindowBehavior b)
            {
                var oldValue = (IParentCloseable)e.OldValue;
                b.OnCloseTrigger(oldValue);
            }
        }

        private void OnCloseTrigger(IParentCloseable old)
        {
            if (old != null)
            {
                old.RequestCloseParent -= CloseTrigger_RequestCloseParent;
            }

            if (CloseTrigger != null)
            {
                CloseTrigger.RequestCloseParent += CloseTrigger_RequestCloseParent;
            }
        }

        private void CloseTrigger_RequestCloseParent(object sender, EventArgs e)
        {
            AssociatedObject.Close();
        }
    }
}
