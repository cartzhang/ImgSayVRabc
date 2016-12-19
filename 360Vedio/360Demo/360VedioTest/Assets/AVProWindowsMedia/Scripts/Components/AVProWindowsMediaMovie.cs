using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[System.Serializable]
public class AVProWindowsMediaMovieClip
{
	public string name;
	public int inPoint = -1;
	public int outPoint = -1;

    public AVProWindowsMediaMovieClip(string name, int inPoint, int outPoint)
    {
        this.name = name;
        this.inPoint = inPoint;
        this.outPoint = outPoint;
    }
}

[AddComponentMenu("AVPro Windows Media/Movie")]
[ExecuteInEditMode]
public class AVProWindowsMediaMovie : MonoBehaviour
{
	protected AVProWindowsMedia _moviePlayer;
	public string _folder = "./";
	public string _filename = "movie.avi";
	public bool _useStreamingAssetsPath;
	public bool _loop = false;
	public ColourFormat _colourFormat = ColourFormat.YCbCr_HD;
	public bool _allowAudio = true;
	public bool _useAudioDelay = false;
	public bool _useAudioMixer = false;
	public bool _useDisplaySync = false;
	public bool _loadOnStart = true;
	public bool _playOnStart = true;
	public bool _editorPreview = false;
	public bool _ignoreFlips = true;
	public float _volume = 1.0f;
	public float _audioBalance = 0.0f;

	public FilterMode _textureFilterMode = FilterMode.Bilinear;
	public TextureWrapMode _textureWrapMode = TextureWrapMode.Clamp;

    [SerializeField]
    private List<AVProWindowsMediaMovieClip> _clips;
    private Dictionary<string, AVProWindowsMediaMovieClip> _clipLookup = new Dictionary<string, AVProWindowsMediaMovieClip>();
    private AVProWindowsMediaMovieClip _currentClip;

	public enum ColourFormat
	{
		RGBA32,
		YCbCr_SD,
		YCbCr_HD,
	}
	
	public Texture OutputTexture  
	{
		get { if (_moviePlayer != null) return _moviePlayer.OutputTexture; return null; }
	}
	
	public AVProWindowsMedia MovieInstance
	{
		get { return _moviePlayer; }
	}

	public virtual void Start()
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
			LoadMovie(_playOnStart);
		}
	}
	
	public virtual bool LoadMovie(bool autoPlay)
	{
		bool result = true;
		
		if (_moviePlayer == null)
			_moviePlayer = new AVProWindowsMedia();

        LoadClips();
		
		bool allowNativeFormat = (_colourFormat != ColourFormat.RGBA32);

        //string filePath = GetFilePath();
        string filePath = ConfigFile.Cconfig.VediofullPathName;

        if (_moviePlayer.StartVideo(filePath, allowNativeFormat, _colourFormat == ColourFormat.YCbCr_HD, _allowAudio, _useAudioDelay, _useAudioMixer, _useDisplaySync, _ignoreFlips, _textureFilterMode, _textureWrapMode))
		{
			if (_allowAudio)
			{
				_moviePlayer.Volume = _volume;
				_moviePlayer.AudioBalance = _audioBalance;
			}
			_moviePlayer.Loop = _loop;
			if (autoPlay)
			{
				_moviePlayer.Play();
			}
		}
		else
		{
			Debug.LogWarning("[AVProWindowsMedia] Couldn't load movie " + _filename);
			UnloadMovie();
			result = false;
		}
		
		return result;
	}
	
	public bool LoadMovieFromMemory(bool autoPlay, string name, System.IntPtr moviePointer, long movieLength, FilterMode textureFilterMode, TextureWrapMode textureWrapMode)
	{
		bool result = true;
		
		if (_moviePlayer == null)
			_moviePlayer = new AVProWindowsMedia();
		
		bool allowNativeFormat = (_colourFormat != ColourFormat.RGBA32);
		
		if (_moviePlayer.StartVideoFromMemory(name, moviePointer, movieLength, allowNativeFormat, _colourFormat == ColourFormat.YCbCr_HD, _allowAudio, _useAudioDelay, _useAudioMixer, _useDisplaySync, _ignoreFlips, textureFilterMode, textureWrapMode))
		{
			if (_allowAudio)
			{
				_moviePlayer.Volume = _volume;
				_moviePlayer.AudioBalance = _audioBalance;
			}
			_moviePlayer.Loop = _loop;
			if (autoPlay)
			{
				_moviePlayer.Play();
			}
		}
		else
		{
			Debug.LogWarning("[AVProWindowsMedia] Couldn't load movie " + _filename);
			UnloadMovie();
			result = false;
		}
		
		return result;
	}
	
	public void Update()
	{
		if (_moviePlayer != null)
		{
			_volume = Mathf.Clamp01(_volume);
			if (_allowAudio)
			{
				if (_volume != _moviePlayer.Volume)
					_moviePlayer.Volume = _volume;
				if (_audioBalance != _moviePlayer.AudioBalance)
					_moviePlayer.AudioBalance = _audioBalance;
			}
			
			if (_loop != _moviePlayer.Loop)
				_moviePlayer.Loop = _loop;

			_moviePlayer.Update(false);
			
			// When the movie finishes playing, send a message so it can be handled
			if (!_moviePlayer.Loop && _moviePlayer.IsPlaying && _moviePlayer.IsFinishedPlaying)
			{
				_moviePlayer.Pause();
				this.SendMessage("MovieFinished", this, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void Play()
	{
		if (_moviePlayer != null)
			_moviePlayer.Play();
	}
	
	public void Pause()
	{
		if (_moviePlayer != null)
			_moviePlayer.Pause();
	}

    public int NumClips
    {
        get { if (_clips != null) return _clips.Count; return 0; }
    }

    public string GetClipName(int index)
    {
        string result = string.Empty;
        if (_clips != null && index >= 0 && index < _clips.Count)
            result = _clips[index].name;
        return result;
    }

    public void ClearClips()
    {
        _currentClip = null;
        _clips.Clear();
        _clipLookup.Clear();
    }

    public void AddClip(string name, int inPoint, int outPoint)
    {
        AVProWindowsMediaMovieClip clip = new AVProWindowsMediaMovieClip(name, inPoint, outPoint);
        _clips.Add(clip);
        _clipLookup.Add(name, clip);
    }

    public string GetCurrentClipName()
    {
        string result = string.Empty;
        if (_currentClip != null)
            result = _currentClip.name;
        return result;
    }

    public void LoadClips()
    {
        _clipLookup.Clear();
        if (_clips != null && _clips.Count > 0)
        {
            for (int i = 0; i < _clips.Count; i++)
            {
				if (!string.IsNullOrEmpty(_clips[i].name))
				{
	                _clipLookup.Add(_clips[i].name, _clips[i]);
				}
            }
        }
    }
    public void ResetClip()
    {
        _currentClip = null;
        MovieInstance.SetFrameRange(-1, -1);
    }

	public void PlayClip(string name, bool loop, bool startPaused)
	{
		if (MovieInstance == null)
			throw new System.Exception("Movie instance is null");
        if (!_clipLookup.ContainsKey(name))
			throw new System.Exception("Frame range key not found");
		
        MovieInstance.Loop = loop;
        _currentClip = _clipLookup[name];
        MovieInstance.SetFrameRange(_currentClip.inPoint, _currentClip.outPoint);
        MovieInstance.PositionFrames = (uint)_currentClip.inPoint;
        if (!startPaused)
            MovieInstance.Play();
        else
            MovieInstance.Pause();
	}
	
	public virtual void UnloadMovie()
	{
		if (_moviePlayer != null)
		{
			_moviePlayer.Dispose();
			_moviePlayer = null;
		}
	}

	public void OnDestroy()
	{
		UnloadMovie();
	}

	public string GetFilePath()
	{
		string filePath = Path.Combine(_folder, _filename);
		
		if (_useStreamingAssetsPath)
		{
			filePath = Path.Combine(Application.streamingAssetsPath, filePath);
		}
		// If we're running outside of the editor we may need to resolve the relative path
		// as the working-directory may not be that of the application EXE.
		else if (!Application.isEditor && !Path.IsPathRooted(filePath))
		{
			string rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
			filePath = Path.Combine(rootPath, filePath);
		}

		return filePath;
	}

#if UNITY_EDITOR && !UNITY_WEBPLAYER
	[ContextMenu("Save PNG")]
	private void SavePNG()
	{
		if (OutputTexture != null && _moviePlayer != null)
		{
			Texture2D tex = new Texture2D(OutputTexture.width, OutputTexture.height, TextureFormat.ARGB32, false);
			RenderTexture.active = (RenderTexture)OutputTexture;
			tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
			tex.Apply(false, false);
			
			byte[] pngBytes = tex.EncodeToPNG();
			System.IO.File.WriteAllBytes("AVProWindowsMedia-image" + Random.Range(0, 65536).ToString("X") + ".png", pngBytes);
			
			RenderTexture.active = null;
			Texture2D.Destroy(tex);
			tex = null;
		}
	}
#endif
}