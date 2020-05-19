package mono.com.facebook.drawee.controller;


public class ControllerListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.drawee.controller.ControllerListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onFailure:(Ljava/lang/String;Ljava/lang/Throwable;)V:GetOnFailure_Ljava_lang_String_Ljava_lang_Throwable_Handler:Com.Facebook.Drawee.Controller.IControllerListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"n_onFinalImageSet:(Ljava/lang/String;Ljava/lang/Object;Landroid/graphics/drawable/Animatable;)V:GetOnFinalImageSet_Ljava_lang_String_Ljava_lang_Object_Landroid_graphics_drawable_Animatable_Handler:Com.Facebook.Drawee.Controller.IControllerListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"n_onIntermediateImageFailed:(Ljava/lang/String;Ljava/lang/Throwable;)V:GetOnIntermediateImageFailed_Ljava_lang_String_Ljava_lang_Throwable_Handler:Com.Facebook.Drawee.Controller.IControllerListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"n_onIntermediateImageSet:(Ljava/lang/String;Ljava/lang/Object;)V:GetOnIntermediateImageSet_Ljava_lang_String_Ljava_lang_Object_Handler:Com.Facebook.Drawee.Controller.IControllerListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"n_onRelease:(Ljava/lang/String;)V:GetOnRelease_Ljava_lang_String_Handler:Com.Facebook.Drawee.Controller.IControllerListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"n_onSubmit:(Ljava/lang/String;Ljava/lang/Object;)V:GetOnSubmit_Ljava_lang_String_Ljava_lang_Object_Handler:Com.Facebook.Drawee.Controller.IControllerListenerInvoker, Naxam.FrescoDrawee.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Facebook.Drawee.Controller.IControllerListenerImplementor, Naxam.FrescoDrawee.Droid", ControllerListenerImplementor.class, __md_methods);
	}


	public ControllerListenerImplementor ()
	{
		super ();
		if (getClass () == ControllerListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Facebook.Drawee.Controller.IControllerListenerImplementor, Naxam.FrescoDrawee.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onFailure (java.lang.String p0, java.lang.Throwable p1)
	{
		n_onFailure (p0, p1);
	}

	private native void n_onFailure (java.lang.String p0, java.lang.Throwable p1);


	public void onFinalImageSet (java.lang.String p0, java.lang.Object p1, android.graphics.drawable.Animatable p2)
	{
		n_onFinalImageSet (p0, p1, p2);
	}

	private native void n_onFinalImageSet (java.lang.String p0, java.lang.Object p1, android.graphics.drawable.Animatable p2);


	public void onIntermediateImageFailed (java.lang.String p0, java.lang.Throwable p1)
	{
		n_onIntermediateImageFailed (p0, p1);
	}

	private native void n_onIntermediateImageFailed (java.lang.String p0, java.lang.Throwable p1);


	public void onIntermediateImageSet (java.lang.String p0, java.lang.Object p1)
	{
		n_onIntermediateImageSet (p0, p1);
	}

	private native void n_onIntermediateImageSet (java.lang.String p0, java.lang.Object p1);


	public void onRelease (java.lang.String p0)
	{
		n_onRelease (p0);
	}

	private native void n_onRelease (java.lang.String p0);


	public void onSubmit (java.lang.String p0, java.lang.Object p1)
	{
		n_onSubmit (p0, p1);
	}

	private native void n_onSubmit (java.lang.String p0, java.lang.Object p1);

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
