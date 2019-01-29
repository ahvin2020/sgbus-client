using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace SgBusApp {
    public class BusStopListView : View {
        public delegate void LoadBusArrivals(int busStopIndex, Action<int> OnbusArrivalsLoaded);
        public delegate void OnBusArrivalClicked(int busStopIndex, int busArrivalIndex);

        public LoadBusArrivals LoadBusArrivalsFunc;
        public OnBusArrivalClicked OnBusArrivalClickedFunc;
        
        //public delegate void OnBusArrivalsLoaded(int busStopIndex);
        //public OnBusArrivalsLoaded OnBusArrivalsLoadedFunc;

        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create("Items", typeof(List<BusStop>), typeof(BusStopListView), new List<BusStop>());

        public List<BusStop> Items {
            get { return (List<BusStop>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;

        public void NotifyItemSelected(object item) {
            //ItemSelected?.Invoke(this, new SelectedItemChangedEventArgs(item));
            if (ItemSelected != null) {
                ItemSelected(this, new SelectedItemChangedEventArgs(item));
            }
        }
    }
}