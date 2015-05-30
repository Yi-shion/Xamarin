package com.example.gesturelock;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.widget.Button;
import android.widget.Toast;

import com.example.gesturelock.GestureLockView.OnGestureFinishListener;

public class MainActivity extends Activity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		requestWindowFeature(Window.FEATURE_NO_TITLE);
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		GestureLockView gv = (GestureLockView) findViewById(R.id.gv);
		String result=getSharedPreferences("liyus", Context.MODE_PRIVATE).getString("result", "");
		Button button = (Button) findViewById(R.id.button);
		button.setText("÷ÿ÷√√‹¬Î");
		gv.setKey(result);
		if(TextUtils.isEmpty(result)){
			Toast.makeText(MainActivity.this, "«Î ‰»Î–¬√‹¬Î£°£°", Toast.LENGTH_SHORT).show();
		}
//		gv.setKey("0124678"); // Z ◊÷–Õ
		gv.setOnGestureFinishListener(new OnGestureFinishListener() {
			@Override
			public void OnGestureFinish(boolean success) {
				Toast.makeText(MainActivity.this, String.valueOf(success), Toast.LENGTH_SHORT).show();
			}
		});
		
		button.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View v) {
				SharedPreferences sharedPreferences = getSharedPreferences("liyus", Context.MODE_PRIVATE);
				sharedPreferences.edit().putString("result", "").commit();
				Toast.makeText(MainActivity.this, "«Î ‰»Î–¬√‹¬Î£°£°", Toast.LENGTH_SHORT).show();
			}
		});
	}
}
