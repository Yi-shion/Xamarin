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
    public class LeftFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.left_fragment, null);
            LinearLayout userLayout = view.FindViewById<LinearLayout>(Resource.Id.userLayout);
            userLayout.Click += (sneder, e) =>
            {
                UserFragment user = new UserFragment();
                Android.Support.V4.App.FragmentTransaction ft = Activity.SupportFragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.center_frame, user);
                ft.AddToBackStack("User");
                ft.Commit();
                ((MainActivity)Activity).ShowLeft();
            };


            LinearLayout mainPage = view.FindViewById<LinearLayout>(Resource.Id.mainPage);

            mainPage.Click += (sneder, e) =>
            {
                UserFragment user = new UserFragment();
                Android.Support.V4.App.FragmentTransaction ft = Activity.SupportFragmentManager.BeginTransaction();
                ft.Replace(Resource.Id.center_frame, new SampleListFragment());
                ft.Commit();
                ((MainActivity)Activity).ShowLeft();
            };
            
            return view;
        }
    }
}