using XF = Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;
using Codeco.CrossPlatform.CustomRenderers;
using System.ComponentModel;
using Codeco.Windows10.CustomRenderers;

[assembly: ExportRenderer(typeof(OptionalSelectionListView), typeof(OptionalSelectionListViewRenderer))]
namespace Codeco.Windows10.CustomRenderers
{
    public class OptionalSelectionListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<XF.ListView> e)
        {
            base.OnElementChanged(e);            

            if (e.OldElement != null)
            {
                // clean up stuff?
            }

            if (e.NewElement != null)
            {
                var listView = Control as ListView;
                var xfList = e.NewElement as OptionalSelectionListView;
                listView.SelectionMode = xfList.IsSelectionEnabled ? ListViewSelectionMode.Single : ListViewSelectionMode.None;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == OptionalSelectionListView.IsSelectionEnabledProperty.PropertyName)
            {
                var listView = Control as ListView;
                var xfList = Element as OptionalSelectionListView;
                listView.SelectionMode = xfList.IsSelectionEnabled ? ListViewSelectionMode.Single : ListViewSelectionMode.None;
            }
        }
    }
}
