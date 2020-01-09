using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace HomeM8
{
    public class AccountInfoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        public string HomeName { get; set; }
        public string HomeAddress { get; set; }
        public List<HomeInfoModel> ConnectedHomesInfo { get; set; }
        public List<string> HomeMembers { get; set; }
        public string HomeManager { get; set; }
        public List<string> HomeRules { get; set; }
        public List<string> HomePermissions { get; set; }
    }
}
