using HomeM8.Views.PartialView;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeM8
{
    public class AccountPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public AccountInfoModel AccountInfo { get; set; }

        public List<GetHomesByNameModel> SearchResult { get; set; }

        public string HomeSearchBarValue { get; set; }
        public bool HomeNotFoundStringVisible { get; set; }

        public async Task SetAccountInfo()
        {
            var parameters = JsonConvert.SerializeObject(new { AccessToken = Utility.User.accessToken });

            var response = await Helper.ApiCall<AccountResponseModel>(RequestType.Post, ControllerType.User, "getaccountinfo", parameters);

            if (response.responseVal == 0)
            {
                AccountInfo = response.accountInfo;
            }
            else
            {
                //show err
            }
        }

        public void ClearSearchArguments()
        {
            HomeSearchBarValue = "";
            SearchResult = new List<GetHomesByNameModel>();
        }

        public async Task GetHomesByName()
        {
            await PopupNavigation.Instance.PushAsync(new ErrorPopup());

            if (string.IsNullOrWhiteSpace(HomeSearchBarValue)) return;

            HomeNotFoundStringVisible = false;

            SearchResult = new List<GetHomesByNameModel>();

            var parameters = JsonConvert.SerializeObject(new { AccessToken = Utility.User.accessToken, Substring = HomeSearchBarValue });

            var response = await Helper.ApiCall<GetHomesByNameResponseModel>(RequestType.Post, ControllerType.Home, "gethomesbyname", parameters);

            if (response.responseVal == 0)
            {
                List<GetHomesByNameModel> buffer = new List<GetHomesByNameModel>();

                foreach (var item in response.requestedHomes)
                {
                    if (Utility.User.GetConnectedHomes()?.Contains(item.HomeID) ?? false) { }
                    else
                    {
                        buffer.Add(item);
                    }
                }
                if (buffer.Count == 0)
                {
                    HomeNotFoundStringVisible = true;
                }
                else
                {
                    SearchResult = buffer;
                }
            }
            else
            {
                //err
            }
        }
    }
}
