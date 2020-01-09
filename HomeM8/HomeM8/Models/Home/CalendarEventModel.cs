using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class CalendarEventModel:INotifyPropertyChanged
    {
        public string PayerName { get; set; }
        public string EventExplanation { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool Paid { get; set; }
        public string ExpectedDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
