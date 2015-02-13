using System;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Lang;
using Java.Util.Regex;
using Object = Java.Lang.Object;
using Pattern = Android.OS.Pattern;

namespace EditTextFocusDemo
{
    [Activity(Label = "EditTextFocusDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int count = 1;
        private EditText editText;
        private EditText txtLength;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            editText = FindViewById<EditText>(Resource.Id.MyButton);
            txtLength = FindViewById<EditText>(Resource.Id.txtLength);
            var mTextWatcher = new TextWatcher(editText, txtLength);
            //editText.AddTextChangedListener(mTextWatcher);

            editText.AddTextChangedListener(mTextWatcher);
            editText.SetSelection(editText.Length()); // 将光标移动最后一个字符后面  
        }

        private static int MAX_COUNT = 20;
        public class TextWatcher : Java.Lang.Object, ITextWatcher
        {
            private EditText mEditText;
            private EditText mTextView;
            public TextWatcher(EditText editText, EditText editLength)
            {
                this.mEditText = editText;
                this.mTextView = editLength;
            }

            private int editStart;
            private int editEnd;
            public void AfterTextChanged(IEditable s)
            {
                editStart = mEditText.SelectionStart;
                editEnd = mEditText.SelectionEnd;

                // 先去掉监听器，否则会出现栈溢出  
                mEditText.RemoveTextChangedListener(this);

                // 注意这里只能每次都对整个EditText的内容求长度，不能对删除的单个字符求长度  
                // 因为是中英文混合，单个字符而言，calculateLength函数都会返回1  
                while (calculateLength(s) > MAX_COUNT)
                { // 当输入字符个数超过限制的大小时，进行截断操作  
                    s.Delete(editStart - 1, editEnd);
                    editStart--;
                    editEnd--;
                }
                // mEditText.setText(s);将这行代码注释掉就不会出现后面所说的输入法在数字界面自动跳转回主界面的问题了，多谢@ainiyidiandian的提醒  
                mEditText.SetSelection(editStart);

                // 恢复监听器  
                mEditText.AddTextChangedListener(this);

                setLeftCount();
            }

            public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
            {
            }

            public void OnTextChanged(ICharSequence s, int start, int before, int count)
            {
            }

            /** 
             * 计算分享内容的字数，一个汉字=两个英文字母，一个中文标点=两个英文标点 注意：该函数的不适用于对单个字符进行计算，因为单个字符四舍五入后都是1 
             *  
             * @param c 
             * @return 
             */
            private long calculateLength(ICharSequence c)
            {
                double len = 0;
                for (var i = 0; i < c.Length(); i++)
                {
                    var tmp = (int)c.CharAt(i);
                    if (tmp >= 0x4e00 && tmp <= 0x9fbb)//中文
                    {
                        len += 2;
                    }
                    else
                    {
                        len += 1;
                    }
                    //if (tmp > 0 && tmp < 127)
                    //{
                    //    len += 0.5;
                    //}
                    //else
                    //{
                    //    len++;
                    //}
                }
                return (long)System.Math.Round(len);
            }

            /** 
             * 刷新剩余输入字数,最大值新浪微博是140个字，人人网是200个字 
             */
            private void setLeftCount()
            {
                mTextView.Text = (MAX_COUNT - getInputCount()).ToString();
            }

            /** 
             * 获取用户输入的分享内容字数 
             *  
             * @return 
             */
            private long getInputCount()
            {
                return calculateLength(mEditText.EditableText);//.getText().toString()
            }

        }
    }

    //public class TextWatcher : Java.Lang.Object, ITextWatcher
    //{
    //    private EditText mEditText;
    //    private EditText mTextView;
    //    public TextWatcher(EditText editText, EditText editLength)
    //    {
    //        this.mEditText = editText;
    //        this.mTextView = editLength;
    //    }

    //    public void AfterTextChanged(IEditable s)
    //    {
    //        editStart = mEditText.SelectionStart;
    //        editEnd = mEditText.SelectionEnd;
    //        mTextView.Text = "您输入了" + temp.Length() + "个字符";
    //        if (temp.Length() > 10)
    //        {
    //            s.Delete(editStart - 1, editEnd);
    //            var tempSelection = editStart;
    //            mTextView.SetText(s, TextView.BufferType.Editable);
    //            mEditText.SetSelection(tempSelection);
    //        }
    //    }

    //    public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
    //    {
    //        mTextView.SetText(s, TextView.BufferType.Editable);
    //    }

    //    public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
    //    {
    //        temp = s;

    //    }

    //    private ICharSequence temp;
    //    private int editStart;
    //    private int editEnd;
    //}

    //private class NameLengthFilter : Java.Lang.Object, IInputFilter
    //{
    //    int MAX_EN;// 最大英文/数字长度 一个汉字算两个字母  
    //    string regEx = "[\\u4e00-\\u9fa5]"; // unicode编码，判断是否为汉字  

    //    public NameLengthFilter(int mAX_EN)
    //        : base()
    //    {
    //        MAX_EN = mAX_EN;
    //    }

    //    public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
    //    {
    //        int destCount = dest.ToString().Length + GetChineseCount(dest.ToString());
    //        int sourceCount = source.ToString().Length + GetChineseCount(source.ToString());
    //        if (destCount + sourceCount > MAX_EN)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            return source;
    //        }
    //    }

    //    private int GetChineseCount(string str)
    //    {
    //        int count = 0;
    //        var m = new Regex(regEx);
    //        //while (m.Find())
    //        //{
    //        //    for (int i = 0; i <= m.groupCount(); i++)
    //        //    {
    //        //        count = count + 1;
    //        //    }
    //        //}


    //        return count;
    //    }
    //}
}

