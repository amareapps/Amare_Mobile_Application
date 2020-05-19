package mono.me.relex.photodraweeview;


public class OnScaleDragGestureListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		me.relex.photodraweeview.OnScaleDragGestureListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDrag:(FF)V:GetOnDrag_FFHandler:ME.Relex.Photodraweeview.IOnScaleDragGestureListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"n_onFling:(FFFF)V:GetOnFling_FFFFHandler:ME.Relex.Photodraweeview.IOnScaleDragGestureListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"n_onScale:(FFF)V:GetOnScale_FFFHandler:ME.Relex.Photodraweeview.IOnScaleDragGestureListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"n_onScaleEnd:()V:GetOnScaleEndHandler:ME.Relex.Photodraweeview.IOnScaleDragGestureListenerInvoker, Stormlion.PhotoDraweeView\n" +
			"";
		mono.android.Runtime.register ("ME.Relex.Photodraweeview.IOnScaleDragGestureListenerImplementor, Stormlion.PhotoDraweeView", OnScaleDragGestureListenerImplementor.class, __md_methods);
	}


	public OnScaleDragGestureListenerImplementor ()
	{
		super ();
		if (getClass () == OnScaleDragGestureListenerImplementor.class)
			mono.android.TypeManager.Activate ("ME.Relex.Photodraweeview.IOnScaleDragGestureListenerImplementor, Stormlion.PhotoDraweeView", "", this, new java.lang.Object[] {  });
	}


	public void onDrag (float p0, float p1)
	{
		n_onDrag (p0, p1);
	}

	private native void n_onDrag (float p0, float p1);


	public void onFling (float p0, float p1, float p2, float p3)
	{
		n_onFling (p0, p1, p2, p3);
	}

	private native void n_onFling (float p0, float p1, float p2, float p3);


	public void onScale (float p0, float p1, float p2)
	{
		n_onScale (p0, p1, p2);
	}

	private native void n_onScale (float p0, float p1, float p2);


	public void onScaleEnd ()
	{
		n_onScaleEnd ();
	}

	private native void n_onScaleEnd ();

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
