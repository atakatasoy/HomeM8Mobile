using HomeM8.Models;
using HomeM8.Models.EventArguments;
using HomeM8.Views.PartialView;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeM8.Views.Account
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AccountPage : ContentPage
	{
        AccountPageViewModel viewModel;
		public AccountPage ()
		{
			InitializeComponent ();
            homeNameEntry.Name = "HomeName";
            homeAddressEntry.Name = "HomeAddress";
            noConnectionLabel.Text = noConnectionLabel.Text.Insert(0, "-");
            BindingContext = viewModel = new AccountPageViewModel();
            searchEntry.ReturnCommand = new Command(() => SearchButton(this, null));
		}

        private async void CreateButton(object sender,EventArgs e)
        {
            await CreateHomeAction();
        }

        async Task CreateHomeAction()
        {
            if (await viewModel.CreateHome() is CreateHomeResponseModel response)
            {
                if (response.responseVal == 0)
                {
                    var mainPage = Application.Current.MainPage as MainPage;
                    var homePage = (int)MenuItemType.Home;

                    await PopupNavigation.Instance.PushAsync(new ErrorPopup("Başarılı"));

                    Utility.User.AddHome(response.HomeID);
                    Utility.User.SetValue(nameof(Utility.User.CurrentHome), response.HomeID);

                    await mainPage.NavigateFromMenu(homePage);
                    mainPage.menuPage.ChangeSelectedItemWithoutForwarding(homePage);

                    viewModel.ResetCreateHomeArguments();
                }
                else
                {
                    await PopupNavigation.Instance.PushAsync(new ErrorPopup(response.responseText));
                }
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new ErrorPopup("Birşeyler ters gitti! Lütfen daha sonra tekrar deneyiniz"));
            }
        }

        private async void Entry_Returned(object sender,EntryEventArgs e)
        {
            if (e.Name == "HomeName")
            {
                homeAddressEntry.FocusEntry();
            }
            else
            {
                await CreateHomeAction();
            }
        }

        Grid lastExpanded = null;
        private void Label_Tapped(object sender,EventArgs e)
        {
            loading.IsVisible = false;
            stack.IsVisible = false;
            stackImage.Source = "../rightarrow.png";
            var parent = (Grid)sender;
            if (parent == lastExpanded) { }
            else if (lastExpanded != null)
            {
                CloseLastOpenedStack();
            }
            lastExpanded = parent;
            var scrollView = (ScrollView)parent.Children.FirstOrDefault(each => each.GetType() == typeof(ScrollView));
            var image = ((Image)parent.Children.FirstOrDefault(each => each.GetType() == typeof(Image)));
            scrollView.IsVisible = !scrollView.IsVisible;
            image.Source = (!scrollView.IsVisible) ? "../rightarrow.png" : "../downarrow.png";
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.SetAccountInfo();
            CloseLastOpenedStack();
            StackLayoutSetup();
        }
        private async void SearchButton(object sender,EventArgs e)
        {
            loading.IsVisible = true;
            loading.Play();
            await viewModel.GetHomesByName();
            loading.Pause();
            loading.IsVisible = false;
        }
        void StackLayoutSetup()
        {
            ClearStacksSetup();

            if (viewModel.AccountInfo.HomeName == null)
            {
                currentHomeInfoStack.IsVisible = false;
                noConnectionLabel.IsVisible = true;
            }
            else
            {
                currentHomeInfoStack.IsVisible = true;
                noConnectionLabel.IsVisible = false;
            }

            #region membersStack

            SetProfileStack();

            #endregion

            #region rulesStack

            var target = viewModel.AccountInfo.HomeRules;
            if (target == null || target.Count == 0) rulesStack.Children.Add(
                        new Label()
                        {
                            Text = AppConfigurationModel.Instance.AccountPageContent.NoRulesString,
                            Margin = new Thickness(13, 5, 0, 5),
                            TextColor = (Color)Application.Current.Resources["WarningColor"],
                            FontSize = 14,
                            VerticalTextAlignment = TextAlignment.Center
                        });
            else
            {
                int ruleCount = 1;
                foreach (var rules in viewModel.AccountInfo.HomeRules)
                {
                    rulesStack.Children.Add(
                        new StackLayout()
                        {
                            Spacing = 0,
                            Margin = new Thickness(13, 3, 0, 3),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                            new Label()
                            {
                                Text = $"{ruleCount})  " ,
                                FontSize =14,
                                VerticalTextAlignment =TextAlignment.Center
                            },
                            new Label()
                            {
                                Text = rules,
                                TextColor = (Color)Application.Current.Resources["WarningColor"],
                                FontSize =14 ,
                                VerticalTextAlignment =TextAlignment.Center
                            }
                            }
                        });
                    ruleCount++;
                }
            }

            #endregion

            #region permissionStack

            var targetStack = viewModel.AccountInfo.HomePermissions;
            if (targetStack == null || targetStack.Count == 0) permissionStack.Children.Add(
                   new Label()
                   {
                       Text = AppConfigurationModel.Instance.AccountPageContent.NoPermissionsString,
                       Margin = new Thickness(13, 5, 0, 5),
                       TextColor = (Color)Application.Current.Resources["PermissionColor"],
                       FontSize = 14,
                       VerticalTextAlignment = TextAlignment.Center
                   });
            else
            {
                int count = 1;
                foreach (var permission in viewModel.AccountInfo.HomePermissions)
                {
                    permissionStack.Children.Add(
                        new StackLayout()
                        {
                            Spacing = 0,
                            Margin = new Thickness(13, 3, 0, 3),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                            new Label()
                            {
                                Text = $"{count})  " ,
                                FontSize =14,
                                VerticalTextAlignment =TextAlignment.Center
                            },
                            new Label()
                            {
                                Text = permission,
                                TextColor = (Color)Application.Current.Resources["PermissionColor"],
                                FontSize =14 ,
                                VerticalTextAlignment =TextAlignment.Center
                            }
                            }
                        });
                }

            }

            #endregion

            #region homeStack

            SetHomesStack();

            #endregion
        }

        void SetProfileStack()
        {
            var target = viewModel.AccountInfo.HomeMembers;
            if (target == null || target.Count == 0) membersStack.Children.Add(
                           new Label()
                           {
                               Text = AppConfigurationModel.Instance.AccountPageContent.NoConnectedPersonString,
                               Margin = new Thickness(13, 5, 0, 5),
                               TextColor = (Color)Application.Current.Resources["NotificationOwnerTextColor"],
                               FontSize = 14,
                               VerticalTextAlignment = TextAlignment.Center
                           });
            else
            {
                foreach (var members in viewModel.AccountInfo.HomeMembers)
                {
                    int count = 1;
                    membersStack.Children.Add(
                        new StackLayout()
                        {
                            Spacing = 0,
                            Margin = new Thickness(13, 3, 0, 3),
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                            new Label()
                            {
                                Text = $"{count})  " ,
                                FontSize =14,
                                VerticalTextAlignment =TextAlignment.Center
                            },
                            new Label()
                            {
                                Text = members,
                                TextColor = (Color)Application.Current.Resources["NotificationOwnerTextColor"],
                                FontSize =14 ,
                                VerticalTextAlignment =TextAlignment.Center
                            }
                            }
                        });
                    count++;
                }
            }
        }

        void ClearStacksSetup()
        {
            viewModel.ClearSearchArguments();

            membersStack.Children.Clear();
            rulesStack.Children.Clear();
            permissionStack.Children.Clear();
            homeStack.Children.Clear();
        }

        void SetHomesStack()
        {
            var target = viewModel.AccountInfo.ConnectedHomesInfo;
            if (target == null || target.Count == 0 || target[0].HomeID == 0/*service returns anonymous type so if list is empty it will return one empty object in it*/) homeStack.Children.Add(
                              new Label()
                              {
                                  Text = $"-{AppConfigurationModel.Instance.ApplicationContent.NoConnectedHomeString}",
                                  Margin = new Thickness(13, 5, 0, 5),
                                  TextColor = (Color)Application.Current.Resources["WarningColor"],
                                  FontSize = 15,
                                  VerticalTextAlignment = TextAlignment.Center
                              });
            else
            {
                int count = 1;
                foreach (var home in viewModel.AccountInfo.ConnectedHomesInfo)
                {
                    bool IsCurrentHome = home.HomeName == viewModel.AccountInfo.HomeName;
                    var stack = new StackLayout()
                    {
                        Spacing = 0,
                        Margin = new Thickness(0, 0, 0, 0),
                        Orientation = StackOrientation.Horizontal,
                        Children =
                    {
                        new Grid()
                        {
                            BackgroundColor=(IsCurrentHome)?(Color)Application.Current.Resources["PermissionColor"]:Color.Transparent,
                            WidthRequest=2,
                            Margin=new Thickness(0,-3,6,-3),
                        },
                        new Label()
                        {
                            Text = $"{count})  " ,
                            FontSize =15 ,
                            VerticalTextAlignment =TextAlignment.Center
                        },
                        new Label()
                        {
                            Text = home.HomeName,
                            FontSize = 15,
                            VerticalTextAlignment =TextAlignment.Center
                        }
                    }
                    };
                    var grid = new Grid()
                    {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HeightRequest = 30,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                    {
                         new Label()
                         {
                             Text = (IsCurrentHome) ? AppConfigurationModel.Instance.AccountPageContent.CurrentHomeIndicatorString
                             : AppConfigurationModel.Instance.AccountPageContent.ChangeHomeIndicatorString,
                             TextColor = (IsCurrentHome) ? (Color)Application.Current.Resources["PermissionColor"]
                             : (Color)Application.Current.Resources["NotificationOwnerTextColor"],
                             VerticalOptions = LayoutOptions.Center,
                             HorizontalOptions = LayoutOptions.Center,
                             VerticalTextAlignment = TextAlignment.Center,
                             HorizontalTextAlignment = TextAlignment.Center
                         },
                    }
                    };
                    if (!IsCurrentHome)
                        grid.GestureRecognizers.Add(new TapGestureRecognizer()
                        {
                            Command = new Command(async () =>
                            {
                                var parameters = JsonConvert.SerializeObject(new
                                {
                                    AccessToken = Utility.User.accessToken,
                                    home.HomeID
                                });
                                Utility.ShowIndicator = true;

                                var response = await Helper.ApiCall<BaseResponseModel>(RequestType.Post, ControllerType.User, "changecurrenthome", parameters);

                                if (response.responseVal == 0)
                                {
                                    await viewModel.SetAccountInfo();
                                    StackLayoutSetup();
                                    Utility.User.SetValue(nameof(Utility.User.CurrentHome), home.HomeID);
                                    Utility.ShowIndicator = false;
                                }
                                else
                                {
                                    await PopupNavigation.Instance.PushAsync(new ErrorPopup(response.responseText));
                                }
                            })
                        });
                    Grid.SetColumn(stack, 0);
                    Grid.SetColumn(grid, 1);
                    homeStack.Children.Add(
                        new Grid()
                        {
                            Margin = new Thickness(8, 6, 8, 5),
                            ColumnDefinitions = new ColumnDefinitionCollection()
                            {
                            new ColumnDefinition()
                            {
                                Width = new GridLength(1, GridUnitType.Star)
                            },
                            new ColumnDefinition()
                            {
                                Width = GridLength.Auto
                            }
                            },
                            Children =
                            {
                                stack,
                                grid
                            }
                        });
                    count++;
                }
            }
        }

        private async void StackTapped(object sender,EventArgs e)
        {
            if (((StackLayout)sender) == add)
            {
                //open request
                await DisplayAlert("HomeM8", "İstekler", "Tamam");
            }
            else
            {
                if(await DisplayAlert("HomeM8","Evden ayrılmak istediğinize emin misiniz ?", "Evet", "Hayır"))
                {
                    Utility.ShowIndicator = true;

                    var response = await Helper.ApiCall<BaseResponseModel>(RequestType.Post, ControllerType.User, "leavehome", JsonConvert.SerializeObject(new
                    {
                        AccessToken = Utility.User.accessToken,
                        HomeID = Utility.User.CurrentHome
                    }));

                    Utility.ShowIndicator = false;

                    if (response.responseVal == 0)
                    {
                        await PopupNavigation.Instance.PushAsync(new ErrorPopup("Başarılı"));
                        //First we should delete home from the interface
                        //viewModel.DeleteHome((int)Utility.User.CurrentHome);
                        await viewModel.SetAccountInfo();
                        //then we delete it from the db
                        Utility.User.LeaveHome();
                        StackLayoutSetup();
                    }
                    else
                    {
                        await PopupNavigation.Instance.PushAsync(new ErrorPopup(response.responseText));
                    }
                }
            }
        }
        void CloseLastOpenedStack()
        {
            ScrollView previousScrollView = (ScrollView)lastExpanded?.Children.FirstOrDefault(each => each.GetType() == typeof(ScrollView));
            if (previousScrollView != null)
            {
                ((Image)lastExpanded.Children.FirstOrDefault(each => each.GetType() == typeof(Image))).Source = "../rightarrow.png";
                previousScrollView.IsVisible = false;
            }
        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            loading.IsVisible = false;
            if (lastExpanded != null)
            {
                CloseLastOpenedStack();
            }
            stack.IsVisible = !stack.IsVisible;
            stackImage.Source = (!stack.IsVisible) ? "../rightarrow.png" : "../downarrow.png";
        }
    }
}