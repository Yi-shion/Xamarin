using System;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Com.Baidu.Location;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Model;


namespace App
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class BaiduMapSample : Activity
    {
        private MapView bmapView;
        private BaiduMap mBaiduMap;



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SDKInitializer.Initialize(ApplicationContext);
            SetContentView(Resource.Layout.layout_map);

            InitialView();
            InitialLocation();
        }

        private void InitialView()
        {
            this.bmapView = FindViewById<MapView>(Resource.Id.bmapView);

        }

        private void InitialLocation()
        {
            mBaiduMap = bmapView.Map;
            mBaiduMap.MyLocationEnabled = true;
            var mLocOption = new LocationClientOption
            {
                OpenGps = true,
                CoorType = "bd0911",
                ScanSpan = 5000,
                AddrType = "all"
            };

            var mLocClient = new LocationClient(this)
            {
                LocOption = mLocOption
            };
            mLocClient.Start();
            mLocClient.RequestLocation();
            mLocClient.RegisterLocationListener(new MapLocationListenner { Context = this });
        }

        protected override void OnPause()
        {
            base.OnPause();
            bmapView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            bmapView.OnResume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            bmapView.OnDestroy();
        }
        private class MapLocationListenner : Java.Lang.Object, IBDLocationListener
        {
            public Activity Context { private get; set; }

            public void OnReceiveLocation(Com.Baidu.Location.BDLocation location)
            {
                var context = (BaiduMapSample)Context;
                var locData = new MyLocationData.Builder()
                  .Accuracy(location.Radius).Latitude(location.Latitude)
                  .Longitude(location.Longitude).Build();
                context.mBaiduMap.SetMyLocationData(locData);
                var latlng = new LatLng(location.Latitude, location.Longitude);
                var update = MapStatusUpdateFactory.NewLatLng(latlng);
                context.mBaiduMap.AnimateMapStatus(update);
            }
        }
    }
}

