using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SgBusApp {
    public class BusStop : Object {
        public string busStopCode;
        public string roadName;
        public string description;
        public float longitude;
        public float latitude;
        public float distance;

        public List<BusArrival> busArrivals;

        public BusStop(JSONNode jsonNode) {
            busStopCode = jsonNode["bus_stop_code"].Value;
            roadName = jsonNode["road_name"] != null ? jsonNode["road_name"].Value : "";
            description = jsonNode["description"] != null ? jsonNode["description"].Value : "";
            longitude = jsonNode["longitude"] != null ? jsonNode["longitude"].AsFloat : -1;
            latitude = jsonNode["latitude"] != null ? jsonNode["latitude"].AsFloat : -1;
            distance = jsonNode["distance"] != null ? jsonNode["distance"].AsFloat : -1;

            busArrivals = new List<BusArrival>();
        }
    }
}
