using System.Windows.Input;
using ARDistancing.Views;
using ReactiveUI;
using System;
using Xamarin.Essentials;

namespace ARDistancing
{
    public class StartPageViewModel : BasePageViewModel
    {
        public StartPageViewModel()
        {
            GotoArPageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var permissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    _viewStackService?.PushPage<MainPageViewModel>(animate: true).Subscribe();
                }
                else if (permissionStatus == PermissionStatus.Denied)
                {
                    var requestStatus = await Permissions.RequestAsync<Permissions.Camera>();
                    if (requestStatus == PermissionStatus.Granted)
                    {
                        _viewStackService?.PushPage<MainPageViewModel>(animate: true).Subscribe();
                    }
                }
            });
        }

        public ICommand GotoArPageCommand { get; }
    }
}