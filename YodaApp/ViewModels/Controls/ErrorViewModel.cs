using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodaApp.ViewModels.Controls
{
    class ErrorViewModel : ViewModelBase
    {
        private string info;

        public string Info
        {
            get { return info; }
            set => Set(ref info, nameof(Info), value);
        }


        private string detailedInfo;

        public string DetailedInfo
        {
            get { return detailedInfo; }
            set => Set(ref detailedInfo, nameof(DetailedInfo), value);
        }


        private string type;

        public string Type
        {
            get { return type; }
            set => Set(ref type, nameof(Type), value);
        }

    }
}
