using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using RestSharp;

namespace FortniteLauncher
{
    internal class Programa
    {
        public static string GenerarToken(string authCode)
        {
            Console.WriteLine("Requesting access token...");
            var cliente = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token");
            var peticion = new RestRequest(Method.POST);

            peticion.AddHeader("Authorization", "basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=");
            peticion.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            peticion.AddParameter("grant_type", "authorization_code");
            peticion.AddParameter("code", authCode);

            string respuesta = cliente.Execute(peticion).Content;
            var TokenAcceso = JsonSerializer.Deserialize<Token>(respuesta);
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

        public static string GenerarExchange(string token)
        {
            Console.WriteLine("Requesting exchange code...");
            var cliente = new RestClient("https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange");
            var peticion = new RestRequest(Method.GET);

            peticion.AddHeader("Authorization", $"bearer {token}");

            string respuesta = cliente.Execute(peticion).Content;
            var exchangeCode = JsonSerializer.Deserialize<Exchange>(respuesta);
            Console.WriteLine("Exchange code successfully generated");
            return exchangeCode.Code;
        }

        static void Main(string[] args)
        {
            string token = "unused";
            string exchange = "unused";
            string authType = "unused";
            Console.WriteLine("Auth? Y | N");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("Authorization requiered:");
                token = GenerarToken(Console.ReadLine());
                exchange = GenerarExchange(token);
                authType = "exchangecode";
            }

            Process _fnProceso = new Process
            {
                StartInfo =
                {
                    FileName = "FortniteClient-Win64-Shipping",
                    Arguments = $"-obfuscationid=CPq5rJkwv1mtzq9tgkidFHE_L9wZqg -AUTH_LOGIN=unused -AUTH_PASSWORD={exchange} -AUTH_TYPE={authType} -epicapp=Fortnite -epicenv=Prod -EpicPortal -noeac -nobe -fltoken=fdd9g715h4i20110dd40d7d3",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            _fnProceso.Start();
            AsyncStreamReader asyncOutputReader = new AsyncStreamReader(_fnProceso.StandardOutput);
            AsyncStreamReader asyncErrorReader = new AsyncStreamReader(_fnProceso.StandardError);

            asyncOutputReader.DataReceived += delegate (object sender, string data)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(data);
            };

            asyncErrorReader.DataReceived += delegate (object sender, string data)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(data);
            };

            asyncOutputReader.Start();
            asyncErrorReader.Start();

            _fnProceso.WaitForExit();
        }
    }
}
