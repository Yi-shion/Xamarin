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
using Android.Util;

namespace Mobile
{
    public class ScrollOverListView : ListView
    {
        private int mLastY;
        private int mTopPosition;
        private int mBottomPosition;

        public ScrollOverListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            init();
        }
        public ScrollOverListView(Context context)
            : base(context)
        {
            init();
        }
        public ScrollOverListView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            init();
        }
        public ScrollOverListView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            init();
        }
        private void init()
        {
            mTopPosition = 0;
            mBottomPosition = 0;
        }
        public override bool OnTouchEvent(MotionEvent e)
        {

            MotionEventActions action = e.Action;
            int y = (int)e.RawY;
            switch (action)
            {
                case MotionEventActions.Down:
                    mLastY = y;
                    bool isHandled = _mOnScrollOverListener.onMotionDown(e);
                    if (isHandled)
                    {
                        mLastY = y;
                        return isHandled;
                    }
                    break;
                case MotionEventActions.Move:
                    int childCount = this.ChildCount;
                    if (childCount > 0)
                    {
                        int itemCount = this.Adapter.Count - mBottomPosition;
                        int deltaY = y - mLastY;
                        int firstTop = this.GetChildAt(0).Top;
                        int listPadding = this.ListPaddingTop;
                        int lastBottom = GetChildAt(childCount - 1).Bottom;
                        int end = Height - PaddingBottom;
                        int firstVisiblePosition = this.FirstVisiblePosition;
                        bool isHandleMotionMove = _mOnScrollOverListener.onMotionMove(e, deltaY);
                        if (isHandleMotionMove)
                        {
                            mLastY = y;
                            return true;
                        }

                        if (firstVisiblePosition <= mTopPosition && firstTop >= listPadding && deltaY > 0)
                        {
                            bool isHandleOnListViewTopAndPullDown;
                            isHandleOnListViewTopAndPullDown = _mOnScrollOverListener.onListViewTopAndPullDown(deltaY);
                            if (isHandleOnListViewTopAndPullDown)
                            {
                                mLastY = y;
                                return true;
                            }
                        }

                        if (firstVisiblePosition + childCount >= itemCount && lastBottom <= end && deltaY < 0)
                        {
                            bool isHandleOnListViewBottomAndPullDown;
                            isHandleOnListViewBottomAndPullDown = _mOnScrollOverListener.onListViewBottomAndPullUp(deltaY);
                            if (isHandleOnListViewBottomAndPullDown)
                            {
                                mLastY = y;
                                return true;
                            }
                        }
                    }
                    break;
                case MotionEventActions.Up:
                    bool isHandlerMotionUp = _mOnScrollOverListener.onMotionUp(e);
                    if (isHandlerMotionUp)
                    {
                        mLastY = y;
                        return true;
                    }
                    break;
            }

            mLastY = y;
            return base.OnTouchEvent(e);
        }


        /**
    * 可以自定义其中一个条目为头部，头部触发的事件将以这个为准，默认为第一个
    *
    * @param index 正数第几个，必须在条目数范围之内
    */
        public void setTopPosition(int index)
        {

            if (Adapter == null)
                return;
                //throw new Java.Lang.NullPointerException("You must set adapter before setTopPosition!");
            if (index < 0)
                throw new Java.Lang.IllegalArgumentException("Top position must > 0");

            mTopPosition = index;
        }

        /**
    * 可以自定义其中一个条目为尾部，尾部触发的事件将以这个为准，默认为最后一个
    *
    * @param index 倒数第几个，必须在条目数范围之内
    */
        public void setBottomPosition(int index)
        {
            if (Adapter == null)
                return;
               // throw new Java.Lang.NullPointerException("You must set adapter before setBottonPosition!");
            if (index < 0)
                throw new Java.Lang.IllegalArgumentException("Bottom position must > 0");

            mBottomPosition = index;
        }


        public OnScrollOverListener _mOnScrollOverListener = new mOnScrollOverListener();

        public class mOnScrollOverListener : OnScrollOverListener
        {

            bool OnScrollOverListener.onListViewTopAndPullDown(int delta)
            {
                return false;
            }

            bool OnScrollOverListener.onListViewBottomAndPullUp(int delta)
            {
                return false;
            }

            bool OnScrollOverListener.onMotionDown(MotionEvent ev)
            {
                return false;
            }

            bool OnScrollOverListener.onMotionMove(MotionEvent ev, int delta)
            {
                return false;
            }

            bool OnScrollOverListener.onMotionUp(MotionEvent ev)
            {
                return false;
            }
        }


        /**
     * 设置这个Listener可以监听是否到达顶端，或者是否到达低端等事件</br>
     *
     * @see OnScrollOverListener
     */
        public void setOnScrollOverListener(OnScrollOverListener onScrollOverListener)
        {
            _mOnScrollOverListener = onScrollOverListener;
        }
    }

    /**
     * 滚动监听接口</br>
     * @see ScrollOverListView#setOnScrollOverListener(OnScrollOverListener)
     *
     */
    public interface OnScrollOverListener
    {

      
        /// <summary>
        /// 到达最顶部触发
        /// </summary>
        /// <param name="delta">手指点击移动产生的偏移量</param>
        /// <returns></returns>
        bool onListViewTopAndPullDown(int delta);

        /// <summary>
        /// 到达最底部触发
        /// </summary>
        /// <param name="delta"> 手指点击移动产生的偏移量</param>
        /// <returns></returns>
        bool onListViewBottomAndPullUp(int delta); 

        /// <summary>
        /// 手指触摸按下触发，相当于{@link MotionEvent#ACTION_DOWN}
        /// </summary>
        /// <param name="ev">@see View#onTouchEvent(MotionEvent)</param>
        /// <returns>返回true表示自己处理</returns>
        bool onMotionDown(MotionEvent ev);

         /// <summary>
        /// 手指触摸移动触发，相当于{@link MotionEvent#ACTION_MOVE}
         /// </summary>
        /// <param name="ev"> 返回true表示自己处理</param>
        /// <param name="delta">View#onTouchEvent(MotionEvent)</param>
         /// <returns></returns>
        bool onMotionMove(MotionEvent ev, int delta);

        /// <summary>
        /// 手指触摸后提起触发，相当于{@link MotionEvent#ACTION_UP}
        /// </summary>
        /// <param name="ev">返回true表示自己处理</param>
        /// <returns> View#onTouchEvent(MotionEvent)</returns>
        bool onMotionUp(MotionEvent ev);

    }
}