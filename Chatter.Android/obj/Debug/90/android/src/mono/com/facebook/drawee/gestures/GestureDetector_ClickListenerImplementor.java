package mono.com.facebook.drawee.gestures;


public class GestureDetector_ClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.drawee.gestures.GestureDetector.ClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:()Z:GetOnClickHandler:Com.Facebook.Drawee.Gestures.GestureDetector/IClickListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Facebook.Drawee.Gestures.GestureDetector+IClickListenerImplementor, Naxam.FrescoDrawee.Droid", GestureDetector_ClickListenerImplementor.class, __md_methods);
	}


	public GestureDetector_ClickListenerImplementor ()
	{
		super ();
		if (getClass () == GestureDetector_ClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Facebook.Drawee.Gestures.GestureDetector+IClickListenerImplementor, Naxam.FrescoDrawee.Droid", "", this, new java.lang.Object[] {  });
	}


	public boolean onClick ()
	{
		return n_onClick ();
	}

	private native boolean n_onClick ();

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
