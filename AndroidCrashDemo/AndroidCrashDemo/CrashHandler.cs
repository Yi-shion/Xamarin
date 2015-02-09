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

        //ϵͳĬ�ϵ�UncaughtException������   
        private Thread.IUncaughtExceptionHandler mDefaultHandler;
        //CrashHandlerʵ��  
        private static CrashHandler INSTANCE = new CrashHandler();
        //�����Context����  
        private Context mContext;
        //�����洢�豸��Ϣ���쳣��Ϣ  
        private Dictionary<string, string> infos = new Dictionary<string, string>();

        //���ڸ�ʽ������,��Ϊ��־�ļ�����һ����  
        private string formatter = "yyyy-MM-dd-HH-mm-ss";

        /** ��ֻ֤��һ��CrashHandlerʵ�� */
        private CrashHandler()
        {

        }
        /** ��ȡCrashHandlerʵ�� ,����ģʽ */
        public static CrashHandler getInstance()
        {
            return INSTANCE;
        }
        /** 
         * ��ʼ�� 
         *  
         * @param context 
         */
        public void init(Context context)
        {
            mContext = context;
            //��ȡϵͳĬ�ϵ�UncaughtException������  
            mDefaultHandler = Thread.DefaultUncaughtExceptionHandler;
            //���ø�CrashHandlerΪ�����Ĭ�ϴ�����  
            Thread.DefaultUncaughtExceptionHandler = this;
        }



        //void Thread.IUncaughtExceptionHandler.UncaughtException(Thread thread, Throwable ex)
        //{
        //    Toast.MakeText(mContext, "�ܱ�Ǹ,��������쳣,�����˳�.", ToastLength.Long).Show();

        //    if (!handleException(ex) && mDefaultHandler != null)
        //    {
        //        //����û�û�д�������ϵͳĬ�ϵ��쳣������������  
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
        //        //�˳�����  
        //        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        //        //System.exit(1);
        //    }
        //}

        /** 
         * �Զ��������,�ռ�������Ϣ ���ʹ��󱨸�Ȳ������ڴ����. 
         *  
         * @param ex 
         * @return true:��������˸��쳣��Ϣ;���򷵻�false. 
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
                Toast.MakeText(mContext, "�ܱ�Ǹ,��������쳣,�����˳�.", ToastLength.Long).Show();
                Looper.Loop();
            }).Start();


            //ʹ��Toast����ʾ�쳣��Ϣ  
            //new Thread() {  
            //    @Override  
            //    public void run() {  
            //        Looper.prepare();  
            //        Toast.makeText(mContext, "�ܱ�Ǹ,��������쳣,�����˳�.", Toast.LENGTH_LONG).show();  
            //        Looper.loop();  
            //    }  
            //}.start();  
            //�ռ��豸������Ϣ   
            collectDeviceInfo(mContext);
            ////������־�ļ�   
            //saveCrashInfo2File(ex);  
            return true;
        }

        /** 
         * �ռ��豸������Ϣ 
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
        // * ���������Ϣ���ļ��� 
        // *  
        // * @param ex 
        // * @return  �����ļ�����,���ڽ��ļ����͵������� 
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
            Toast.MakeText(mContext, "�ܱ�Ǹ,��������쳣,�����˳�.", ToastLength.Long).Show();

            if (!handleException(ex) && mDefaultHandler != null)
            {
                //����û�û�д�������ϵͳĬ�ϵ��쳣������������  
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
                    //�˳�����  
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    //System.exit(1);
                }
            }
        }
    }
}