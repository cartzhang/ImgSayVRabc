using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

public class AVProWindowsMediaControlPlayOnEnable : MonoBehaviour
{
	public AVProWindowsMediaMovie _movie;
	public bool _rewindOnDisable;
	public int _minFrame = -1;
	public int _maxFrame = -1;
	public bool _loop;

	public bool _enableLoopWhenInRange;

	// TODO: video sequencer
	// play until frame 50
	// wait
	// play range 30-40 looped, with pause of 5 seconds
	// pause

	/*
	void Update()
	{
		if (_loop)
		{
			if (_movie.MovieInstance != null)
			{
				if (_movie.MovieInstance.DisplayFrame >= _maxFrame)
				{
					_movie.MovieInstance.PositionFrames = (uint)_minFrame;
				}
			}
		}
	}*/

	void OnEnable()
	{
		if (!_enableLoopWhenInRange)
		{
			if (_movie.MovieInstance != null)
			{
				_movie.MovieInstance.SetFrameRange(_minFrame, _maxFrame);
			}
		}
		else
		{
			_movie._loop = true;
		}
		_movie.Play();
	}

	void Update()
	{
		if (_enableLoopWhenInRange)
		{
			if (_movie.MovieInstance != null && _movie.MovieInstance.IsPlaying)
			{
				if (!_loop)
				{
					if (_movie.MovieInstance.DisplayFrame >= _minFrame)
					{
						_movie.MovieInstance.SetFrameRange(_minFrame, _maxFrame);
						_loop = true;
					}
				}
				else
				{
					if (_movie.MovieInstance.DisplayFrame >= _maxFrame)
					{
						_movie.MovieInstance.PositionFrames = (uint)_minFrame+1;
					}
				}
			}
		}
	}

	void OnDisable()
	{
		if (_enableLoopWhenInRange)
		{
			_loop = false;
            if (_movie.MovieInstance != null)
            {
                _movie.MovieInstance.SetFrameRange(-1, -1);
            }
		}

		if (_rewindOnDisable)
		{
			if (_movie.MovieInstance != null)
			{
				if (_movie.MovieInstance.IsPlaying)
					_movie.Pause();
				if (_movie.MovieInstance.DisplayFrame > 1)
					_movie.MovieInstance.Rewind();
				
				//AVProWindowsMediaPlugin.FlushFrameBuffers(_movie.MovieInstance.Handle);
			}
		}
	}
}
