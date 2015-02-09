using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace SilderDemo
{
    [Activity(Label = "SilderDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static String TAG = "Main Activity";
        private View mMenu;
        private int mMenuWidth;

        private SlideLayout mSlideLayout;
        private ImageView mMenuButton;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.activity_main);

            mMenu = FindViewById<View>(Resource.Id.menu);
            mSlideLayout = FindViewById<SlideLayout>(Resource.Id.slide_layout);

            ViewTreeObserver vto = mMenu.ViewTreeObserver;
            //vto.addOnGlobalLayoutListener(new OnGlobalLayoutListener() { 
            //    @Override
            //    public void onGlobalLayout() {
            //        mMenu.getViewTreeObserver().removeGlobalOnLayoutListener(this);
            //        mMenuWidth = mMenu.getWidth();
            //        mSlideLayout.setMaxScrollX(mMenuWidth);
            //        Log.v(TAG, "Max Scroll Distance: " + mMenuWidth);
            //    }
            //});
            vto.GlobalLayout += (sender, e) =>
            {
                //mMenu.ViewTreeObserver.RemoveGlobalOnLayoutListener();
                mMenuWidth = mMenu.Width;
                mSlideLayout.SetMaxScrollX(mMenuWidth);
                Log.Verbose(TAG, "Max Scroll Distance: " + mMenuWidth);
            };

            mMenuButton = FindViewById<ImageView>(Resource.Id.menuButton);
            mMenuButton.Click += (sender, e) =>
            {
                var imageView = sender as ImageView;
                switch (imageView.Id)
                {
                    case Resource.Id.menuButton:
                        {
                            if (mSlideLayout.IsMenuOpen())
                            {
                                mSlideLayout.CloseMenu();
                            }
                            else
                            {
                                mSlideLayout.OpenMenu();
                                //mMenuWidth = mMenu.Width;
                                //mSlideLayout.SetMaxScrollX(mMenuWidth);
                            }
                            break;
                        }
                    default:
                        break;
                }
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            Log.Verbose(TAG, "Main Touch Here");
            return base.OnTouchEvent(e);
        }
    }

    public class SlideLayout : ViewGroup
    {
        private static String TAG = "SlideMenuLayout";

        private Context mContext;
        private Scroller mScroller;    //Android 提供的滑动辅助类
        private int mTouchSlop = 0;    //在被判定为滚动之前用户手指可以移动的最大值
        private VelocityTracker mVelocityTracker;    //用于计算手指滑动的速度
        public static int SNAP_VELOCITY = 200;    //滚动显示和隐藏左侧布局时，手指滑动需要达到的速度：每秒200个像素点
        private int mMaxScrollX = 0;    //最大滚动距离，等于menu的宽度
        public void SetMaxScrollX(int maxScrollX)
        {
            this.mMaxScrollX = maxScrollX;
        }

        private float mDownX;    //一次按下抬起的动作中，按下时的X坐标，用于和抬起时的X比较，判断移动距离。少于mTouchSlop则判定为原地点击
        private float mLastX;    //记录滑动过程中的X坐标

        private bool isMenuOpen = false;    //菜单界面是否被打开，只有完全打开才为true
        public bool IsMenuOpen()
        {
            return this.isMenuOpen;
        }

        private bool isTouchFinished = true;

        private View mContent;

        public SlideLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            //super(context, attrs);
            mContext = context;
            Init();
        }

        private void Init()
        {
            Log.Verbose(TAG, "init start");
            mScroller = new Scroller(mContext);
            mTouchSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;
        }

        public override void ComputeScroll()
        {
            if (mScroller.ComputeScrollOffset())
            {
                ScrollTo(mScroller.CurrX, mScroller.CurrY);
                PostInvalidate();
            }
        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {

            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            SetMeasuredDimension(width, height);

            int childCount = ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                View child = GetChildAt(i);
                child.Measure(widthMeasureSpec, heightMeasureSpec);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            mContent = GetChildAt(1);
            if (mContent != null)
            {
                mContent.Layout(l, t, l + mContent.MeasuredWidth, t + mContent.MeasuredHeight);
            }

            int menuWidth = 0;
            View menu = GetChildAt(0);
            if (menu != null)
            {
                ViewGroup.LayoutParams layoutParams = menu.LayoutParameters;
                menuWidth = layoutParams.Width;
                menu.Layout(l - menuWidth, t, l, t + menu.MeasuredHeight);
            }
        }


        public override bool OnTouchEvent(MotionEvent e)
        {
            CreateVelocityTracker(e);
            int curScrollX = this.ScrollX;
            // 检查触摸点是否在滑动布局(内容content)中，如果不是则返回false，即本View不处理该事件
            if (mContent != null && isTouchFinished)
            {
                Rect rect = new Rect();
                mContent.GetHitRect(rect);
                if (!rect.Contains((int)e.GetX() + curScrollX, (int)e.GetY()))
                {
                    return false;
                }
            }

            float x = e.GetX();    //取得本次event的X坐标
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mDownX = x;
                    mLastX = x;
                    isTouchFinished = false;
                    //			Log.v(TAG, "(Down)mDownX: " + x);
                    //			Log.v(TAG, "(Down)mLastX: " + x);
                    break;
                case MotionEventActions.Move:
                    //			Log.v(TAG, "(Move)mLastX: " + x);
                    int deltaX = (int)(mLastX - x);
                    if ((curScrollX + deltaX) < -mMaxScrollX)
                    {
                        deltaX = -mMaxScrollX - curScrollX;
                    }
                    if ((curScrollX + deltaX) > 0)
                    {
                        deltaX = -curScrollX;
                    }

                    if (deltaX != 0)
                    {
                        ScrollBy(deltaX, 0);
                    }
                    mLastX = x;
                    //			Log.v(TAG, "(Move)x: " + x);
                    break;
                case MotionEventActions.Up:
                    //			Log.v(TAG, "(Up)x: " + x);
                    //			Log.v(TAG, "(Up)mDownX: " + mDownX);
                    //			Log.v(TAG, "(Up)curScrollX: " + curScrollX);
                    int velocityX = GetScrollVelocity();
                    int offsetX = (int)(x - mDownX);

                    //成立表明移动距离已经达到被判断为滑动的最低标准
                    //不成立表明不被判断为滑动，则认为是单一的点击，则关闭menu
                    if (Math.Abs(offsetX) >= mTouchSlop)
                    {

                        //成立表明手指移动速度达标，则进行自动滑动；
                        //不成立表明速度不达标，但仍然需要判断当前SlideLayout的位置
                        //如果已经超过一半，则继续自动完成剩下的滑动，如果没有超过一半，则反向滑动
                        if (Math.Abs(velocityX) >= SNAP_VELOCITY)
                        {
                            if (velocityX > 0)
                            {
                                OpenMenu();
                            }
                            else if (velocityX < 0)
                            {
                                CloseMenu();
                            }
                        }
                        else
                        {
                            if (curScrollX >= -mMaxScrollX / 2)
                            {
                                CloseMenu();
                            }
                            else
                            {
                                OpenMenu();
                            }
                        }
                    }
                    else
                    {
                        CloseMenu();
                    }

                    RecycleVelocityTracker();
                    isTouchFinished = true;
                    break;
            }
            return true;
        }

        private void CreateVelocityTracker(MotionEvent e)
        {
            if (mVelocityTracker == null)
            {
                mVelocityTracker = VelocityTracker.Obtain();
            }
            mVelocityTracker.AddMovement(e);
        }

        //获取手指在View上的滑动速度,以每秒钟移动了多少像素值为单位
        private int GetScrollVelocity()
        {
            mVelocityTracker.ComputeCurrentVelocity(1000);
            return (int)mVelocityTracker.XVelocity;
        }

        private void RecycleVelocityTracker()
        {
            mVelocityTracker.Recycle();
            mVelocityTracker = null;
        }

        //打开Menu布局
        public void OpenMenu()
        {
            int curScrollX = ScrollX;
            ScrollToDestination(-mMaxScrollX - curScrollX);
            isMenuOpen = true;
        }

        //关闭Menu布局
        public void CloseMenu()
        {
            int curScrollX = ScrollX;
            ScrollToDestination(-curScrollX);
            isMenuOpen = false;
        }

        private void ScrollToDestination(int x)
        {
            if (x == 0)
                return;
            mScroller.StartScroll(ScrollX, 0, x, 0, Math.Abs(x));
            Invalidate();
        }
    }


    //public class SlideLayout : Android.Widget.RelativeLayout
    //{
    //    private static String TAG = "SlideMenuLayout";

    //    private Context mContext;
    //    private Scroller mScroller;    //Android 提供的滑动辅助类
    //    private int mTouchSlop = 0;    //在被判定为滚动之前用户手指可以移动的最大值
    //    private VelocityTracker mVelocityTracker;    //用于计算手指滑动的速度
    //    public static int SNAP_VELOCITY = 200;    //滚动显示和隐藏左侧布局时，手指滑动需要达到的速度：每秒200个像素点
    //    private int mMaxScrollX = 0;    //最大滚动距离，等于menu的宽度
    //    public void SetMaxScrollX(int maxScrollX)
    //    {
    //        this.mMaxScrollX = maxScrollX;
    //    }

    //    private float mDownX;    //一次按下抬起的动作中，按下时的X坐标，用于和抬起时的X比较，判断移动距离。少于mTouchSlop则判定为原地点击
    //    private float mLastX;    //记录滑动过程中的X坐标

    //    private bool isMenuOpen = false;    //菜单界面是否被打开，只有完全打开才为true
    //    public bool IsMenuOpen()
    //    {
    //        return this.isMenuOpen;
    //    }

    //    private bool isTouchFinished = true;

    //    private View mContent;

    //    public SlideLayout(Context context, IAttributeSet attrs)
    //        : base(context, attrs)
    //    {
    //        //super(context, attrs);
    //        mContext = context;
    //        Init();
    //    }

    //    private void Init()
    //    {
    //        Log.Verbose(TAG, "init start");
    //        mScroller = new Scroller(mContext);
    //        mTouchSlop = ViewConfiguration.Get(Context).ScaledTouchSlop;
    //    }

    //    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    //    {
    //        base.OnLayout(changed, l, t, r, b);
    //        if (changed)
    //        {
    //            mContent = this.GetChildAt(0);
    //        }
    //    }

    //    public override bool OnTouchEvent(MotionEvent e)
    //    {
    //        CreateVelocityTracker(e);
    //        int curScrollX = this.ScrollX;
    //        // 检查触摸点是否在滑动布局(内容content)中，如果不是则返回false，即本View不处理该事件
    //        if (mContent != null && isTouchFinished)
    //        {
    //            Rect rect = new Rect();
    //            mContent.GetHitRect(rect);
    //            if (!rect.Contains((int)e.GetX() + curScrollX, (int)e.GetY()))
    //            {
    //                return false;
    //            }
    //        }

    //        float x = e.GetX();    //取得本次event的X坐标
    //        switch (e.Action)
    //        {
    //            case MotionEventActions.Down:
    //                mDownX = x;
    //                mLastX = x;
    //                isTouchFinished = false;
    //                //			Log.v(TAG, "(Down)mDownX: " + x);
    //                //			Log.v(TAG, "(Down)mLastX: " + x);
    //                break;
    //            case MotionEventActions.Move:
    //                //			Log.v(TAG, "(Move)mLastX: " + x);
    //                int deltaX = (int)(mLastX - x);
    //                if ((curScrollX + deltaX) < -mMaxScrollX)
    //                {
    //                    deltaX = -mMaxScrollX - curScrollX;
    //                }
    //                if ((curScrollX + deltaX) > 0)
    //                {
    //                    deltaX = -curScrollX;
    //                }

    //                if (deltaX != 0)
    //                {
    //                    ScrollBy(deltaX, 0);
    //                }
    //                mLastX = x;
    //                //			Log.v(TAG, "(Move)x: " + x);
    //                break;
    //            case MotionEventActions.Up:
    //                //			Log.v(TAG, "(Up)x: " + x);
    //                //			Log.v(TAG, "(Up)mDownX: " + mDownX);
    //                //			Log.v(TAG, "(Up)curScrollX: " + curScrollX);
    //                int velocityX = GetScrollVelocity();
    //                int offsetX = (int)(x - mDownX);

    //                //成立表明移动距离已经达到被判断为滑动的最低标准
    //                //不成立表明不被判断为滑动，则认为是单一的点击，则关闭menu
    //                if (Math.Abs(offsetX) >= mTouchSlop)
    //                {

    //                    //成立表明手指移动速度达标，则进行自动滑动；
    //                    //不成立表明速度不达标，但仍然需要判断当前SlideLayout的位置
    //                    //如果已经超过一半，则继续自动完成剩下的滑动，如果没有超过一半，则反向滑动
    //                    if (Math.Abs(velocityX) >= SNAP_VELOCITY)
    //                    {
    //                        if (velocityX > 0)
    //                        {
    //                            OpenMenu();
    //                        }
    //                        else if (velocityX < 0)
    //                        {
    //                            CloseMenu();
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (curScrollX >= -mMaxScrollX / 2)
    //                        {
    //                            CloseMenu();
    //                        }
    //                        else
    //                        {
    //                            OpenMenu();
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    CloseMenu();
    //                }

    //                RecycleVelocityTracker();
    //                isTouchFinished = true;
    //                break;
    //        }
    //        return true;
    //    }

    //    private void CreateVelocityTracker(MotionEvent e)
    //    {
    //        if (mVelocityTracker == null)
    //        {
    //            mVelocityTracker = VelocityTracker.Obtain();
    //        }
    //        mVelocityTracker.AddMovement(e);
    //    }

    //    //获取手指在View上的滑动速度,以每秒钟移动了多少像素值为单位
    //    private int GetScrollVelocity()
    //    {
    //        mVelocityTracker.ComputeCurrentVelocity(1000);
    //        return (int)mVelocityTracker.XVelocity;
    //    }

    //    private void RecycleVelocityTracker()
    //    {
    //        mVelocityTracker.Recycle();
    //        mVelocityTracker = null;
    //    }

    //    //打开Menu布局
    //    public void OpenMenu()
    //    {
    //        int curScrollX = ScrollX;
    //        ScrollToDestination(-mMaxScrollX - curScrollX);
    //        isMenuOpen = true;
    //    }

    //    //关闭Menu布局
    //    public void CloseMenu()
    //    {
    //        int curScrollX = ScrollX;
    //        ScrollToDestination(-curScrollX);
    //        isMenuOpen = false;
    //    }

    //    private void ScrollToDestination(int x)
    //    {
    //        if (x == 0)
    //            return;
    //        mScroller.StartScroll(ScrollX, 0, x, 0, Math.Abs(x));
    //        Invalidate();
    //    }
    //}
}

