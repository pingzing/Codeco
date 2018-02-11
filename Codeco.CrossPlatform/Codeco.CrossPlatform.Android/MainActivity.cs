using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using AndroidGraphics = Android.Graphics; //Avoid namespaace collision witih Codeco.CrossPlatform.Android
using Acr.UserDialogs;
using Rg.Plugins.Popup;
using Android.Util;
using Codeco.CrossPlatform.Droid.DependencyServices;

namespace Codeco.CrossPlatform.Droid
{
    [Activity(Label = "Codeco.CrossPlatform", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Popup.Init(this.ApplicationContext, bundle);
            UserDialogs.Init(this);

            // Populate accent color before we init the Xamarin runtime
            var themeAccentColor = new TypedValue();
            this.Theme.ResolveAttribute(Resource.Attribute.colorAccent, themeAccentColor, true);
            AccentColorService.ContextAccentColor = new AndroidGraphics.Color(themeAccentColor.Data);

            global::Xamarin.Forms.Forms.Init(this, bundle);            

            LoadApplication(new App());
        }
    }
}

