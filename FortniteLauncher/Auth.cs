using System;
using System.Text.Json;
using RestSharp;

namespace FortniteLauncher
{
    public partial class Auth
    {
        public static string GetToken(string authCode)
        {
            Console.WriteLine("Requesting access token...");
            var client = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Authorization", "basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", authCode);

            string reply = client.Execute(request).Content;
            Console.WriteLine(reply);
            var TokenAcceso = JsonSerializer.Deserialize<Token>(reply);
            if (TokenAcceso.AccessToken != null)
            {
                Console.WriteLine("Access token successfully generated");
                return TokenAcceso.AccessToken;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(TokenAcceso.ErrorCode);
            System.Threading.Thread.Sleep(5000);
            Environment.Exit(0);
            return "error";
        }

        public static string GetExchange(string token)
        {
            Console.WriteLine("Requesting exchange code...");
            var client = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");
            var request = new RestRequest(Method.GET);

            request.AddHeader("Authorization", $"bearer {token}");

            string reply = client.Execute(request).Content;
            var exchangeCode = JsonSerializer.Deserialize<Exchange>(reply);
            Console.WriteLine("Exchange code successfully generated");
            return exchangeCode.Code;
        }

        /*
        public static bool KillToken(string token)
        {
            Console.WriteLine("Killing access token...");
            var client = new RestClient($"https://account-public-service-prod.ol.epicgames.com/account/api/oauth/sessions/kill/{token}");
            var request = new RestRequest(Method.DELETE);
            request.AddHeader("Authorization", $"bearer {token}");
            string reply = client.Execute(request).Content;
            Console.WriteLine(reply);
            Console.WriteLine("Access token killed");
            return true;
        }*/
    }
}
