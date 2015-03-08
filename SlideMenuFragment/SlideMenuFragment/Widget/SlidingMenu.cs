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
	     * �����������view
	     * 
	     * @param view
	     */
        //@SuppressWarnings("deprecation")
        public void SetLeftView(View view)
        {
            LayoutParams behindParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.FillParent);
            behindParams.AddRule(LayoutRules.AlignParentLeft);// �ڸ��ؼ������
            AddView(view, behindParams);
            mMenuView = view;
        }

        /**
         * ����Ҳ������view
         * 
         * @param view
         */
        public void SetRightView(View view)
        {
            LayoutParams behindParams = new LayoutParams(LayoutParams.WrapContent, LayoutParams.FillParent);
            behindParams.AddRule(LayoutRules.AlignParentRight);// �ڸ��ؼ����ұ�
            AddView(view, behindParams);
            mDetailView = view;
        }

        /**
         * ����м����ݵ�view
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