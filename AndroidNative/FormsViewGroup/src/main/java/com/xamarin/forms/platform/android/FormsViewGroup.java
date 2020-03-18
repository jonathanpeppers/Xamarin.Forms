package com.xamarin.forms.platform.android;

import android.content.Context;
import android.util.AttributeSet;
import android.view.*;

public class FormsViewGroup extends ViewGroup {

	public FormsViewGroup(Context context) {
		super(context);
	}

	public FormsViewGroup(Context context, AttributeSet attrs) {
		super(context, attrs);
	}

	public FormsViewGroup(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
	}

	public void measureAndLayout (int widthMeasureSpec, int heightMeasureSpec, int l, int t, int r, int b)
	{
		measure (widthMeasureSpec, heightMeasureSpec);
		layout (l, t, r, b);
	}
	
	@Override
	protected void onLayout(boolean changed, int l, int t, int r, int b) {
	}

	boolean inputTransparent;
	
	protected void setInputTransparent (boolean value)
	{
		inputTransparent = value;
	}
	
	protected boolean getInputTransparent ()
	{
		return inputTransparent;
	}
	
	@Override
	public boolean onInterceptTouchEvent (MotionEvent ev)
	{
		if (inputTransparent)
			return false;
		
		return super.onInterceptTouchEvent(ev);
	}
	
	@Override
	public boolean onTouchEvent (MotionEvent ev)
	{
		if (inputTransparent)
			return false;
		
		return super.onTouchEvent(ev);
	}
	
	public void sendBatchUpdate (BatchUpdateRequest request) {
	    inputTransparent = request.inputTransparent;
		setPivotX (request.pivotX);
		setPivotY (request.pivotY);
		
		if (getVisibility () != request.visibility)
			setVisibility (request.visibility);
		
		if (isEnabled () != request.enabled)
			setEnabled (request.enabled);
		
		setAlpha (request.opacity);
		setRotation (request.rotation);
		setRotationX (request.rotationX);
		setRotationY (request.rotationY);
		setScaleX (request.scaleX);
		setScaleY (request.scaleY);
		setTranslationX (request.translationX);
		setTranslationY (request.translationY);
	}

	public static void sendViewBatchUpdate (BatchUpdateRequest request) {
	    View view = request.view;
		view.setPivotX (request.pivotX);
		view.setPivotY (request.pivotY);

		if (view.getVisibility () != request.visibility)
			view.setVisibility (request.visibility);

		if (view.isEnabled () != request.enabled)
			view.setEnabled (request.enabled);

		view.setAlpha (request.opacity);
		view.setRotation (request.rotation);
		view.setRotationX (request.rotationX);
		view.setRotationY (request.rotationY);
		view.setScaleX (request.scaleX);
		view.setScaleY (request.scaleY);
		view.setTranslationX (request.translationX);
		view.setTranslationY (request.translationY);
	}
}
