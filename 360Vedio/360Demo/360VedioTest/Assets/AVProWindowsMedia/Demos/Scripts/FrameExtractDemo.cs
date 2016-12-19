using UnityEngine;
using System.Collections;
using System.IO;

public class FrameExtractDemo : MonoBehaviour 
{	
	public string _folder;
	public string _filename;
	public bool _useStreamingAssetsPath;
	public GUISkin _guiSkin;
	public bool _async = true;
	private static GUIStyle _gridStyle;
	
	private AVProWindowsMedia _movie;
	private GUIContent[] _contents;
	private Texture2D[] _textures;
	private bool _isExtracting;
	private int _textureIndex;
	private uint _targetFrame;
	private uint _frameStep;
	
	private void DestroyTextures()
	{
		if (_textures != null)
		{
			for (int i = 0; i < _textures.Length; i++)
			{
				if (_textures[i])
				{
					Texture2D.Destroy(_textures[i]);
					_textures[i] = null;
				}
			}
		}
	}
	
	private bool StartExtractFrames(string filePath, uint numSamples)
	{
		DestroyTextures();
		
		if (_movie.StartVideo(filePath, true, true, false, false, false, false, false, FilterMode.Bilinear, TextureWrapMode.Clamp))
		{
			_textures = new Texture2D[numSamples];
			_contents = new GUIContent[numSamples];
			for (int i = 0; i < numSamples; i++)
			{
				_contents[i] = new GUIContent(" ");
			}
			
			uint numFrames = _movie.DurationFrames;
			_frameStep = numFrames / numSamples;
			_targetFrame = 0;
			_textureIndex = 0;
			
			if (!_async)
			{
				_isExtracting = true;
				while (_isExtracting)
				{
#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1
					GL.IssuePluginEvent(AVProWindowsMediaPlugin.GetRenderEventFunc(), (int)AVProWindowsMediaPlugin.PluginEvent.UpdateAllTextures);
#else
					GL.IssuePluginEvent(AVProWindowsMediaPlugin.PluginID | (int)AVProWindowsMediaPlugin.PluginEvent.UpdateAllTextures);
#endif

					UpdateExtracting();
				}
				
				return false;
			}
			
			return true;
		}
		
		return false;
	}
	
	void Start()
	{
		_movie = new AVProWindowsMedia();
	}
	
	void Update()
	{
		if (_isExtracting)
			UpdateExtracting();
	}
	
	private Texture2D CopyRenderTexture(RenderTexture rt)
	{
		RenderTexture prevRT = RenderTexture.active;
		RenderTexture.active = rt;

		Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
		texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		texture.Apply(false, false);

		RenderTexture.active = prevRT;
		
		return texture;
	}
	
	private void UpdateExtracting()
	{		
		_movie.Update(false);
		if (_movie.DisplayFrame == _targetFrame)
		{
			if (_textureIndex < _textures.Length)
			{			
				Texture2D texture = CopyRenderTexture((RenderTexture)_movie.OutputTexture);	
				texture.Apply(false, false);
				_contents[_textureIndex] = new GUIContent("Frame " + _targetFrame.ToString(), texture);
				_textures[_textureIndex++] = texture;
			}
			
			NextFrame();
		}		
	}
	
	private void NextFrame()
	{
		_targetFrame += _frameStep;
		if (_targetFrame < _movie.DurationFrames)
		{
			// Seek to frame
			_movie.PositionFrames = _targetFrame;
		}
		else
		{
			_isExtracting = false;
		}
	}
	
	void OnDestroy()
	{
		DestroyTextures();
		if (_movie != null)
		{
			_movie.Dispose();
			_movie = null;
		}
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

	void OnGUI()
	{
		GUI.skin = _guiSkin;
		
		if (_gridStyle == null)
		{
			_gridStyle = GUI.skin.GetStyle("ExtractFrameGrid");
		}
		
		GUI.enabled = !_isExtracting;
		
		GUILayout.BeginVertical(GUILayout.Width(Screen.width));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Folder: ", GUILayout.Width(80));
		_folder = GUILayout.TextField(_folder, 192, GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("File: ", GUILayout.Width(80));
		_filename = GUILayout.TextField(_filename, 128, GUILayout.MinWidth(440), GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Extract Frames", GUILayout.ExpandWidth(true)))
		{
			_isExtracting = StartExtractFrames(GetFilePath(), 24);
		}
		
		_async = GUILayout.Toggle(_async, "ASync");
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		
		GUI.enabled = true;
		
		if (_textures != null)
		{
			if (_gridStyle != null)
				GUILayout.SelectionGrid(-1, _contents, 6, _gridStyle, GUILayout.Height(Screen.height-96));
			else
				GUILayout.SelectionGrid(-1, _contents, 6, GUILayout.Height(Screen.height-96));
		}
	}
}