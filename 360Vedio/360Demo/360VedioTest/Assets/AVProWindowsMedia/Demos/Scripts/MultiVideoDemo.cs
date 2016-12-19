using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiVideoDemo : MonoBehaviour 
{
	public GUISkin _skin;
	public int _guiDepth;
	private string _folder = string.Empty;
	private string _filename = string.Empty;	
	private bool _visible = true;
	private float _alpha = 1.0f;
	private GameObject _root;
	private List<AVProWindowsMediaGUIDisplay> _movies;
	private AVProWindowsMediaGUIDisplay _activeMovie;
	private AVProWindowsMediaGUIDisplay _removeMovie;

	void Update()
	{
		Vector2 screenMouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

		// Show/Hide controls based on mouse cursor position
		Rect r = new Rect(0, 0, Screen.width/2, Screen.height);
		if (r.Contains(screenMouse))
		{
			_visible = true;
			_alpha = 1.0f;
		}
		else
		{
			_alpha -= Time.deltaTime * 4f;
			if (_alpha <= 0.0f)
			{
				_alpha = 0.0f;
				_visible = false;
			}
		}

		// Remove any movie scheduled for removal
		if (_removeMovie)
		{
			Remove(_removeMovie);
			_removeMovie = null;
		}

		// Activate movie under mouse cursor
		_activeMovie = null;
		foreach (AVProWindowsMediaGUIDisplay gui in _movies)
		{
			Rect rect = gui.GetRect();
			if (rect.Contains(screenMouse))
			{
				gui._color = Color.white;
				_activeMovie = gui;
			}
			else
			{
				//gui._color = Color.white * 0.8f;
				gui._color = new Color(0.5f, 0.5f, 0.5f, 0.9f);
			}
		}
	}

	void Start()
	{
		_root = new GameObject("Movies");
		_movies = new List<AVProWindowsMediaGUIDisplay>();

		// Add some initial videos
		string folder = "";
		Add(folder, "sample-blue-480x256-divx.avi");
		Add(folder, "sample-green-480x256-divx.avi");
		Add(folder, "sample-purple-480x256-divx.avi");
		Add(folder, "sample-yellow-480x256-divx.avi");
	}

	private void Add(string folder, string filename)
	{
		GameObject go = new GameObject();
		go.transform.parent = _root.transform;
		
		AVProWindowsMediaMovie movie = go.AddComponent<AVProWindowsMediaMovie>();
		movie._folder = folder;
		movie._filename = filename;
		movie._loop = true;
		movie._loadOnStart = false;
		movie._playOnStart = false;
		movie._useStreamingAssetsPath = true;

		AVProWindowsMediaGUIDisplay gui = go.AddComponent<AVProWindowsMediaGUIDisplay>();
		gui._movie = movie;
		gui._scaleMode = ScaleMode.StretchToFill;
		gui._fullScreen = false;
		gui._alphaBlend = false;
		gui._depth = 5;
		gui._color = new Color(0.8f, 0.8f, 0.8f, 1.0f);

		_movies.Add(gui);

		if (!movie.LoadMovie(true))
		{
			Remove(gui);
			return;
		}

		UpdateLayout();
	}

	private void Remove(AVProWindowsMediaGUIDisplay movie)
	{
		if (movie)
		{
			_movies.Remove(movie);
			Destroy(movie.gameObject);
			UpdateLayout();
		}
	}

	private void UpdateLayout()
	{
		int numMovies = _movies.Count;
		int numColRows = Mathf.CeilToInt(Mathf.Sqrt(numMovies));

		float width = 1.0f / numColRows;
		float height = 1.0f / numColRows;

		for (int i = 0; i < numMovies; i++)
		{
			AVProWindowsMediaGUIDisplay gui = _movies[i];

			int x = i % numColRows;
			int y = i / numColRows;

			gui._x = width * x;
			gui._y = height * y;
			gui._width = width;
			gui._height = height;
		}
	}

	public void ControlWindow(int id)
	{			
		GUILayout.BeginVertical("box", GUILayout.MinWidth(400));
				
		GUILayout.BeginHorizontal();
		GUILayout.Label("Folder: ", GUILayout.Width(100));
		_folder = GUILayout.TextField(_folder, 192);
		GUILayout.EndHorizontal();
		GUILayout.Space(16f);
				
		GUILayout.BeginHorizontal();
		GUILayout.Label("File Name: ", GUILayout.Width(100));
		_filename = GUILayout.TextField(_filename, 192, GUILayout.MinWidth(256f));
		if (GUILayout.Button("Add Video", GUILayout.Width(128)))
		{
			Add(_folder, _filename);
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(16f);

		if (GUILayout.Button("Remove All"))
		{
			for (int i = 0; i < _movies.Count; i++)
			{
				Destroy(_movies[i].gameObject);
				_movies[i] = null;
			}
			_movies.Clear();
			UpdateLayout();
		}
		
		GUILayout.EndVertical();
	}

	private void DrawVideoControls(Rect area, AVProWindowsMediaGUIDisplay movieGUI)
	{
		AVProWindowsMediaMovie movie = movieGUI._movie;
		AVProWindowsMedia player = movie.MovieInstance;
		if (player == null)
			return;

		// Close button
		if (GUI.Button(new Rect(area.x + (area.width - 32) ,area.y, 32, 32), "X"))
		{
			_removeMovie = movieGUI;
		}

		// Duplicate button
		if (GUI.Button(new Rect(area.x + (area.width - 64) ,area.y, 32, 32), "+"))
		{
			Add(movie._folder, movie._filename);
		}

		// Video properties
		GUILayout.BeginArea(new Rect(area.x, area.y, area.width/2, area.height/2));
		GUILayout.Label(player.Width + "x" + player.Height + "/" + player.FrameRate.ToString("F2") + "hz");
		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(area.x, area.y + (area.height - 32), area.width, 32));
		GUILayout.BeginHorizontal();
		float position = player.PositionSeconds;
		float newPosition = GUILayout.HorizontalSlider(position, 0.0f, player.DurationSeconds, GUILayout.ExpandWidth(true));
		if (position != newPosition)
		{
			player.PositionSeconds = newPosition;
		}
		if (player.IsPlaying)
		{
			if (GUILayout.Button("Pause", GUILayout.ExpandWidth(false)))
			{
				player.Pause();
			}
		}
		else
		{
			if (GUILayout.Button("Play", GUILayout.ExpandWidth(false)))
			{
				player.Play();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void OnGUI()
	{
		GUI.skin = _skin;
		GUI.depth = _guiDepth;

		if (_activeMovie)
		{
			DrawVideoControls(_activeMovie.GetRect(), _activeMovie);
		}

		if (_visible)
		{
			GUI.color = new Color(1f, 1f, 1f, _alpha);
			GUILayout.Box("Demo Controls");
			//GUILayout.BeginArea(new Rect(0, 0, 440, 200), GUI.skin.box);
			ControlWindow(0);
		}
		else
		{
			GUI.color = new Color(1f, 1f, 1f, 1f - _alpha);
			GUI.Box(new Rect(0, 0, 128, 32), "Demo Controls");
		}
	}
}