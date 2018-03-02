using Android.Content;
using Xamarin.Forms.Platform.Android;
using XF = Xamarin.Forms;
using AndroidWidget = Android.Widget;
using AndroidResource = Android.Resource;
using AndroidGraphics = Android.Graphics;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using Codeco.CrossPlatform.CustomRenderers;
using System.ComponentModel;
using Codeco.CrossPlatform.Droid.CustomRenderer;

[assembly: XF.ExportRenderer(typeof(OptionalSelectionListView), typeof(OptionalSelectionListViewRenderer))]
namespace Codeco.CrossPlatform.Droid.CustomRenderer
{
    public class OptionalSelectionListViewRenderer : ListViewRenderer
    {
        private Drawable _originalHighlightSelector;
        private AndroidGraphics.Color _originalCacheColorHint;

        public OptionalSelectionListViewRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<XF.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                //really, is there anything to clean up?
            }

            if (e.NewElement != null)
            {
                var listView = Control as AndroidWidget.ListView;

                // cache the original highlight color and cachecolorhint in case we need them later
                _originalHighlightSelector = listView.Selector;
                _originalCacheColorHint = listView.CacheColorHint;                

                var xfList = e.NewElement as OptionalSelectionListView;

                if (!xfList.IsSelectionEnabled)
                {
                    var transparentColorResourceId = ContextCompat.GetColor(this.Context, AndroidResource.Color.Transparent);
                    var transparentColor = new AndroidGraphics.Color(transparentColorResourceId);
                    listView.SetSelector(AndroidResource.Color.Transparent);
                    listView.CacheColorHint = transparentColor;
                }
                else
                {
                    // Do nothing, as this is the default.
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == OptionalSelectionListView.IsSelectionEnabledProperty.PropertyName)
            {
                var listView = Control as AndroidWidget.ListView;
                var xfList = Element as OptionalSelectionListView;

                if (!xfList.IsSelectionEnabled)
                {
                    var transparentColorResourceId = ContextCompat.GetColor(this.Context, AndroidResource.Color.Transparent);
                    var transparentColor = new AndroidGraphics.Color(transparentColorResourceId);
                    listView.SetSelector(AndroidResource.Color.Transparent);
                    listView.CacheColorHint = transparentColor;
                }
                else
                {
                    listView.Selector = _originalHighlightSelector;
                    listView.CacheColorHint = _originalCacheColorHint;
                }
            }
        }
    }
}
