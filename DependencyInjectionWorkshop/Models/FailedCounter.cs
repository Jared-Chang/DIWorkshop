using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public class FailedCounter
    {
        public FailedCounter()
        {
        }

        public void Increase(string accountId, HttpClient httpClient)
        {
            var response = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            response.EnsureSuccessStatusCode();
        }

        public void Reset(string accountId, HttpClient httpClient)
        {
            var response = httpClient.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            response.EnsureSuccessStatusCode();
        }

        public bool IsLocked(string accountId, HttpClient httpClient)
        {
            var isLockedResponse = httpClient.PostAsJsonAsync("api/failedCounter/IsLocked", accountId).Result;

            isLockedResponse.EnsureSuccessStatusCode();
            var isLocked = isLockedResponse.Content.ReadAsAsync<bool>().Result;
            return isLocked;
        }

        public int FailedCount(string accountId, HttpClient httpClient)
        {
            var failedCountResponse =
                httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;

            failedCountResponse.EnsureSuccessStatusCode();

            var failedCount = failedCountResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }
    }
}