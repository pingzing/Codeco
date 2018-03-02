using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Codeco.CrossPlatform.CustomRenderers
{
    public class OptionalSelectionListView : ListView
    {
        public static readonly BindableProperty IsSelectionEnabledProperty = BindableProperty.Create(
            nameof(IsSelectionEnabled),
            typeof(bool),
            typeof(OptionalSelectionListView),
            defaultValue: true);

        public bool IsSelectionEnabled
        {
            get => (bool)GetValue(IsSelectionEnabledProperty);
            set => SetValue(IsSelectionEnabledProperty, value);
        }

        public OptionalSelectionListView(ListViewCachingStrategy strategy) : base(strategy)
        {

        }
    }
}
