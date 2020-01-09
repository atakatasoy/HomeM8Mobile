using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class AppColors : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public string PageWrapperColor { get; set; }
        public string ButtonColor { get; set; }
        public string ButtonTextColor { get; set; }
        public string InputFrameBorderColor { get; set; }
        public string AppInfoStringsColor { get; set; }
        public string LoginEntryBackground { get; set; }
        public string NavigationPrimary { get; set; }
        public string GridLinesColor { get; set; }
        public string NotificationOwnerTextColor { get; set; }
        public string MoneyColor { get; set; }
        public string WarningColor { get; set; }
        public string PermissionColor { get; set; }
    }
}
