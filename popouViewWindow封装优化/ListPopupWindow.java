package com.example.mp3service;

import java.util.List;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.drawable.BitmapDrawable;
import android.support.v4.app.FragmentActivity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.ListView;
import android.widget.PopupWindow;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;
/**
 * 用于显示含listview的popupwindows
 * @author liyu
 * 
 */
public class ListPopupWindow {
	private ListView popuplistview;   
	private PopupWindow popupListsign;     
	private popuplva adapter;
	private Activity activity;       //也可以写成context
	private List<String> popupStr;   //数据源
	private Isettext msettext;       //接口
	public ListPopupWindow(Activity activity,List<String> popupStr,Isettext msettext){
		this.activity=activity;
		this.popupStr=popupStr;
		this.msettext=msettext;
	}

	public ListPopupWindow(FragmentActivity activity2, List<String> lists,
			Isettext isettext) {
		this.activity=activity2;
		this.popupStr=lists;
		this.msettext=isettext;
	}

	public  PopupWindow initPopupListSign() {
		popuplistview=new ListView(activity);
		adapter=new popuplva(); 
		popuplistview.setAdapter(adapter);
		popuplistview.setOnItemClickListener(new OnItemClickListener() {
			@Override
			public void onItemClick(AdapterView<?> parent, View view,
					int position, long id) {
				     if(msettext!=null){
				    	 msettext.settext(popupStr.get(position),position);
				     }
				popupListsign.dismiss();
			}
		});
		popupListsign=new PopupWindow(popuplistview, 150, 134);  //第一个值表示view ，第二个值是宽，第三个值是高
		popupListsign.setTouchable(true);
		popupListsign.setOutsideTouchable(true);
		popupListsign.setBackgroundDrawable(new BitmapDrawable(activity.getResources(), (Bitmap) null));
		popupListsign.setFocusable(true);
		return popupListsign;
	}
	public static interface Isettext{
		public void settext(String str,int positon);
	}
	
	private class popuplva extends BaseAdapter{

		@Override
		public int getCount() {
			
			return popupStr.size();
		}

		@Override
		public Object getItem(int position) {
			// TODO Auto-generated method stub
			return popupStr.get(position);
		}

		@Override
		public long getItemId(int position) {
			// TODO Auto-generated method stub
			return position;
		}

		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			if(convertView==null){
			convertView=new TextView(activity);
			}
			((TextView) convertView).setText(popupStr.get(position));
			return convertView;
		}
	}
	
	public void notifyDataSetChange(List<String> list){
		popupStr.clear();
		popupStr.addAll(list);
		adapter.notifyDataSetChanged();
	}
}

