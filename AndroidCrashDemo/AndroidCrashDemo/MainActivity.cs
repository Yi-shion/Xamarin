using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.IO;
using File = System.IO.File;

namespace AndroidCrashDemo
{
    [Activity(Label = "AndroidCrashDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private string str;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var button = FindViewById<Button>(Resource.Id.MyButton);
            var app = (CrashApplication)this.Application;
            button.Click += delegate
            {
                str.Equals("abc");
                return;

                string path = "/sdcard/crash/";//CacheDir.AbsolutePath
                File.WriteAllText(Path.Combine(path, "WriteAllText.txt"), "Test WriteAllText:Yishion");

                using (StreamWriter sw = new StreamWriter(Path.Combine(path, "StreamWriter.txt")))
                {
                    sw.Write("Test StreamWriter:Yishion");
                }

                using (var stream = File.CreateText(Path.Combine(path, "CreateText.txt")))
                {
                    stream.Write("Test CreateText:Yishion");
                }


                //string time = DateTime.Now.ToString("yyyyMMdd");
                //string fileName = "crash-" + time + "-" + DateTime.Now.ToString("fff") + ".txt";
                //if (Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted))
                //{
                //    string path = "/sdcard/crash/";
                //    if (!Directory.Exists(path))
                //    {
                //        Directory.CreateDirectory(path);
                //    }
                //    using (StreamWriter streamWriter = System.IO.File.AppendText(path + fileName))
                //    {
                //        streamWriter.WriteLine("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, "Test");
                //    }
                //}
            };
        }
    }
}

