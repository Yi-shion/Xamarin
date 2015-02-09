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
     * ˢ���¼��ӿ�
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

        private const int START_PULL_DEVIATION = 50; // �ƶ����
        private const int AUTO_INCREMENTAL = 10;     // �����������ڻص�

        private const int WHAT_DID_LOAD_DATA = 1;    // Handler what ���ݼ������
        private const int WHAT_ON_REFRESH = 2;       // Handler what ˢ����
        private const int WHAT_DID_REFRESH = 3;      // Handler what �Ѿ�ˢ����
        private const int WHAT_SET_HEADER_HEIGHT = 4;// Handler what ���ø߶�
        private const int WHAT_DID_MORE = 5;         // Handler what �Ѿ���ȡ�����

        private const int DEFAULT_HEADER_VIEW_HEIGHT = 105;  // ͷ���ļ�ԭ���ĸ߶�

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

        private static int mHeaderIncremental; // ����
        private static float mMotionDownLastY; // ����ʱ���Y������

        private static bool mIsDown;            // �Ƿ���
        private static bool mIsRefreshing;      // �Ƿ�����ˢ����
        private static bool mIsFetchMoreing;    // �Ƿ��ȡ������
        private static bool mIsPullUpDone;      // �Ƿ�������
        private static bool mEnableAutoFetchMore;   // �Ƿ������Զ���ȡ����

        // ͷ���ļ���״̬
        private const int HEADER_VIEW_STATE_IDLE = 0;            // ����
        private const int HEADER_VIEW_STATE_NOT_OVER_HEIGHT = 1; // û�г���Ĭ�ϸ߶�
        private const int HEADER_VIEW_STATE_OVER_HEIGHT = 2;     // ����Ĭ�ϸ߶�
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
     * ֪ͨ�����������ݣ�Ҫ����Adapter.notifyDataSetChanged����
     * ������������ݵ�ʱ�򣬵������notifyDidLoad()
     * �Ż�����ͷ��������ʼ�����ݵ�
     */
        public void notifyDidLoad()
        {
            mUIHandler.SendEmptyMessage(WHAT_DID_LOAD_DATA);
        }


        /**
	 * ֪ͨ�Ѿ�ˢ�����ˣ�Ҫ����Adapter.notifyDataSetChanged����
	 * ����ִ����ˢ������֮�󣬵������notifyDidRefresh()
	 * �Ż����ص�ͷ���ļ��Ȳ���
	 */
        public void notifyDidRefresh()
        {
            mUIHandler.SendEmptyMessage(WHAT_DID_REFRESH);
        }

        /**
	 * ֪ͨ�Ѿ���ȡ������ˣ�Ҫ����Adapter.notifyDataSetChanged����
	 * ����ִ�����������֮�󣬵������notyfyDidMore()
	 * �Ż����ؼ���Ȧ�Ȳ���
	 */
        public void notifyDidMore()
        {
            mUIHandler.SendEmptyMessage(WHAT_DID_MORE);
        }
        /**
	 * ���ü�����
	 * @param listener
	 */
        public void setOnPullDownListener(OnPullDownListener listener)
        {
            mOnPullDownListener = listener;
        }
        /**
             * ��ȡ��Ƕ��listview
             * @return ScrollOverListView
             */
        public ListView getListView()
        {
            return mListView;
        }
        /**
	 * �Ƿ����Զ���ȡ����
	 * �Զ���ȡ���࣬��������footer�����ڵ���ײ���ʱ���Զ�ˢ��
	 * @param index �����ڼ�������
	 */
        public void enableAutoFetchMore(bool enable, int index)
        {
            if (enable)
            {
                //�ز���λ��
                mListView.setBottomPosition(index);
                //�ɼ���
                mFooterLoadingView.Visibility = ViewStates.Visible;
            }
            else
            {
                mFooterTextView.Text = "�����ȡ����";
                mFooterLoadingView.Visibility = ViewStates.Gone;
            }
            mEnableAutoFetchMore = enable;
        }
            /**
     * ��ʼ������
     */
        private void initHeaderViewAndFooterViewAndListView(Context context)
        {

            this.Orientation = Orientation.Vertical;
            //setDrawingCacheEnabled(false);
            /*
             * �Զ���ͷ���ļ�
             * ������������Ϊ���ǵ��ܶ���涼��Ҫʹ��
             * ���Ҫ�޸ģ�������ص����ö�Ҫ����
             */

            mHeaderView = LayoutInflater.From(context).Inflate(Resource.Layout.pulldown_header, null);
            mHeaderViewParams = new LayoutParams(LayoutParams.FillParent, LayoutParams.WrapContent);
            this.AddView(mHeaderView, 0, mHeaderViewParams);

            mHeaderTextView = (TextView)mHeaderView.FindViewById<TextView>(Resource.Id.pulldown_header_text);
            mHeaderArrowView = (ImageView)mHeaderView.FindViewById<ImageView>(Resource.Id.pulldown_header_arrow);
            mHeaderLoadingView = mHeaderView.FindViewById(Resource.Id.pulldown_header_loading);
          


            // ע�⣬ͼƬ��ת֮����ִ����ת����������¿�ʼ����
            mRotateOTo180Animation = new RotateAnimation(0, 180, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            mRotateOTo180Animation.Duration = 250;
            mRotateOTo180Animation.FillAfter = true;

            mRotate180To0Animation = new RotateAnimation(0, 180, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            mRotate180To0Animation.Duration = 250;
            mRotate180To0Animation.FillAfter = true;
            /**
             * �Զ���Ų��ļ�
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
        /// ���Ե���
        /// </summary>
        public  mSetOnClick IOnClickListenerDelegate=new mSetOnClick ();
        /// <summary>
        /// �̳�һ������¼���IOnClickListener
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
 * �������ͻ��Ƶ�ʱ����ͷ���ļ���״̬</br>
 * ���������Ĭ�ϸ߶ȣ�����ʾ�ɿ�����ˢ�£�
 * ������ʾ��������ˢ��
 */
        private void checkHeaderViewState()
        {
            if (mHeaderViewParams.Height >= DEFAULT_HEADER_VIEW_HEIGHT)
            {
                if (mHeaderViewState == HEADER_VIEW_STATE_OVER_HEIGHT) return;
                mHeaderViewState = HEADER_VIEW_STATE_OVER_HEIGHT;
                mHeaderTextView.Text = "�ɿ�����ˢ��";
                mHeaderArrowView.StartAnimation(mRotateOTo180Animation);
            }
            else
            {
                if (mHeaderViewState == HEADER_VIEW_STATE_NOT_OVER_HEIGHT
                        || mHeaderViewState == HEADER_VIEW_STATE_IDLE) return;
                mHeaderViewState = HEADER_VIEW_STATE_NOT_OVER_HEIGHT;
                mHeaderTextView.Text = "��������ˢ��";
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
	 * �Զ����ض���
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
	 * �Զ���ʾ����
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
	 * ��ʾ�Ų��Ų��ļ�
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
         * ��Ŀ�Ƿ�����������Ļ
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
                        //���ݼ�����ϡ���
                    case WHAT_DID_LOAD_DATA:
                        mHeaderViewParams.Height = 0;
                        mHeaderLoadingView.Visibility = ViewStates.Gone;

                        mHeaderTextView.Text = "��������ˢ��";
                        mHeaderViewDateView = (TextView)mHeaderView.FindViewById(Resource.Id.pulldown_header_date);
                        mHeaderViewDateView.Visibility = ViewStates.Visible;
                        mHeaderViewDateView.Text = "�����ڣ�" + DateTime.Now.ToString("HH:mm:ss");
                        mHeaderArrowView.Visibility = ViewStates.Visible;
                        showFooterView();
                        break;
                    case WHAT_ON_REFRESH:
                        // Ҫ����������������޷����� 
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
                        mHeaderViewDateView.Text = "�����ڣ�" + DateTime.Now.ToString("HH:mm:ss");
                        setHeaderHeight(0);
                        showFooterView();
                        break;
                    case WHAT_SET_HEADER_HEIGHT:
                        setHeaderHeight(mHeaderIncremental);
                        break;
                    case WHAT_DID_MORE:
                        mIsFetchMoreing = false;
                        mFooterTextView.Text = "�����ȡ����";
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
            // ����������Ļ�Ŵ���
            if (isFillScreenItem())
            {
                mIsFetchMoreing = true;
                mFooterTextView.Text=("���ظ�����...");
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
            //��ͷ���ļ�������ʧ��ʱ�򣬲��������
            if (mIsPullUpDone) return true;

            // �����ʼ���µ��������벻�������ֵ���򲻻���
            int absMotionY = (int)Math.Abs(ev.RawY - mMotionDownLastY);
            if (absMotionY < START_PULL_DEVIATION) return true;

            int absDelta = Math.Abs(delta);
            int i = (int)Math.Ceiling((double)absDelta / 2);

            // onTopDown�ڶ��������ϻ��ƺ�onTopUp���
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
            // ����͵���¼���ͻ
            if (mHeaderViewParams.Height > 0)
            {
                // �ж�ͷ�ļ������ľ������趨�ĸ߶ȣ�С�˾����أ����˾͹̶��߶�
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