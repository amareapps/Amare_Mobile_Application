package mono.com.facebook.common.lifecycle;


public class AttachDetachListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.common.lifecycle.AttachDetachListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onAttachToView:(Landroid/view/View;)V:GetOnAttachToView_Landroid_view_View_Handler:Com.Facebook.Common.Lifecycle.IAttachDetachListenerInvoker, Naxam.FrescoCore.Droid\n" +
			"n_onDetachFromView:(Landroid/view/View;)V:GetOnDetachFromView_Landroid_view_View_Handler:Com.Facebook.Common.Lifecycle.IAttachDetachListenerInvoker, Naxam.FrescoCore.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Facebook.Common.Lifecycle.IAttachDetachListenerImplementor, Naxam.FrescoCore.Droid", AttachDetachListenerImplementor.class, __md_methods);
	}


	public AttachDetachListenerImplementor ()
	{
		super ();
		if (getClass () == AttachDetachListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Facebook.Common.Lifecycle.IAttachDetachListenerImplementor, Naxam.FrescoCore.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onAttachToView (android.view.View p0)
	{
		n_onAttachToView (p0);
	}

	private native void n_onAttachToView (android.view.View p0);


	public void onDetachFromView (android.view.View p0)
	{
		n_onDetachFromView (p0);
	}

	private native void n_onDetachFromView (android.view.View p0);

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
