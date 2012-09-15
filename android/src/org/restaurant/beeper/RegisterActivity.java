package org.restaurant.beeper;

import java.net.URI;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.json.JSONObject;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class RegisterActivity extends Activity {
	private static final int SCANNER_REQUEST = 1000;
	
	private Button makeReservation = null;
	private TextView reservationInfo = null;
	private Button confirmReservation = null;
	
	private ReservationTask reservationTask = new ReservationTask();
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.register);
		
		makeReservation = (Button)findViewById(R.id.make_reservation);
		reservationInfo = (TextView)findViewById(R.id.reservation_info);
		confirmReservation = (Button)findViewById(R.id.confirm_reservation);
		
		makeReservation.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent scannerIntent = new Intent("com.google.zxing.client.android.SCAN");
				startActivityForResult(scannerIntent, SCANNER_REQUEST);
			}
		});
		
		confirmReservation.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
			}
		});
	}
	
	@Override
	public void onResume() {
		super.onResume();
		
		if( !PollingService.isRunning() && !hasDependencies()) {
			Intent dependencyResolverIntent = new Intent(this, DependencyResolverActivity.class);
			startActivity(dependencyResolverIntent);
		}
	}
	
	private boolean hasDependencies() {
		Intent scannerIntent = new Intent("com.google.zxing.client.android.SCAN");
		return scannerIntent.resolveActivity(getPackageManager()) != null;
	}
	
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		if( requestCode == SCANNER_REQUEST && resultCode == Activity.RESULT_OK) {
			String url = (data == null ? null : data.getStringExtra("SCAN_RESULT"));
			Log.d("RegisterActivity", String.format("Url: %1$s", url));
			
			if( url == null ) {
				Toast.makeText(this, "Cannot make reservation", Toast.LENGTH_LONG).show();
			} else {
				reservationTask.cancel(true);
				reservationTask = new ReservationTask();
				reservationTask.execute(Uri.parse(url));
			}
		}
		
		super.onActivityResult(requestCode, resultCode, data);
	}
	
	private class ReservationTask extends AsyncTask<Uri, Integer, Uri> {
		@Override
		protected Uri doInBackground(Uri... uris) {
			if( uris == null || uris.length != 1 ) {
				return null;
			}
			
			HttpClient httpClient = new DefaultHttpClient();
			HttpGet getReservation = new HttpGet(URI.create(uris[0].toString()));
			try {
				HttpResponse reservationResponse = httpClient.execute(getReservation);
				JSONObject reservationData = new JSONObject(EntityUtils.toString(reservationResponse.getEntity()));
				String pollUrlString =  reservationData.getString("poll_url");
				if( pollUrlString != null ) {
					return Uri.parse(pollUrlString);
				}
			} catch (Exception e) {
				Log.e("ReservationTask", e.getMessage());
			}
			
			return null;
		}
		
		@Override
		protected void onPostExecute(Uri pollUri) {
			if( pollUri == null ) {
				Toast.makeText(RegisterActivity.this, "Failed to make reservation", Toast.LENGTH_LONG).show();
			} else {
				reservationInfo.setVisibility(View.VISIBLE);
				//TODO: Get reservation information
				reservationInfo.setText("Make a reservation for:");
				confirmReservation.setVisibility(View.VISIBLE);
				
				//TODO: Undo shortcut
				Intent statusUpdate = new Intent(RegisterActivity.this, StatusUpdateActivity.class);
				statusUpdate.setData(pollUri);
				startActivity(statusUpdate);
			}
		}
	}
}
