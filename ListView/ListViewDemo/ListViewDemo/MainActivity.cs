using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace ListViewDemo
{
    [Activity(Label = "ListViewDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, OnPullDownListener
    {
        private const int WHAT_DID_LOAD_DATA = 0;
        private const int WHAT_DID_REFRESH = 1;
        private const int WHAT_DID_MORE = 2;

        private ListView mListView;
        private TestAdapter mAdapter;
        private PullDownView mPullDownView;
        private List<String> mStrings = new List<String>();
        private String[] mStringArray = {
            "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
            "Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale",
            "Aisy Cendre", "Allgauer Emmentaler", "Alverca", "Ambert", "American Cheese"
        };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.pulldown);
            /*
		     * 1.使用PullDownView
		     * 2.设置OnPullDownListener
		     * 3.从mPullDownView里面获取ListView
		     */
            mPullDownView = FindViewById<PullDownView>(Resource.Id.pull_down_view);
            mPullDownView.setOnPullDownListener(this);
            mListView = mPullDownView.getListView();

            mAdapter = new TestAdapter { Context = this, Items = this.mStrings };
            mListView.Adapter = mAdapter;

            mPullDownView.enableAutoFetchMore(true, 1);

            LoadData();
        }

        private void LoadData()
        {
            foreach (var body in mStringArray)
            {
                mStrings.Add(body);
            }
            HandleData(WHAT_DID_LOAD_DATA);
        }


        public void onRefresh()
        {
            HandleData(WHAT_DID_REFRESH);
        }

        public void onMore()
        {
            HandleData(WHAT_DID_MORE);
        }

        public void HandleData(int value)
        {
            switch (value)
            {
                case WHAT_DID_LOAD_DATA:
                    {
                        if (this.mStrings != null && this.mStrings.Count > 0)
                        {
                            mAdapter.NotifyDataSetChanged();
                        }
                        // 诉它数据加载完毕;
                        mPullDownView.notifyDidLoad();
                        break;
                    }
                case WHAT_DID_REFRESH:
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            this.mStrings.Insert(0, i.ToString());
                        }
                        mAdapter.NotifyDataSetChanged();
                        // 告诉它更新完毕
                        mPullDownView.notifyDidRefresh();
                        break;
                    }

                case WHAT_DID_MORE:
                    {
                        for (var i = 0; i < 10; i++)
                        {
                            this.mStrings.Add(i.ToString());
                        }
                        mAdapter.NotifyDataSetChanged();
                        // 告诉它获取更多完毕
                        mPullDownView.notifyDidMore();
                        break;
                    }
            }
        }

    }

    public class TestAdapter : BaseAdapter<string>
    {
        public List<string> Items { get; set; }
        public Activity Context { get; set; }

        public override string this[int position]
        {
            get { return Items[position]; }
        }

        public override int Count
        {
            get { return Items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)//当前没有可以可快取用的VIEW时
            {
                convertView = Context.LayoutInflater.Inflate(Resource.Layout.pulldown_item, null);//实例化一个view出来
            }
            var txt = convertView.FindViewById<TextView>(Resource.Id.text1);//由view中找到按钮
            txt.Text = Items[position];
            return convertView;

        }
    }
}

