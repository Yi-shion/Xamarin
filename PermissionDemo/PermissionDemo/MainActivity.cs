using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace PermissionDemo
{
    [Activity(Label = "PermissionDemo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected int requestImageCapture = 1001;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            var button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate
            {
                PackageManager pm = this.PackageManager;
                bool permission = (Permission.Granted == pm.CheckPermission("android.permission.CAMERA", this.PackageName));
                if (permission)
                {
                    Toast.MakeText(this, "有这个权限", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(this, "木有这个权限", ToastLength.Long).Show();
                }

                string dirPath = System.IO.Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath, "GreatGateApp");
                if (!System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);
                var _tmepCameraPath = System.IO.Path.Combine(dirPath, DateTime.UtcNow.ToString("yyyyMMddHHmmssffff") + ".png");
                Intent i = new Intent(MediaStore.ActionImageCapture);
                i.PutExtra(Android.Provider.MediaStore.ExtraOutput, Android.Net.Uri.FromFile(new Java.IO.File(_tmepCameraPath)));
                if (i.ResolveActivity(this.PackageManager) != null)
                {
                    TaskCompletionSource<string> _onTakePictureBack = new TaskCompletionSource<string>();
                    this.StartActivityForResult(i, requestImageCapture);
                }
            };
        }
    }
}

