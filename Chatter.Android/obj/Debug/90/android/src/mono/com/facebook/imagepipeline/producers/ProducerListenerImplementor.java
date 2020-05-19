package mono.com.facebook.imagepipeline.producers;


public class ProducerListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.imagepipeline.producers.ProducerListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onProducerEvent:(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V:GetOnProducerEvent_Ljava_lang_String_Ljava_lang_String_Ljava_lang_String_Handler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"n_onProducerFinishWithCancellation:(Ljava/lang/String;Ljava/lang/String;Ljava/util/Map;)V:GetOnProducerFinishWithCancellation_Ljava_lang_String_Ljava_lang_String_Ljava_util_Map_Handler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"n_onProducerFinishWithFailure:(Ljava/lang/String;Ljava/lang/String;Ljava/lang/Throwable;Ljava/util/Map;)V:GetOnProducerFinishWithFailure_Ljava_lang_String_Ljava_lang_String_Ljava_lang_Throwable_Ljava_util_Map_Handler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"n_onProducerFinishWithSuccess:(Ljava/lang/String;Ljava/lang/String;Ljava/util/Map;)V:GetOnProducerFinishWithSuccess_Ljava_lang_String_Ljava_lang_String_Ljava_util_Map_Handler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"n_onProducerStart:(Ljava/lang/String;Ljava/lang/String;)V:GetOnProducerStart_Ljava_lang_String_Ljava_lang_String_Handler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"n_onUltimateProducerReached:(Ljava/lang/String;Ljava/lang/String;Z)V:GetOnUltimateProducerReached_Ljava_lang_String_Ljava_lang_String_ZHandler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"n_requiresExtraMap:(Ljava/lang/String;)Z:GetRequiresExtraMap_Ljava_lang_String_Handler:Com.Facebook.Imagepipeline.Producers.IProducerListenerInvoker, Naxam.FrescoImagePipeline.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Facebook.Imagepipeline.Producers.IProducerListenerImplementor, Naxam.FrescoImagePipeline.Droid", ProducerListenerImplementor.class, __md_methods);
	}


	public ProducerListenerImplementor ()
	{
		super ();
		if (getClass () == ProducerListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Facebook.Imagepipeline.Producers.IProducerListenerImplementor, Naxam.FrescoImagePipeline.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onProducerEvent (java.lang.String p0, java.lang.String p1, java.lang.String p2)
	{
		n_onProducerEvent (p0, p1, p2);
	}

	private native void n_onProducerEvent (java.lang.String p0, java.lang.String p1, java.lang.String p2);


	public void onProducerFinishWithCancellation (java.lang.String p0, java.lang.String p1, java.util.Map p2)
	{
		n_onProducerFinishWithCancellation (p0, p1, p2);
	}

	private native void n_onProducerFinishWithCancellation (java.lang.String p0, java.lang.String p1, java.util.Map p2);


	public void onProducerFinishWithFailure (java.lang.String p0, java.lang.String p1, java.lang.Throwable p2, java.util.Map p3)
	{
		n_onProducerFinishWithFailure (p0, p1, p2, p3);
	}

	private native void n_onProducerFinishWithFailure (java.lang.String p0, java.lang.String p1, java.lang.Throwable p2, java.util.Map p3);


	public void onProducerFinishWithSuccess (java.lang.String p0, java.lang.String p1, java.util.Map p2)
	{
		n_onProducerFinishWithSuccess (p0, p1, p2);
	}

	private native void n_onProducerFinishWithSuccess (java.lang.String p0, java.lang.String p1, java.util.Map p2);


	public void onProducerStart (java.lang.String p0, java.lang.String p1)
	{
		n_onProducerStart (p0, p1);
	}

	private native void n_onProducerStart (java.lang.String p0, java.lang.String p1);


	public void onUltimateProducerReached (java.lang.String p0, java.lang.String p1, boolean p2)
	{
		n_onUltimateProducerReached (p0, p1, p2);
	}

	private native void n_onUltimateProducerReached (java.lang.String p0, java.lang.String p1, boolean p2);


	public boolean requiresExtraMap (java.lang.String p0)
	{
		return n_requiresExtraMap (p0);
	}

	private native boolean n_requiresExtraMap (java.lang.String p0);

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
