using System.Threading.Tasks;
using Crosschat.Client.Model.Contracts;
using Crosschat.Client.Model.Managers;
using Crosschat.Client.Seedwork;
using Crosschat.Client.ViewModels;
using Crosschat.Server.Application.DataTransferObjects.Enums;
using Crosschat.Server.Infrastructure.Protocol;
using Xamarin.Forms;

namespace Crosschat.Client.Views
{
    public class SplashscreenPage : ContentPage
    {
        private static ApplicationManager _applicationManager;

        public SplashscreenPage()
        {
            Title = "";

            Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                        {
                            new Label
                                {
                                    Text = "Connecting...", 
                                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                                    Font = Font.BoldSystemFontOfSize(24),
                                },
                            new ActivityIndicator 
                                {
                                    IsRunning = true, 
                                }
                        }
                };

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);
        }

        protected override async void OnAppearing()
        {
            if (_applicationManager == null)
            {
                _applicationManager = new ApplicationManager(
                    DependencyService.Get<ITransportResource>(),
                    DependencyService.Get<IDtoSerializer>(),
                    DependencyService.Get<IStorage>(),
                    DependencyService.Get<IDeviceInfo>());
                _applicationManager.ConnectionManager.ConnectionDropped += () => Navigation.PushAsync(new SplashscreenPage());
            }

            AuthenticationResponseType result;

            try
            {
                result = await _applicationManager.AccountManager.ValidateAccount();
            }
            catch (System.Exception)
            {
                DisplayAlert(";(", "Server is not available", "Ok", null);
                return;
            }

            if (result == AuthenticationResponseType.Success)
            {
                await Navigation.PushAsync(new HomePage(new HomeViewModel(_applicationManager)));
            }
            else
            {
                await Navigation.PushAsync(new RegistrationPage(new RegistrationViewModel(_applicationManager)));
            }

            base.OnAppearing();
        }

    }
}
