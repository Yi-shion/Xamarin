using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Baidu.Mapapi;

namespace App
{
    [Application]
    public class AndroidApplication : Application
    {
        public AndroidApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }
        public override void OnCreate()
        {
            base.OnCreate();
            // 在使用 SDK 各组间之前初始化 context 信息，传入 ApplicationContext
           //SDKInitializer.Initialize(ApplicationContext);

        }

    }
}