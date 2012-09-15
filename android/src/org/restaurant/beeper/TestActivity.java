package org.restaurant.beeper;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class TestActivity extends Activity {
	private final static int SCANNER_REQUEST = 1000;
	
	private Button launchScanner = null;
	//private Button launchMarket = null;
	private TextView output = null;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.test);
		
		launchScanner = (Button)findViewById(R.id.launchScanner);
		//launchMarket = (Button)findViewById(R.id.launchMarket);
		output = (TextView)findViewById(R.id.output);
		
		launchScanner.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				/*
				Intent scannerIntent = new Intent("com.google.zxing.client.android.SCAN");
				if( scannerIntent.resolveActivity(getPackageManager()) == null ) {
					Toast.makeText(TestActivity.this, "Must install ZXING", Toast.LENGTH_LONG).show();
				} else {
					startActivityForResult(scannerIntent, SCANNER_REQUEST);
				}
				*/
				
				Intent marketIntent = new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=com.google.zxing.client.android"));
				startActivity(marketIntent);
			}
		});
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		String decodedData = null;
		if( data != null ) {
			decodedData = data.getStringExtra("SCAN_RESULT");
		}
		output.setText(String.format("Request Code: %1$s\nResult Code: %2$s\nData: %3$s", requestCode, resultCode, decodedData));
	}
}
