using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SlideMenuFragment
{
    public class SlidingView : ViewGroup
    {
        private FrameLayout mContainer;
        private Scroller mScroller;
        private VelocityTracker mVelocityTracker;
        private int mTouchSlop;
        private float mLastMotionX;
        private float mLastMotionY;
        private static int SNAP_VELOCITY = 1000;
        private View mMenuView;
        private View mDetailView;

        public SlidingView(Context context)
            : base(context)
        {
            Init();
        }

        public SlidingView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init();
        }

        public SlidingView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init();
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            mContainer.Measure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int width = r - l;
            int height = b - t;
            mContainer.Layout(0, 0, width, height);
        }

        private void Init()
        {
            mContainer = new FrameLayout(Context);
            //mContainer.SetBackgroundColor(0xff000000);
            mScroller = new Scroller(Context);
            mTouchSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;
            base.AddView(mContainer);
        }

        public void setView(View v)
        {
            if (mContainer.ChildCount > 0)
            {
                mContainer.RemoveAllViews();
            }
            mContainer.AddView(v);
        }

        public override void ScrollTo(int x, int y)
        {
            base.ScrollTo(x, y);
            PostInvalidate();
        }

        public override void ComputeScroll()
        {
            if (!mScroller.IsFinished)
            {
                if (mScroller.ComputeScrollOffset())
                {
                    int oldX = ScrollX;
                    int oldY = ScrollY;
                    int x = mScroller.CurrX;
                    int y = mScroller.CurrY;
                    if (oldX != x || oldY != y)
                    {
                        ScrollTo(x, y);
                    }
                    // Keep on drawing until the animation has finished.
                    Invalidate();
                }
                else
                {
                    clearChildrenCache();
                }
            }
            else
            {
                clearChildrenCache();
            }
        }

        private bool mIsBeingDragged;

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            MotionEventActions action = ev.Action;
            float x = ev.GetX();
            float y = ev.GetY();

            switch (action)
            {
                case MotionEventActions.Down:
                    mLastMotionX = x;
                    mLastMotionY = y;
                    mIsBeingDragged = false;
                    break;

                case MotionEventActions.Move:
                    float dx = x - mLastMotionX;
                    float xDiff = Math.Abs(dx);
                    float yDiff = Math.Abs(y - mLastMotionY);
                    if (xDiff > mTouchSlop && xDiff > yDiff)
                    {
                        mIsBeingDragged = true;
                        mLastMotionX = x;
                    }
                    break;

            }
            return mIsBeingDragged;
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {

            if (mVelocityTracker == null)
            {
                mVelocityTracker = VelocityTracker.Obtain();
            }
            mVelocityTracker.AddMovement(ev);

            MotionEventActions action = ev.Action;
            float x = ev.GetX();
            float y = ev.GetY();

            switch (action)
            {
                case MotionEventActions.Down:
                    if (!mScroller.IsFinished)
                    {
                        mScroller.AbortAnimation();
                    }
                    mLastMotionX = x;
                    mLastMotionY = y;
                    if (ScrollX == -getMenuViewWidth() && mLastMotionX < getMenuViewWidth())
                    {
                        return false;
                    }

                    if (ScrollX == getDetailViewWidth() && mLastMotionX > getMenuViewWidth())
                    {
                        return false;
                    }

                    break;
                case MotionEventActions.Move:
                    if (mIsBeingDragged)
                    {
                        enableChildrenCache();
                        float deltaX = mLastMotionX - x;
                        mLastMotionX = x;
                        float oldScrollX = ScrollX;
                        float scrollX = oldScrollX + deltaX;

                        if (deltaX < 0 && oldScrollX < 0)
                        {
                            // left view
                            float leftBound = 0;
                            float rightBound = -getMenuViewWidth();
                            if (scrollX > leftBound)
                            {
                                scrollX = leftBound;
                            }
                            else if (scrollX < rightBound)
                            {
                                scrollX = rightBound;
                            }
                            // mDetailView.setVisibility(View.INVISIBLE);
                            // mMenuView.setVisibility(View.VISIBLE);
                        }
                        else if (deltaX > 0 && oldScrollX > 0)
                        {
                            // right view
                            float rightBound = getDetailViewWidth();
                            float leftBound = 0;
                            if (scrollX < leftBound)
                            {
                                scrollX = leftBound;
                            }
                            else if (scrollX > rightBound)
                            {
                                scrollX = rightBound;
                            }
                            // mDetailView.setVisibility(View.VISIBLE);
                            // mMenuView.setVisibility(View.INVISIBLE);
                        }

                        ScrollTo((int)scrollX, ScrollY);

                    }
                    break;
                case MotionEventActions.Cancel:
                case MotionEventActions.Up:
                    if (mIsBeingDragged)
                    {
                        VelocityTracker velocityTracker = mVelocityTracker;
                        velocityTracker.ComputeCurrentVelocity(1000);
                        //参数---------
                        int velocityX = (int)velocityTracker.GetXVelocity(10);
                        velocityX = 0;
                        //Log.e("ad", "velocityX == " + velocityX);
                        int oldScrollX = ScrollX;
                        int dx = 0;
                        if (oldScrollX < 0)
                        {
                            if (oldScrollX < -getMenuViewWidth() / 2 || velocityX > SNAP_VELOCITY)
                            {
                                dx = -getMenuViewWidth() - oldScrollX;
                            }
                            else if (oldScrollX >= -getMenuViewWidth() / 2 || velocityX < -SNAP_VELOCITY)
                            {
                                dx = -oldScrollX;
                            }
                        }
                        else
                        {
                            if (oldScrollX > getDetailViewWidth() / 2 || velocityX < -SNAP_VELOCITY)
                            {
                                dx = getDetailViewWidth() - oldScrollX;
                            }
                            else if (oldScrollX <= getDetailViewWidth() / 2 || velocityX > SNAP_VELOCITY)
                            {
                                dx = -oldScrollX;
                            }
                        }

                        smoothScrollTo(dx);
                        clearChildrenCache();

                    }

                    break;

            }
            if (mVelocityTracker != null)
            {
                mVelocityTracker.Recycle();
                mVelocityTracker = null;
            }

            return true;
        }
        private int getMenuViewWidth()
        {
            if (mMenuView == null)
            {
                return 0;
            }
            return mMenuView.Width;
        }

        private int getDetailViewWidth()
        {
            if (mDetailView == null)
            {
                return 0;
            }
            return mDetailView.Width;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }

        public View getDetailView()
        {
            return mDetailView;
        }

        public void setDetailView(View mDetailView)
        {
            this.mDetailView = mDetailView;
        }

        public View getMenuView()
        {
            return mMenuView;
        }

        public void setMenuView(View mMenuView)
        {
            this.mMenuView = mMenuView;
        }

        /**
         * 左侧边栏的关闭与显示
         */
        public void showLeftView()
        {
            int menuWidth = mMenuView.Width;
            int oldScrollX = ScrollX;
            if (oldScrollX == 0)
            {
                smoothScrollTo(-menuWidth);
            }
            else if (oldScrollX == -menuWidth)
            {
                smoothScrollTo(menuWidth);
            }
        }

        /**
         * 右侧边栏的关闭与显示
         */
        public void showRightView()
        {
            int menuWidth = mDetailView.Width;
            int oldScrollX = ScrollX;
            if (oldScrollX == 0)
            {
                smoothScrollTo(menuWidth);
            }
            else if (oldScrollX == menuWidth)
            {
                smoothScrollTo(-menuWidth);
            }
        }

        void smoothScrollTo(int dx)
        {
            int duration = 500;
            int oldScrollX = ScrollX;
            mScroller.StartScroll(oldScrollX, ScrollY, dx, ScrollY, duration);
            Invalidate();
        }

        void enableChildrenCache()
        {
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View layout = (View)GetChildAt(i);
                layout.DrawingCacheEnabled = true;
            }
        }

        void clearChildrenCache()
        {
            int count = ChildCount;
            for (int i = 0; i < count; i++)
            {
                View layout = (View)GetChildAt(i);
                layout.DrawingCacheEnabled = false;
            }
        }
    }
}