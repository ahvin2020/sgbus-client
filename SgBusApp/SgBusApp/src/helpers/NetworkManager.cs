using System;
using System.Threading.Tasks;
using SimpleJSON;
using System.Net;
using System.Net.Http;

namespace SgBusApp {
    public class NetworkManager {
        private static NetworkManager instance;

        public static string ERROR_KEY = "Error";
        public static string MSG_KEY = "Msg";

        public static NetworkManager Instance {
            get {
                if (instance == null) {
                    instance = new NetworkManager();
                }
                return instance;
            }
        }

        public async Task<JSONNode> CreateGet(string url) {
            JSONNode resData = null;
            bool hasRetried = false;

            // create request
            using (HttpClient client = new HttpClient()) {
                client.Timeout = TimeSpan.FromMilliseconds(AppConstants.WEB_TIMEOUT);
                client.BaseAddress = new Uri(AppConstants.WEB_URL);

                retry:
                try {
                    HttpResponseMessage result = await client.GetAsync(url);
                    if (result.IsSuccessStatusCode && result.StatusCode == HttpStatusCode.OK) {
                        string jsonString = await result.Content.ReadAsStringAsync();
                        resData = JSON.Parse(jsonString);
                    } else {
                        // didn't manage to get data
                        resData = GetErrorJson(result.ReasonPhrase);
                    }
                } catch (TaskCanceledException ex) {
                    if (!hasRetried) {
                        hasRetried = true;
                        goto retry; // retry again
                    } else {
                        resData = GetErrorJson("Timeout occured. Please try again later.");
                    }
                } catch (Exception ex) {
                    // catch the remaining exception
                    resData = GetErrorJson(ex.Message);
                }
            }

            return resData;
        }

        private JSONClass GetErrorJson(string message) {
            JSONClass json = new JSONClass();
            json[ERROR_KEY].AsInt = 1;
            json[MSG_KEY] = message;
            return json;
        }
    }
}