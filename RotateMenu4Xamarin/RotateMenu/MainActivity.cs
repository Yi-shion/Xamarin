using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using RoundSpinViewDemo.Views;

namespace RotateMenu
{
    [Activity(Label = "RotateMenu", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, OnRoundSpinViewListener
    {
        private RoundSpinView rsv_test;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);
            initView();
        }

        private void initView(){
		    rsv_test = (RoundSpinView)this.FindViewById(Resource.Id.rsv_test);
		    rsv_test.setOnRoundSpinViewListener(this);
		}

        public void onSingleTapUp(int position)
        {
            // TODO Auto-generated method stub
            switch (position)
            {
                case 0:
                    Toast.MakeText(this, "place:0", 0).Show();
                    break;
                case 1:
                    Toast.MakeText(this, "place:1", 0).Show();
                    break;
                case 2:
                    Toast.MakeText(this, "place:2", 0).Show();
                    break;
                default:
                    break;
            }
        }

	

    }
}

