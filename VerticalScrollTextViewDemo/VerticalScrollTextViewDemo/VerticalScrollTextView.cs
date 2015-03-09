using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using String = System.String;
using Thread = System.Threading.Thread;

namespace VerticalScrollTextViewDemo
{
    public class VerticalScrollTextView : TextView
    {

        private Paint mPaint;
        private float mX;
        private Paint mPathPaint;
        public int index = 0;
        private List<Sentence> list;
        public float mTouchHistoryY;
        private int mY;
        private float middleY;// y���м�
        private static int DY = 40; // ÿһ�еļ��

        public VerticalScrollTextView(Context context)
            : base(context)
        {
            Init();
        }

        public VerticalScrollTextView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init();
        }

        protected VerticalScrollTextView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Init();
        }
        public VerticalScrollTextView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Init();
        }

        private void Init()
        {
            this.Focusable = true;
            if (list == null)
            {
                list = new List<Sentence>();
                var sen = new Sentence(0, "��ʱû��֪ͨ����");
                list.Add(sen);
            }

            // �Ǹ�������
            mPaint = new Paint
            {
                AntiAlias = true,
                TextSize = 16,
                Color = Color.Black
            };
            mPaint.SetTypeface(Typeface.Serif);

            // �������� ��ǰ���
            mPathPaint = new Paint
            {
                AntiAlias = true,
                Color = Color.Red,
                TextSize = 18
            };
            mPathPaint.SetTypeface(Typeface.SansSerif);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            //canvas.DrawColor(0xEFeffff);
            Paint p = mPaint;
            Paint p2 = mPathPaint;
            p.TextAlign = Android.Graphics.Paint.Align.Center;
            if (index == -1)
                return;
            p2.TextAlign = Android.Graphics.Paint.Align.Center;
            // �Ȼ���ǰ�У�֮���ٻ�����ǰ��ͺ��棬�����ͱ��ֵ�ǰ�����м��λ��
            canvas.DrawText(list[index].Name, mX, middleY, p2);
            float tempY = middleY;
            // ��������֮ǰ�ľ���
            for (int i = index - 1; i >= 0; i--)
            {
                tempY = tempY - DY;
                if (tempY < 0)
                {
                    break;
                }
                canvas.DrawText(list[i].Name, mX, tempY, p);
            }
            tempY = middleY;
            // ��������֮��ľ���
            for (int i = index + 1; i < list.Count; i++)
            {
                // ��������
                tempY = tempY + DY;
                if (tempY > mY)
                {
                    break;
                }
                canvas.DrawText(list[i].Name, mX, tempY, p);
            }
        }
        protected override void OnSizeChanged(int w, int h, int ow, int oh)
        {
            base.OnSizeChanged(w, h, ow, oh);
            mX = w * 0.5f;
            mY = h;
            middleY = h * 0.5f;
        }

        public List<Sentence> GetList()
        {
            return list;
        }

        public void SetList(List<Sentence> list)
        {
            this.list = list;
        }
        public int UpdateIndex(int index)
        {
            if (index == -1)
                return -1;
            this.index = index;
            return index;
        }

        public void UpdateUi()
        {
            new Thread(() =>
            {
                var intervalTime = 1000;//���ʱ�䣬��λ������
                var currentIndex = 0;//��ǰ��ʾ��������
                while (true)
                {
                    var nextIndex = UpdateIndex(currentIndex);
                    intervalTime += nextIndex;
                    Post(Invalidate);
                    if (nextIndex == -1)
                    {
                        return;
                    }
                    Thread.Sleep(intervalTime);
                    currentIndex++;
                    if (currentIndex == this.GetList().Count)
                    {
                        currentIndex = 0;
                    }
                }
            }).Start();
        }
    }

    public class Sentence
    {
        public Sentence(int index, String name)
        {
            this.Name = name;
            this.Index = index;
        }

        public String Name { get; set; }

        public int Index { get; set; }

    }
}