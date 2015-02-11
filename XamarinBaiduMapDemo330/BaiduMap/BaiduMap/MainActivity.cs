using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Widget;
using Com.Baidu.Lbsapi;
using Com.Baidu.Location;
using Com.Baidu.Mapapi;
using Com.Baidu.Mapapi.Map;
using Com.Baidu.Mapapi.Model;
using Com.Baidu.Navisdk;
using Com.Baidu.Nplatform.Comapi.Basestruct;
using Com.Baidu.Nplatform.Comapi.Map;

namespace BaiduMap
{
    [Activity(Label = "BaiduMap", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        // 定位相关
        LocationClient mLocClient;
        public MyLocationListenner myListener = new MyLocationListenner();
        private Com.Baidu.Mapapi.Map.MyLocationConfiguration.LocationMode mCurrentMode;
        BitmapDescriptor mCurrentMarker;

        public Com.Baidu.Mapapi.Map.BaiduMap mBaiduMap { get; set; }
        public MapView mMapView { get; set; }

        public bool isFirstLoc = true;// 是否首次定位

        // UI相关
        RadioGroup.IOnCheckedChangeListener radioButtonListener;
        Button requestLocButton;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            //在使用SDK各组件之前初始化context信息，传入ApplicationContext  
            //注意该方法要再setContentView方法之前实现  
            SDKInitializer.Initialize(ApplicationContext);
            SetContentView(Resource.Layout.Main);
            //获取地图控件引用  
            //mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            //mBaiduMap = mMapView.Map;

            // 地图初始化
            mMapView = FindViewById<MapView>(Resource.Id.bmapView);
            mBaiduMap = mMapView.Map;


            mCurrentMode = MyLocationConfiguration.LocationMode.Normal;
            //requestLocButton.Text = "普通";
            //requestLocButton.Click += delegate
            //{
            if (mCurrentMode.Equals(MyLocationConfiguration.LocationMode.Normal))
            {
                //requestLocButton.Text = "跟随";
                mCurrentMode = MyLocationConfiguration.LocationMode.Following;
                mBaiduMap.SetMyLocationConfigeration(new MyLocationConfiguration(mCurrentMode, true, mCurrentMarker));
            }
            //    else if (mCurrentMode.Equals(MyLocationConfiguration.LocationMode.Compass))
            //    {
            //        requestLocButton.Text = "普通";
            //        mCurrentMode = MyLocationConfiguration.LocationMode.Normal;
            //        mBaiduMap
            //                .SetMyLocationConfigeration(new MyLocationConfiguration(
            //                        mCurrentMode, true, mCurrentMarker));
            //    }
            //    else if (mCurrentMode.Equals(MyLocationConfiguration.LocationMode.Following))
            //    {
            //        requestLocButton.Text = "罗盘";
            //        mCurrentMode = MyLocationConfiguration.LocationMode.Compass;
            //        mBaiduMap
            //                .SetMyLocationConfigeration(new MyLocationConfiguration(
            //                        mCurrentMode, true, mCurrentMarker));
            //    }
            //};

            //RadioGroup group = this.FindViewById<RadioGroup>(Resource.Id.radioGroup);
            // group.CheckedChange += delegate(object sender, RadioGroup.CheckedChangeEventArgs args) { };
            //group.CheckedChange += (sender, args) =>
            //{
            //    int CheckedId = args.CheckedId;

            //    if (CheckedId == Resource.Id.defaulticon)
            //    {
            //        // 传入null则，恢复默认图标
            //        mCurrentMarker = null;
            //        mBaiduMap
            //                .SetMyLocationConfigeration(new MyLocationConfigeration(
            //                        mCurrentMode, true, null));
            //    }
            //    if (CheckedId == Resource.Id.customicon)
            //    {
            //        // 修改为自定义marker
            //        mCurrentMarker = BitmapDescriptorFactory
            //                .FromResource(Resource.Drawable.icon_geo);
            //        mBaiduMap
            //                .SetMyLocationConfigeration(new MyLocationConfigeration(
            //                        mCurrentMode, true, mCurrentMarker));
            //    }
            //};

            // 开启定位图层
            mBaiduMap.MyLocationEnabled = true;
            // 定位初始化
            mLocClient = new LocationClient(this);
            mLocClient.RegisterLocationListener(myListener);
            LocationClientOption option = new LocationClientOption();
            option.OpenGps = true;// 打开gps
            option.CoorType = "bd09ll"; // 设置坐标类型
            option.ScanSpan = 1000;
            mLocClient.LocOption = option;
            mLocClient.Start();

        }

        public class MyLocationListenner : Java.Lang.Object, IBDLocationListener
        {

            public MainActivity _Activity;

            public MyLocationListenner()
            {
            }

            public MyLocationListenner(MainActivity activity)
            {
                this._Activity = activity;
            }

            public void OnReceiveLocation(BDLocation location)
            {
                // map view 销毁后不在处理新接收的位置
                if (location == null || _Activity.mMapView == null)
                    return;
                MyLocationData locData = new MyLocationData.Builder()
                        .Accuracy(location.Radius)
                    // 此处设置开发者获取到的方向信息，顺时针0-360
                        .Direction(100).Latitude(location.Latitude)
                        .Longitude(location.Longitude).Build();
                _Activity.mBaiduMap.SetMyLocationData(locData);
                if (_Activity.isFirstLoc)
                {
                    _Activity.isFirstLoc = false;
                    LatLng ll = new LatLng(location.Latitude, location.Longitude);
                    MapStatusUpdate u = MapStatusUpdateFactory.NewLatLng(ll);
                    _Activity.mBaiduMap.AnimateMapStatus(u);
                }
            }
        }
    }
}

