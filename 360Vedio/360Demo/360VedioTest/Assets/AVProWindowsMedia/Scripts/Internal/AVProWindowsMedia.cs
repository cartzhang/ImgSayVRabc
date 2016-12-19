#if UNITY_EDITOR
	#define AVPROWINDOWSMEDIA_FPS
#endif

using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

public class AVProWindowsMedia : System.IDisposable
{
	private int _movieHandle = -1;
	private AVProWindowsMediaFormatConverter _formatConverter;

#if AVPROWINDOWSMEDIA_FPS
	private int _frameCount;
	private float _startFrameTime;
	
	public float DisplayFPS
	{
		get;
		private set;
	}
	
	public int FramesTotal
	{
		get;
		private set;
	}	
#endif
	
	//-----------------------------------------------------------------------------
	
	public int Handle
	{
		get { return _movieHandle; }
	}
	
	// Movie Properties

	public string Filename
	{
		get; private set;
	}
	
	public int Width
	{
		get; private set;
	}
	
	public int Height
	{
		get; private set;
	}
	
	public float AspectRatio
	{
		get { return (Width / (float)Height); }
	}	
	
	public float FrameRate
	{
		get; private set;
	}

	public float DurationSeconds
	{
		get; private set;
	}
	
	public uint DurationFrames
	{
		get; private set;
	}

	public uint LastFrame
	{
		get { return (uint)Mathf.Max(0, (int)DurationFrames - 1); }
	}	
	
	// Playback State
	
	public bool IsPlaying
	{
		get; private set;
	}
	
	private bool _isLooping = false;
	public bool Loop
	{
		set { _isLooping = value; AVProWindowsMediaPlugin.SetLooping(_movieHandle, value); }
		get { return _isLooping; }
	}	
	
	private int _audioDelay = 0;
	public int AudioDelay
	{
		set { _audioDelay = value; AVProWindowsMediaPlugin.SetAudioDelay(_movieHandle, _audioDelay); }
		get { return _audioDelay; }
	}	

	private float _volume = 1.0f;
	public float Volume 
	{
		set { _volume = value; AVProWindowsMediaPlugin.SetVolume(_movieHandle, _volume); }
		get { return _volume; }
	}
	
	public float PlaybackRate 
	{
		set { AVProWindowsMediaPlugin.SetPlaybackRate(_movieHandle, value); }
		get { return AVProWindowsMediaPlugin.GetPlaybackRate(_movieHandle); }
	}
	
	public float PositionSeconds
	{
		get { return AVProWindowsMediaPlugin.GetCurrentPositionSeconds(_movieHandle); }
		set { AVProWindowsMediaPlugin.SeekSeconds(_movieHandle, value); }
	}

	public uint PositionFrames
	{
		get { return (uint)DisplayFrame; } //return AVProWindowsMediaPlugin.GetCurrentPositionFrames(_movieHandle); }
		set { AVProWindowsMediaPlugin.SeekFrames(_movieHandle, value); }
	}
	
	public float AudioBalance
	{
		get { return AVProWindowsMediaPlugin.GetAudioBalance(_movieHandle); }
		set { AVProWindowsMediaPlugin.SetAudioBalance(_movieHandle, value); }
	}
	
	public bool IsFinishedPlaying 
	{
        get { return AVProWindowsMediaPlugin.IsFinishedPlaying(_movieHandle); }
	}
	
	// Display

	public bool RequiresFlipY
	{
		get;
		private set;
	}
	
	public Texture OutputTexture
	{
		get { if (_formatConverter != null && _formatConverter.ValidPicture) return _formatConverter.OutputTexture; return null;}
	}
	
	public int DisplayFrame
	{
		get { if (_formatConverter != null && _formatConverter.ValidPicture) return _formatConverter.DisplayFrame; return -1; }
	}
	
	//-------------------------------------------------------------------------

	public bool StartVideo(string filename, bool allowNativeFormat, bool useBT709, bool allowAudio, bool useAudioDelay, bool useAudioMixer, bool useDisplaySync, bool ignoreFlips, FilterMode textureFilterMode, TextureWrapMode textureWrapMode)
	{
		Filename = filename;
		if (!string.IsNullOrEmpty(Filename))
		{
            if (System.IO.File.Exists(filename))
            {
                if (_movieHandle < 0)
                {
                    _movieHandle = AVProWindowsMediaPlugin.GetInstanceHandle();
                }

                // Note: we're marshaling the string to IntPtr as the pointer of type wchar_t 
                System.IntPtr filenamePtr = Marshal.StringToHGlobalUni(Filename);
                if (AVProWindowsMediaPlugin.LoadMovie(_movieHandle, filenamePtr, false, allowNativeFormat, allowAudio, useAudioDelay, useAudioMixer, useDisplaySync))
                {
                    CompleteVideoLoad(useBT709, ignoreFlips, textureFilterMode, textureWrapMode);
                }
                else
                {
                    Debug.LogError("[AVProWindowsMedia] Movie failed to load - do you have the required codecs installed?  See documentation for details.");
					if (filename.ToLower().EndsWith(".mp4"))
					{
						Debug.LogError("[AVProWindowsMedia] For MP4 files you need an MP4 splitter such as Haali Media Splitter or GDCL.");
						Debug.LogError("[AVProWindowsMedia] For HIGH profile H.264 videos you need to install an external H.264 decoder.  See documentation for details.");
					}
                    Close();
                }
                Marshal.FreeHGlobal(filenamePtr);
            }
            else
            {
                Debug.LogError("[AVProWindowsMedia] File not found " + filename);
                Close();
            }
		}
		else
		{
			Debug.LogError("[AVProWindowsMedia] No movie file specified");
			Close();
		}
		
		return _movieHandle >= 0;
	}
	

	public bool StartVideoFromMemory(string name, System.IntPtr moviePointer, long movieLength, bool allowNativeFormat, bool useBT709, bool allowAudio, bool useAudioDelay, bool useAudioMixer, bool useDisplaySync, bool ignoreFlips, FilterMode textureFilterMode, TextureWrapMode textureWrapMode)
	{
		Filename = name;
		if (moviePointer != System.IntPtr.Zero && movieLength > 0)
		{
			if (_movieHandle < 0)
			{
				_movieHandle = AVProWindowsMediaPlugin.GetInstanceHandle();
			}
						
			if (AVProWindowsMediaPlugin.LoadMovieFromMemory(_movieHandle, moviePointer, movieLength, allowNativeFormat, allowAudio, useAudioDelay, useAudioMixer, useDisplaySync))
			{
				CompleteVideoLoad(useBT709, ignoreFlips, textureFilterMode, textureWrapMode);
			}
			else
			{
				Debug.LogWarning("[AVProWindowsMedia] Movie failed to load");
				Close();
			}
		}
		else
		{
			Debug.LogWarning("[AVProWindowsMedia] No movie file specified");
			Close();
		}
		
		return _movieHandle >= 0;
	}

	public Material GetConversionMaterial()
	{
		Material result = null;
		if (_formatConverter != null)
		{
			result = _formatConverter.GetConversionMaterial();
		}
		return result;
	}
	
	private void CompleteVideoLoad(bool useBT709, bool ignoreFlips, FilterMode textureFilterMode, TextureWrapMode textureWrapMode)
	{
		RequiresFlipY = false;
		Loop = false;
		Volume = _volume;
		
		// Gather properties
		Width = AVProWindowsMediaPlugin.GetWidth(_movieHandle);
		Height = AVProWindowsMediaPlugin.GetHeight(_movieHandle);
		FrameRate = AVProWindowsMediaPlugin.GetFrameRate(_movieHandle);
		DurationSeconds = AVProWindowsMediaPlugin.GetDurationSeconds(_movieHandle);
		DurationFrames = AVProWindowsMediaPlugin.GetDurationFrames(_movieHandle);

		AVProWindowsMediaPlugin.VideoFrameFormat sourceFormat = (AVProWindowsMediaPlugin.VideoFrameFormat)AVProWindowsMediaPlugin.GetFormat(_movieHandle);

		if (AVProWindowsMediaManager.Instance._logVideoLoads)
		{
			Debug.Log(string.Format("[AVProWindowsMedia] Loaded video '{0}' ({1}x{2} @ {3} fps) {4} frames, {5} seconds - format: {6}", Filename, Width, Height, FrameRate.ToString("F2"), DurationFrames, DurationSeconds.ToString("F2"), sourceFormat.ToString()));
		}

		// Create format converter
		bool hasVideo = (Width > 0 && Width <= 8192 || Height > 0 && Height <= 8192);
		if (!hasVideo)
		{
			Width = Height = 0;
			if (_formatConverter != null)
			{
				_formatConverter.Dispose();
				_formatConverter = null;
			}
		}
		else
		{
			bool isTopDown = AVProWindowsMediaPlugin.IsOrientedTopDown(_movieHandle);
								
			if (_formatConverter == null)
			{
				_formatConverter = new AVProWindowsMediaFormatConverter();
			}

			bool flipX = false;
			bool flipY = isTopDown;
			if (ignoreFlips)
			{
				if (flipY)
				{
					RequiresFlipY = true;
				}

				flipX = flipY = false;
			}
			if (!_formatConverter.Build(_movieHandle, Width, Height, sourceFormat, useBT709, flipX, flipY, textureFilterMode, textureWrapMode))
			{
				Debug.LogError("[AVProWindowsMedia] unable to convert video format");
				Width = Height = 0;
				if (_formatConverter != null)
				{
					_formatConverter.Dispose();
					_formatConverter = null;
                    Close();
				}
			}
		}
		
		PreRoll();		
	}
	
	public bool StartAudio(string filename)
	{
		Filename = filename;
		Width = Height = 0;
		if (!string.IsNullOrEmpty(Filename))
		{
			if (_movieHandle < 0)
			{
				_movieHandle = AVProWindowsMediaPlugin.GetInstanceHandle();
			}
			
			if (_formatConverter != null)
			{
				_formatConverter.Dispose();
				_formatConverter = null;
			}

			// Note: we're marshaling the string to IntPtr as the pointer of type wchar_t 
			System.IntPtr filenamePtr = Marshal.StringToHGlobalUni(Filename);
			if (AVProWindowsMediaPlugin.LoadMovie(_movieHandle, filenamePtr, false, false, true, false, false, false))
			{
				Volume = _volume;
				DurationSeconds = AVProWindowsMediaPlugin.GetDurationSeconds(_movieHandle);
				
				Debug.Log("[AVProWindowsMedia] Loaded audio " + Filename + " " + DurationSeconds.ToString("F2") + " sec");
			}
			else
			{
                Debug.LogError("[AVProWindowsMedia] Movie failed to load");
				Close();
			}
			Marshal.FreeHGlobal(filenamePtr);
		}
		else
		{
			Debug.LogError("[AVProWindowsMedia] No movie file specified");
			Close();			
		}
		
		return _movieHandle >= 0;
	}	
	
	private void PreRoll()
	{
		if (_movieHandle < 0)
			return;
		
		return;
		
		float vol = Volume;
		Volume = 0.0f;
		Play();
		Pause();
		AVProWindowsMediaPlugin.SeekFrames(_movieHandle, 0);
		Volume = vol;
	}
	
	public bool Update(bool force)
	{
		bool updated = false;
		if (_movieHandle >= 0)
		{
			AVProWindowsMediaPlugin.Update(_movieHandle);
			if (_formatConverter != null)
			{
				updated = _formatConverter.Update();
#if AVPROWINDOWSMEDIA_FPS
				if (updated)
				{
					UpdateFPS();
				}
#endif
			}
		}
		return updated;
	}

#if AVPROWINDOWSMEDIA_FPS
	protected void ResetFPS()
	{
		_frameCount = 0;
		FramesTotal = 0;
		DisplayFPS = 0.0f;
		_startFrameTime = 0.0f;
	}
	
	public void UpdateFPS()
	{
		_frameCount++;
		FramesTotal++;
		
		float timeNow = Time.realtimeSinceStartup;
		float timeDelta = timeNow - _startFrameTime;
		if (timeDelta >= 1.0f)
		{
			DisplayFPS = (float)_frameCount / timeDelta;
			_frameCount  = 0;
			_startFrameTime = timeNow;
		}
	}	
#endif

	public void Play()
	{
		if (_movieHandle >= 0)
		{
			AVProWindowsMediaPlugin.Play(_movieHandle);
			IsPlaying = true;
		}
	}
	
	public void Pause()
	{
		if (_movieHandle >= 0)
		{
			AVProWindowsMediaPlugin.Pause(_movieHandle);
			IsPlaying = false;
		}
	}
	
	public void Rewind()
	{
		if (_movieHandle >= 0)
		{
			PositionSeconds = 0.0f;
		}
	}
	
	public void Dispose()
	{
		Close();
		if (_formatConverter != null)
		{
			_formatConverter.Dispose();
			_formatConverter = null;
		}
	}

	public void SetFrameRange(int min, int max)
	{
		AVProWindowsMediaPlugin.SetDisplayFrameRange(_movieHandle, min, max);
	}

	public void ClearFrameRange()
	{
		SetFrameRange(-1, -1);
	}

	private void Close()
	{
#if AVPROWINDOWSMEDIA_FPS
		ResetFPS();
#endif

		Width = Height = 0;
		
		if (_movieHandle >= 0)
		{
			Pause();
			AVProWindowsMediaPlugin.Stop(_movieHandle);
			AVProWindowsMediaPlugin.FreeInstanceHandle(_movieHandle);
			_movieHandle = -1;
		}
	}
}