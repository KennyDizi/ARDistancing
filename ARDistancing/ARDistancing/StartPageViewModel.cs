using System.Windows.Input;
using ARDistancing.Views;
using ReactiveUI;
using System;
using Sextant;

namespace ARDistancing
{
    public class StartPageViewModel : BasePageViewModel
    {
        public StartPageViewModel()
        {
            GotoArPageCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                // todo ask camera permission
                // todo navigate to AR Page, integration with Setant
                _viewStackService?.PushPage<MainPageViewModel>(animate: true).Subscribe();
            });
        }

        public ICommand GotoArPageCommand { get; }
    }
}