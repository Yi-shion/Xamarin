using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Handmark.Pulltorefresh.Library;
using Java.Lang;
using Java.Nio.Channels;
using Exception = System.Exception;
using Object = Java.Lang.Object;
using Com.Handmark.Pulltorefresh.Library.Extras;

namespace App
{
    [Activity(Label = "@string/ApplicationName",  Icon = "@drawable/icon")]
    public class Pull2RefreshSample : ListActivity, PullToRefreshBase.IOnRefreshListener,
        PullToRefreshBase.IOnLastItemVisibleListener
    {
        private List<string> mListItems;
        private PullToRefreshListView mPullRefreshListView;
        private ArrayAdapter<string> mAdapter;

        internal const int MENU_MANUAL_REFRESH = 0;
        internal const int MENU_DISABLE_SCROLL = 1;
        internal const int MENU_SET_MODE = 2;
        internal const int MENU_DEMO = 3;

        private string[] mStrings =
        {
            "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
            "Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
            "Allgauer Emmentaler", "Abbaye de Belloc", "Abbaye du Mont des Cats", "Abertam", "Abondance", "Ackawi",
            "Acorn", "Adelost", "Affidelice au Chablis", "Afuega'l Pitu", "Airag", "Airedale", "Aisy Cendre",
            "Allgauer Emmentaler"
        };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.layout_listview);

            mPullRefreshListView = FindViewById<PullToRefreshListView>(Resource.Id.pull_refresh_list);
            var actualListView = (ListView)mPullRefreshListView.RefreshableView;
            mListItems = new List<string>(mStrings);
            mAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mListItems);

            var soundListener = new SoundPullEventListener(this);
            soundListener.AddSoundEvent(PullToRefreshBase.PullToRefreshState.PullToRefresh, Resource.Raw.pull_event);
            soundListener.AddSoundEvent(PullToRefreshBase.PullToRefreshState.Reset, Resource.Raw.reset_sound);
            soundListener.AddSoundEvent(PullToRefreshBase.PullToRefreshState.Refreshing, Resource.Raw.refreshing_sound);

            mPullRefreshListView.SetOnPullEventListener(soundListener);
            mPullRefreshListView.SetOnRefreshListener(this);
            mPullRefreshListView.SetOnLastItemVisibleListener(this);

            actualListView.Adapter = mAdapter;


            InitialContextMenu();
        }

        private class GetDataTask : AsyncTask<Java.Lang.Void, Java.Lang.Void, string[]>
        {
            private readonly Pull2RefreshSample _mainActivity;

            public GetDataTask(Pull2RefreshSample mainActivity)
            {
                _mainActivity = mainActivity;
            }

            protected override string[] RunInBackground(params Java.Lang.Void[] @params)
            {
                try
                {
                    Thread.Sleep(2500);
                }
                catch (InterruptedException)
                {
                }

                return _mainActivity.mStrings;
            }

            protected override void OnPostExecute(Object result)
            {
                _mainActivity.mAdapter.Insert("added after refresh:" + DateTime.Now.ToString("t"), 0);
                _mainActivity.mAdapter.NotifyDataSetChanged();

                _mainActivity.mPullRefreshListView.OnRefreshComplete();

                base.OnPostExecute(result);
            }
        }

        public void OnRefresh(PullToRefreshBase p0)
        {
            p0.GetLoadingLayoutProxy(true, true).SetLastUpdatedLabel(string.Format("上次更新:{0:t}", DateTime.Now));
            new GetDataTask(this).Execute();
        }

        public void OnLastItemVisible()
        {
            Toast.MakeText(this, "End of List", ToastLength.Short).Show();
        }
        private void InitialContextMenu()
        {
            var actualListView = (View)mPullRefreshListView.RefreshableView;
            RegisterForContextMenu(actualListView);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {

            menu.Add(0, MENU_MANUAL_REFRESH, 0, "Manual Refresh");
            menu.Add(0, MENU_DISABLE_SCROLL, 1,
                mPullRefreshListView.DisableScrollingWhileRefreshing ? "Disable Scrolling while Refreshing"
                        : "Enable Scrolling while Refreshing");
            menu.Add(0, MENU_SET_MODE, 0, mPullRefreshListView.Mode == PullToRefreshBase.PullToRefreshMode.Both ? "Change to MODE_PULL_DOWN"
                   : "Change to MODE_PULL_BOTH");
            menu.Add(0, MENU_DEMO, 0, "Demo");
            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            var info = (AdapterView.AdapterContextMenuInfo)menuInfo;
            menu.SetHeaderTitle("Item: " + this.ListView.GetItemAtPosition(info.Position));
            menu.Add("Item 1");
            menu.Add("Item 2");
            menu.Add("Item 3");
            menu.Add("Item 4");
            base.OnCreateContextMenu(menu, v, menuInfo);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var disableItem = menu.FindItem(MENU_DISABLE_SCROLL);
            disableItem
                    .SetTitle(mPullRefreshListView.DisableScrollingWhileRefreshing ? "Disable Scrolling while Refreshing"
                            : "Enable Scrolling while Refreshing");

            var setModeItem = menu.FindItem(MENU_SET_MODE);
            setModeItem.SetTitle(mPullRefreshListView.Mode == PullToRefreshBase.PullToRefreshMode.Both ? "Change to MODE_FROM_START"
                    : "Change to MODE_PULL_BOTH");


            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case MENU_MANUAL_REFRESH:
                    new GetDataTask(this).Execute();
                    mPullRefreshListView.SetRefreshing();
                    break;
                case MENU_DISABLE_SCROLL:
                    mPullRefreshListView.ScrollingWhileRefreshingEnabled = !mPullRefreshListView
                        .DisableScrollingWhileRefreshing;
                    break;
                case MENU_SET_MODE:
                    mPullRefreshListView.Mode = mPullRefreshListView.Mode == PullToRefreshBase.PullToRefreshMode.Both
                        ? PullToRefreshBase.PullToRefreshMode.PullFromStart
                        : PullToRefreshBase.PullToRefreshMode.Both;
                    break;
                case MENU_DEMO:
                    mPullRefreshListView.Demo();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }


}