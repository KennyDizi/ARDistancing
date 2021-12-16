using System.Reactive.Disposables;
using System.Reactive.Linq;
using ARDistancing.Views;
using ReactiveUI;
using System;

namespace ARDistancing
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            _ = Observable.FromEventPattern<ObjectMeasurement>(x => ARViewControl.DistancingDetected += x,
                    x => ARViewControl.DistancingDetected -= x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(e => ViewModel?.SetTreeMeasurement(e.EventArgs))
                .DisposeWith(_compositeDisposable);
        }
    }
}