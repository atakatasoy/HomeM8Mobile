﻿using HomeM8.Views;
using HomeM8.Views.PartialView;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeM8
{
    public class GetHomesByNameModel : INotifyPropertyChanged
    {
        public string HomeName { get; set; }
        public string HomeManager { get; set; }
        public int PeopleCount { get; set; }
        public int HomeID { get; set; }
        public bool AlreadyRequested { get; set; }
        public ICommand JoinCommand { get; set; }

        public GetHomesByNameModel()
        {
            JoinCommand = new Command(async () =>
            {
                var parameters = JsonConvert.SerializeObject(new { AccessToken = Utility.User.accessToken, HomeID });

                var response = await Helper.ApiCall<SendRequestHomeResponseModel>(RequestType.Post, ControllerType.Home, "sendrequesthome", parameters);

                if (response.responseVal == 0)
                {
                    AlreadyRequested = true;
                    await PopupNavigation.Instance.PushAsync(new ErrorPopup("Başarılı"));
                }
                else
                {
                    await PopupNavigation.Instance.PushAsync(new ErrorPopup("Beklenmedik hata"));
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
