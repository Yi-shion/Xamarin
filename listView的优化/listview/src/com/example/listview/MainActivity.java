package com.example.listview;

import java.util.ArrayList;
import java.util.List;

import android.os.Bundle;
import android.app.Activity;
import android.content.ContentValues;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;

public class MainActivity extends Activity {
	private ListView mListView;
	private ArrayList<Bean> lists;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		lists = new ArrayList<Bean>();
		for (int i = 0; i < 20; i++) {
			Bean b = new Bean();
			b.isVisibility = false;
			b.name = "text" + i;
			lists.add(b);
		}
		mListView = (ListView) findViewById(R.id.listView1);
		mListView.setAdapter(mAdapter);

	}

	private BaseAdapter mAdapter = new BaseAdapter() {

		@Override
		public View getView(final int arg0, View conentView, ViewGroup arg2) {

			viewHolder vh = null;
			if (conentView == null) {
				System.out.println("�´�����contentview" + arg0);
				conentView = getLayoutInflater().inflate(R.layout.list_item,
						null);
				vh = new viewHolder();
				vh.tv = (TextView) conentView.findViewById(R.id.textView1);
				vh.button = (Button) conentView.findViewById(R.id.button1);
				vh.tv1 = (TextView) conentView.findViewById(R.id.textView2);
				conentView.setTag(vh); // ��vh���浽contentview��ȥ
			} else {
				vh = (viewHolder) conentView.getTag();// �ѻ��浽contentview��ȥ
				System.out.println("���õ�contentview" + arg0);
			}
			vh.tv.setText(lists.get(arg0).name);
			if (lists.get(arg0).isVisibility) {
				vh.tv1.setVisibility(View.VISIBLE); // View.visible�ǿɼ�
			} else {
				vh.tv1.setVisibility(View.GONE); // VIew.gone ���ɼ��Ҳ�ռ�ռ�
			}
			final TextView tv1 = vh.tv1;
			Button button = vh.button; // contentview�Ḳ��ǰ���ֵ
										// ��contentview��Ӧ���������µ�ֵ
			button.setOnClickListener(new OnClickListener() {
				@Override
				public void onClick(View arg4) {
					lists.get(arg0).isVisibility = true;
					mAdapter.notifyDataSetChanged();

				}
			});
			return conentView;
		}

		@Override
		public long getItemId(int arg0) {

			return 0;
		}

		@Override
		public Object getItem(int arg0) {

			return null;
		}

		@Override
		public int getCount() {

			return lists.size();
		}

		class viewHolder {
			TextView tv;
			Button button;
			TextView tv1;
		}
	};
}
