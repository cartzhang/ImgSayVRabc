using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

public class AVProWindowsMediaPlugin
{
	public enum VideoFrameFormat
	{
		RAW_BGRA32,
		YUV_422_YUY2,
		YUV_422_UYVY,
		YUV_422_YVYU,
		YUV_422_HDYC,
		YUV_420_NV12=7,

		Hap_RGB=9,			// Standard quality 24-bit RGB, using DXT1 compression
		Hap_RGBA,			// Standard quality 32-bit RGBA, using DXT5 compression		
		Hap_RGB_HQ,			// High quality 24-bit, using DXT5 compression with YCoCg color-space trick
	}
	
	// Used by GL.IssuePluginEvent
	public const int PluginID = 0xFA10000;
	public enum PluginEvent
	{
		UpdateAllTextures = 0,
	}

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1
	[DllImport("AVProWindowsMedia")]
	public static extern System.IntPtr GetRenderEventFunc();
#endif
	
	//////////////////////////////////////////////////////////////////////////
	// Initialisation
	
	[DllImport("AVProWindowsMedia")]
	public static extern bool Init();

	[DllImport("AVProWindowsMedia")]
	public static extern void Deinit();

	[DllImport("AVProWindowsMedia")]
	public static extern void SetUnityFeatures(bool supportExternalTextures);


	[DllImport("AVProWindowsMedia")]
	public static extern float GetPluginVersion();
	
	// Open and Close handle

	[DllImport("AVProWindowsMedia")]
	public static extern int GetInstanceHandle();

	[DllImport("AVProWindowsMedia")]
	public static extern void FreeInstanceHandle(int handle);
	
	//////////////////////////////////////////////////////////////////////////
	// Loading

	[DllImport("AVProWindowsMedia")]
	public static extern bool LoadMovie(int handle, System.IntPtr filename, bool playFromMemory, bool allowNativeFormat, bool allowAudio, bool useAudioDelay, bool useAudioMixer, bool useDisplaySync);

	[DllImport("AVProWindowsMedia")]
	public static extern bool LoadMovieFromMemory(int handle, System.IntPtr moviePointer, long movieLength, bool allowNativeFormat, bool allowAudio, bool useAudioDelay, bool useAudioMixer, bool useDisplaySync);

	
	//////////////////////////////////////////////////////////////////////////
	// Get Properties

	[DllImport("AVProWindowsMedia")]
	public static extern int GetWidth(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetHeight(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern float GetFrameRate(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern long GetFrameDuration(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetFormat(int handle);
	
	[DllImport("AVProWindowsMedia")]
	public static extern float GetDurationSeconds(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern uint GetDurationFrames(int handle);
	
	[DllImport("AVProWindowsMedia")]
	public static extern bool IsOrientedTopDown(int handle);
	
	//////////////////////////////////////////////////////////////////////////
	// Playback
	
	[DllImport("AVProWindowsMedia")]
	public static extern void Play(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void Pause(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void Stop(int handle);
	
	//////////////////////////////////////////////////////////////////////////
	// Seeking & Position
	
	[DllImport("AVProWindowsMedia")]
	public static extern void SeekUnit(int handle, float position);

	[DllImport("AVProWindowsMedia")]
	public static extern void SeekSeconds(int handle, float position);

	[DllImport("AVProWindowsMedia")]
	public static extern void SeekFrames(int handle, uint position);
	
	[DllImport("AVProWindowsMedia")]
	public static extern float GetCurrentPositionSeconds(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern uint GetCurrentPositionFrames(int handle);
	
	//////////////////////////////////////////////////////////////////////////
	// Get Current State
	
	[DllImport("AVProWindowsMedia")]
	public static extern bool IsLooping(int handle);
	
	[DllImport("AVProWindowsMedia")]
	public static extern float GetPlaybackRate(int handle);
	
	[DllImport("AVProWindowsMedia")]
	public static extern float GetAudioBalance(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool IsFinishedPlaying(int handle);
	
	//////////////////////////////////////////////////////////////////////////
	// Set Current State

	[DllImport("AVProWindowsMedia")]
	public static extern void SetVolume(int handle, float volume);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetLooping(int handle, bool loop);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetDisplayFrameRange(int handle, int min, int max);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetPlaybackRate(int handle, float rate);
	
	[DllImport("AVProWindowsMedia")]
	public static extern void SetAudioBalance(int handle, float balance);
	
	[DllImport("AVProWindowsMedia")]
	public static extern void SetAudioChannelMatrix(int handle, float[] values, int numValues);

	[DllImport("AVProWindowsMedia")]
	public static extern void SetAudioDelay(int handle, int ms);
	
	//////////////////////////////////////////////////////////////////////////
	// Update
	
	[DllImport("AVProWindowsMedia")]
	public static extern bool Update(int handle);
		
	//////////////////////////////////////////////////////////////////////////
	// Frame Update
	
	[DllImport("AVProWindowsMedia")]
	public static extern bool IsNextFrameReadyForGrab(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetLastFrameUploaded(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern bool UpdateTextureGL(int handle, int textureID, ref int frameNumber);

	[DllImport("AVProWindowsMedia")]
	public static extern bool GetFramePixels(int handle, System.IntPtr data, int bufferWidth, int bufferHeight, ref int frameNumber);
	
	[DllImport("AVProWindowsMedia")]
	public static extern bool SetTexturePointer(int handle, System.IntPtr data);

	[DllImport("AVProWindowsMedia")]
	public static extern System.IntPtr GetTexturePointer(int handle);

	//////////////////////////////////////////////////////////////////////////
	// Live Stats

	[DllImport("AVProWindowsMedia")]
	public static extern float GetCaptureFrameRate(int handle);
	
	//////////////////////////////////////////////////////////////////////////
	// Internal Frame Buffering

	[DllImport("AVProWindowsMedia")]
	public static extern void SetFrameBufferSize(int handle, int read, int write);
	
	[DllImport("AVProWindowsMedia")]
	public static extern long GetLastFrameBufferedTime(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern System.IntPtr GetLastFrameBuffered(int handle);
	
	[DllImport("AVProWindowsMedia")]
	public static extern System.IntPtr GetFrameFromBufferAtTime(int handle, long time);

	//////////////////////////////////////////////////////////////////////////
	// Debugging

	[DllImport("AVProWindowsMedia")]
	public static extern int GetNumFrameBuffers(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern void GetFrameBufferTimes(int handle, System.IntPtr dest, int destSizeBytes);

	[DllImport("AVProWindowsMedia")]
	public static extern void FlushFrameBuffers(int handle);

	[DllImport("AVProWindowsMedia")]
	public static extern int GetLastBufferUploaded(int handle);
	[DllImport("AVProWindowsMedia")]
	public static extern int GetReadWriteBufferDistance(int handle);

	//-----------------------------------------------------------------------------
}