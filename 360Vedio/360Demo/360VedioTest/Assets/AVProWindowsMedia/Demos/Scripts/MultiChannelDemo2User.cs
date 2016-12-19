using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiChannelDemo2User : MonoBehaviour 
{
	public GUISkin _guiSkin;
	public MultiChannelDemo2 _parent;
	private AVProWindowsMediaMovie _movie;
	private Rect windowRect = new Rect(0, 0, 320, 220);
	private float[] _audioMatrixValues;
	
	private Vector2 Position
	{
		get { return new Vector2((windowRect.x + (windowRect.width / 2)) / Screen.width, windowRect.y / Screen.height); }
	}
	
	void Start()
	{
		_audioMatrixValues = new float[_parent.NumChannels];
		windowRect = new Rect(Screen.width / 2, Screen.height / 2, windowRect.width, windowRect.height);
		_movie = this.gameObject.AddComponent<AVProWindowsMediaMovie>();
		_movie._useAudioMixer = true;
		_movie._loadOnStart = false;
		_movie._playOnStart = false;
	}
	
	void Update()
	{
		if (_movie.MovieInstance != null)
		{
			// Generate audio matrix values
			_parent.UpdateAudioMatrix(Position, ref _audioMatrixValues);
		
			// Apply matrix values to movie instance
			/*for (int i = 0; i < _audioMatrixValues.Length; i++)
				Debug.Log("v " + i + " " + _audioMatrixValues[i]);
			Debug.Log("apply to " + _movie.MovieInstance.Handle);*/
			AVProWindowsMediaPlugin.SetAudioChannelMatrix(_movie.MovieInstance.Handle, _audioMatrixValues, _audioMatrixValues.Length);
		}
	}

	void OnGUI()
	{	
		GUI.skin = _guiSkin;
		
		windowRect = GUI.Window(this.name.GetHashCode(), windowRect, DoMyWindow, this.name);
		//GUI.DrawTexture(new Rect(Position.x * Screen.width - (_target.width / 2), Position.y * Screen.height - (_target.height / 2), _target.width, _target.height), _target);
		
		_parent.DrawFalloff(new Vector2(Position.x * Screen.width, (1.0f-Position.y) * Screen.height));
	}
	
	
    void DoMyWindow(int windowID)
	{	
		if (GUILayout.Button("Play Video"))
		{
			_movie._folder = "";
			_movie._filename = "sample-1920x1024-divx.avi";
			_movie._useStreamingAssetsPath = true;
			_movie._volume = 1.0f;
			_movie._loop = true;
			if (_movie.LoadMovie(false))
			{
				_movie.Play();
			}
		}
		
		if (_movie.OutputTexture)
		{
			Rect r = GUILayoutUtility.GetRect(320, 180);
			GUI.DrawTexture(r, _movie.OutputTexture, ScaleMode.ScaleToFit);
		}
		
        GUI.DragWindow();
    }
}
