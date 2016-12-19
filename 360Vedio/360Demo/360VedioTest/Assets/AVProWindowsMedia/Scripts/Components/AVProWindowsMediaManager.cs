// Support for using externally created native textures, from Unity 4.2 upwards
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
	#define AVPROWINDOWSMEDIA_UNITYFEATURE_EXTERNALTEXTURES
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;
using System.Text;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Windows Media/Manager (required)")]
[ExecuteInEditMode]
public class AVProWindowsMediaManager : MonoBehaviour
{
	private static AVProWindowsMediaManager _instance;

	public bool _logVideoLoads = true;

	// Format conversion
	private Shader _shaderBGRA32;
	private Shader _shaderYUY2;
	private Shader _shaderYUY2_709;
	private Shader _shaderUYVY;
	private Shader _shaderYVYU;
	private Shader _shaderHDYC;
	private Shader _shaderNV12;
	private Shader _shaderCopy;
	private Shader _shaderHap_YCoCg;
	
	private bool _isInitialised;
	[HideInInspector] public bool _useExternalTextures = false;
	
	//-------------------------------------------------------------------------

	public static AVProWindowsMediaManager Instance  
	{
		get
		{
			if (_instance == null)
			{
				_instance = (AVProWindowsMediaManager)GameObject.FindObjectOfType(typeof(AVProWindowsMediaManager));
				if (_instance == null)
				{
					Debug.LogWarning("AVProWindowsMediaManager component required - adding dynamically now");
					GameObject go = new GameObject("AVProWindowsMediaManager");
					_instance = go.AddComponent<AVProWindowsMediaManager>();
				}
			}

			if (_instance == null)
			{
				Debug.LogError("AVProWindowsMediaManager component required");
			}
			else
			{
				if (!_instance._isInitialised)
					_instance.Init();
			}
			
			return _instance;
		}
	}
	
	//-------------------------------------------------------------------------
	
	void Awake()
	{
		if (!_isInitialised)
		{
			_instance = this;
			Init();
		}
	}
	
	void OnDestroy()
	{
		Deinit();
	}
			
	protected bool Init()
	{
		if (_isInitialised)
			return true;

		try
		{
			if (AVProWindowsMediaPlugin.Init())
			{
				if (UnityEngine.Application.isPlaying)
				{
					Debug.Log("[AVProWindowsMedia] version " + AVProWindowsMediaPlugin.GetPluginVersion().ToString("F2") + " initialised");
				}
			}
			else
			{
				Debug.LogError("[AVProWindowsMedia] failed to initialise.");
				this.enabled = false;
				Deinit();
				return false;
			}
		}
		catch (System.DllNotFoundException e)
		{
			Debug.LogError("[AVProWindowsMedia] Unity couldn't find the DLL, did you move the 'Plugins' folder to the root of your project?");
			throw e;
		}

		LoadShaders();
		GetConversionMethod();
		SetUnityFeatures();
		
//		StartCoroutine("FinalRenderCapture");
		_isInitialised = true;

		return _isInitialised;
	}

	private void SetUnityFeatures()
	{
#if !AVPROWINDOWSMEDIA_UNITYFEATURE_EXTERNALTEXTURES
		_useExternalTextures = false;
#endif
		AVProWindowsMediaPlugin.SetUnityFeatures(_useExternalTextures);
	}

	private void GetConversionMethod()
	{
		bool swapRedBlue = false;

		// TODO: perhaps we don't need to check for DX11 here?
		if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11"))
        {
#if UNITY_5
			// DX11 has red and blue channels swapped around
			if (!SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32))
				swapRedBlue = true;
#else
            swapRedBlue = true;
#endif
        }

		if (swapRedBlue)
		{
			Shader.DisableKeyword("SWAP_RED_BLUE_OFF");
			Shader.EnableKeyword("SWAP_RED_BLUE_ON");
		}
		else
		{
			Shader.DisableKeyword("SWAP_RED_BLUE_ON");
			Shader.EnableKeyword("SWAP_RED_BLUE_OFF");
		}
		
		Shader.DisableKeyword("AVPRO_GAMMACORRECTION");
		Shader.EnableKeyword("AVPRO_GAMMACORRECTION_OFF");
		if (QualitySettings.activeColorSpace == ColorSpace.Linear)
		{
			Shader.DisableKeyword("AVPRO_GAMMACORRECTION_OFF");
			Shader.EnableKeyword("AVPRO_GAMMACORRECTION");
		}
	}
	
	private IEnumerator FinalRenderCapture()
	{
		while (Application.isPlaying)
		{
			int flags = AVProWindowsMediaPlugin.PluginID | (int)AVProWindowsMediaPlugin.PluginEvent.UpdateAllTextures;
#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1
			GL.IssuePluginEvent(AVProWindowsMediaPlugin.GetRenderEventFunc(), flags);
#else
			GL.IssuePluginEvent(flags);
#endif

			yield return new WaitForEndOfFrame();
		}
	}

	public void Update()
	{
#if UNITY_EDITOR
		if (_instance == null)
			return;
#endif
		int flags = AVProWindowsMediaPlugin.PluginID | (int)AVProWindowsMediaPlugin.PluginEvent.UpdateAllTextures;
#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1
		GL.IssuePluginEvent(AVProWindowsMediaPlugin.GetRenderEventFunc(), flags);
#else
		GL.IssuePluginEvent(flags);
#endif
	}
	
	public void Deinit()
	{
		// Clean up any open movies
		AVProWindowsMediaMovie[] movies = (AVProWindowsMediaMovie[])FindObjectsOfType(typeof(AVProWindowsMediaMovie));
		if (movies != null && movies.Length > 0)
		{
			for (int i = 0; i < movies.Length; i++)
			{
				movies[i].UnloadMovie();
			}
		}
		
		_instance = null;
		_isInitialised = false;
		
		AVProWindowsMediaPlugin.Deinit();
	}

	private void LoadShaders()
	{
		_shaderBGRA32 = Resources.Load<Shader>("AVProWindowsMedia_RAW_BGRA2RGBA");
		_shaderYUY2 = Resources.Load<Shader>("AVProWindowsMedia_YUV_YUY22RGBA");
		_shaderYUY2_709 = Resources.Load<Shader>("AVProWindowsMedia_YUV_YUY27092RGBA");
		_shaderUYVY = Resources.Load<Shader>("AVProWindowsMedia_YUV_UYVY2RGBA");
		_shaderYVYU = Resources.Load<Shader>("AVProWindowsMedia_YUV_YVYU2RGBA ");
		_shaderHDYC = Resources.Load<Shader>("AVProWindowsMedia_YUV_HDYC2RGBA");
		_shaderNV12 = Resources.Load<Shader>("AVProWindowsMedia_YUV_NV12_709");
		_shaderCopy = Resources.Load<Shader>("AVProWindowsMedia_Copy");
		_shaderHap_YCoCg = Resources.Load<Shader>("AVProWindowsMedia_YCoCg2RGB");
	}

	public Shader GetPixelConversionShader(AVProWindowsMediaPlugin.VideoFrameFormat format, bool useBT709)
	{
		Shader result = null;
		switch (format)
		{
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YUY2:
			result = _shaderYUY2;
			if (useBT709)
				result = _shaderYUY2_709;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_UYVY:
			result = _shaderUYVY;
			if (useBT709)
				result = _shaderHDYC;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YVYU:
			result = _shaderYVYU;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_HDYC:
			result = _shaderHDYC;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_420_NV12:
			result = _shaderNV12;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB:
			result = _shaderCopy;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGBA:
			result = _shaderCopy;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB_HQ:
			result = _shaderHap_YCoCg;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32:
			result= _shaderBGRA32;
			break;
		default:
			Debug.LogError("[AVProWindowsMedia] Unknown pixel format '" + format);
			break;
		}
		return result;
	}
}