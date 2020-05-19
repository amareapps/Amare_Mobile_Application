package mono.com.stfalcon.frescoimageviewer;


public class ImageViewer_OnDismissListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.stfalcon.frescoimageviewer.ImageViewer.OnDismissListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDismiss:()V:GetOnDismissHandler:Com.Stfalcon.Frescoimageviewer.ImageViewer/IOnDismissListenerInvoker, Stormlion.FrescoImageViewer\n" +
			"";
		mono.android.Runtime.register ("Com.Stfalcon.Frescoimageviewer.ImageViewer+IOnDismissListenerImplementor, Stormlion.FrescoImageViewer", ImageViewer_OnDismissListenerImplementor.class, __md_methods);
	}


	public ImageViewer_OnDismissListenerImplementor ()
	{
		super ();
		if (getClass () == ImageViewer_OnDismissListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Stfalcon.Frescoimageviewer.ImageViewer+IOnDismissListenerImplementor, Stormlion.FrescoImageViewer", "", this, new java.lang.Object[] {  });
	}


	public void onDismiss ()
	{
		n_onDismiss ();
	}

	private native void n_onDismiss ();

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
