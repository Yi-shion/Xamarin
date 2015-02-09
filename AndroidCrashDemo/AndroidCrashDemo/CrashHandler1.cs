using System;
using System.IO;
using Android.Content;
using Android.Content.PM;
using Android.Widget;
using Java.Lang;

namespace AndroidCrashDemo
{
    public class CrashHandler1 : Java.Lang.Object, Thread.IUncaughtExceptionHandler
    {
        private static string TAG = "MyCrashHandler";
        private Context context;
        //单例设计模式
        private static CrashHandler1 myCrashHandler;
        private CrashHandler1()
        {

        }
        public static CrashHandler1 getInstance()
        {
            if (myCrashHandler == null)
            {
                myCrashHandler = new CrashHandler1();
            }
            return myCrashHandler;
        }

        //对其进行初始化，后面获取应用相关信息需要使用到上下文
        public void Init(Context context)
        {
            this.context = context;
        }

        public void UncaughtException(Thread thread, Throwable ex)
        {
            //1.获取应用程序版本信息
            string version = string.Empty;
            PackageManager pm = context.PackageManager;
            try
            {
                PackageInfo info = pm.GetPackageInfo(context.PackageName, 0);
                string versionName = info.VersionName;
                version += ("程序版本号为:" + versionName);

                string time = DateTime.Now.ToString("yyyyMMdd");
                string fileName = "crash-" + time + "-" + DateTime.Now.ToString("fff") + ".log";
                if (Android.OS.Environment.ExternalStorageState.Equals(Android.OS.Environment.MediaMounted))
                {
                    var path = "/sdcard/crash/";
                   
                    using (StreamWriter streamWriter = System.IO.File.AppendText(path + fileName))
                    {
                        streamWriter.WriteLine("{0:yyyy-MM-dd HH:mm:ss} {1}", DateTime.Now, version);
                    }
                }
            }
            catch (System.Exception e)
            {
            }
            Toast.MakeText(context, "很抱歉,程序出现异常,即将退出.", ToastLength.Long).Show();
            //最后让应用程序自杀
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}