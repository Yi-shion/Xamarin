using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TextViewCountDownDemo
{
    [Activity(Label = "TextViewCountDownDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ButtonCountDown btnSmsCode;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            #region MyRegion
            var button1 = this.FindViewById<Button>(Resource.Id.button1);
            button1.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(Activity1));
                this.StartActivity(intent);
            };
            #endregion


            btnSmsCode = this.FindViewById<ButtonCountDown>(Resource.Id.btnSmsCode);
            btnSmsCode.TempActivity = this;
            btnSmsCode.DefaultValue = "发送验证码";
            btnSmsCode.DefaultCountDown = 20;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.btnSmsCode.ClearTimer();
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (this.btnSmsCode.ButtonTimer != null)
            {
                this.btnSmsCode.StopTimer();
            }
        }
    }
}

