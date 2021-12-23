using System;
using ReactiveUI;
using Sextant;
using Splat;

namespace ARDistancing.Views
{
    public abstract class BasePageViewModel : ReactiveObject, IViewModel
    {
        protected IParameterViewStackService _viewStackService;

        protected BasePageViewModel()
        {
            _viewStackService = Locator
                .Current
                .GetService<IParameterViewStackService>();
            Id = new Random().Next().ToString();
        }

        public string Id { get; }
    }
}