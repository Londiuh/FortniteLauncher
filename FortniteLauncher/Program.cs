using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using RestSharp;

namespace FortniteLauncher
{
    internal class Program
    {
        public static readonly string binPath = @"C:\Program Files\Epic Games\Fortnite\FortniteGame\Binaries\Win64\";
        public static readonly string launcherExe = $"{binPath}FortniteLauncher.exe";
        public static readonly string shippingExe = $"{binPath}FortniteClient-Win64-Shipping.exe";
        public static readonly string eacShippingExe = $"{binPath}FortniteClient-Win64-Shipping_EAC.exe";
        //public static readonly string beShippingExe = $"{binPath}FortniteClient-Win64-Shipping_BE.exe";
        public static readonly string obfuscationid = "2DgGDiN4ZBB0TKHnkkCtGEJ2EzroWg";
        static Process _fnProcess;
        static Process _fnEacProcess;
        static Process _fnLauncher;

        public static string GetToken(string authCode)
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
        
        public static string GetExchange(string token)
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
            Console.WriteLine("FortniteLauncher made by ElLondiuh");
            if (!File.Exists(launcherExe) | !File.Exists(shippingExe) | !File.Exists(eacShippingExe))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] Something is wrong with your Fortnite installation or can't find your installation");
                Thread.Sleep(3000);
                Environment.Exit(2);
            }
            string token = "unused";
            string exchange = "unused";
            string authType = "unused";
            Console.WriteLine("Do you want to authenticate? Y | N");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("Authorization requiered:");
                token = GetToken(Console.ReadLine());
                exchange = GetExchange(token);
                authType = "exchangecode";
            }
            var launchArgs = $"-obfuscationid={obfuscationid} -AUTH_LOGIN=unused -AUTH_PASSWORD={exchange} -AUTH_TYPE={authType} -epicapp=Fortnite -epicenv=Prod -EpicPortal -noeac -nobe -fltoken=fdd9g715h4i20110dd40d7d3";
            _fnLauncher = new Process
            {
                StartInfo =
                {
                    FileName = launcherExe,
                    Arguments = launchArgs
                    
                }
            };
            _fnLauncher.Start();
            foreach (ProcessThread thread in _fnLauncher.Threads)
            {
                Win32.Thread_Suspend(Win32.Thread_GetHandle(thread.Id));
            }

            _fnEacProcess = new Process
            {
                StartInfo =
                {
                    FileName = eacShippingExe,
                    Arguments = launchArgs

                }
            };
            _fnEacProcess.Start();
            foreach (ProcessThread thread in _fnEacProcess.Threads)
            {
                Win32.Thread_Suspend(Win32.Thread_GetHandle(thread.Id));
            }

            _fnProcess = new Process
            {
                StartInfo =
                {
                    FileName = shippingExe,
                    Arguments = launchArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            _fnProcess.Start();
            AsyncStreamReader asyncOutputReader = new AsyncStreamReader(_fnProcess.StandardOutput);
            AsyncStreamReader asyncErrorReader = new AsyncStreamReader(_fnProcess.StandardError);

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

            _fnProcess.WaitForExit();
            _fnLauncher.Kill();
            _fnEacProcess.Kill();
        }
    }
}
