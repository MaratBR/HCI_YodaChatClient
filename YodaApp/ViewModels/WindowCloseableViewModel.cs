using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YodaApp.Behaviors;

namespace YodaApp.ViewModels
{
    class WindowCloseableViewModel : ViewModelBase, IParentCloseable
    {
        public event EventHandler RequestCloseParent;

        public void CloseParent() => RequestCloseParent?.Invoke(this, EventArgs.Empty);
    }
}
