using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace VerticalScrollTextViewDemo
{
    [Activity(Label = "VerticalScrollTextViewDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var txtMsg = FindViewById<VerticalScrollTextView>(Resource.Id.txtMsg);
            var lst = new List<Sentence>();
            for (var i = 0; i < 30; i++)
            {
                if (i % 2 == 0)
                {
                    var sen = new Sentence(i, i + "、金球奖三甲揭晓 C罗梅西哈维入围 ");
                    lst.Add(sen);
                }
                else
                {
                    var sen = new Sentence(i, i + "、公牛欲用三大主力换魔兽？？？？");
                    lst.Add(sen);
                }
            }
            //给View传递数据
            txtMsg.SetList(lst);
            //更新View
            txtMsg.UpdateUi();
        }
    }
}

