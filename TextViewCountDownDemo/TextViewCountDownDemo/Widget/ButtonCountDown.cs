using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TextViewCountDownDemo
{
    public class ButtonCountDown : Button
    {
        public string DefaultValue { get; set; }
        public int DefaultCountDown { get; set; }
        private int _interval = 1000;
        public int Interval
        {
            get { return this._interval; }
            set { this._interval = value; }
        }
        public Timer ButtonTimer { get; set; }
        public Activity TempActivity { get; set; }

        private void InitTimer()
        {
            if (ButtonTimer != null)
            {
                return;
            }
            ButtonTimer = new Timer
            {
                Interval = this.Interval,
                Enabled = true
            };
            ButtonTimer.Stop();
            ButtonTimer.Elapsed += (sender, e) =>
            {
                DefaultCountDown--;
                TempActivity.RunOnUiThread(() =>
                {
                    this.Text = DefaultCountDown == 0 ? this.DefaultValue : this.DefaultCountDown.ToString();
                });
                if (DefaultCountDown != 0)
                {
                    return;
                }
                ButtonTimer.Stop();
                DefaultCountDown = 15;
            };
        }

        public void StartTimer()
        {
            if (ButtonTimer != null)
            {
                ButtonTimer.Start();
            }
        }

        public void StopTimer()
        {
            if (ButtonTimer != null)
            {
                ButtonTimer.Stop();
            }
        }

        public void ClearTimer()
        {
            if (ButtonTimer != null)
            {
                this.ButtonTimer = null;
            }
        }

        void ButtonwCountDown_Click(object sender, EventArgs e)
        {
            StartTimer();
        }

        public ButtonCountDown(Context context)
            : base(context)
        {
            this.Click += ButtonwCountDown_Click;
        }























        public ButtonCountDown(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            InitTimer();
            this.Click += ButtonwCountDown_Click;
        }

        protected ButtonCountDown(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            InitTimer();
            this.Click += ButtonwCountDown_Click;
        }

        public ButtonCountDown(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            InitTimer();
            this.Click += ButtonwCountDown_Click;
        }
    }
}