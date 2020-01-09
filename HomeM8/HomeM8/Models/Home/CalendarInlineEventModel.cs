using Syncfusion.SfCalendar.XForms;
using System.ComponentModel;

namespace HomeM8.Models.Home
{
    public class CalendarInlineEventModel : CalendarInlineEvent
    {
        string mPayerName;
        public string PayerName
        {
            get
            {
                return mPayerName;
            }
            set
            {
                if (mPayerName != value)
                {
                    mPayerName = value;
                    OnPropertyChanged(nameof(PayerName));
                }
            }
        }

        decimal mPaymentAmount;
        public decimal PaymentAmount
        {
            get
            {
                return mPaymentAmount;
            }
            set
            {
                if (mPaymentAmount != value)
                {
                    mPaymentAmount = value;
                    OnPropertyChanged(nameof(PaymentAmount));
                }
            }
        }

        bool mPaid;
        public bool Paid
        {
            get
            {
                return mPaid;
            }
            set
            {
                if (mPaid != value)
                {
                    mPaid = value;
                    OnPropertyChanged(nameof(Paid));
                }
            }
        }
    }
}
