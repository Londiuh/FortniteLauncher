using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FortniteLauncher
{
    internal class Program
    {
        //Change this path if you have Fortnite installed in a custom directory
        static readonly string binPath = @"C:\Program Files\Epic Games\Fortnite\FortniteGame\Binaries\Win64\";
        static readonly string launcherExe = $"{binPath}FortniteLauncher.exe";
        static readonly string shippingExe = $"{binPath}FortniteClient-Win64-Shipping.exe";
        static readonly string eacShippingExe = $"{binPath}FortniteClient-Win64-Shipping_EAC.exe";
        //public static readonly string beShippingExe = $"{binPath}FortniteClient-Win64-Shipping_BE.exe";
        //The obfuscationid changes in every fortnite update
        static readonly string obfuscationid = "52NaH6zbK4lg_U3VrUqEZicBycMpZw";
        static Process _fnProcess;
        static Process _fnEacProcess;
        static Process _fnLauncher;
        // Default values, this values are changed if you auth
        static string token = "unused";
        static string exchange = "unused";
        static string authType = "unused";
        //Change this path to your ssl pinning bypass
        //If the path doesn't exist, the injection will be skipped
        public static string sslBypassDLL = @".\Platanium.dll";

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
            Console.WriteLine("Do you want to authenticate? Y | N");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("\nAuthorization requiered:");
                token = Auth.GetToken(Console.ReadLine());
                exchange = Auth.GetExchange(token);
                authType = "exchangecode";
            }

            //You don't really need to use the args for the launcher and the eac shipping but the EGL does that
            var launchArgs = $"-obfuscationid={obfuscationid} -AUTH_LOGIN=unused -AUTH_PASSWORD={exchange} -AUTH_TYPE={authType} -epicapp=Fortnite -epicenv=Prod -EpicPortal -noeac -nobe -fltoken=fdd9g715h4i20110dd40d7d3";
            // I launch FortniteLauncher and FortniteClient-Win64-Shipping_EAC and freeze them to bypass the protection
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
                Win32.SuspendThread(Win32.OpenThread(0x0002, false, thread.Id));
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
                Win32.SuspendThread(Win32.OpenThread(0x0002, false, thread.Id));
            }

            //AsyncStreamReaders and RedirectStandardOutput to have fortnite output in the console
            _fnProcess = new Process
            {
                StartInfo =
                {
                    FileName = shippingExe,
                    Arguments = launchArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            _fnProcess.Start();
            AsyncStreamReader asyncOutputReader = new AsyncStreamReader(_fnProcess.StandardOutput);

            asyncOutputReader.DataReceived += delegate (object sender, string data)
            {
                Console.WriteLine(data);
            };

            asyncOutputReader.Start();

            Injector.InjectDll(_fnProcess.Id, sslBypassDLL);

            //If the game is closed kill the launcher and the eac shipping
            _fnProcess.WaitForExit();
            _fnLauncher.Kill();
            _fnEacProcess.Kill();
        }
    }
}
