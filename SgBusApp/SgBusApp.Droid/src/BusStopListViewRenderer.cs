using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SgBusApp;
using SgBusApp.Droid;
using Android.Widget;
using static SgBusApp.Droid.AnimatedExpandableListView;

[assembly: ExportRenderer(typeof(BusStopListView), typeof(BusStopListViewRenderer))]
namespace SgBusApp.Droid {
    public class BusStopListViewRenderer : ViewRenderer<BusStopListView, AnimatedExpandableListView> {

        private BusStopDataAdapter busStopDataAdapter;

        protected override void OnElementChanged(ElementChangedEventArgs<BusStopListView> elementChangedEvent) {
            base.OnElementChanged(elementChangedEvent);

            if (Control == null) {
                SetNativeControl(new AnimatedExpandableListView(Forms.Context));
            }

            if (elementChangedEvent.OldElement != null) {
                // unsubscribe
                Control.GroupClick -= OnGroupClick;
                Control.ChildClick -= OnChildClick;
            }

            if (elementChangedEvent.NewElement != null) {
                if (busStopDataAdapter == null) {
                    busStopDataAdapter = new BusStopDataAdapter(Forms.Context as Android.App.Activity);
                    busStopDataAdapter.DataList = elementChangedEvent.NewElement.Items;
                    Control.SetAdapter(busStopDataAdapter);

                    //Element.OnBusArrivalsLoadedFunc += OnBusArrivalsLoaded;
                }
                Control.GroupClick += OnGroupClick;
                Control.ChildClick += OnChildClick;
                
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == BusStopListView.ItemsProperty.PropertyName) {
                busStopDataAdapter.DataList = Element.Items;
                busStopDataAdapter.NotifyDataSetChanged();
            }
        }

        private void OnGroupClick(object sender, ExpandableListView.GroupClickEventArgs e) {
            if (Control.IsGroupExpanded(e.GroupPosition)) {
                Control.CollapseGroupWithAnimation(e.GroupPosition);
            } else {
                if (Element.LoadBusArrivalsFunc != null) {
                    Element.LoadBusArrivalsFunc(e.GroupPosition, OnBusArrivalsLoaded);
                }
            }
        }

        private void OnChildClick(object sender, ExpandableListView.ChildClickEventArgs e) {
            Element.OnBusArrivalClickedFunc(e.GroupPosition, e.ChildPosition);
        }

        public void OnBusArrivalsLoaded(int groupPosition) {
            Control.ExpandGroupWithAnimation(groupPosition);
        }
    }
}