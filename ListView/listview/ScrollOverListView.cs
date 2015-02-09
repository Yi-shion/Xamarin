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
    * �����Զ�������һ����ĿΪͷ����ͷ���������¼��������Ϊ׼��Ĭ��Ϊ��һ��
    *
    * @param index �����ڼ�������������Ŀ����Χ֮��
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
    * �����Զ�������һ����ĿΪβ����β���������¼��������Ϊ׼��Ĭ��Ϊ���һ��
    *
    * @param index �����ڼ�������������Ŀ����Χ֮��
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
     * �������Listener���Լ����Ƿ񵽴ﶥ�ˣ������Ƿ񵽴�Ͷ˵��¼�</br>
     *
     * @see OnScrollOverListener
     */
        public void setOnScrollOverListener(OnScrollOverListener onScrollOverListener)
        {
            _mOnScrollOverListener = onScrollOverListener;
        }
    }

    /**
     * ���������ӿ�</br>
     * @see ScrollOverListView#setOnScrollOverListener(OnScrollOverListener)
     *
     */
    public interface OnScrollOverListener
    {

      
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="delta">��ָ����ƶ�������ƫ����</param>
        /// <returns></returns>
        bool onListViewTopAndPullDown(int delta);

        /// <summary>
        /// ������ײ�����
        /// </summary>
        /// <param name="delta"> ��ָ����ƶ�������ƫ����</param>
        /// <returns></returns>
        bool onListViewBottomAndPullUp(int delta); 

        /// <summary>
        /// ��ָ�������´������൱��{@link MotionEvent#ACTION_DOWN}
        /// </summary>
        /// <param name="ev">@see View#onTouchEvent(MotionEvent)</param>
        /// <returns>����true��ʾ�Լ�����</returns>
        bool onMotionDown(MotionEvent ev);

         /// <summary>
        /// ��ָ�����ƶ��������൱��{@link MotionEvent#ACTION_MOVE}
         /// </summary>
        /// <param name="ev"> ����true��ʾ�Լ�����</param>
        /// <param name="delta">View#onTouchEvent(MotionEvent)</param>
         /// <returns></returns>
        bool onMotionMove(MotionEvent ev, int delta);

        /// <summary>
        /// ��ָ���������𴥷����൱��{@link MotionEvent#ACTION_UP}
        /// </summary>
        /// <param name="ev">����true��ʾ�Լ�����</param>
        /// <returns> View#onTouchEvent(MotionEvent)</returns>
        bool onMotionUp(MotionEvent ev);

    }
}