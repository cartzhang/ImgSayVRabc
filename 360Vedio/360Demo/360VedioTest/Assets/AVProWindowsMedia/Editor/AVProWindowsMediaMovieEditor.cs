// Support for Editor.RequiresConstantRepaint()
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
	#define AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
#endif
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[CustomEditor(typeof(AVProWindowsMediaMovie))]
public class AVProWindowsMediaMovieEditor : Editor
{
	private AVProWindowsMediaMovie _movie;
	private bool _showAlpha;

	#if AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
	public override bool RequiresConstantRepaint()
	{
		return (_movie != null && _movie._editorPreview && _movie.MovieInstance != null);
	}
	#endif

#if UNITY_EDITOR_WIN
	private static void ShowInExplorer(string itemPath)
	{
		itemPath = System.IO.Path.GetFullPath(itemPath.Replace(@"/", @"\"));   // explorer doesn't like front slashes
		if (System.IO.File.Exists(itemPath))
		{
			System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
		}
	}
#endif

	private static bool Browse(string startPath, ref string filename, ref string folder, ref bool isStreamingAsset)
	{
		bool result = false;
		string path = UnityEditor.EditorUtility.OpenFilePanel("Browse video file", startPath, "*");
		if (!string.IsNullOrEmpty(path) && !path.EndsWith(".meta"))
		{
			string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
			projectRoot = projectRoot.Replace('\\', '/');

			if (path.StartsWith(projectRoot))
			{
				if (path.StartsWith(Application.streamingAssetsPath))
				{
					// Must be StreamingAssets relative path
					path = path.Remove(0, Application.streamingAssetsPath.Length);
					filename = System.IO.Path.GetFileName(path);
					path = System.IO.Path.GetDirectoryName(path);
					if (path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()) || path.StartsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
					{
						path = path.Remove(0, 1);
					}
					folder = path;
					isStreamingAsset = true;
				}
				else
				{
					// Must be project relative path
					path = path.Remove(0, projectRoot.Length);
					filename = System.IO.Path.GetFileName(path);
					path = System.IO.Path.GetDirectoryName(path);
					if (path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()) || path.StartsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
					{
						path = path.Remove(0, 1);
					}
					folder = path;
					isStreamingAsset = false;
				}
			}
			else
			{
				// Must be absolute path
				filename = System.IO.Path.GetFileName(path);
				folder = System.IO.Path.GetDirectoryName(path);
				isStreamingAsset = false;
			}

			result = true;
		}
		return result;
	}

	public override void OnInspectorGUI()
	{
		_movie = (this.target) as AVProWindowsMediaMovie;

		EditorGUILayout.Separator();
		GUILayout.Label("File Location", EditorStyles.boldLabel);
		//DrawDefaultInspector();

		_movie._useStreamingAssetsPath = EditorGUILayout.Toggle("Use StreamingAssets", _movie._useStreamingAssetsPath);
		_movie._folder = EditorGUILayout.TextField("Folder", _movie._folder);
		_movie._filename = EditorGUILayout.TextField("Filename", _movie._filename);

		GUILayout.BeginHorizontal();
		GUI.enabled = System.IO.File.Exists(_movie.GetFilePath());
		
#if UNITY_EDITOR_WIN
		if (GUILayout.Button("Show"))
		{
			ShowInExplorer(_movie.GetFilePath());
		}
#endif
		GUI.enabled &= _movie._useStreamingAssetsPath;

		if (GUILayout.Button("Select"))
		{
			string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
			projectRoot = projectRoot.Replace('\\', '/');

			string path = _movie.GetFilePath();
			path = path.Remove(0, projectRoot.Length + 1);
					
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;

		GUI.color = Color.green;
		if (GUILayout.Button("BROWSE"))
		{
			Browse(Application.streamingAssetsPath, ref _movie._filename, ref _movie._folder, ref _movie._useStreamingAssetsPath);
		}
		GUI.color = Color.white;

		if (string.IsNullOrEmpty(_movie.GetFilePath()))
		{
			if (_movie._loadOnStart)
			{
				GUI.color = Color.red;
				GUILayout.TextArea("Error: No file specfied");
				GUI.color = Color.white;
			}
			else
			{
				GUI.color = Color.yellow;
				GUILayout.TextArea("Warning: No file specfied");
				GUI.color = Color.white;
			}
		}
		else if (!System.IO.File.Exists(_movie.GetFilePath()))
		{
			GUI.color = Color.red;
			GUILayout.TextArea("Error: File not found");
			GUI.color = Color.white;
		}
		else
		{
			if (!_movie._useStreamingAssetsPath)
			{
				GUI.color = Color.yellow;
				GUILayout.TextArea("Warning: Files not in StreamingAssets will require manual copying for builds");
				GUI.color = Color.white;
			}
		}
		if (System.IO.Path.IsPathRooted(_movie._folder))
		{
			GUI.color = Color.yellow;
			GUILayout.TextArea("Warning: Absolute path is not ideal.  Better to use files relative to the project root");
			GUI.color = Color.white;
		}

		GUILayout.Space(10f);

		EditorGUILayout.Separator();
		GUILayout.Label("Load Options", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Colour Format");
		_movie._colourFormat = (AVProWindowsMediaMovie.ColourFormat)EditorGUILayout.EnumPopup(_movie._colourFormat);
		EditorGUILayout.EndHorizontal();
		_movie._useDisplaySync = EditorGUILayout.Toggle("Use Display Sync", _movie._useDisplaySync);

		if (_movie._useDisplaySync)
		{
			if (
#if UNITY_5
				PlayerSettings.d3d11FullscreenMode != D3D11FullscreenMode.ExclusiveMode || 
#endif
				PlayerSettings.d3d9FullscreenMode != D3D9FullscreenMode.ExclusiveMode)
			{
				GUI.color = Color.cyan;
				GUILayout.TextArea("Perf: For display sync fullscreen mode should be set to EXCLUSIVE in Player Settings");
				GUI.color = Color.white;
			}
			if (QualitySettings.vSyncCount != 1 && QualitySettings.vSyncCount != 2)
			{
				GUI.color = Color.cyan;
				GUILayout.TextArea("Perf: For display sync vsync must be set to 1 or 2 in Quality Settings");
				GUI.color = Color.white;
			}
		}

		_movie._allowAudio = EditorGUILayout.Toggle("Allow Audio", _movie._allowAudio);
		GUI.enabled = _movie._allowAudio;
		{
			_movie._useAudioDelay = EditorGUILayout.Toggle("Use Audio Delay", _movie._useAudioDelay);
			_movie._useAudioMixer = EditorGUILayout.Toggle("Use Audio Mixer", _movie._useAudioMixer);
		}
		GUI.enabled = true;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Texture Filter");
		_movie._textureFilterMode = (FilterMode)EditorGUILayout.EnumPopup(_movie._textureFilterMode);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Texture Wrap");
		_movie._textureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup(_movie._textureWrapMode);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Start Options", EditorStyles.boldLabel);
		_movie._loadOnStart = EditorGUILayout.Toggle("Load On Start", _movie._loadOnStart);
		GUI.enabled = _movie._loadOnStart;
		if (!_movie._loadOnStart)
			_movie._playOnStart = false;
		_movie._playOnStart = EditorGUILayout.Toggle("Play On Start", _movie._playOnStart);
		GUI.enabled = true;
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Performance", EditorStyles.boldLabel);
		_movie._ignoreFlips = EditorGUILayout.Toggle("Ignore Flips", _movie._ignoreFlips);
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		EditorGUILayout.Separator();
		GUILayout.Label("Playback", EditorStyles.boldLabel);

		_movie._loop = EditorGUILayout.Toggle("Loop", _movie._loop);
		//_movie._editorPreview = EditorGUILayout.Toggle("Editor Preview", _movie._editorPreview);		

		GUI.enabled = _movie._allowAudio;
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Audio Volume");
			_movie._volume = EditorGUILayout.Slider(_movie._volume, 0.0f, 1.0f);
			EditorGUILayout.EndHorizontal();

			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Audio Balance");
			_movie._audioBalance = EditorGUILayout.Slider(_movie._audioBalance, -1.0f, 1.0f);
			EditorGUILayout.EndHorizontal();
		}
		GUI.enabled = true;
		
		GUILayout.Space(8.0f);

        SerializedProperty tps = serializedObject.FindProperty("_clips");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(tps, new GUIContent("Clips"), true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        GUILayout.Space(8.0f);

		if (!Application.isPlaying)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Load"))
			{
				if (AVProWindowsMediaManager.Instance != null)
				{
					_movie.LoadMovie(_movie._playOnStart);
				}
			}

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
			using (var scope = new EditorGUI.DisabledScope(_movie.MovieInstance == null))
#else
			EditorGUI.BeginDisabledGroup(_movie.MovieInstance == null);
#endif
			{
				if (GUILayout.Button("Unload"))
				{
					_movie.UnloadMovie();
				}
			}
#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
#else
			EditorGUI.EndDisabledGroup();
#endif

			EditorGUILayout.EndHorizontal();
		}

		AVProWindowsMedia media = _movie.MovieInstance;
		if (media != null)
		{
			GUI.enabled = (_movie != null && _movie.MovieInstance != null);
            _movie._editorPreview = EditorGUILayout.Foldout(_movie._editorPreview, "Video Preview");
			GUI.enabled = true;

            if (_movie._editorPreview && _movie.MovieInstance != null)
			{
				{
					Texture texture = _movie.OutputTexture;
					if (texture == null)
						texture = EditorGUIUtility.whiteTexture;

					float ratio = (float)texture.width / (float)texture.height;


					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					Rect textureRect = GUILayoutUtility.GetRect(Screen.width/2, Screen.width/2, (Screen.width / 2) / ratio, (Screen.width / 2) / ratio);
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					_showAlpha = GUILayout.Toggle(_showAlpha, "Show Alpha Channel");
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
					 
					Matrix4x4 prevMatrix = GUI.matrix;
					if (_movie.MovieInstance.RequiresFlipY)
					{
						GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, textureRect.y + (textureRect.height / 2)));
					}

					if (!_showAlpha)
						GUI.DrawTexture(textureRect, texture, ScaleMode.ScaleToFit);
					else
						EditorGUI.DrawTextureAlpha(textureRect, texture, ScaleMode.ScaleToFit);

					GUI.matrix = prevMatrix;
				
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Select Texture", GUILayout.ExpandWidth(false)))
					{
						Selection.activeObject = texture;
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					GUILayout.Label(string.Format("{0}x{1} @ {2}fps {3} secs", media.Width, media.Height, media.FrameRate.ToString("F2"), media.DurationSeconds.ToString("F2")));		
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (media.FramesTotal > 30)
					{
						GUILayout.Label("Displaying at " + media.DisplayFPS.ToString("F1") + " fps");
					}
					else
					{
						GUILayout.Label("Displaying at ... fps");	
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();
				}

				if (_movie.enabled)
				{
					GUILayout.Space(8.0f);

					EditorGUILayout.LabelField("Frame:");
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("<", GUILayout.ExpandWidth(false)))
					{
						media.PositionFrames--;
					}
					uint currentFrame = media.PositionFrames;
					if (currentFrame != uint.MaxValue)
					{
						int newFrame = EditorGUILayout.IntSlider((int)currentFrame, 0, (int)media.LastFrame);
						if (newFrame != currentFrame)
						{
							media.PositionFrames = (uint)newFrame;
						}
					}
					if (GUILayout.Button(">", GUILayout.ExpandWidth(false)))
					{
						media.PositionFrames++;
					}
					EditorGUILayout.EndHorizontal();

                    if (_movie.NumClips > 0)
                    {
                        EditorGUILayout.Separator();
                        EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);
                        for (int i = 0; i < _movie.NumClips; i++)
                        {
                            GUILayout.BeginHorizontal();
                            string clipName = _movie.GetClipName(i);
                            GUILayout.Label(clipName);
                            if (GUILayout.Button("Loop"))
                            {
                                _movie.PlayClip(clipName, true, false);
                            }
                            GUILayout.EndHorizontal();
                        }
                        if (GUILayout.Button("Reset Clip"))
                            _movie.ResetClip();
                        EditorGUILayout.Separator();
                    }

					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button("Rewind"))
					{
						_movie.MovieInstance.Rewind();
					}

					if (!media.IsPlaying)
					{
						if (GUILayout.Button("Play"))
						{
							_movie.Play();
						}						
					}
					else
					{
						if (GUILayout.Button("Pause"))
						{
							_movie.Pause();
						}
					}
					EditorGUILayout.EndHorizontal();

#if !AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
					this.Repaint();
#endif
				}
			}
        }
		if (GUI.changed)
		{
			EditorUtility.SetDirty(_movie);
		}

        // If the app isn't running but the media is we may need to manually update it
        if (!Application.isPlaying && _movie.MovieInstance != null)
        {
            _movie.Update();
            AVProWindowsMediaManager.Instance.Update();
            EditorUtility.SetDirty(_movie);
        }
	}
}