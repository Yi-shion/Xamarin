using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using RotateMenu;

namespace RoundSpinViewDemo.Views
{
    class SpinHandler : Handler
    {
        private RoundSpinView curView;

        public SpinHandler(RoundSpinView view)
            : base()
        {
            this.curView = view;
        }

        public override void HandleMessage(Message msg)
        {
            switch (msg.What)
            {
                case RoundSpinView.TO_ROTATE_BUTTON:
                    float velocity = float.Parse(msg.Obj.ToString());
                    this.curView.rotateButtons(velocity / 75);
                    velocity /= 1.0666F;
                    Thread t = new Thread(new ParameterizedThreadStart(this.curView.FlingRunner));
                    t.Start(velocity);
                    break;

                default:
                    break;
            }
        }
    }

    // 圆盘式的view
    class RoundSpinView : View
    {
        private Paint mPaint = new Paint();
        private PaintFlagsDrawFilter pfd;

        private int startMenu;   //菜单的第一张图片的资源id

        // stone列表
        private BigStone[] mStones;
        // 数目
        private static int STONE_COUNT = 3;

        // 圆心坐标
        private int mPointX = 0, mPointY = 0;
        // 半径
        private int mRadius = 0;
        // 每两个点间隔的角度
        private int mDegreeDelta;

        private int menuRadius; // 菜单的半径

        private int mCur = -1; // 正在被移动的menu;

        private bool[] quadrantTouched;   //对每个象限触摸情况的记录

        // Touch detection
        private GestureDetector mGestureDetector;

        private OnRoundSpinViewListener mListener;  //自定义事件监听器

        public const int TO_ROTATE_BUTTON = 0;  //旋转按钮；

        public SpinHandler handler;

        public void FlingRunner(object velocity)
        {
            float cur_velocity = (float)velocity;
            if (Math.Abs(cur_velocity) >= 200)
            {
                Message message = Message.Obtain();
                message.What = TO_ROTATE_BUTTON;
                message.Obj = cur_velocity;
                this.handler.SendMessage(message);
            }
        }


        public RoundSpinView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            this.handler = new SpinHandler(this);

            if (attrs != null)
            {
                TypedArray a =
                    this.Context.ObtainStyledAttributes(attrs, Resource.Styleable.RoundSpinView);
                startMenu = a.GetResourceId(Resource.Styleable.RoundSpinView_menuStart, 0);
            }
            pfd = new PaintFlagsDrawFilter(0, PaintFlags.AntiAlias | PaintFlags.FilterBitmap);
            mPaint.Color = Color.White;
            mPaint.StrokeWidth = 2;
            mPaint.AntiAlias = true; //消除锯齿  
            mPaint.SetStyle(Paint.Style.Stroke); //绘制空心圆 
            PathEffect effects = new DashPathEffect(new float[] { 5, 5, 5, 5 }, 1);
            mPaint.SetPathEffect(effects);


            quadrantTouched = new Boolean[] { false, false, false, false, false };
            mGestureDetector = new GestureDetector(this.Context, new MyGestureListener(this));

            setupStones();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // TODO Auto-generated method stub
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            mPointX = this.MeasuredWidth / 2;
            mPointY = this.MeasuredHeight / 2;

            //初始化半径和菜单半径
            mRadius = mPointX - mPointX / 5;
            menuRadius = (int)(mPointX / 5.5);

            computeCoordinates();
        }

        /**
         * 初始化每个点
         */
        private void setupStones()
        {
            mStones = new BigStone[STONE_COUNT];
            BigStone stone;
            int angle = 270;
            mDegreeDelta = 360 / STONE_COUNT;

            for (int index = 0; index < STONE_COUNT; index++)
            {
                stone = new BigStone();
                if (angle >= 360)
                {
                    angle -= 360;
                }
                else if (angle < 0)
                {
                    angle += 360;
                }
                stone.angle = angle;
                stone.bitmap = BitmapFactory.DecodeResource(this.Resources, startMenu + index);
                angle += mDegreeDelta;

                mStones[index] = stone;
            }
        }

        /**
         * 重新计算每个点的角度
         */
        private void resetStonesAngle(float x, float y)
        {
            int angle = computeCurrentAngle(x, y);
            Log.Debug("RoundSpinView", "angle:" + angle);
            for (int index = 0; index < STONE_COUNT; index++)
            {
                mStones[index].angle = angle;
                angle += mDegreeDelta;
            }
        }

        /**
         * 计算每个点的坐标
         */
        private void computeCoordinates()
        {
            BigStone stone;
            for (int index = 0; index < STONE_COUNT; index++)
            {
                stone = mStones[index];
                stone.x = mPointX
                        + (float)(mRadius * Math.Cos(this.ToRadians(stone.angle))); // Math.toRadians(stone.angle)
                stone.y = mPointY
                        + (float)(mRadius * Math.Sin(this.ToRadians(stone.angle)));
            }
        }

        private double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }

        /**
         * 计算某点的角度
         * 
         * @param x
         * @param y
         * @return
         */
        private int computeCurrentAngle(float x, float y)
        {
            float distance =
                (float)Math.Sqrt(((x - mPointX) * (x - mPointX) + (y - mPointY) * (y - mPointY)));
            int degree = (int)(Math.Acos((x - mPointX) / distance) * 180 / Math.PI);
            if (y < mPointY)
            {
                degree = -degree;
            }

            Log.Debug("RoundSpinView", "x:" + x + ",y:" + y + ",degree:" + degree);
            return degree;
        }

        private double startAngle;

        public override bool DispatchTouchEvent(MotionEvent eventx)
        {
            // resetStonesAngle(event.getX(), event.getY());
            // computeCoordinates();
            // invalidate();

            int x, y;
            if (eventx.Action == MotionEventActions.Down)
            {
                x = (int)eventx.GetX();
                y = (int)eventx.GetY();
                mCur = getInCircle(x, y);
                if (mCur == -1)
                {
                    startAngle = computeCurrentAngle(x, y);
                }
            }
            else if (eventx.Action == MotionEventActions.Move)
            {
                x = (int)eventx.GetX();
                y = (int)eventx.GetY();
                if (mCur != -1)
                {
                    mStones[mCur].x = x;
                    mStones[mCur].y = y;
                    Invalidate();
                }
                else
                {
                    double currentAngle = computeCurrentAngle(x, y);
                    rotateButtons(startAngle - currentAngle);
                    startAngle = currentAngle;
                }
            }
            else if (eventx.Action == MotionEventActions.Up)
            {
                x = (int)eventx.GetX();
                y = (int)eventx.GetY();
                if (mCur != -1)
                {
                    computeCoordinates();
                    int cur = getInCircle(x, y);
                    if (cur != mCur && cur != -1)
                    {
                        int angle = mStones[mCur].angle;
                        mStones[mCur].angle = mStones[cur].angle;
                        mStones[cur].angle = angle;
                    }
                    computeCoordinates();
                    Invalidate();
                    mCur = -1;
                }
            }

            // set the touched quadrant to true
            quadrantTouched[getQuadrant(eventx.GetX() - mPointX,
                mPointY - eventx.GetY())] = true;
            mGestureDetector.OnTouchEvent(eventx);
            return true;
        }

        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            // http://www.tuicool.com/articles/RJjyAb

            private RoundSpinView curView = null;
            public MyGestureListener(RoundSpinView view)
            {
                this.curView = view;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX,
                    float velocityY)
            {
                // get the quadrant of the start and the end of the fling
                int q1 = getQuadrant(e1.GetX() - this.curView.mPointX, this.curView.mPointY - e1.GetY());
                int q2 = getQuadrant(e2.GetX() - this.curView.mPointX, this.curView.mPointY - e2.GetY());

                // the inversed rotations
                if ((q1 == 2 && q2 == 2 && Math.Abs(velocityX) < Math.Abs(velocityY))
                        || (q1 == 3 && q2 == 3)
                        || (q1 == 1 && q2 == 3)
                        || (q1 == 4 && q2 == 4 && Math.Abs(velocityX) > Math
                                .Abs(velocityY))
                        || ((q1 == 2 && q2 == 3) || (q1 == 3 && q2 == 2))
                        || ((q1 == 3 && q2 == 4) || (q1 == 4 && q2 == 3))
                        || (q1 == 2 && q2 == 4 && this.curView.quadrantTouched[3])
                        || (q1 == 4 && q2 == 2 && this.curView.quadrantTouched[3]))
                {

                    // CircleLayout.this.post(new FlingRunnable(-1
                    // * (velocityX + velocityY)));
                    new Thread(new ParameterizedThreadStart(this.curView.FlingRunner))
                        .Start(velocityX + velocityY);
                }
                else
                {
                    // the normal rotation
                    // CircleLayout.this
                    // .post(new FlingRunnable(velocityX + velocityY));
                    new Thread(new ParameterizedThreadStart(this.curView.FlingRunner))
                        .Start(-(velocityX + velocityY));
                }

                return true;

            }

            public override bool OnSingleTapUp(MotionEvent e)
            {

                int cur = this.curView.getInCircle((int)e.GetX(), (int)e.GetY());
                if (cur != -1)
                {
                    if (this.curView.mListener != null)
                    {
                        this.curView.mListener.onSingleTapUp(cur);
                    }
                    //	Toast.makeText(getContext(), "position:"+cur, 0).show();
                    return true;
                }
                return false;
            }

        }

        /**
         * @return The selected quadrant.
         */
        private static int getQuadrant(double x, double y)
        {
            if (x >= 0)
            {
                return y >= 0 ? 1 : 4;
            }
            else
            {
            }
            return y >= 0 ? 2 : 3;
        }

        /*
         * 旋转菜单按钮
         */
        public void rotateButtons(double degree)
        {
            for (int i = 0; i < STONE_COUNT; i++)
            {
                mStones[i].angle -= (int)degree;
                if (mStones[i].angle < 0)
                {
                    mStones[i].angle += 360;
                }
                else if (mStones[i].angle >= 360)
                {
                    mStones[i].angle -= 360;
                }
            }

            computeCoordinates();
            this.Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            //画一个白色的圆环
            canvas.DrawCircle(mPointX, mPointY, mRadius, mPaint);

            //将每个菜单画出来
            for (int index = 0; index < STONE_COUNT; index++)
            {
                if (!mStones[index].isVisible)
                    continue;
                drawInCenter(canvas, mStones[index].bitmap, mStones[index].x,
                        mStones[index].y);
            }
        }

        /**
         * 把中心点放到中心处
         * 
         * @param canvas
         * @param bitmap
         * @param left
         * @param top
         */
        private void drawInCenter(Canvas canvas, Bitmap bitmap, float left,
                float top)
        {
            Rect dst = new Rect();
            dst.Left = (int)(left - menuRadius);
            dst.Right = (int)(left + menuRadius);
            dst.Top = (int)(top - menuRadius);
            dst.Bottom = (int)(top + menuRadius);
            canvas.DrawFilter = pfd;
            canvas.DrawBitmap(bitmap, null, dst, mPaint);
        }

        private int getInCircle(int x, int y)
        {
            for (int i = 0; i < STONE_COUNT; i++)
            {
                BigStone stone = mStones[i];
                int mx = (int)stone.x;
                int my = (int)stone.y;
                if (((x - mx) * (x - mx) + (y - my) * (y - my)) < menuRadius
                        * menuRadius)
                {
                    return i;
                }
            }
            return -1;
        }

        public void setOnRoundSpinViewListener(OnRoundSpinViewListener listener)
        {
            this.mListener = listener;
        }


    }

    class BigStone
    {

        // 图片
        public Bitmap bitmap;

        // 角度
        public int angle;

        // x坐标
        public float x;

        // y坐标
        public float y;

        // 是否可见
        public bool isVisible = true;
    }

    interface OnRoundSpinViewListener
    {
        void onSingleTapUp(int position);  //监听每个菜单的单击事件
    }

}
