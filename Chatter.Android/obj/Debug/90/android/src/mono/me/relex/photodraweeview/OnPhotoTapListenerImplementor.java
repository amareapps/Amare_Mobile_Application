package mono.me.relex.photodraweeview;


public class OnPhotoTapListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		me.relex.photodraweeview.OnPhotoTapListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onPhotoTap:(Landroid/view/View;FF)V:GetOnPhotoTap_Landroid_view_View_FFHandler:ME.Relex.Photodraweeview.IOnPhotoTapListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"";
		mono.android.Runtime.register ("ME.Relex.Photodraweeview.IOnPhotoTapListenerImplementor, Stormlion.PhotoDraweeView", OnPhotoTapListenerImplementor.class, __md_methods);
	}


	public OnPhotoTapListenerImplementor ()
	{
		super ();
		if (getClass () == OnPhotoTapListenerImplementor.class)
			mono.android.TypeManager.Activate ("ME.Relex.Photodraweeview.IOnPhotoTapListenerImplementor, Stormlion.PhotoDraweeView", "", this, new java.lang.Object[] {  });
	}


	public void onPhotoTap (android.view.View p0, float p1, float p2)
	{
		n_onPhotoTap (p0, p1, p2);
	}

	private native void n_onPhotoTap (android.view.View p0, float p1, float p2);

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
