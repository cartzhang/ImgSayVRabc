// Support for using externally created native textures, from Unity 4.2 upwards
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
	#define AVPROWINDOWSMEDIA_UNITYFEATURE_EXTERNALTEXTURES
#endif

using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

public class AVProWindowsMediaFormatConverter : System.IDisposable
{
	private int _movieHandle;
	
	// Format conversion and texture output
	private Texture2D _rawTexture;
	private bool _isExternalTexture;
	private RenderTexture _finalTexture;
	private Texture _outputTexture;
	private Material _conversionMaterial;
	private int _usedTextureWidth, _usedTextureHeight;
	private Vector4 _uv;	
	private int _lastFrameUploaded = -1;
	
	// Conversion params
	private int _width;
	private int _height;
	private bool _flipX;
	private bool _flipY;
	private AVProWindowsMediaPlugin.VideoFrameFormat _sourceVideoFormat;
	private bool _useBT709;
	private bool _requiresTextureCrop;
	private bool _requiresConversion;

	public bool RequiresConversion
	{
		get { return _requiresTextureCrop; } 
	}

	public Texture OutputTexture
	{
		get { return _outputTexture; }
	}
	
	public int DisplayFrame
	{
		get { return _lastFrameUploaded; }
	}

	public bool	ValidPicture { get; private set; }
	
	public void Reset()
	{
		ValidPicture = false;
		_lastFrameUploaded = -1;
	}

	public Material GetConversionMaterial()
	{
		if (!_requiresConversion)
			return _conversionMaterial;
		return null;
	}
	
	public bool Build(int movieHandle, int width, int height, AVProWindowsMediaPlugin.VideoFrameFormat format, bool useBT709, bool flipX, bool flipY, FilterMode filterMode, TextureWrapMode wrapMode)
	{
		Reset();

		_outputTexture = null;
		_movieHandle = movieHandle;

		_width = width;
		_height = height;
		_sourceVideoFormat = format;
		_flipX = flipX;
		_flipY = flipY;
		_useBT709 = useBT709;
		
#if AVPROWINDOWSMEDIA_UNITYFEATURE_EXTERNALTEXTURES
		if (AVProWindowsMediaManager.Instance._useExternalTextures)
			CreateExternalTexture();
		else
#endif
			CreateTexture();

		if (_rawTexture != null)
		{
			_requiresConversion = false;
			_requiresTextureCrop = (_usedTextureWidth != _rawTexture.width || _usedTextureHeight != _rawTexture.height);
			if (_requiresTextureCrop)
			{
				SetFlip(_flipX, _flipY);
				_requiresConversion = true;
			}

			if (!_isExternalTexture)
				AVProWindowsMediaPlugin.SetTexturePointer(_movieHandle, _rawTexture.GetNativeTexturePtr());

			if (!_requiresConversion)
			{
				bool isDX11 = SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11");

				if (_flipX || _flipY)
				{
					_requiresConversion = true;
				}
				else if (_sourceVideoFormat == AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32 && isDX11)
				{
#if UNITY_5
					// DX11 has red and blue channels swapped around
					if (!SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32))
						_requiresConversion = true;
#else
                    _requiresConversion = true;
#endif
				}
				else if (_sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB &&
					 	_sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGBA &&
						_sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32)
				{
					_requiresConversion = true;
				}
			}

			if (_requiresConversion)
			{
				if (CreateMaterial())
				{
					CreateRenderTexture();
					_outputTexture = _finalTexture;

					_conversionMaterial.mainTexture = _rawTexture;
					SetFlip(_flipX, _flipY);
					bool formatIs422 = (_sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32);
					if (formatIs422)
					{
						_conversionMaterial.SetFloat("_TextureWidth", _finalTexture.width);
					}
				}
			}
			else
			{

				bool formatIs422 = (_sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32 &&
				                    _sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB &&
				                    _sourceVideoFormat != AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGBA);
				if (formatIs422)
				{
					CreateMaterial();
					_conversionMaterial.SetFloat("_TextureWidth", _width);
					_rawTexture.filterMode = FilterMode.Point;
				}
				else
				{
					_rawTexture.filterMode = FilterMode.Bilinear;
				}
				//_rawTexture.wrapMode = TextureWrapMode.Repeat;
				_outputTexture = _rawTexture;
			}
		}

		if (_outputTexture != null)
		{
			_outputTexture.filterMode = filterMode;
			_outputTexture.wrapMode = wrapMode;
		}
		
		return (_outputTexture != null);
	}
	
	public bool Update()
	{
		bool result = UpdateTexture();
		if (_requiresConversion)
		{
			if (result)
			{
				DoFormatConversion();
			}
			else
			{
				if (_finalTexture != null && !_finalTexture.IsCreated())
				{
					Reset();
				}
			}
		}
		else
		{
			if (result)
				ValidPicture = true;
		}
		return result;
	}
	
	private bool UpdateTexture()
	{
		bool result = false;
	
		// We update all the textures from AVProQuickTimeManager.Update()
		// so just check if the update was done
		int lastFrameUploaded = AVProWindowsMediaPlugin.GetLastFrameUploaded(_movieHandle);
		if (_lastFrameUploaded != lastFrameUploaded)
		{			
			_lastFrameUploaded = lastFrameUploaded;
			result = true;
		}

		return result;
	}
	
	public void Dispose()
	{
		ValidPicture = false;
		_width = _height = 0;
		
		if (_conversionMaterial != null)
		{
			_conversionMaterial.mainTexture = null;
#if UNITY_EDITOR
			Material.DestroyImmediate(_conversionMaterial);
#else
            Material.Destroy(_conversionMaterial);
#endif
			_conversionMaterial = null;
		}

		_outputTexture = null;
		
		if (_finalTexture != null)
		{
			RenderTexture.ReleaseTemporary(_finalTexture);
			_finalTexture = null;
		}
		
		if (_rawTexture != null)
		{			
			if (!_isExternalTexture)
			{
#if UNITY_EDITOR
				Texture2D.DestroyImmediate(_rawTexture);
#else
				Texture2D.Destroy(_rawTexture);
#endif
			}
			_rawTexture = null;
		}
	}

	private bool CreateMaterial()
	{	
		Shader shader = AVProWindowsMediaManager.Instance.GetPixelConversionShader(_sourceVideoFormat, _useBT709);
		if (shader)
		{
			if (_conversionMaterial != null)
			{
				if (_conversionMaterial.shader != shader)
				{
					Material.Destroy(_conversionMaterial);
					_conversionMaterial = null;
				}
			}
			
			if (_conversionMaterial == null)
			{
				_conversionMaterial = new Material(shader);
				_conversionMaterial.name = "AVProWindowsMedia-Material";
			}
		}
		
		return (_conversionMaterial != null);
	}	

#if AVPROWINDOWSMEDIA_UNITYFEATURE_EXTERNALTEXTURES
	private void CreateExternalTexture()
	{
		System.IntPtr texPtr = AVProWindowsMediaPlugin.GetTexturePointer(_movieHandle);
		if (texPtr != System.IntPtr.Zero)
		{
			int texWidth = _width;
			TextureFormat textureFormat = TextureFormat.ARGB32;
			switch (_sourceVideoFormat)
			{
			case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGBA:
			case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB_HQ:
				textureFormat = TextureFormat.DXT5;
				break;
			case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB:
				textureFormat = TextureFormat.DXT1;
				break;
			case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YUY2:
			case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_UYVY:
			case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YVYU:
			case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_HDYC:
				texWidth = _width / 2;
				break;
			}
			
			_usedTextureWidth = texWidth;
			_usedTextureHeight = _height;
			_rawTexture = Texture2D.CreateExternalTexture(texWidth, _height, textureFormat, false, false, texPtr);

			_rawTexture.wrapMode = TextureWrapMode.Clamp;
			_rawTexture.filterMode = FilterMode.Point;
			_rawTexture.name = "AVProWindowsMedia-RawExternal";

			_isExternalTexture = true;
		}
	}
#endif
	
	private void CreateTexture()
	{
		_usedTextureWidth = _width;
		_usedTextureHeight = _height;
		
		// Calculate texture size and format
		int textureWidth = _usedTextureWidth;
		int textureHeight = _usedTextureHeight;
		TextureFormat textureFormat = TextureFormat.RGBA32;


		switch (_sourceVideoFormat)
		{
		case AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32:
			textureFormat = TextureFormat.RGBA32;
            if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11"))
            {
#if UNITY_5
			    // DX11 has red and blue channels swapped around
			    if (SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32))
				    textureFormat = TextureFormat.BGRA32;
#endif                
            }
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGBA:
		case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB_HQ:
			textureFormat = TextureFormat.DXT5;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB:
			textureFormat = TextureFormat.DXT1;
			break;
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YUY2:
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_UYVY:
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_HDYC:
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_422_YVYU:
		case AVProWindowsMediaPlugin.VideoFrameFormat.YUV_420_NV12:
			textureFormat = TextureFormat.RGBA32;
            if (SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D 11"))
            {
#if UNITY_5
			    // DX11 has red and blue channels swapped around
			    if (SystemInfo.SupportsTextureFormat(TextureFormat.BGRA32))
				    textureFormat = TextureFormat.BGRA32;
#endif
            }
            _usedTextureWidth /= 2;	// YCbCr422 modes need half width
			textureWidth = _usedTextureWidth;
			break;
		}

		bool requiresPOT = (SystemInfo.npotSupport == NPOTSupport.None);

		// If the texture isn't a power of 2
		if (requiresPOT)
		{
			// We use a power-of-2 texture as Unity makes these internally anyway and not doing it seems to break things for texture updates
			if (!Mathf.IsPowerOfTwo(_width) || !Mathf.IsPowerOfTwo(_height))
			{
				textureWidth = Mathf.NextPowerOfTwo(textureWidth);
				textureHeight = Mathf.NextPowerOfTwo(textureHeight);
			}
		}
				
		// Create texture that stores the initial raw frame
		// If there is already a texture, only destroy it if it's not the same requirements
		if (_rawTexture != null)
		{
			if (_rawTexture.width != textureWidth || 
			    _rawTexture.height != textureHeight ||
			    _rawTexture.format != textureFormat)
			{
				Texture2D.Destroy(_rawTexture);
				_rawTexture = null;
			}
		}

		if (_rawTexture == null)
		{
			bool isLinear = true;
			if (!_requiresConversion)
			{
				if (AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGBA == _sourceVideoFormat ||
					AVProWindowsMediaPlugin.VideoFrameFormat.Hap_RGB == _sourceVideoFormat ||
					AVProWindowsMediaPlugin.VideoFrameFormat.RAW_BGRA32 == _sourceVideoFormat)
				{
					isLinear = false;
				}
			}

			_rawTexture = new Texture2D(textureWidth, textureHeight, textureFormat, false, isLinear);
			_rawTexture.wrapMode = TextureWrapMode.Clamp;
			_rawTexture.filterMode = FilterMode.Point;
			_rawTexture.name = "AVProWindowsMedia-RawTexture";
			_rawTexture.Apply(false, true);
			_isExternalTexture = false;
		}
	}
	
	private void CreateRenderTexture()
	{	
		// Create RenderTexture for post transformed frames
		// If there is already a renderTexture, only destroy it smaller than desired size
		if (_finalTexture != null)
		{
			if (_finalTexture.width != _width || _finalTexture.height != _height)
			{
				RenderTexture.ReleaseTemporary(_finalTexture);
				_finalTexture = null;
			}
		}

		if (_finalTexture == null)
		{
			ValidPicture = false;
			_finalTexture = RenderTexture.GetTemporary(_width, _height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
			_finalTexture.wrapMode = TextureWrapMode.Clamp;
			_finalTexture.filterMode = FilterMode.Bilinear;
			_finalTexture.name = "AVProWindowsMedia-FinalTexture";
			_finalTexture.Create();
		}
	}
	
	private void DoFormatConversion()
	{
		if (_finalTexture == null)
			return;

		_finalTexture.DiscardContents();

		RenderTexture prev = RenderTexture.active;
		if (!_requiresTextureCrop)
		{
			Graphics.Blit(_rawTexture, _finalTexture, _conversionMaterial, 0);
		}
		else
		{
			RenderTexture.active = _finalTexture;

			_conversionMaterial.SetPass(0);

			GL.PushMatrix();
			GL.LoadOrtho();
			DrawQuad(_uv);
			GL.PopMatrix();

		}
		RenderTexture.active = prev;

		ValidPicture = true;
	}

	private void SetFlip(bool flipX, bool flipY)
	{
		_flipX = flipX;
		_flipY = flipY;

		if (_requiresTextureCrop)
		{
			CreateUVs(_flipX, _flipY);
		}
		else
		{
			if (_conversionMaterial != null)
			{
				// Flip and then offset back to get back to normalised range
				Vector2 scale = new Vector2(1f, 1f);
				Vector2 offset = new Vector2(0f, 0f);
				if (_flipX)
				{
					scale = new Vector2(-1f, scale.y);
					offset = new Vector2(1f, offset.y);
				}
				if (_flipY)
				{
					scale = new Vector2(scale.x, -1f);
					offset = new Vector2(offset.x, 1f);
				}

				_conversionMaterial.mainTextureScale = scale;
				_conversionMaterial.mainTextureOffset = offset;
				// Since Unity 5.3 Graphics.Blit ignores mainTextureOffset/Scale
#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
				_conversionMaterial.SetVector("_MainTex_ST2", new Vector4(scale.x, scale.y, offset.x, offset.y));
#endif
			}
		}
	}
	
	private void CreateUVs(bool invertX, bool invertY)
	{				
		float x1, x2;
		float y1, y2;
		if (invertX)
		{
			x1 = 1.0f; x2 = 0.0f;
		}
		else
		{
			x1 = 0.0f; x2 = 1.0f;
		}
		if (invertY)
		{
			y1 = 1.0f; y2 = 0.0f;
		}
		else
		{
			y1 = 0.0f; y2 = 1.0f;
		}
		
		// Alter UVs if we're only using a portion of the texture
		if (_usedTextureWidth != _rawTexture.width)
		{
			float xd = _usedTextureWidth / (float)_rawTexture.width;
			x1 *= xd; x2 *= xd;
		}
		if (_usedTextureHeight != _rawTexture.height)
		{
			float yd = _usedTextureHeight / (float)_rawTexture.height;
			y1 *= yd; y2 *= yd;
		}
			
		_uv = new Vector4(x1, y1, x2, y2);
	}
	
	private static void DrawQuad(Vector4 uv)
	{
		GL.Begin(GL.QUADS);
		
		GL.TexCoord2(uv.x, uv.y);
		GL.Vertex3(0.0f, 0.0f, 0.1f);
		
		GL.TexCoord2(uv.z, uv.y);
		GL.Vertex3(1.0f, 0.0f, 0.1f);
		
		GL.TexCoord2(uv.z, uv.w);		
		GL.Vertex3(1.0f, 1.0f, 0.1f);
		
		GL.TexCoord2(uv.x, uv.w);
		GL.Vertex3(0.0f, 1.0f, 0.1f);
		
		GL.End();
	}	
}