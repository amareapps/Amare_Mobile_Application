package crc6428a8384f010fa92a;


public class ImageOverlayView
	extends android.widget.RelativeLayout
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
		mono.android.Runtime.register ("Stormlion.PhotoBrowser.Droid.ImageOverlayView, Stormlion.PhotoBrowser.Android", ImageOverlayView.class, __md_methods);
	}


	public ImageOverlayView (android.content.Context p0)
	{
		super (p0);
		if (getClass () == ImageOverlayView.class)
			mono.android.TypeManager.Activate ("Stormlion.PhotoBrowser.Droid.ImageOverlayView, Stormlion.PhotoBrowser.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public ImageOverlayView (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == ImageOverlayView.class)
			mono.android.TypeManager.Activate ("Stormlion.PhotoBrowser.Droid.ImageOverlayView, Stormlion.PhotoBrowser.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ImageOverlayView (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == ImageOverlayView.class)
			mono.android.TypeManager.Activate ("Stormlion.PhotoBrowser.Droid.ImageOverlayView, Stormlion.PhotoBrowser.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ImageOverlayView (android.content.Context p0, android.util.AttributeSet p1, int p2, int p3)
	{
		super (p0, p1, p2, p3);
		if (getClass () == ImageOverlayView.class)
			mono.android.TypeManager.Activate ("Stormlion.PhotoBrowser.Droid.ImageOverlayView, Stormlion.PhotoBrowser.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2, p3 });
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
