using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class ForgotPasswordPageTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public string ThirdGridInformationString { get; set; }
        public string FirstGridInformationString { get; set; }
        public string NewPasswordEntryPlaceholderString { get; set; }
        public string RepeatNewPasswordEntryPlaceHolderString { get; set; }
        public string SecondGridValidationString { get; set; }
        public string SendButtonString { get; set; }
        public string PageTitle { get; set; }
        public string UsernameEntryPlaceholderString { get; set; }
        public string SecondGridValidationEntryPlaceholderString { get; set; }
    }
}
