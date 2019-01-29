using System;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using static SgBusApp.Droid.AnimatedExpandableListView;
using Android.Content;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;

namespace SgBusApp.Droid {
    public class BusStopDataAdapter : AnimatedExpandableListAdapter {
        readonly Activity Context;
        private List<BusStop> dataList;

        /*
        public BusStopDataAdapter(Activity newContext, List<BusStop> newList) : base() {
            Context = newContext;
            DataList = newList;
        }
        */

        public BusStopDataAdapter(Activity newContext) : base() {
            Context = newContext;
        }

        public List<BusStop> DataList {
            get {
                return dataList;
            }

            set {
                dataList = value;
            }
        }

        public override int GroupCount {
            get {
                if (DataList != null) {
                    return DataList.Count;
                } else {
                    return 0;
                }
            }
        }

        public override bool HasStableIds {
            get {
                return true;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition) {
            throw new NotImplementedException();
            //return DataList[groupPosition].busArrivals[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition) {
            return childPosition;
        }

        public override int GetRealChildrenCount(int groupPosition) {
            return DataList[groupPosition].busArrivals.Count;
        }

        public override View GetRealChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent) {
            View row = convertView;
            if (row == null) {
                row = Context.LayoutInflater.Inflate(Resource.Layout.BusArrivalListItem, null);
            }

            BusArrival busArrival = DataList[groupPosition].busArrivals[childPosition];

            // TODO: handle for no bus services
            row.FindViewById<TextView>(Resource.Id.BusArrivalItemServiceNo).Text = busArrival.serviceNo;

            //string busArrivalTimings = "";

            TextView busArrivalTimingTextView = row.FindViewById<TextView>(Resource.Id.BusArrivalItemTiming);
            busArrivalTimingTextView.Text = "";

            SpannableString span;

            // has bus timings?
            if (busArrival.estimatedArrivals[0] >= 0) {
                for (int i = 0; i < busArrival.estimatedArrivals.Length; i++) {
                    if (busArrival.estimatedArrivals[i] >= 0) {
                        int fontSize = 2;
                        if (i != 0) {
                            fontSize = 1;

                            // create span for comma
                            span = new SpannableString(", ");
                            span.SetSpan(new ForegroundColorSpan(Color.Gray), 0, span.Length(), SpanTypes.ExclusiveExclusive);
                            busArrivalTimingTextView.Append(span);
                        }

                        // create span for bus arrival
                        span = new SpannableString(((int)busArrival.estimatedArrivals[i]).ToString());
                        span.SetSpan(new ForegroundColorSpan(Color.ParseColor(AppConstants.LOAD_COLORS[busArrival.load[i]])), 0, span.Length(), SpanTypes.ExclusiveExclusive);
                        span.SetSpan(new RelativeSizeSpan(fontSize), 0, span.Length(), SpanTypes.ExclusiveExclusive);
                        
                        busArrivalTimingTextView.Append(span);
                    }
                }
            } else {
                span = new SpannableString("no timing available");
                span.SetSpan(new ForegroundColorSpan(Color.Gray), 0, span.Length(), SpanTypes.ExclusiveExclusive);
                busArrivalTimingTextView.Append(span);
            }

            //System.Diagnostics.Debug.WriteLine(busArrivalTimings);
            //row.FindViewById<TextView>(Resource.Id.BusArrivalItemTiming).TextFormatted = Html.FromHtml(busArrivalTimings);
            row.FindViewById<TextView>(Resource.Id.BusArrivalItemDestination).Text = busArrival.terminatingDescription;

            return row;
        }

        public override Java.Lang.Object GetGroup(int groupPosition) {
            //return DataList[groupPosition];
            throw new NotImplementedException();
        }

        public override long GetGroupId(int groupPosition) {
            return groupPosition;
        }

        // show bus stops
        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent) {
            View header = convertView;
            if (header == null) {
                header = Context.LayoutInflater.Inflate(Resource.Layout.BusStopListGroup, null);
            }

            BusStop busStop = DataList[groupPosition];
            header.FindViewById<TextView>(Resource.Id.BusStopHeaderDescription).Text = busStop.description;

            TextView busStopHeaderDetails = header.FindViewById<TextView>(Resource.Id.BusStopHeaderDetails);
            if (busStop.distance >= 0) {
                int distance = (int)(busStop.distance * 100);
                busStopHeaderDetails.Text = string.Format("{0}  \u2022  ", distance);
            } else {
                busStopHeaderDetails.Text = "";
            }
            busStopHeaderDetails.Append(string.Format("{0}  \u2022  {1}", busStop.busStopCode, busStop.roadName));

            header.FindViewById<TextView>(Resource.Id.BusStopHeaderServices).Text = string.Format("-");

            return header;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition) {
            return true;
        }

        /*
        public override Android.Views.View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent) {
            View header = convertView;
            if (header == null) {
                header = Context.LayoutInflater.Inflate(Resource.Layout.NativeListViewCell, null);
            }
            header.FindViewById<TextView>(Resource.Id.txtView).Text = DataList[groupPosition].Title;
            return header;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent) {
            View row = convertView;
            if (row == null) {
                row = Context.LayoutInflater.Inflate(Resource.Layout.ChildCell, null);
            }
            List<EntityClass> newValue = new List<EntityClass>();
            GetChildViewHelper(groupPosition, out newValue);
            row.FindViewById<TextView>(Resource.Id.txtTitle).Text = newValue[childPosition].Title;
            return row;
        }

        public override int GetChildrenCount(int groupPosition) {
            var results = DataList[groupPosition].ChildItems == null ? 0 : DataList[groupPosition].ChildItems.Count;
            return results;
        }

        public override int GroupCount {
            get {
                return DataList.Count;
            }
        }

        private void GetChildViewHelper(int groupPosition, out List<EntityClass> Value) {
            var results = DataList[groupPosition].ChildItems;
            Value = results;
        }

        #region implemented abstract members of BaseExpandableListAdapter

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition) {

            throw new NotImplementedException();
        }

        public override long GetChildId(int groupPosition, int childPosition) {
            return childPosition;
        }

        public override Java.Lang.Object GetGroup(int groupPosition) {
            throw new NotImplementedException();
        }

        public override long GetGroupId(int groupPosition) {
            return groupPosition;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition) {
            var item = DataList[groupPosition].ChildItems[childPosition];
            if (item.OnClickListener != null) {
                item.OnClickListener.Invoke(item);
            }
            //			return item.IsSelected = !item.IsSelected;
            return true;
        }

        public override bool HasStableIds {
            get {
                return true;
            }
        }

        #endregion
        */
    }
}