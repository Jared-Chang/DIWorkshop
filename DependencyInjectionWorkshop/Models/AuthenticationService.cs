using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using Dapper;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        public bool Varify(string accountId, string plainText, string otp)
        {
            var password = "";
            using (var connection = new SqlConnection("my connection string"))
            {
                password = connection.Query<string>("spGetUserPassword", new {Id = accountId},
                    commandType: CommandType.StoredProcedure).SingleOrDefault();
            }

            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            var hashedPlainText = hash.ToString();

            var httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.com/")};
            var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
            var currentOtp = "";
            if (response.IsSuccessStatusCode)
            {
                currentOtp = response.Content.ReadAsAsync<string>().Result;
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            if (password == hashedPlainText && currentOtp == otp)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}