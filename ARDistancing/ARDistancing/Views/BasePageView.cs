using System.Reactive.Disposables;
using ReactiveUI.XamForms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace ARDistancing.Views
{
    public class BasePageView<T> : ReactiveContentPage<T> where T : BasePageViewModel
    {
        public readonly CompositeDisposable _compositeDisposable;

        public BasePageView()
        {
            _compositeDisposable = new CompositeDisposable();
            _ = On<iOS>().SetUseSafeArea(false);
        }
    }
}