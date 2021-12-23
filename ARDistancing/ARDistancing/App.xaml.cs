using System;
using ReactiveUI;
using Sextant;
using Sextant.XamForms;
using Splat;

namespace ARDistancing
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            Locator
                .CurrentMutable
                .RegisterNavigationView(() =>
                    new NavigationView(RxApp.MainThreadScheduler, RxApp.TaskpoolScheduler, ViewLocator.Current))
                .RegisterParameterViewStackService()
                .RegisterViewForNavigation(() => new MainPage(), () => new MainPageViewModel())
                .RegisterViewForNavigation(() => new StartPageView(), () => new StartPageViewModel());

            Locator
                .Current
                .GetService<IParameterViewStackService>()
                ?.PushPage(new StartPageViewModel(), null, true, false)
                .Subscribe();

            MainPage = Locator.Current.GetNavigationView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}