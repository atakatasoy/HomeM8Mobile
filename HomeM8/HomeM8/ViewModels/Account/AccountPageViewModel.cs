using HomeM8.Views.PartialView;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public ObservableCollection<GetHomesByNameModel> SearchResult { get; set; }

        public string HomeSearchBarValue { get; set; }
        public string CreateHome_HomeName { get; set; }
        public string CreateHome_HomeAddress { get; set; }
        public bool HomeNotFoundStringVisible { get; set; }
        public ICommand CreateHome_CreateButtonCommand { get; set; }

        public AccountPageViewModel()
        {
            CreateHome_CreateButtonCommand = new Command(async () => await CreateHome());
        }

        public async Task<BaseResponseModel> CreateHome()
        {
            Utility.ShowIndicator = true;

            var response = await Helper.ApiCall<CreateHomeResponseModel>(RequestType.Post, ControllerType.Home, "createhome", JsonConvert.SerializeObject(new
            {
                AccessToken = Utility.User.accessToken,
                HomeName = CreateHome_HomeName,
                HomeAddress = CreateHome_HomeAddress
            }));

            Utility.ShowIndicator = false;
            return response;
        }

        public void ResetCreateHomeArguments()
        {
            CreateHome_HomeAddress = "";
            CreateHome_HomeName = "";
        }

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
                await PopupNavigation.Instance.PushAsync(new ErrorPopup(response.responseText));
            }
        }

        public void ClearSearchArguments()
        {
            HomeSearchBarValue = "";
            SearchResult = new ObservableCollection<GetHomesByNameModel>();
        }

        public async Task GetHomesByName()
        {
            if (string.IsNullOrWhiteSpace(HomeSearchBarValue)) return;

            HomeNotFoundStringVisible = false;

            SearchResult = new ObservableCollection<GetHomesByNameModel>();

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
                    SearchResult = new ObservableCollection<GetHomesByNameModel>(buffer);
                }
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new ErrorPopup(response.responseText));
            }
        }
    }
}
