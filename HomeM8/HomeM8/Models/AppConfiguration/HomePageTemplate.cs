using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class HomePageTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public string InstantNotificationsHeaderText { get; set; }
        public string GeneralNotificationsHeaderText { get; set; }
        public string CalendarPayerText { get; set; }
        public string CalendarAmountText { get; set; }
    }
}
