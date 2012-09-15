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

public class StatusUpdateActivity extends Activity {
	public static final String RESERVATION_URL = "RESERVATION_URL";
	
	private TextView estimatedWait = null;
	private Button delayReservation = null;
	private Button cancelReservation = null;
	private Button takeReservation = null;
	
	private Uri reservationUrl = null;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.status_update);
		
		reservationUrl = getIntent().getData();
		if( reservationUrl == null ) {
			Toast.makeText(this, "Reservation Url not received!", Toast.LENGTH_LONG).show();
			finish();
		}
		
		estimatedWait = (TextView)findViewById(R.id.estimated_wait);
		delayReservation = (Button)findViewById(R.id.delay_reservation);
		cancelReservation = (Button)findViewById(R.id.cancel_reservation);
		takeReservation = (Button)findViewById(R.id.take_reservation);
		
		//TODO: Start Update Service
		
		delayReservation.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				//TODO: Confirm with Dialog
				//TODO: Delay on Server
				//TODO: Update Status 
			}
		});
		
		cancelReservation.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				//TODO: Confirm with Dialog
				//TODO: Cancel on Server
				//TODO: Perform Cleanup and Exit application
			}
		});
		
		takeReservation.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				//TODO: Perform Cleanup and Exit application
			}
		});
		
		Intent pollingIntent = new Intent(this, PollingService.class);
		startService(pollingIntent);
	}
	
	@Override
	public void onBackPressed() {
		Intent pollingIntent = new Intent(this, PollingService.class);
		stopService(pollingIntent);
		
		super.onBackPressed();
	}
}
