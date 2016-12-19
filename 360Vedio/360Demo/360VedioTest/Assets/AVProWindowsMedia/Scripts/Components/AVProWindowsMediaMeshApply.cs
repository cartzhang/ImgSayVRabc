using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Windows Media/Mesh Apply")]
public class AVProWindowsMediaMeshApply : MonoBehaviour 
{
	public MeshRenderer _mesh;
	public AVProWindowsMediaMovie _movie;
	public Texture2D _defaultTexture;

	void Update()
	{
		bool applied = false;
		if (_movie != null && _movie.MovieInstance != null)
		{
			Texture texture = _movie.OutputTexture;
			if (texture != null)
			{
				ApplyMapping(texture, _movie.MovieInstance.RequiresFlipY);
				applied = true;
			}
		}

		if (!applied)
		{
			ApplyMapping(_defaultTexture, false);
		}
	}

	private void ApplyMapping(Texture texture, bool requiresYFlip)
	{
		if (_mesh != null && texture != null)
		{
			Vector2 scale = Vector2.one;
			Vector2 offset = Vector2.zero;
			if (requiresYFlip)
			{
				scale = new Vector2(1.0f, -1.0f);
				offset = new Vector3(0.0f, 1.0f);
			}

			foreach (Material m in _mesh.materials)
			{
				m.mainTexture = texture;
				m.mainTextureScale = scale;
				m.mainTextureOffset = offset;
			}
		}
	}

	void OnEnable()
	{
		Update();
	}

	void OnDisable()
	{
		ApplyMapping(_defaultTexture, false);
	}
}