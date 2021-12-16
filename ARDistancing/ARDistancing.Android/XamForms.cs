using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using ARDistancing.Droid;
using ARDistancing.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms.Platform.Android;

namespace SAFETREES.Droid
{
    public static class XamForms
    {
        /// <summary>
        /// function to get current Context
        /// </summary>
        public static Context Context => CrossCurrentActivity.Current.AppContext;

        /// <summary>
        /// function to get current Activity
        /// </summary>
        public static Activity Activity => CrossCurrentActivity.Current.Activity;

#if __ANDROID_29__
        public static AndroidX.Fragment.App.FragmentManager SupportFragmentManager => CrossCurrentActivity.Current
            .Activity.As<MainActivity>(context: "XamForms").SupportFragmentManager;
#else
        public static Android.Support.V4.App.FragmentManager SupportFragmentManager => CrossCurrentActivity.Current
            .Activity.As<PlatformAppCompatMainActivity>(context: "XamForms").SupportFragmentManager;
#endif

        public static Drawable GetDrawable(string resourceName)
        {
            var context = CrossCurrentActivity.Current.AppContext;
            return context.GetDrawable(resourceName);
        }

        public static Drawable GetDrawable(int resourceId)
        {
            var context = CrossCurrentActivity.Current.AppContext;
            return context.GetDrawable(resourceId);
        }

        public static BuildVersionCodes GetSdkInt()
        {
            return Build.VERSION.SdkInt;
        }

        public static int GetColor(int resourceId)
        {
            var context = CrossCurrentActivity.Current.AppContext;
            return context.GetColor(resourceId);
        }
    }
}