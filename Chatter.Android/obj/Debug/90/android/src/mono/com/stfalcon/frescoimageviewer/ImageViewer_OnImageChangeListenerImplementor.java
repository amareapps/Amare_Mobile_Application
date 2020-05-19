package mono.com.stfalcon.frescoimageviewer;


public class ImageViewer_OnImageChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.stfalcon.frescoimageviewer.ImageViewer.OnImageChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onImageChange:(I)V:GetOnImageChange_IHandler:Com.Stfalcon.Frescoimageviewer.ImageViewer/IOnImageChangeListenerInvoker, Stormlion.FrescoImageViewer\n" +
			"";
		mono.android.Runtime.register ("Com.Stfalcon.Frescoimageviewer.ImageViewer+IOnImageChangeListenerImplementor, Stormlion.FrescoImageViewer", ImageViewer_OnImageChangeListenerImplementor.class, __md_methods);
	}


	public ImageViewer_OnImageChangeListenerImplementor ()
	{
		super ();
		if (getClass () == ImageViewer_OnImageChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Stfalcon.Frescoimageviewer.ImageViewer+IOnImageChangeListenerImplementor, Stormlion.FrescoImageViewer", "", this, new java.lang.Object[] {  });
	}


	public void onImageChange (int p0)
	{
		n_onImageChange (p0);
	}

	private native void n_onImageChange (int p0);

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
