using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Windows Media/Material Apply")]
public class AVProWindowsMediaMaterialApply : MonoBehaviour 
{
	public Material _material;
	public string _textureName;
	public AVProWindowsMediaMovie _movie;
	public Texture2D _defaultTexture;
	private Vector2 _scale = Vector2.one;
	private Vector2 _offset = Vector2.zero;
	private Texture _texture;

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
		if (_material != null && texture != null)
		{
			Vector2 scale = _scale;
			Vector2 offset = _offset;
			if (requiresYFlip)
			{
				scale.Scale(new Vector2(1f, -1f));
				offset.y += 1f;
			}

			if (string.IsNullOrEmpty(_textureName))
			{
				_material.mainTexture = texture;
				_material.mainTextureScale = scale;
				_material.mainTextureOffset = offset;
			}
			else
			{
				_material.SetTexture(_textureName, texture);
				_material.SetTextureScale(_textureName, scale);
				_material.SetTextureOffset(_textureName, offset);
			}
		}
	}

	void OnEnable()
	{
		if (_material != null)
		{
			_scale = _material.mainTextureScale;
			_offset = _material.mainTextureOffset;
			_texture = _material.mainTexture;
		}
		Update();
	}

	void OnDisable()
	{
		ApplyMapping(_defaultTexture, false);
		if (_material != null)
		{
			_material.mainTextureScale = _scale;
			_material.mainTextureOffset = _offset;
			_material.mainTexture = _texture;
		}
	}
}
