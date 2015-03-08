using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SlideMenuFragment
{
    [Activity(Label = "SlideMenuFragment", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Android.Support.V4.App.FragmentActivity
    {
        private SlidingMenu mSlidingMenu;// 侧边栏的view
        private LeftFragment leftFragment; // 左侧边栏的碎片化view
        private RightFragment rightFragment; // 右侧边栏的碎片化view
        private SampleListFragment centerFragment;// 中间内容碎片化的view
        private Android.Support.V4.App.FragmentTransaction ft; // 碎片化管理的事务
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // 去标题栏
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_main);
            mSlidingMenu = FindViewById<SlidingMenu>(Resource.Id.slidingMenu);
            mSlidingMenu.SetLeftView(LayoutInflater.Inflate(Resource.Layout.left_frame, null));
            mSlidingMenu.SetRightView(LayoutInflater.Inflate(Resource.Layout.right_frame, null));
            mSlidingMenu.SetCenterView(LayoutInflater.Inflate(Resource.Layout.center_frame, null));

            ft = this.SupportFragmentManager.BeginTransaction();
            leftFragment = new LeftFragment();
            rightFragment = new RightFragment();
            ft.Replace(Resource.Id.left_frame, leftFragment);
            ft.Replace(Resource.Id.right_frame, rightFragment);

            centerFragment = new SampleListFragment();
            ft.Replace(Resource.Id.center_frame, centerFragment);
            ft.Commit();

        }
        public void ShowLeft()
        {
            mSlidingMenu.ShowLeftView();
        }

        public void ShowRight()
        {
            mSlidingMenu.ShowRightView();
        }

        public override void OnBackPressed()
        {
            //if (mBackHandedFragment == null || !mBackHandedFragment.onBackPressed())
            //{
            var backStackCount = SupportFragmentManager.BackStackEntryCount;
            if (backStackCount == 0)
            {
                base.OnBackPressed();
            }
            else
            {
                if (backStackCount == 1)
                {
                    // btnSecond.setVisibility(View.VISIBLE);
                }
                SupportFragmentManager.PopBackStack();
            }
            //}
        }
    }
}

