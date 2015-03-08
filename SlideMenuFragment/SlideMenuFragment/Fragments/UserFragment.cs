using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SlideMenuFragment
{
    public class UserFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.user, null);
            var left = view.FindViewById<ImageView>(Resource.Id.iv_user_left);
            left.Click += (sender, e) =>
            {
                ((MainActivity)Activity).ShowLeft();
            };
            return view;
        }

    }
}