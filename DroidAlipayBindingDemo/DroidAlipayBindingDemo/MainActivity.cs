using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Alipay.Sdk.App;

namespace DroidAlipayBindingDemo
{
    [Activity(Label = "DroidAlipayBindingDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var button = FindViewById<Button>(Resource.Id.MyButton);
            var alipayHandler=new AlipayHandler();
            button.Click += delegate
            {
                var payTask = new PayTask(this);
                var order =
                    "partner=\"2088001152192050\"&seller_id=\"acct@sportsgg.com\"&out_trade_no=\"w2413209423\"&subject=\"payw2413209423\"&body=\"payw2413209423\"&total_fee=\"0.01\"&notify_url=\"http://221.122.53.190:6804/NewWebPay/FromPhoneAlipay.aspx\"&service=\"mobile.securitypay.pay\"&payment_type=\"1\"&_input_charset=\"utf-8\"&sign=\"E0tHnnq40wfnMn6G6T54%2fBmshvUlSUYAcWhgZjoxQu02W8gspQFHogCDDAmnfo9WFUG%2b%2f32x8IIl1hUkaK9sqP64Fvk6PnIyMgoms8en1PpIL0Z4VUSsqjtGrOVrhwfOnb5sWlPATEu7MuXWnoH1yp%2fWJdyPdHmzljEF%2fFINyZE%3d\"&sign_type=\"RSA\"";
                var result = payTask.Pay(order);
                var msg = new Message
                {
                    Obj = result
                };
                alipayHandler.SendMessage(msg);
            };
        }
    }

    public class AlipayHandler : Handler
    {
        public void SendMessage(Message msg)
        {
            var reslut = (string)msg.Obj;
        }
    }
}

