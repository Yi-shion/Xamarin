using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using System.IO;
using System.Threading;

namespace AndroidCrashDemo
{

    [Application]
    public class CrashApplication : Application
    {
        public CrashApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //new Thread(new ThreadStart(() =>
            //{

            //using (var streamWriter = File.AppendText("/sdcard/crash/CreateText.txt"))
            //{
            //    var exception = e.ExceptionObject;
            //    streamWriter.WriteLine("{0}:{1}", "时间", exception + "\n");
            //}

            //var stream = File.CreateText(Path.Combine("/sdcard/crash/", "CreateText.txt"));
            //stream.Write("Test CreateText:Yishion");
            //stream.Close();

            //})).Start();

            //if (Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted))
            //{
            const string path = "/sdcard/crash/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var exception = e.ExceptionObject;
            using (StreamWriter streamWriter = File.AppendText(path + "ErrorLog.txt"))
            {
                streamWriter.WriteLine("{0}:{1}", "异常信息", exception + "\n");
            }
            //}
        }
    }
}