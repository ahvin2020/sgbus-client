using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SgBusApp {
    public class BusStopLoader {
        private AlphanumComparatorFast busArrivalComparator;

        public BusStopLoader() {
            busArrivalComparator = new AlphanumComparatorFast();
        }

        public async Task<List<BusStop>> SearchBusStops(ContentPage page, string searchText) {
            string url = string.Format("/busstops/search/{0}", searchText);
            JSONNode jsonNode = await NetworkManager.Instance.CreateGet(url);
            return await HandleBusStopJson(page, jsonNode);
        }

        public async Task<List<BusStop>> LoadNearbyBusStops(ContentPage page, double longitude, double latitude) {
            string url = string.Format("/busstops/nearby/{0}/{1}", longitude, latitude);
            JSONNode jsonNode = await NetworkManager.Instance.CreateGet(url);
            return await HandleBusStopJson(page, jsonNode);
        }

        private async Task<List<BusStop>> HandleBusStopJson(ContentPage page, JSONNode jsonNode) {
            if (jsonNode[NetworkManager.ERROR_KEY] != null) {
                await page.DisplayAlert("Error", jsonNode[NetworkManager.MSG_KEY], "OK");
                return null;
            } else {
                List<BusStop> busStops = new List<BusStop>();

                JSONArray busStopsJson = jsonNode["BusStops"] as JSONArray;
                foreach (JSONNode busStopJson in busStopsJson) {
                    BusStop busStop = new BusStop(busStopJson);
                    busStops.Add(busStop);
                }

                return busStops;
            }
        }

        public async Task<List<BusArrival>> LoadBusArrivals(ContentPage page, string busStopCode) {
            string url = string.Format("/busarrivals/get/{0}", busStopCode);
            JSONNode jsonNode = await NetworkManager.Instance.CreateGet(url);

            if (jsonNode[NetworkManager.ERROR_KEY] != null) {
                await page.DisplayAlert("Error", jsonNode[NetworkManager.MSG_KEY], "OK");
                return null;
            } else {
                List<BusArrival> busArrivals = new List<BusArrival>();

                JSONArray busArrivalsJson = jsonNode["BusArrivals"] as JSONArray;
                foreach (JSONNode busArrivalJson in busArrivalsJson) {
                   BusArrival busArrival = new BusArrival(busArrivalJson);
                   busArrivals.Add(busArrival);
                }

                // sort this
                busArrivals.Sort((s1, s2) =>
                {
                    return busArrivalComparator.Compare(s1.serviceNo, s2.serviceNo);
                });

                return busArrivals;
            }
        }



    }
}
