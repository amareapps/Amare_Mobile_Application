package crc644a18109494bb9679;


public class XFGlossSwitchRenderer
	extends crc643f46942d9dd1fff9.SwitchRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", XFGlossSwitchRenderer.class, __md_methods);
	}


	public XFGlossSwitchRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == XFGlossSwitchRenderer.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public XFGlossSwitchRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == XFGlossSwitchRenderer.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public XFGlossSwitchRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == XFGlossSwitchRenderer.class)
			mono.android.TypeManager.Activate ("XFGloss.Droid.Renderers.XFGlossSwitchRenderer, XFGloss", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}

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
