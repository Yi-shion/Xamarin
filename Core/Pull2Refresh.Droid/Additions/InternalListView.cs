using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Com.Handmark.Pulltorefresh.Library
{
    // Metadata.xml XPath class reference: path="/api/package[@name='com.handmark.pulltorefresh.library']/class[@name='PullToRefreshListView.InternalListView']"
    [global::Android.Runtime.Register("com/handmark/pulltorefresh/library/PullToRefreshListView$InternalListView", DoNotGenerateAcw = true)]
    public partial class InternalListView : global::Android.Widget.ListView, global::Com.Handmark.Pulltorefresh.Library.Internal.IEmptyViewMethodAccessor
    {

        internal static IntPtr java_class_handle;
        internal static IntPtr class_ref
        {
            get
            {
                return JNIEnv.FindClass("com/handmark/pulltorefresh/library/PullToRefreshListView$InternalListView", ref java_class_handle);
            }
        }

        protected override IntPtr ThresholdClass
        {
            get { return class_ref; }
        }

        protected override global::System.Type ThresholdType
        {
            get { return typeof(InternalListView); }
        }

        protected InternalListView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

        static IntPtr id_ctor_Lcom_handmark_pulltorefresh_library_PullToRefreshListView_Landroid_content_Context_Landroid_util_AttributeSet_;
        // Metadata.xml XPath constructor reference: path="/api/package[@name='com.handmark.pulltorefresh.library']/class[@name='PullToRefreshListView.InternalListView']/constructor[@name='PullToRefreshListView.InternalListView' and count(parameter)=3 and parameter[1][@type='com.handmark.pulltorefresh.library.PullToRefreshListView'] and parameter[2][@type='android.content.Context'] and parameter[3][@type='android.util.AttributeSet']]"
        [Register(".ctor", "(Lcom/handmark/pulltorefresh/library/PullToRefreshListView;Landroid/content/Context;Landroid/util/AttributeSet;)V", "")]
        public InternalListView(global::Com.Handmark.Pulltorefresh.Library.PullToRefreshListView __self, global::Android.Content.Context p1, global::Android.Util.IAttributeSet p2)
            : base(IntPtr.Zero, JniHandleOwnership.DoNotTransfer)
        {
            if (Handle != IntPtr.Zero)
                return;

            if (GetType() != typeof(InternalListView))
            {
                SetHandle(
                        global::Android.Runtime.JNIEnv.StartCreateInstance(GetType(), "(L" + global::Android.Runtime.JNIEnv.GetJniName(GetType().DeclaringType) + ";Landroid/content/Context;Landroid/util/AttributeSet;)V", new JValue(__self), new JValue(p1), new JValue(p2)),
                        JniHandleOwnership.TransferLocalRef);
                global::Android.Runtime.JNIEnv.FinishCreateInstance(Handle, "(L" + global::Android.Runtime.JNIEnv.GetJniName(GetType().DeclaringType) + ";Landroid/content/Context;Landroid/util/AttributeSet;)V", new JValue(__self), new JValue(p1), new JValue(p2));
                return;
            }

            if (id_ctor_Lcom_handmark_pulltorefresh_library_PullToRefreshListView_Landroid_content_Context_Landroid_util_AttributeSet_ == IntPtr.Zero)
                id_ctor_Lcom_handmark_pulltorefresh_library_PullToRefreshListView_Landroid_content_Context_Landroid_util_AttributeSet_ = JNIEnv.GetMethodID(class_ref, "<init>", "(Lcom/handmark/pulltorefresh/library/PullToRefreshListView;Landroid/content/Context;Landroid/util/AttributeSet;)V");
            SetHandle(
                    global::Android.Runtime.JNIEnv.StartCreateInstance(class_ref, id_ctor_Lcom_handmark_pulltorefresh_library_PullToRefreshListView_Landroid_content_Context_Landroid_util_AttributeSet_, new JValue(__self), new JValue(p1), new JValue(p2)),
                    JniHandleOwnership.TransferLocalRef);
            JNIEnv.FinishCreateInstance(Handle, class_ref, id_ctor_Lcom_handmark_pulltorefresh_library_PullToRefreshListView_Landroid_content_Context_Landroid_util_AttributeSet_, new JValue(__self), new JValue(p1), new JValue(p2));
        }

        static Delegate cb_setEmptyViewInternal_Landroid_view_View_;
#pragma warning disable 0169
        static Delegate GetSetEmptyViewInternal_Landroid_view_View_Handler()
        {
            if (cb_setEmptyViewInternal_Landroid_view_View_ == null)
                cb_setEmptyViewInternal_Landroid_view_View_ = JNINativeWrapper.CreateDelegate((Action<IntPtr, IntPtr, IntPtr>)n_SetEmptyViewInternal_Landroid_view_View_);
            return cb_setEmptyViewInternal_Landroid_view_View_;
        }

        static void n_SetEmptyViewInternal_Landroid_view_View_(IntPtr jnienv, IntPtr native__this, IntPtr native_p0)
        {
            global::Com.Handmark.Pulltorefresh.Library.InternalListView __this = global::Java.Lang.Object.GetObject<global::Com.Handmark.Pulltorefresh.Library.InternalListView>(jnienv, native__this, JniHandleOwnership.DoNotTransfer);
            global::Android.Views.View p0 = global::Java.Lang.Object.GetObject<global::Android.Views.View>(native_p0, JniHandleOwnership.DoNotTransfer);
            __this.SetEmptyViewInternal(p0);
        }
#pragma warning restore 0169

        static IntPtr id_setEmptyViewInternal_Landroid_view_View_;
        // Metadata.xml XPath method reference: path="/api/package[@name='com.handmark.pulltorefresh.library']/class[@name='PullToRefreshListView.InternalListView']/method[@name='setEmptyViewInternal' and count(parameter)=1 and parameter[1][@type='android.view.View']]"
        [Register("setEmptyViewInternal", "(Landroid/view/View;)V", "GetSetEmptyViewInternal_Landroid_view_View_Handler")]
        public virtual void SetEmptyViewInternal(global::Android.Views.View p0)
        {
            if (id_setEmptyViewInternal_Landroid_view_View_ == IntPtr.Zero)
                id_setEmptyViewInternal_Landroid_view_View_ = JNIEnv.GetMethodID(class_ref, "setEmptyViewInternal", "(Landroid/view/View;)V");

            if (GetType() == ThresholdType)
                JNIEnv.CallVoidMethod(Handle, id_setEmptyViewInternal_Landroid_view_View_, new JValue(p0));
            else
                JNIEnv.CallNonvirtualVoidMethod(Handle, ThresholdClass, JNIEnv.GetMethodID(ThresholdClass, "setEmptyViewInternal", "(Landroid/view/View;)V"), new JValue(p0));
        }
        // Metadata.xml XPath method reference: path="/api/package[@name='com.handmark.pulltorefresh.library.internal']/interface[@name='EmptyViewMethodAccessor']/method[@name='setEmptyView' and count(parameter)=1 and parameter[1][@type='android.view.View']]"
        [Register("setEmptyView", "(Landroid/view/View;)V", "GetSetEmptyView_Landroid_view_View_Handler:Com.Handmark.Pulltorefresh.Library.Internal.IEmptyViewMethodAccessorInvoker, Pull2Refresh.Droid")]
        public void SetEmptyView(global::Android.Views.View p0) { }
    }
}
