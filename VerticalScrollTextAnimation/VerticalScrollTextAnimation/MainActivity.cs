using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Android.OS;
using Java.Lang;

namespace VerticalScrollTextAnimation
{
    [Activity(Label = "VerticalScrollTextAnimation", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Animation anim_in, anim_out;
        private LinearLayout llContainer;
        private Handler mHandler;
        private bool runFlag = true;
        private int index = 0;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            // 如果是Home键等其他操作导致销毁重建，那么判断是否存在，  
            // 如果存在，那么获取上次滚动到状态的index  
            // 如果不存在，那么调用初始化方法  
            if (null != bundle)
            {

                index = bundle.GetInt("currIndex");
                //Log.d("tag", "The savedInstanceState.getInt value is" + index);
            }
            else
            {
                Init();
            }
        }

        private void Init()
        {
            // TODO Auto-generated method stub  
            // 找到装载这个滚动TextView的LinearLayout  
            llContainer = FindViewById<LinearLayout>(Resource.Id.ll_container);
            // 加载进入动画  
            anim_in = AnimationUtils.LoadAnimation(this, Resource.Animation.anim_tv_marquee_in);
            // 加载移除动画  
            anim_out = AnimationUtils.LoadAnimation(this, Resource.Animation.anim_tv_marquee_out);
            // 填充装文字的list  
            List<string> list = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                list.Add("滚动的文字" + i);
            }
            // 根据list的大小，动态创建同样个数的TextView  
            for (int i = 0; i < list.Count; i++)
            {
                var tvTemp = new TextView(this);
                var lp = new ActionBar.LayoutParams(WindowManagerLayoutParams.WrapContent,
                        WindowManagerLayoutParams.WrapContent);
                lp.Gravity = GravityFlags.Center;
                tvTemp.Gravity = (GravityFlags.Center);
                tvTemp.Text = (list[i]);
                tvTemp.Id = (i + 10000);
                llContainer.AddView(tvTemp);
            }

            mHandler = new HandlerDemo(this.anim_in, this.anim_out);
        }

        /*** 
         * 停止动画 
         */
        private void stopEffect()
        {
            runFlag = false;
        }

        /*** 
         * 启动动画 
         */
        private void startEffect()
        {

            runFlag = true;
            new Thread(() =>
            {

                while (runFlag)
                {
                    try
                    {
                        // 每隔2秒轮换一次  
                        Thread.Sleep(2000);
                        // 至于这里还有一个if(runFlag)判断是为什么？大家自己试验下就知道了  
                        if (runFlag)
                        {
                            // 获取第index个TextView开始移除动画  
                            var tvTemp = (TextView)llContainer.GetChildAt(index);
                            mHandler.ObtainMessage(0, tvTemp).SendToTarget();
                            if (index < llContainer.ChildCount)
                            {
                                index++;
                                if (index == llContainer.ChildCount)
                                {
                                    index = 0;
                                }
                                // index+1个动画开始进入动画  
                                tvTemp = (TextView)llContainer.GetChildAt(index);
                                mHandler.ObtainMessage(1, tvTemp).SendToTarget();
                            }
                        }
                    }
                    catch
                    {
                        // TODO Auto-generated catch block  
                        // 如果有异常，那么停止轮换。当然这种情况很难发生  
                        runFlag = false;
                        // e.printStackTrace();  
                    }
                }

            }).Start();
        }

        /*** 
         * 当页面暂停，那么停止轮换 
         */
        protected override void OnPause()
        {
            // TODO Auto-generated method stub  
            base.OnPause();
            stopEffect();
        }

        /*** 
         * 当页面可见，开始轮换 
         */
        protected override void OnResume()
        {
            // TODO Auto-generated method stub  
            base.OnResume();
            startEffect();
        }

        /*** 
         * 用于保存当前index的,结合onCreate方法 
         */
        protected override void OnSaveInstanceState(Bundle outState)
        {
            // TODO Auto-generated method stub  
            base.OnSaveInstanceState(outState);
            outState.PutInt("currIndex", index);
        }
    }

    public class HandlerDemo : Handler
    {
        private Animation anim_in, anim_out;

        public HandlerDemo(Animation animIn, Animation animOut)
        {
            this.anim_in = animIn;
            this.anim_out = animOut;
        }

        public override void HandleMessage(Message msg)
        {
            base.HandleMessage(msg);
            switch (msg.What)
            {
                case 0:
                    // 移除  
                    TextView tvTemp = (TextView)msg.Obj;
                    //Log.d("tag", "out->" + tvTemp.getId());
                    tvTemp.StartAnimation(anim_out);
                    tvTemp.Visibility = (ViewStates.Gone);
                    break;
                case 1:
                    // 进入  
                    TextView tvTemp2 = (TextView)msg.Obj;
                    //Log.d("tag", "in->" + tvTemp2.getId());
                    tvTemp2.StartAnimation(anim_in);
                    tvTemp2.Visibility = ViewStates.Visible;
                    break;
            }
        }
    }
}

