package mono.me.relex.photodraweeview;


public class OnViewTapListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		me.relex.photodraweeview.OnViewTapListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onViewTap:(Landroid/view/View;FF)V:GetOnViewTap_Landroid_view_View_FFHandler:ME.Relex.Photodraweeview.IOnViewTapListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"";
		mono.android.Runtime.register ("ME.Relex.Photodraweeview.IOnViewTapListenerImplementor, Stormlion.PhotoDraweeView", OnViewTapListenerImplementor.class, __md_methods);
	}


	public OnViewTapListenerImplementor ()
	{
		super ();
		if (getClass () == OnViewTapListenerImplementor.class)
			mono.android.TypeManager.Activate ("ME.Relex.Photodraweeview.IOnViewTapListenerImplementor, Stormlion.PhotoDraweeView", "", this, new java.lang.Object[] {  });
	}


	public void onViewTap (android.view.View p0, float p1, float p2)
	{
		n_onViewTap (p0, p1, p2);
	}

	private native void n_onViewTap (android.view.View p0, float p1, float p2);

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
