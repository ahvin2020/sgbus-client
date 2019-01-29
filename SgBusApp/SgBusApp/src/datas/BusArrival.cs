using SimpleJSON;
using System;

namespace SgBusApp {
    public class BusArrival : Object {
        public long id;
        public string busStopCode;
        public string serviceNo;
        public int status;
        public string busOperator;
        public string originatingId;
        public string terminatingId;
        public string terminatingDescription;

        public float[] estimatedArrivals;
        public float[] latitude;
        public float[] longitude;
        public int[] visitNumber;
        public int[] load;
        public string[] feature;

        public BusArrival(JSONNode jsonNode) {
            id = jsonNode["id"].AsLong;
            busStopCode = jsonNode["bus_stop_code"].Value;
            serviceNo = jsonNode["service_no"].Value;
            status = jsonNode["status"].AsInt;
            busOperator = jsonNode["operator"].Value;
            originatingId = jsonNode["originating_id"].Value;
            terminatingId = jsonNode["terminating_id"].Value;
            terminatingDescription = jsonNode["terminating_description"].Value;

            estimatedArrivals = new float[3] { -1f, -1f, -1f };
            latitude = new float[3] { -1f, -1f, -1f };
            longitude = new float[3] { -1f, -1f, -1f };
            visitNumber = new int[3] { -1, -1, -1 };
            load = new int[3] { -1, -1, -1 };
            feature = new string[3] { null, null, null };

            // first arrival
            if (jsonNode["1_load"] != null) {
                estimatedArrivals[0] = jsonNode["1_estimated_arrival"] != null ? jsonNode["1_estimated_arrival"].AsFloat : -1f;
                latitude[0] = jsonNode["1_latitude"] != null ? jsonNode["1_latitude"].AsFloat : -1f;
                longitude[0] = jsonNode["1_longitude"] != null ? jsonNode["1_longitude"].AsFloat : -1f;
                visitNumber[0] = jsonNode["1_visit_number"] != null ? jsonNode["1_visit_number"].AsInt : -1;
                load[0] = jsonNode["1_load"].AsInt;
                feature[0] = jsonNode["1_feature"] != null ? jsonNode["1_feature"].Value : null;
            }

            // second arrival
            if (jsonNode["2_load"] != null) {
                estimatedArrivals[1] = jsonNode["2_estimated_arrival"] != null ? jsonNode["2_estimated_arrival"].AsFloat : -1f;
                latitude[1] = jsonNode["2_latitude"] != null ? jsonNode["2_latitude"].AsFloat : -1f;
                longitude[1] = jsonNode["2_longitude"] != null ? jsonNode["2_longitude"].AsFloat : -1f;
                visitNumber[1] = jsonNode["2_visit_number"] != null ? jsonNode["2_visit_number"].AsInt : -1;
                load[1] = jsonNode["2_load"].AsInt;
                feature[1] = jsonNode["2_feature"] != null ? jsonNode["2_feature"].Value : null;
            }

            // third arrival
            if (jsonNode["3_load"] != null) {
                estimatedArrivals[2] = jsonNode["3_estimated_arrival"] != null ? jsonNode["3_estimated_arrival"].AsFloat : -1f;
                latitude[2] = jsonNode["3_latitude"] != null ? jsonNode["3_latitude"].AsFloat : -1f;
                longitude[2] = jsonNode["3_longitude"] != null ? jsonNode["3_longitude"].AsFloat : -1f;
                visitNumber[2] = jsonNode["3_visit_number"] != null ? jsonNode["3_visit_number"].AsInt : -1;
                load[2] = jsonNode["3_load"].AsInt;
                feature[2] = jsonNode["3_feature"] != null ? jsonNode["3_feature"].Value : null;
            }
        }
    }
}
