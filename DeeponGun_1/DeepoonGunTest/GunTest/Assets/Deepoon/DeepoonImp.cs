using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
#if UNITY_EDITOR 
using UnityEditor;
#endif

public class DeepoonImp
{
	//deepoon unity
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static bool dpnuInit
		( IntPtr hwnd
		 , int expected_version
		 , int sensor_index
		 , float INTERPUPILLARY_DISTANCE
		 );
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static void dpnuUninit();
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static DeepoonCommon.dpnRect dpnuGetRenderTargetSize();
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static IntPtr dpnuCreateSurface
		( IntPtr texture
		 , DeepoonCommon.dpncAPPLY apply
		 , DeepoonCommon.dpncMSAA msaa
		 , DeepoonCommon.dpncEYE eye
		 , int depth
		 , float texux
		 , float texuy
		 , float texvx
		 , float texvy
		 );
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static bool dpnuDestroySurface
		( IntPtr surface
		 );
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static DeepoonCommon.dpnTransform dpnuGetTransform();
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static DeepoonCommon.dpnSensorData dpnuGetSensorData();
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static void dpnuDrawSurface
		( IntPtr surface );
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static bool dpnuCompose( bool swap );
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static bool dpnuWaitVsync();
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static int dpnuGetLastError();
	
	[DllImport("deepoon_unity", CallingConvention = CallingConvention.Cdecl)]
	public extern static int dpnuCheckMessage();

	//[DllImport("user32.dll")]
	//static extern IntPtr GetForegroundWindow ();
	
	//[DllImport("user32.dll")]
	//static extern IntPtr SetActiveWindow ( IntPtr hwnd );

	private const int NUM_EYE = 2;
	private string[] EYE_NAMES = null;
	private const int RENDER_TEXTURE_DEPTH = 24;
	private const RenderTextureFormat RENDER_TEXTURE_FORMAT = RenderTextureFormat.Default;
	private IntPtr[] _surface_eyes = null;
	private IntPtr[] _surface_gui = null;

	public GameObject owner = null;
	public Camera[] eyes = null;
	//public RenderTexture target_gui = null;

	private static bool _initialized = false;

	public bool Init( GameObject p , int version , float fov , float ipd , bool use_gui , RenderTexture[] tar_gui )
	{
		owner = p;

		//eye names
		EYE_NAMES = new string[NUM_EYE]{ "DeepoonEyeLeft" , "DeepoonEyeRight" , };

		//find eyes
		GameObject[] eye_objs = new GameObject[NUM_EYE];
		for( int i = 0 ; i < NUM_EYE ; ++i )
		{
			//create game object to put camera
			eye_objs[i] = p.transform.FindChild( EYE_NAMES[i] ).gameObject; //new GameObject( EYE_NAMES[i] );
			//eye_objs[i].transform.localPosition = new Vector3( -ipd / 2 + i * ipd  , nh , 0 );
			//eye_objs[i].transform.SetParent( owner.transform );
		}

		//calculate ipd
		//float ipd = eye_objs[1].transform.localPosition.x - eye_objs[0].transform.localPosition.x;

		//init deepoon
		bool ret = dpnuInit (IntPtr.Zero, version, 0, ipd);
		if (ret)
		{
			//get render target size, which covers two eyes
			DeepoonCommon.dpnRect target_size = dpnuGetRenderTargetSize ();
			
			//create render target for gui
/*			if( use_gui )
			{

				target_gui = tar_gui;
				if( target_gui.IsCreated() ) target_gui.Release();
				target_gui.width = target_size.w / 2;
				target_gui.height = target_size.h;
				target_gui.depth = RENDER_TEXTURE_DEPTH;
				target_gui.format = RENDER_TEXTURE_FORMAT;
				target_gui.antiAliasing = 1; //(QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing;
				if (!target_gui.IsCreated ()) target_gui.Create ();
			}*/
			
			//create render textures
			RenderTexture[] textures = new RenderTexture[NUM_EYE];
			_surface_eyes = new IntPtr[NUM_EYE];
			if( use_gui )
			{
				_surface_gui = new IntPtr[NUM_EYE];
			}
			eyes = new Camera[NUM_EYE];
			for( int i = 0 ; i < NUM_EYE ; ++i )
			{
				//create render target
				textures[i] = new RenderTexture
					(target_size.w / 2, target_size.h
					 , RENDER_TEXTURE_DEPTH, RENDER_TEXTURE_FORMAT);
				textures[i].antiAliasing = 1; //(QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing;
				if (!textures[i].IsCreated ()) textures[i].Create ();

				if( use_gui )
				{
					if (!tar_gui[i].IsCreated ()) tar_gui[i].Create ();
				}

				//create camera
				//eye_objs[i].AddComponent< Camera >();
				eyes[i] = eye_objs[i].GetComponent< Camera >();
				eyes[i].fieldOfView = fov;
				eyes[i].targetTexture = textures[i];
				
				//create surfaces for eyes
				_surface_eyes[i] = dpnuCreateSurface
					( textures[i].GetNativeTexturePtr()
					 , DeepoonCommon.dpncAPPLY.dpncAPPLY_DISTORTION_TIMEWARP
					 , DeepoonCommon.dpncMSAA.dpncMSAA_1x , (DeepoonCommon.dpncEYE)i , 0
					 , 1 , 0 , 0 , -1 );
				if( IntPtr.Zero == _surface_eyes[i] )
				{
					dpnuUninit();
#if UNITY_EDITOR 
					UnityEditor.EditorUtility.DisplayDialog("Deepoon", "WARNING: Failed to create surface for eyes!", "OK");
#endif
					Debug.Log ("WARNING: Failed to create surface for eyes!");
					return false;
				}
				
				//create surfaces for gui
				if( use_gui )
				{
					_surface_gui[i] = dpnuCreateSurface
						( tar_gui[i].GetNativeTexturePtr()
						 , DeepoonCommon.dpncAPPLY.dpncAPPLY_DISTORTION
						 , DeepoonCommon.dpncMSAA.dpncMSAA_1x , (DeepoonCommon.dpncEYE)i , 1
						 , 1 , 0 , 0 , -1 );
					if( IntPtr.Zero == _surface_gui[i] )
					{
						dpnuUninit();
#if UNITY_EDITOR 
						UnityEditor.EditorUtility.DisplayDialog("Deepoon", "WARNING: Failed to create surface for GUI!", "OK");
#endif
						Debug.Log ("WARNING: Failed to create surface for GUI!");
						return false;
					}
				}
			}
		}
		else
		{
#if UNITY_EDITOR 
			UnityEditor.EditorUtility.DisplayDialog("Deepoon", "WARNING: Failed to initialize DeepoonUnity!", "OK");
#endif
			Debug.Log ("WARNING: Failed to initialize DeepoonUnity!");
			return false;
		}

		//
		{
			//SetActiveWindow( GetForegroundWindow() );
		}
		_initialized = true;
		return true;
	}

	public void Update()
	{
		if (false == _initialized)
			return;
		DeepoonCommon.dpnTransform trans = dpnuGetTransform();
		for( int i = 0 ; i < NUM_EYE ; ++i )
		{
			dpnuDrawSurface( _surface_eyes[i] );
			if( _surface_gui != null )
			{
				dpnuDrawSurface( _surface_gui[i] );
			}
		}
		owner.transform.localRotation = DeepoonCommon.ToQuaternion( trans.q );
	}

	public void Render()
	{
		if (false == _initialized)
			return;
		for( int i = 0 ; i < NUM_EYE ; ++i )
		{
			eyes[i].Render();
		}
	}

	public void Compose()
	{
		if (false == _initialized)
			return;
		IssuePluginEvent (DeepoonCommon.RENDER_EVENT.COMPOSE);
	}

	public void Uninit()
	{
		if (false == _initialized)
			return;
		if( null!= _surface_gui )
		{
			for( int i = 0 ; i < NUM_EYE ; ++i )
			{
				dpnuDestroySurface( _surface_gui[i] );
			}
		}
		if( null!= _surface_eyes )
		{
			for( int i = 0 ; i < NUM_EYE ; ++i )
			{
				dpnuDestroySurface( _surface_eyes[i] );
			}
		}
		dpnuUninit ();
		_initialized = false;
	}

	public int CheckMessage()
	{
		return dpnuCheckMessage();
	}

	public bool HasInitialized()
	{
		return _initialized;
	}
	
	private static void IssuePluginEvent( DeepoonCommon.RENDER_EVENT evt )
	{
		if (false == _initialized)
			return;
		GL.IssuePluginEvent ((int)evt);
	}
}
