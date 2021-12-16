using System.Windows.Input;
using ARDistancing.Views;
using ReactiveUI;
using Xamarin.Forms;

namespace ARDistancing
{
    internal class MainPageViewModel : BasePageViewModel
    {
        private readonly ObjectMeasurement _objectMeasurement;

        internal MainPageViewModel()
        {
            _objectMeasurement = new ObjectMeasurement();

            OnSelectImageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (_objectMeasurement is { Distancing: > 0 })
                {
                    var mainPage = Application.Current.MainPage;
                    if (mainPage != null)
                        await mainPage.DisplayAlert(title: "Object Distancing Detected",
                            message: $"{_objectMeasurement.Distancing:#.000} inches", cancel: "OK");
                }
            });
        }

        internal void SetTreeMeasurement(ObjectMeasurement treeMeasurement)
        {
            _objectMeasurement.Distancing = treeMeasurement.Distancing;
            _objectMeasurement.Direction = treeMeasurement.Direction;
        }

        public ICommand OnSelectImageCommand { get; }
    }
}