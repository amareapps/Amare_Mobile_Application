package mono.com.facebook.drawee.controller;


public class ControllerViewportVisibilityListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.drawee.controller.ControllerViewportVisibilityListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDraweeViewportEntry:(Ljava/lang/String;)V:GetOnDraweeViewportEntry_Ljava_lang_String_Handler:Com.Facebook.Drawee.Controller.IControllerViewportVisibilityListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"n_onDraweeViewportExit:(Ljava/lang/String;)V:GetOnDraweeViewportExit_Ljava_lang_String_Handler:Com.Facebook.Drawee.Controller.IControllerViewportVisibilityListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Facebook.Drawee.Controller.IControllerViewportVisibilityListenerImplementor, Naxam.FrescoDrawee.Droid", ControllerViewportVisibilityListenerImplementor.class, __md_methods);
	}


	public ControllerViewportVisibilityListenerImplementor ()
	{
		super ();
		if (getClass () == ControllerViewportVisibilityListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Facebook.Drawee.Controller.IControllerViewportVisibilityListenerImplementor, Naxam.FrescoDrawee.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onDraweeViewportEntry (java.lang.String p0)
	{
		n_onDraweeViewportEntry (p0);
	}

	private native void n_onDraweeViewportEntry (java.lang.String p0);


	public void onDraweeViewportExit (java.lang.String p0)
	{
		n_onDraweeViewportExit (p0);
	}

	private native void n_onDraweeViewportExit (java.lang.String p0);

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
