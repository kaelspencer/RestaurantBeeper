package org.restaurant.beeper;

import java.util.ArrayList;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class DependencyResolverActivity extends Activity {
	private TextView dependencyStatement = null;
	private Button resolveDependency = null;

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.dependency_resolver);
		
		dependencyStatement = (TextView)findViewById(R.id.dependency_statement);
		resolveDependency = (Button)findViewById(R.id.resolve_dependency);
		
		resolveDependency.setOnClickListener(new OnClickListener() {
			@Override
			public void onClick(View v) {
				Intent zxing = new Intent(Intent.ACTION_VIEW, Uri.parse("market://details?id=com.google.zxing.client.android"));
				startActivity(zxing);
			}
		});
	}
}
