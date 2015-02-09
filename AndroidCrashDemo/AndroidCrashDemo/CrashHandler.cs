using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Exception = System.Exception;

namespace AndroidCrashDemo
{
    public class CrashHandler : Java.Lang.Object, Thread.IUncaughtExceptionHandler
    {
        public static string TAG = "CrashHandler";

        //系统默认的UncaughtException处理类   
        private Thread.IUncaughtExceptionHandler mDefaultHandler;
        //CrashHandler实例  
        private static CrashHandler INSTANCE = new CrashHandler();
        //程序的Context对象  
        private Context mContext;
        //用来存储设备信息和异常信息  
        private Dictionary<string, string> infos = new Dictionary<string, string>();

        //用于格式化日期,作为日志文件名的一部分  
        private string formatter = "yyyy-MM-dd-HH-mm-ss";

        /** 保证只有一个CrashHandler实例 */
        private CrashHandler()
        {

        }
        /** 获取CrashHandler实例 ,单例模式 */
        public static CrashHandler getInstance()
        {
            return INSTANCE;
        }
        /** 
         * 初始化 
         *  
         * @param context 
         */
        public void init(Context context)
        {
            mContext = context;
            //获取系统默认的UncaughtException处理器  
            mDefaultHandler = Thread.DefaultUncaughtExceptionHandler;
            //设置该CrashHandler为程序的默认处理器  
            Thread.DefaultUncaughtExceptionHandler = this;
        }



        //void Thread.IUncaughtExceptionHandler.UncaughtException(Thread thread, Throwable ex)
        //{
        //    Toast.MakeText(mContext, "很抱歉,程序出现异常,即将退出.", ToastLength.Long).Show();

        //    if (!handleException(ex) && mDefaultHandler != null)
        //    {
        //        //如果用户没有处理则让系统默认的异常处理器来处理  
        //        mDefaultHandler.UncaughtException(thread, ex);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            Thread.Sleep(3000);
        //        }
        //        catch (InterruptedException e)
        //        {
        //            Log.Error(TAG, "error : ", e);
        //        //退出程序  
        //        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        //        //System.exit(1);
        //    }
        //}

        /** 
         * 自定义错误处理,收集错误信息 发送错误报告等操作均在此完成. 
         *  
         * @param ex 
         * @return true:如果处理了该异常信息;否则返回false. 
         */
        private bool handleException(Throwable ex)
        {
            if (ex == null)
            {
                return false;
            }

            new Thread(() =>
            {
                Looper.Prepare();
                Toast.MakeText(mContext, "很抱歉,程序出现异常,即将退出.", ToastLength.Long).Show();
                Looper.Loop();
            }).Start();


            //使用Toast来显示异常信息  
            //new Thread() {  
            //    @Override  
            //    public void run() {  
            //        Looper.prepare();  
            //        Toast.makeText(mContext, "很抱歉,程序出现异常,即将退出.", Toast.LENGTH_LONG).show();  
            //        Looper.loop();  
            //    }  
            //}.start();  
            //收集设备参数信息   
            collectDeviceInfo(mContext);
            ////保存日志文件   
            //saveCrashInfo2File(ex);  
            return true;
        }

        /** 
         * 收集设备参数信息 
         * @param ctx 
         */
        public void collectDeviceInfo(Context ctx)
        {
            try
            {
                PackageManager pm = ctx.PackageManager;
                PackageInfo pi = pm.GetPackageInfo(ctx.PackageName, PackageInfoFlags.Activities);
                if (pi != null)
                {
                    string versionName = pi.VersionName ?? "null";
                    string versionCode = pi.VersionCode + "";
                    infos.Add("versionName", versionName);
                    infos.Add("versionCode", versionCode);
                }
            }
            catch (PackageManager.NameNotFoundException e)
            {
                Log.Error(TAG, "an error occured when collect package info", e);
            }
            //test.GetType().GetProperties()
            //var fields=Build.Type.de


            //Field[] fields = Build.class.getDeclaredFields();  
            //for (Field field : fields) {  
            //    try {  
            //        field.setAccessible(true);  
            //        infos.put(field.getName(), field.get(null).toString());  
            //        Log.d(TAG, field.getName() + " : " + field.get(null));  
            //    } catch (Exception e) {  
            //        Log.e(TAG, "an error occured when collect crash info", e);  
            //    }  
            //}  
        }

        ///** 
        // * 保存错误信息到文件中 
        // *  
        // * @param ex 
        // * @return  返回文件名称,便于将文件传送到服务器 
        // */  
        private string saveCrashInfo2File(Throwable ex)
        {
            StringBuffer sb = new StringBuffer();
            foreach (var info in infos)
            {
                string key = info.Key;
                string value = info.Value;
                sb.Append(key + "=" + value + "\n");
            }
            //for (Map.Entry<String, String> entry : infos.entrySet()) {  
            //    String key = entry.getKey();  
            //    String value = entry.getValue();  
            //    sb.append(key + "=" + value + "\n");  
            //}  
            //Writer writer = new StringWriter();  
            //PrintWriter printWriter = new PrintWriter(writer);  
            //ex.printStackTrace(printWriter);  
            //Throwable cause = ex.getCause();  
            //while (cause != null) {  
            //    cause.printStackTrace(printWriter);  
            //    cause = cause.getCause();  
            //}  
            //printWriter.close();  
            //String result = writer.toString();  
            //sb.append(result);  
            //try {  
            //    long timestamp = System.currentTimeMillis();  
            //    String time = formatter.format(new Date());  
            //    String fileName = "crash-" + time + "-" + timestamp + ".log";  
            //    if (Environment.getExternalStorageState().equals(Environment.MEDIA_MOUNTED)) {  
            //        String path = "/sdcard/crash/";  
            //        File dir = new File(path);  
            //        if (!dir.exists()) {  
            //            dir.mkdirs();  
            //        }  
            //        FileOutputStream fos = new FileOutputStream(path + fileName);  
            //        fos.write(sb.toString().getBytes());  
            //        fos.close();  
            //    }  
            //    return fileName;  
            //} catch (Exception e) {  
            //    Log.e(TAG, "an error occured while writing file...", e);  
            //}  
            return null;
        }


        public void UncaughtException(Thread thread, Throwable ex)
        {
            Toast.MakeText(mContext, "很抱歉,程序出现异常,即将退出.", ToastLength.Long).Show();

            if (!handleException(ex) && mDefaultHandler != null)
            {
                //如果用户没有处理则让系统默认的异常处理器来处理  
                mDefaultHandler.UncaughtException(thread, ex);
            }
            else
            {
                try
                {
                    Thread.Sleep(3000);
                }
                catch (InterruptedException e)
                {
                    Log.Error(TAG, "error : ", e);
                    //退出程序  
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    //System.exit(1);
                }
            }
        }
    }
}