using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Windows Media/Movie From Resource")]
public class AVProWindowsMediaMovieFromResource : AVProWindowsMediaMovie
{
	private TextAsset _textAsset;
	private GCHandle _bytesHandle;
	
	public override void Start()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			return;
#endif

		if (null == AVProWindowsMediaManager.Instance)
		{
			throw new System.Exception("You need to add AVProWindowsMediaManager component to your scene.");
		}
		if (_loadOnStart)
		{
			LoadMovieFromResource(_playOnStart, _filename);
		}
	}

	public override bool LoadMovie(bool autoPlay)
	{
		return LoadMovieFromResource(autoPlay, _filename);
	}

	public bool LoadMovieFromResource(bool autoPlay, string path)
	{
		bool result = false;
		
		UnloadMovie();
		
		_textAsset = Resources.Load(path, typeof(TextAsset)) as TextAsset;
        if (_textAsset != null)
        {
            if (_textAsset.bytes != null && _textAsset.bytes.Length > 0)
            {
                _bytesHandle = GCHandle.Alloc(_textAsset.bytes, GCHandleType.Pinned);
                result = LoadMovieFromMemory(autoPlay, path, _bytesHandle.AddrOfPinnedObject(), (uint)_textAsset.bytes.Length, FilterMode.Bilinear, TextureWrapMode.Clamp);
            }
        }

        if (!result)
        {
            Debug.LogError("[AVProWindowsMedia] Unable to load resource " + path);
        }
		
		return result;
	}
	
	public override void UnloadMovie()
	{
		if (_moviePlayer != null)
		{
			_moviePlayer.Dispose();
			_moviePlayer = null;
		}

		UnloadResource();
	}
	
	private void UnloadResource()
	{
		if (_bytesHandle.IsAllocated)
		{
			_bytesHandle.Free();
		}
#if !UNITY_3_5
		if (_textAsset != null)
		{
			Resources.UnloadAsset(_textAsset);
			_textAsset = null;
		}
#endif
	}
}