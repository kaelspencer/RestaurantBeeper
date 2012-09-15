package org.restaurant.beeper;

import java.net.URI;

import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.json.JSONObject;

import android.app.IntentService;
import android.content.Intent;
import android.net.Uri;
import android.os.Binder;
import android.os.IBinder;
import android.util.Log;
import android.widget.Toast;

public class PollingService extends IntentService {
	private PollingBinder pollingBinder = new PollingBinder();
	private Boolean isCancelled = false;
	private static boolean isRunning = false;
	
	public PollingService() {
		super("Polling Service");
	}
	
	public static boolean isRunning() {
		return isRunning;
	}
	
	@Override
	public IBinder onBind(Intent intent) {
		return pollingBinder;
	}

	@Override
	protected void onHandleIntent(Intent intent) {
		isRunning = true;
		
		Uri pollUri = intent.getData();
		if( pollUri != null ) {
			int consecutiveFailures = 0;
			int lastWaitTime = -1;
			
			HttpClient httpClient = new DefaultHttpClient();
			HttpGet getStatusUpdate = new HttpGet(URI.create(pollUri.toString()));
			
			while(!isCancelled()) {
				try {
					HttpResponse statusUpdateResponse = httpClient.execute(getStatusUpdate);
					JSONObject statusUpdateData = new JSONObject(EntityUtils.toString(statusUpdateResponse.getEntity()));
					
					JSONObject updateData = statusUpdateData.getJSONObject("data");
					int waitTime = updateData.getInt("time_to_wait");
					
					if( lastWaitTime != waitTime ) {
						//Intent updateWaitTimeIntent = new Intent();
						Toast.makeText(this, "New Wait Time: " + waitTime, Toast.LENGTH_SHORT).show();
						
						if( waitTime == 0 ) {
							cancelPolling();
						}
					}
					lastWaitTime = waitTime;
					
					Thread.sleep(5000);
				} catch (Exception e) {
					Log.e("PollingService", String.format("%1$s", e.getMessage()));
					
					++consecutiveFailures;
					if( consecutiveFailures >= 5 ) {
						//TODO: Notify user
					}
				}
			}
		}
		
		isRunning = false;
	}
	
	public void cancelPolling() {
		synchronized(isCancelled) {
			isCancelled = true;
		}
	}
	
	public boolean isCancelled() {
		synchronized(isCancelled) {
			return isCancelled;
		}
	}
	
	private class PollingBinder extends Binder {
		public PollingService getService() {
			return PollingService.this;
		}
	}
}
