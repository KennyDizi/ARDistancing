using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ARDistancing.Utils
{
    public class ImgBtnClickedTriggerAction : TriggerAction<ImageButton>
    {
        public ImgBtnClickedTriggerAction()
        {
        }

        protected override async void Invoke(ImageButton sender)
        {
            await sender.ScaleTo(0.9, easing: Easing.BounceOut);
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            await sender.ScaleTo(1, easing: Easing.SinIn);
            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }
    }
}
