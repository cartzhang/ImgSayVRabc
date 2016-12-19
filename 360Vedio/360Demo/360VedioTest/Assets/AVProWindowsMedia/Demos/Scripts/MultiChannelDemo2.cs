using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiChannelDemo2 : MonoBehaviour 
{	
	public GUISkin _guiSkin;
	public Texture2D _speaker;
	public Material _material;
	public int _numChannels = 8;
	private int _seed = 0;
	private Vector2[] _speakerPositions;
	private bool _normalise;
	private float _falloff = 8.0f;
	
	private int _totalUserCount;
	private List<MultiChannelDemo2User> _activeUsers = new List<MultiChannelDemo2User>(16);
	
	//private Dictionary<string, int> _loadedSounds = new Dictionary<string, int>(16);
	
	private Texture2D _falloffTexture;
	
	public int NumChannels
	{
		get { return _numChannels; }
	}
	
	void Start()
	{
#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3
		Random.InitState(0x1235431 + _seed);
#else
		Random.seed = 0x1235431 + _seed;
#endif

		_speakerPositions = new Vector2[_numChannels];
		UpdateSpeakerPositions();
				
		_falloffTexture = new Texture2D(128, 1, TextureFormat.ARGB32, false);
		_falloffTexture.filterMode = FilterMode.Bilinear;
		_falloffTexture.wrapMode = TextureWrapMode.Clamp;
		UpdateFalloffTexture();
	}
	
	private void UpdateFalloffTexture()
	{
		for (int i = 0; i < _falloffTexture.width; i++)
		{
			float t = i / (float)(_falloffTexture.width - 1);
			float l = GetLevel(t);
			_falloffTexture.SetPixel(i, 1, Color.white * l);
		}
		
		_falloffTexture.Apply(false, false);		
	}
	
	void Update()
	{	
		UpdateFalloffTexture();
	}
	
	public void DrawFalloff(Vector2 position)
	{
		_material.mainTexture = _falloffTexture;
		_material.SetPass(0);
		
		Matrix4x4 m = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
		
		GL.PushMatrix();
		GL.LoadPixelMatrix();
		GL.MultMatrix(m);
		GL.Begin(GL.TRIANGLES);
		
		int numSegments = 36;
		float angle = 0.0f;
		float angleStep = (Mathf.PI * 2.0f) / numSegments;
		for (int i = 0; i < numSegments; i++)
		{
			float x = Mathf.Sin(angle);
			float y = Mathf.Cos(angle);
			x *= Screen.width;
			y *= Screen.height;
			y *= 1.777f;
			float z = 0.5f;
			
			GL.TexCoord2(0, 0);
			GL.Vertex3(0, 0, z);
			
			GL.TexCoord2(1, 0);
			GL.Vertex3(x, y, z);
			
			angle += angleStep;
			x = Mathf.Sin(angle);
			y = Mathf.Cos(angle);
			x *= Screen.width;
			y *= Screen.height;
			y *= 1.777f;
			
			GL.TexCoord2(1, 0);
			GL.Vertex3(x, y, z);
		}
				
		GL.End();
		GL.PopMatrix();
	}
	
	public float GetLevel(float d)
	{
		return 1.0f - Mathf.Clamp01(d * _falloff);
	}
	
	public void UpdateAudioMatrix(Vector2 userPosition, ref float[] values)
	{
		float valTotal = 0f;
		
		for (int i = 0; i < values.Length; i++)
		{
			Vector2 sp = _speakerPositions[i];
			sp.x /= Screen.width;
			sp.y /= Screen.height;
			values[i] = 1.0f - Mathf.Clamp01(Vector2.Distance(sp, userPosition) * _falloff);
			
			valTotal += values[i];
		}
		
		if (_normalise && valTotal > 0.0f)
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] /= valTotal;
			}
		}
	}

	void UpdateSpeakerPositions()
	{
		if (_seed == 0)
		{
			for (int i = 0; i < _speakerPositions.Length; i++)
			{
				_speakerPositions[i] = new Vector2(_speaker.width/2 + (i*(Screen.width - _speaker.width) / _speakerPositions.Length), Screen.height / 2);
			}
		}
		else
		{
			for (int i = 0; i < _speakerPositions.Length; i++)
			{
				_speakerPositions[i] = new Vector2(Random.Range(_speaker.width / 2, Screen.width - _speaker.width / 2), Random.Range(_speaker.height/2, Screen.height - _speaker.height/2));
			}
		}
	}
	
	private void CreateUser()
	{
		GameObject go = new GameObject("User" + _totalUserCount++);
		MultiChannelDemo2User user = go.AddComponent<MultiChannelDemo2User>();
		user.name = go.name;
		user._guiSkin = _guiSkin;
		user._parent = this;
		_activeUsers.Add(user);
	}

	void OnGUI()
	{		
		GUI.skin = _guiSkin;
		
		for (int i = 0; i < _speakerPositions.Length; i++)
		{
			Rect r = new Rect(_speakerPositions[i].x - _speaker.width / 2, _speakerPositions[i].y - _speaker.height / 2, _speaker.width, _speaker.height);
			GUI.DrawTexture(r, _speaker);
			r.width = 16;
			r.y -= _speaker.height;
			r.x += (_speaker.width / 2) - (r.width / 2);
			GUI.Label(r, i.ToString());
		}
		
		GUILayout.BeginVertical("box");
		
		if (GUILayout.Button("Create Instance"))
		{
			CreateUser();
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("Speaker Layout:");
		if (GUILayout.Button("Linear"))
		{
			_seed = 0;
			UpdateSpeakerPositions();
		}
		if (GUILayout.Button("Random"))
		{
			_seed++;
			UpdateSpeakerPositions();
		}
		GUILayout.EndHorizontal();
		
		_normalise = GUILayout.Toggle(_normalise, "Normalise");
		GUILayout.BeginHorizontal();
		GUILayout.Label("Falloff");
		_falloff = GUILayout.HorizontalSlider(_falloff, 1.0f, 20f);
		GUILayout.Label(_falloff.ToString("F1"));
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
	}
}
