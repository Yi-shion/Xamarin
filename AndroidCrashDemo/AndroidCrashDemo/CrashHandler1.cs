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
        //�������ģʽ
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

        //������г�ʼ���������ȡӦ�������Ϣ��Ҫʹ�õ�������
        public void Init(Context context)
        {
            this.context = context;
        }

        public void UncaughtException(Thread thread, Throwable ex)
        {
            //1.��ȡӦ�ó���汾��Ϣ
            string version = string.Empty;
            PackageManager pm = context.PackageManager;
            try
            {
                PackageInfo info = pm.GetPackageInfo(context.PackageName, 0);
                string versionName = info.VersionName;
                version += ("����汾��Ϊ:" + versionName);

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
            Toast.MakeText(context, "�ܱ�Ǹ,��������쳣,�����˳�.", ToastLength.Long).Show();
            //�����Ӧ�ó�����ɱ
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}