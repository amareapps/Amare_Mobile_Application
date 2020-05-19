package mono.me.relex.photodraweeview;


public class OnScaleChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		me.relex.photodraweeview.OnScaleChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onScaleChange:(FFF)V:GetOnScaleChange_FFFHandler:ME.Relex.Photodraweeview.IOnScaleChangeListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"";
		mono.android.Runtime.register ("ME.Relex.Photodraweeview.IOnScaleChangeListenerImplementor, Stormlion.PhotoDraweeView", OnScaleChangeListenerImplementor.class, __md_methods);
	}


	public OnScaleChangeListenerImplementor ()
	{
		super ();
		if (getClass () == OnScaleChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("ME.Relex.Photodraweeview.IOnScaleChangeListenerImplementor, Stormlion.PhotoDraweeView", "", this, new java.lang.Object[] {  });
	}


	public void onScaleChange (float p0, float p1, float p2)
	{
		n_onScaleChange (p0, p1, p2);
	}

	private native void n_onScaleChange (float p0, float p1, float p2);

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
