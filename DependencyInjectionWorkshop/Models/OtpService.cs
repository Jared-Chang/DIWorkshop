using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Models
{
    public interface IOtpService
    {
        string CurrentOtp(string accountId, string otp);
    }

    public class OtpService : IOtpService
    {
        public OtpService()
        {
        }

        public string CurrentOtp(string accountId, string otp)
        {
            var response = new HttpClient() {BaseAddress = new Uri("http://joey.com/")}.PostAsJsonAsync("api/otps", accountId).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            return response.Content.ReadAsAsync<string>().Result;
        }
    }
}