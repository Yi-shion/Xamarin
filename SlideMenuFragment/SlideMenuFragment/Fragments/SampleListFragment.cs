using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SlideMenuFragment
{
    public class SampleListFragment : Android.Support.V4.App.ListFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        private ImageView lv_left;
        private ImageView iv_right;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View mView = inflater.Inflate(Resource.Layout.list, null);
            lv_left = mView.FindViewById<ImageView>(Resource.Id.iv_left);
            iv_right = mView.FindViewById<ImageView>(Resource.Id.iv_right);

            return mView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            IDictionary<String, Object> item1 = new Dictionary<String, Object>();
            item1.Add("list_title", GetString(Resource.String.title1));
            item1.Add("list_image", Resource.Drawable.p1);
            item1.Add("list_contect", GetString(Resource.String.test));

            IDictionary<String, Object> item2 = new Dictionary<String, Object>();
            item2.Add("list_title", GetString(Resource.String.title1));
            item2.Add("list_image", Resource.Drawable.p2);
            item2.Add("list_contect", GetString(Resource.String.test));

            IDictionary<String, Object> item3 = new Dictionary<String, Object>();
            item3.Add("list_title", GetString(Resource.String.title1));
            item3.Add("list_image", Resource.Drawable.p3);
            item3.Add("list_contect", GetString(Resource.String.test));

            IDictionary<String, Object> item4 = new Dictionary<String, Object>();
            item4.Add("list_title", GetString(Resource.String.title1));
            item4.Add("list_image", Resource.Drawable.p4);
            item4.Add("list_contect", GetString(Resource.String.test));

            IDictionary<String, Object> item5 = new Dictionary<String, Object>();
            item5.Add("list_title", GetString(Resource.String.title1));
            item5.Add("list_image", Resource.Drawable.p5);
            item5.Add("list_contect", GetString(Resource.String.test));

            IDictionary<String, Object> item6 = new Dictionary<String, Object>();
            item6.Add("list_title", GetString(Resource.String.title1));
            item6.Add("list_image", Resource.Drawable.p6);
            item6.Add("list_contect", GetString(Resource.String.test));

            IDictionary<String, Object> item7 = new Dictionary<String, Object>();
            item7.Add("list_title", GetString(Resource.String.title1));
            item7.Add("list_image", Resource.Drawable.p7);
            item7.Add("list_contect", GetString(Resource.String.test));

            IList<IDictionary<String, Object>> data = new List<IDictionary<string, object>>();
            data.Add(item1);
            data.Add(item2);
            data.Add(item3);
            data.Add(item4);
            data.Add(item5);
            data.Add(item6);
            data.Add(item7);

            var from = new String[] { "list_title", "list_image", "list_contect" };
            var to = new int[] { Resource.Id.list_title, Resource.Id.list_image, Resource.Id.list_contect };
            var adapter = new SimpleAdapter(Activity, data, Resource.Layout.list_item, from, to);
            //setListAdapter(adapter);

            lv_left.Click += (sender, e) =>
            {
                (this.Activity as MainActivity).ShowLeft();
            }; 
            iv_right.Click += (sender, e) =>
            {
                (this.Activity as MainActivity).ShowRight();
            };
        }
    }
}