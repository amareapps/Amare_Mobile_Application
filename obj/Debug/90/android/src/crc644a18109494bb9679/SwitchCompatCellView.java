package crc644a18109494bb9679;


public class SwitchCompatCellView
	extends crc643f46942d9dd1fff9.BaseCellView
	implements
		mono.android.IGCUserPeer,
		android.widget.CompoundButton.OnCheckedChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCheckedChanged:(Landroid/widget/CompoundButton;Z)V:GetOnCheckedChanged_Landroid_widget_CompoundButton_ZHandler:Android.Widget.CompoundButton/IOnCheckedChangeListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("XFGloss.Droid.Renderers.SwitchCompatCellView, XFGloss", SwitchCompatCellView.class, __md_methods);
	}


	public SwitchCompatCellView (android.content.Context p0)
	{
		super (p0);
		if (getClass () == SwitchCompatCellView.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.SwitchCompatCellView, XFGloss", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public SwitchCompatCellView (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == SwitchCompatCellView.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.SwitchCompatCellView, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public SwitchCompatCellView (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == SwitchCompatCellView.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.SwitchCompatCellView, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public SwitchCompatCellView (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == SwitchCompatCellView.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.SwitchCompatCellView, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
	}


	public void onCheckedChanged (android.widget.CompoundButton p0, boolean p1)
	{
		n_onCheckedChanged (p0, p1);
	}

	private native void n_onCheckedChanged (android.widget.CompoundButton p0, boolean p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
