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
using Java.Lang;

namespace SlideMenuFragment
{
    public class SlidingMenu : RelativeLayout
    {
        private SlidingView mSlidingView;
        private View mMenuView;
        private View mDetailView;

        public SlidingMenu(Context context)
            : base(context)
        {
        }

        public SlidingMenu(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public SlidingMenu(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }

        public void AddViews(View left, View center, View right)
        {
            SetLeftView(left);
            SetRightView(right);
            SetCenterView(center);
        }

        /**
	     * 添加左侧边栏的view
	     * 
	     * @param view
	     */
        //@SuppressWarnings("deprecation")
        public void SetLeftView(View view)
        {
            LayoutParams behindParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.FillParent);
            behindParams.AddRule(LayoutRules.AlignParentLeft);// 在父控件的左边
            AddView(view, behindParams);
            mMenuView = view;
        }

        /**
         * 添加右侧边栏的view
         * 
         * @param view
         */
        public void SetRightView(View view)
        {
            LayoutParams behindParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.FillParent);
            behindParams.AddRule(LayoutRules.AlignParentRight);// 在父控件的右边
            AddView(view, behindParams);
            mDetailView = view;
        }

        /**
         * 添加中间内容的view
         * 
         * @param view
         */
        public void SetCenterView(View view)
        {
            LayoutParams aboveParams = new LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent);
            mSlidingView = new SlidingView(Context);
            mSlidingView.setView(view);
            AddView(mSlidingView, aboveParams);
            mSlidingView.setMenuView(mMenuView);
            mSlidingView.setDetailView(mDetailView);
            mSlidingView.Invalidate();
        }

        public void ShowLeftView()
        {
            mSlidingView.showLeftView();
        }

        public void ShowRightView()
        {
            mSlidingView.showRightView();
        }
    }
}