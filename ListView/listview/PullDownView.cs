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
using Android.Views.Animations;

namespace Mobile
{
    /**
     * 刷新事件接口
     */
    public interface OnPullDownListener
    {
        void onRefresh();
        void onMore();
    }
    public class PullDownListener : OnPullDownListener
    { 
        public void onRefresh()
        { 
        }

        public void onMore()
        { 
        }
    }


    public class PullDownView : LinearLayout, OnScrollOverListener
    {
        private const String TAG = "PullDownView";

        private const int START_PULL_DEVIATION = 50; // 移动误差
        private const int AUTO_INCREMENTAL = 10;     // 自增量，用于回弹

        private const int WHAT_DID_LOAD_DATA = 1;    // Handler what 数据加载完毕
        private const int WHAT_ON_REFRESH = 2;       // Handler what 刷新中
        private const int WHAT_DID_REFRESH = 3;      // Handler what 已经刷新完
        private const int WHAT_SET_HEADER_HEIGHT = 4;// Handler what 设置高度
        private const int WHAT_DID_MORE = 5;         // Handler what 已经获取完更多

        private const int DEFAULT_HEADER_VIEW_HEIGHT = 105;  // 头部文件原本的高度

       // private static SimpleDateFormat dateFormat = new SimpleDateFormat("MM-dd HH:mm");

        private static View mHeaderView;
        private static LayoutParams mHeaderViewParams;
        private static TextView mHeaderViewDateView;
        public static TextView mHeaderTextView;
        public static ImageView mHeaderArrowView;
        public static View mHeaderLoadingView;
        private static View mFooterView;
        private static TextView mFooterTextView;
        private static View mFooterLoadingView;
        private static ScrollOverListView mListView;

        private static OnPullDownListener mOnPullDownListener;
        private RotateAnimation mRotateOTo180Animation;
        private RotateAnimation mRotate180To0Animation;

        private static int mHeaderIncremental; // 增量
        private static float mMotionDownLastY; // 按下时候的Y轴坐标

        private static bool mIsDown;            // 是否按下
        private static bool mIsRefreshing;      // 是否下拉刷新中
        private static bool mIsFetchMoreing;    // 是否获取更多中
        private static bool mIsPullUpDone;      // 是否回推完成
        private static bool mEnableAutoFetchMore;   // 是否允许自动获取更多

        // 头部文件的状态
        private const int HEADER_VIEW_STATE_IDLE = 0;            // 空闲
        private const int HEADER_VIEW_STATE_NOT_OVER_HEIGHT = 1; // 没有超过默认高度
        private const int HEADER_VIEW_STATE_OVER_HEIGHT = 2;     // 超过默认高度
        private static int mHeaderViewState = HEADER_VIEW_STATE_IDLE;

        public PullDownView(Context context, Android.Util.IAttributeSet attrs)
            : base(context, attrs)
        {
            initHeaderViewAndFooterViewAndListView(context);
        }

        public PullDownView(Context context)
            : base(context)
        {
            initHeaderViewAndFooterViewAndListView(context);
        }

        /**
     * 通知加载完了数据，要放在Adapter.notifyDataSetChanged后面
     * 当你加载完数据的时候，调用这个notifyDidLoad()
     * 才会隐藏头部，并初始化数据等
     */
        public void notifyDidLoad()
        {
            mUIHandler.SendEmptyMessage(WHAT_DID_LOAD_DATA);
        }


        /**
	 * 通知已经刷新完了，要放在Adapter.notifyDataSetChanged后面
	 * 当你执行完刷新任务之后，调用这个notifyDidRefresh()
	 * 才会隐藏掉头部文件等操作
	 */
        public void notifyDidRefresh()
        {
            mUIHandler.SendEmptyMessage(WHAT_DID_REFRESH);
        }

        /**
	 * 通知已经获取完更多了，要放在Adapter.notifyDataSetChanged后面
	 * 当你执行完更多任务之后，调用这个notyfyDidMore()
	 * 才会隐藏加载圈等操作
	 */
        public void notifyDidMore()
        {
            mUIHandler.SendEmptyMessage(WHAT_DID_MORE);
        }
        /**
	 * 设置监听器
	 * @param listener
	 */
        public void setOnPullDownListener(OnPullDownListener listener)
        {
            mOnPullDownListener = listener;
        }
        /**
             * 获取内嵌的listview
             * @return ScrollOverListView
             */
        public ListView getListView()
        {
            return mListView;
        }
        /**
	 * 是否开启自动获取更多
	 * 自动获取更多，将会隐藏footer，并在到达底部的时候自动刷新
	 * @param index 倒数第几个触发
	 */
        public void enableAutoFetchMore(bool enable, int index)
        {
            if (enable)
            {
                //地步的位置
                mListView.setBottomPosition(index);
                //可见性
                mFooterLoadingView.Visibility = ViewStates.Visible;
            }
            else
            {
                mFooterTextView.Text = "点击获取更多";
                mFooterLoadingView.Visibility = ViewStates.Gone;
            }
            mEnableAutoFetchMore = enable;
        }
            /**
     * 初始化界面
     */
        private void initHeaderViewAndFooterViewAndListView(Context context)
        {

            this.Orientation = Orientation.Vertical;
            //setDrawingCacheEnabled(false);
            /*
             * 自定义头部文件
             * 放在这里是因为考虑到很多界面都需要使用
             * 如果要修改，和它相关的设置都要更改
             */

            mHeaderView = LayoutInflater.From(context).Inflate(Resource.Layout.pulldown_header, null);
            mHeaderViewParams = new LayoutParams(LayoutParams.FillParent, LayoutParams.WrapContent);
            this.AddView(mHeaderView, 0, mHeaderViewParams);

            mHeaderTextView = (TextView)mHeaderView.FindViewById<TextView>(Resource.Id.pulldown_header_text);
            mHeaderArrowView = (ImageView)mHeaderView.FindViewById<ImageView>(Resource.Id.pulldown_header_arrow);
            mHeaderLoadingView = mHeaderView.FindViewById(Resource.Id.pulldown_header_loading);
          


            // 注意，图片旋转之后，再执行旋转，坐标会重新开始计算
            mRotateOTo180Animation = new RotateAnimation(0, 180, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            mRotateOTo180Animation.Duration = 250;
            mRotateOTo180Animation.FillAfter = true;

            mRotate180To0Animation = new RotateAnimation(0, 180, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            mRotate180To0Animation.Duration = 250;
            mRotate180To0Animation.FillAfter = true;
            /**
             * 自定义脚部文件
             */

            mFooterView = LayoutInflater.From(context).Inflate(Resource.Layout.pulldown_footer, null);
            mFooterTextView = (TextView)mFooterView.FindViewById(Resource.Id.pulldown_footer_text);
            mFooterLoadingView = mFooterView.FindViewById(Resource.Id.pulldown_footer_loading);
            //mFooterView.SetOnClickListener(IOnClickListenerDelegate);
            mFooterView.Click += new EventHandler(mFooterView_Click);
            mListView = new ScrollOverListView(context);
            mListView.setOnScrollOverListener(this);
            mListView.CacheColorHint = Android.Graphics.Color.Argb(0, 0, 0, 0);
            AddView(mListView, LayoutParams.FillParent, LayoutParams.FillParent);
            mOnPullDownListener = new PullDownListener();

         
        }

        void mFooterView_Click(object sender, EventArgs e)
        {

            if (!mIsFetchMoreing)
            {
                mIsFetchMoreing = true;
                mFooterLoadingView.Visibility = ViewStates.Visible;
                mOnPullDownListener.onMore();
            }
        } 

        /// <summary>
        /// 属性调用
        /// </summary>
        public  mSetOnClick IOnClickListenerDelegate=new mSetOnClick ();
        /// <summary>
        /// 继承一个点击事件：IOnClickListener
        /// </summary>
        public class mSetOnClick : View.IOnClickListener
        { 
            void View.IOnClickListener.OnClick(View v)
            {
                if (!mIsFetchMoreing)
                {
                    mIsFetchMoreing = true;
                    mFooterLoadingView.Visibility = ViewStates.Visible;
                    mOnPullDownListener.onMore(); 
                }
            }

            public IntPtr Handle
            {
                get { return this.Handle; }
            }

            public void Dispose()
            {
                this.Dispose();
            }
        }



        /**
 * 在下拉和回推的时候检查头部文件的状态</br>
 * 如果超过了默认高度，就显示松开可以刷新，
 * 否则显示下拉可以刷新
 */
        private void checkHeaderViewState()
        {
            if (mHeaderViewParams.Height >= DEFAULT_HEADER_VIEW_HEIGHT)
            {
                if (mHeaderViewState == HEADER_VIEW_STATE_OVER_HEIGHT) return;
                mHeaderViewState = HEADER_VIEW_STATE_OVER_HEIGHT;
                mHeaderTextView.Text = "松开可以刷新";
                mHeaderArrowView.StartAnimation(mRotateOTo180Animation);
            }
            else
            {
                if (mHeaderViewState == HEADER_VIEW_STATE_NOT_OVER_HEIGHT
                        || mHeaderViewState == HEADER_VIEW_STATE_IDLE) return;
                mHeaderViewState = HEADER_VIEW_STATE_NOT_OVER_HEIGHT;
                mHeaderTextView.Text = "下拉可以刷新";
                mHeaderArrowView.StartAnimation(mRotate180To0Animation);
            }
        }


        private static void setHeaderHeight(int height)
        { 
            mHeaderIncremental = height;
            mHeaderViewParams.Height = height;
            mHeaderView.LayoutParameters = mHeaderViewParams;
        }
     
        	/**
	 * 自动隐藏动画
	 */
        class HideHeaderViewTask : Java.Util.TimerTask
        {

            public override void Run()
            {
                if (mIsDown)
                {
                    Cancel();
                    return;
                }
                mHeaderIncremental -= AUTO_INCREMENTAL;
                if (mHeaderIncremental > 0)
                {
                    mUIHandler.SendEmptyMessage(WHAT_SET_HEADER_HEIGHT);
                }
                else
                {
                    mHeaderIncremental = 0;
                    mUIHandler.SendEmptyMessage(WHAT_SET_HEADER_HEIGHT);
                    Cancel();
                }
            }
        }

        	/**
	 * 自动显示动画
	 */
        class ShowHeaderViewTask : Java.Util.TimerTask
        {

            public override void Run()
            {
                if (mIsDown)
                {
                    Cancel();
                    return;
                }
                mHeaderIncremental -= AUTO_INCREMENTAL;
                if (mHeaderIncremental > DEFAULT_HEADER_VIEW_HEIGHT)
                {
                    mUIHandler.SendEmptyMessage(WHAT_SET_HEADER_HEIGHT);
                }
                else
                {
                    mHeaderIncremental = DEFAULT_HEADER_VIEW_HEIGHT;
                    mUIHandler.SendEmptyMessage(WHAT_SET_HEADER_HEIGHT);
                    if (!mIsRefreshing)
                    {
                        mIsRefreshing = true;
                        mUIHandler.SendEmptyMessage(WHAT_ON_REFRESH);
                    }
                    Cancel();
                }
            }
        }

        /**
	 * 显示脚步脚部文件
	 */
        private static void showFooterView()
        { 
            if (mListView.FooterViewsCount == 0 && isFillScreenItem())
            {
                mListView.AddFooterView(mFooterView);
                mListView.Adapter = mListView.Adapter;
            }
        }

        /**
         * 条目是否填满整个屏幕
         */
        private static bool isFillScreenItem()
        {
            int firstVisiblePosition = mListView.FirstVisiblePosition;
            int lastVisiblePostion = mListView.LastVisiblePosition - mListView.FooterViewsCount;
            int visibleItemCount = lastVisiblePostion - firstVisiblePosition + 1;
            int totalItemCount = mListView.Count - mListView.FooterViewsCount;

            if (visibleItemCount < totalItemCount) return true;
            return false;
        }

        
        private new static UIHandler mUIHandler = new UIHandler();

        private class UIHandler : Handler
        {
            public override void HandleMessage(Message msg)
            {
                //base.HandleMessage(msg);
                switch (msg.What)
                {
                        //数据加载完毕。。
                    case WHAT_DID_LOAD_DATA:
                        mHeaderViewParams.Height = 0;
                        mHeaderLoadingView.Visibility = ViewStates.Gone;

                        mHeaderTextView.Text = "下拉可以刷新";
                        mHeaderViewDateView = (TextView)mHeaderView.FindViewById(Resource.Id.pulldown_header_date);
                        mHeaderViewDateView.Visibility = ViewStates.Visible;
                        mHeaderViewDateView.Text = "更新于：" + DateTime.Now.ToString("HH:mm:ss");
                        mHeaderArrowView.Visibility = ViewStates.Visible;
                        showFooterView();
                        break;
                    case WHAT_ON_REFRESH:
                        // 要清除掉动画，否则无法隐藏 
                        mHeaderArrowView.ClearAnimation();
                        mHeaderArrowView.Visibility = ViewStates.Invisible;
                        mHeaderLoadingView.Visibility = ViewStates.Visible;
                        mOnPullDownListener.onRefresh();
                        break;
                    case WHAT_DID_REFRESH:
                        mIsRefreshing = false;
                        mHeaderViewState = HEADER_VIEW_STATE_IDLE;
                        mHeaderArrowView.Visibility = ViewStates.Visible;
                        mHeaderLoadingView.Visibility = ViewStates.Gone;
                        mHeaderViewDateView.Text = "更新于：" + DateTime.Now.ToString("HH:mm:ss");
                        setHeaderHeight(0);
                        showFooterView();
                        break;
                    case WHAT_SET_HEADER_HEIGHT:
                        setHeaderHeight(mHeaderIncremental);
                        break;
                    case WHAT_DID_MORE:
                        mIsFetchMoreing = false;
                        mFooterTextView.Text = "点击获取更多";
                        mFooterLoadingView.Visibility = ViewStates.Gone;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool onListViewTopAndPullDown(int delta)
        {
            if (mIsRefreshing || mListView.Count - mListView.FooterViewsCount == 0) return false;

            int absDelta = Math.Abs(delta);
            int i = (int)Math.Ceiling((double)absDelta / 2);
            mHeaderIncremental += i;
            if (mHeaderIncremental >= 0)
            { // && mIncremental <= mMaxHeight
                setHeaderHeight(mHeaderIncremental);
                checkHeaderViewState();
            }
            return true;
        }

        public bool onListViewBottomAndPullUp(int delta)
        {
            if (!mEnableAutoFetchMore || mIsFetchMoreing) return false;
            // 数量充满屏幕才触发
            if (isFillScreenItem())
            {
                mIsFetchMoreing = true;
                mFooterTextView.Text=("加载更多中...");
                mFooterLoadingView.Visibility = ViewStates.Visible;
                mOnPullDownListener.onMore();
                return true;
            }
            return false;
        }

        public bool onMotionDown(MotionEvent ev)
        {
            mIsDown = true;
            mIsPullUpDone = false;
            mMotionDownLastY = ev.RawY;
            return false;
        }

        public bool onMotionMove(MotionEvent ev, int delta)
        {
            //当头部文件回推消失的时候，不允许滚动
            if (mIsPullUpDone) return true;

            // 如果开始按下到滑动距离不超过误差值，则不滑动
            int absMotionY = (int)Math.Abs(ev.RawY - mMotionDownLastY);
            if (absMotionY < START_PULL_DEVIATION) return true;

            int absDelta = Math.Abs(delta);
            int i = (int)Math.Ceiling((double)absDelta / 2);

            // onTopDown在顶部，并上回推和onTopUp相对
            if (mHeaderViewParams.Height > 0 && delta < 0)
            {
                mHeaderIncremental -= i;
                if (mHeaderIncremental > 0)
                {
                    setHeaderHeight(mHeaderIncremental);
                    checkHeaderViewState();
                }
                else
                {
                    mHeaderViewState = HEADER_VIEW_STATE_IDLE;
                    mHeaderIncremental = 0;
                    setHeaderHeight(mHeaderIncremental);
                    mIsPullUpDone = true;
                }
                return true;
            }
            return false;
        }

        public bool onMotionUp(MotionEvent ev)
        {
            mIsDown = false;
            // 避免和点击事件冲突
            if (mHeaderViewParams.Height > 0)
            {
                // 判断头文件拉动的距离与设定的高度，小了就隐藏，多了就固定高度
                int x = mHeaderIncremental - DEFAULT_HEADER_VIEW_HEIGHT;
                Java.Util.Timer timer = new Java.Util.Timer(true);
                if (x < 0)
                { 
                    timer.ScheduleAtFixedRate(new HideHeaderViewTask(), 0, 10);
                }
                else
                {
                    timer.ScheduleAtFixedRate(new ShowHeaderViewTask(), 0, 10);
                }
                return true;
            }
            return false;
        }
    }


  
}