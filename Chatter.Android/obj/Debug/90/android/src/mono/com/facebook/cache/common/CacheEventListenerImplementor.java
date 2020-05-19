package mono.com.facebook.cache.common;


public class CacheEventListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.facebook.cache.common.CacheEventListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCleared:()V:GetOnClearedHandler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onEviction:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnEviction_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onHit:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnHit_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onMiss:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnMiss_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onReadException:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnReadException_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onWriteAttempt:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnWriteAttempt_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onWriteException:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnWriteException_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"n_onWriteSuccess:(Lcom/facebook/cache/common/CacheEvent;)V:GetOnWriteSuccess_Lcom_facebook_cache_common_CacheEvent_Handler:Com.Facebook.Cache.Common.ICacheEventListenerInvoker, Naxam.FrescoImagePipelineBase.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Facebook.Cache.Common.ICacheEventListenerImplementor, Naxam.FrescoImagePipelineBase.Droid", CacheEventListenerImplementor.class, __md_methods);
	}


	public CacheEventListenerImplementor ()
	{
		super ();
		if (getClass () == CacheEventListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Facebook.Cache.Common.ICacheEventListenerImplementor, Naxam.FrescoImagePipelineBase.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onCleared ()
	{
		n_onCleared ();
	}

	private native void n_onCleared ();


	public void onEviction (com.facebook.cache.common.CacheEvent p0)
	{
		n_onEviction (p0);
	}

	private native void n_onEviction (com.facebook.cache.common.CacheEvent p0);


	public void onHit (com.facebook.cache.common.CacheEvent p0)
	{
		n_onHit (p0);
	}

	private native void n_onHit (com.facebook.cache.common.CacheEvent p0);


	public void onMiss (com.facebook.cache.common.CacheEvent p0)
	{
		n_onMiss (p0);
	}

	private native void n_onMiss (com.facebook.cache.common.CacheEvent p0);


	public void onReadException (com.facebook.cache.common.CacheEvent p0)
	{
		n_onReadException (p0);
	}

	private native void n_onReadException (com.facebook.cache.common.CacheEvent p0);


	public void onWriteAttempt (com.facebook.cache.common.CacheEvent p0)
	{
		n_onWriteAttempt (p0);
	}

	private native void n_onWriteAttempt (com.facebook.cache.common.CacheEvent p0);


	public void onWriteException (com.facebook.cache.common.CacheEvent p0)
	{
		n_onWriteException (p0);
	}

	private native void n_onWriteException (com.facebook.cache.common.CacheEvent p0);


	public void onWriteSuccess (com.facebook.cache.common.CacheEvent p0)
	{
		n_onWriteSuccess (p0);
	}

	private native void n_onWriteSuccess (com.facebook.cache.common.CacheEvent p0);

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
