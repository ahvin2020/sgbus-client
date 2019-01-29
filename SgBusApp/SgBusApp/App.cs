using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TK.CustomMap;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static SgBusApp.AppConstants;

namespace SgBusApp {
    public class App : Application {
        public BusArrivalPage busArrivalPage;
        public BusStopLoader busStopLoader;

        // options
        public BusArrivalAlertEnum busArrivalAlertEnum;


        public App() {
            busArrivalPage = new BusArrivalPage();
            busStopLoader = new BusStopLoader();

            // The root page of your application
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }

        public void SetBusArrivalAlertEnum(BusArrivalAlertEnum alertEnum) {
            this.busArrivalAlertEnum = alertEnum;
            busArrivalPage.UpdateBusArrivalAlertValue(alertEnum);
        }
    }
}
