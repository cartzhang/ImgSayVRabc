#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
#define UNITY_FEATURE_UGUI
#endif

using UnityEngine;
#if UNITY_FEATURE_UGUI
using UnityEngine.UI;
using System.Collections;
using RenderHeads.Media.AVProVideo;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProWindowsMedia.Demos
{
	public class VcrDemo : MonoBehaviour 
	{
		public AVProWindowsMediaMovie _movie;

		public Slider		_videoSeekSlider;
		private float		_setVideoSeekSliderValue;
		private bool		_wasPlayingOnScrub;

		public Slider		_audioVolumeSlider;
		private float		_setAudioVolumeSliderValue;

		public void OnPlayButton()
		{
			if( _movie )
			{
				_movie.Play();
				SetButtonEnabled( "PlayButton", false );
				SetButtonEnabled( "PauseButton", true );
			}
		}
		public void OnPauseButton()
		{
			if( _movie )
			{
				_movie.Pause();
				SetButtonEnabled( "PauseButton", false );
				SetButtonEnabled( "PlayButton", true );
			}
		}

		public void OnVideoSeekSlider()
		{
			if (_movie && _videoSeekSlider && _videoSeekSlider.value != _setVideoSeekSliderValue)
			{
				_movie.MovieInstance.PositionSeconds = (_videoSeekSlider.value * _movie.MovieInstance.DurationSeconds);
			}
		}
		public void OnVideoSliderDown()
		{
			if( _movie )
			{
				_wasPlayingOnScrub = _movie.MovieInstance.IsPlaying;
				if( _wasPlayingOnScrub )
				{
					_movie.Pause();
					SetButtonEnabled( "PauseButton", false );
					SetButtonEnabled( "PlayButton", true );
				}
				OnVideoSeekSlider();
			}
		}
		public void OnVideoSliderUp()
		{
			if( _movie && _wasPlayingOnScrub )
			{
				_movie.Play();
				_wasPlayingOnScrub = false;

				SetButtonEnabled( "PlayButton", false );
				SetButtonEnabled( "PauseButton", true );
			}			
		}

		public void OnAudioVolumeSlider()
		{
			if (_movie && _audioVolumeSlider && _audioVolumeSlider.value != _setAudioVolumeSliderValue)
			{
				_movie._volume = _audioVolumeSlider.value;
			}
		}

		public void OnRewindButton()
		{
			if( _movie )
			{
				_movie.MovieInstance.Rewind();
			}
		}

		void Start()
		{
			if( _movie )
			{
				if( _audioVolumeSlider )
				{
					// Volume
					float volume = _movie._volume;
					_setAudioVolumeSliderValue = volume;
					_audioVolumeSlider.value = volume;
				}

				SetButtonEnabled( "PlayButton", false );
				SetButtonEnabled( "PauseButton", true );
			}
		}

		void Update()
		{
			if (_movie && _movie.MovieInstance != null)
			{
				float time = _movie.MovieInstance.PositionSeconds;
				float d = time / _movie.MovieInstance.DurationSeconds;
				_setVideoSeekSliderValue = d;
				_videoSeekSlider.value = d;
			}
		}

		private void SetButtonEnabled( string objectName, bool bEnabled )
		{
			Button button = GameObject.Find( objectName ).GetComponent<Button>();
			if( button )
			{
				button.enabled = bEnabled;
				button.GetComponentInChildren<CanvasRenderer>().SetAlpha( bEnabled ? 1.0f : 0.4f );
//				button.GetComponentInChildren<Text>().color = Color.clear;
			}
		}
	}
}
#endif