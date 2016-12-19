#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
	#define UNITY_FEATURE_UGUI
#endif

using UnityEngine;
using UnityEditor;
#if UNITY_FEATURE_UGUI
using UnityEngine.UI;
using UnityEditor.UI;

//-----------------------------------------------------------------------------
// Copyright 2012-2016 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

/// <summary>
/// Editor class used to edit UI Images.
/// </summary>
[CustomEditor(typeof(AVProWindowsMediaUGUIComponent), true)]
[CanEditMultipleObjects]
public class AVProWindowsMediaUGUIComponentEditor : GraphicEditor
{
    SerializedProperty m_Movie;
    SerializedProperty m_UVRect;
	SerializedProperty m_DefaultTexture;
	SerializedProperty m_NoDefaultDisplay;
	SerializedProperty m_SetNativeSize;
	SerializedProperty m_KeepAspectRatio;
    GUIContent m_UVRectContent;

	[MenuItem("GameObject/UI/AVPro Windows Media uGUI", false, 0)]
	public static void CreateGameObject()
	{
		GameObject parent = Selection.activeGameObject;
		RectTransform parentCanvasRenderer = (parent != null) ? parent.GetComponent<RectTransform>() : null;
		if (parentCanvasRenderer)
		{
			GameObject go = new GameObject("AVPro Windows Media");
			go.transform.SetParent(parent.transform, false);
			go.AddComponent<RectTransform>();
			go.AddComponent<CanvasRenderer>();
			go.AddComponent<AVProWindowsMediaUGUIComponent>();
			Selection.activeGameObject = go;
		}
		else
		{
			EditorUtility.DisplayDialog("AVPro Windows Media", "You must make the AVPro Windows Media uGUI object as a child of a Canvas.", "Ok");
		}
	}

	public override bool RequiresConstantRepaint()
	{
		AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;
		return (rawImage != null && rawImage.HasValidTexture());
	}

    protected override void OnEnable()
    {
        base.OnEnable();

        // Note we have precedence for calling rectangle for just rect, even in the Inspector.
        // For example in the Camera component's Viewport Rect.
        // Hence sticking with Rect here to be consistent with corresponding property in the API.
        m_UVRectContent = new GUIContent("UV Rect");

        m_Movie = serializedObject.FindProperty("m_movie");
        m_UVRect = serializedObject.FindProperty("m_UVRect");
		m_SetNativeSize = serializedObject.FindProperty("_setNativeSize");
		m_KeepAspectRatio = serializedObject.FindProperty("_keepAspectRatio");

		m_NoDefaultDisplay = serializedObject.FindProperty("_noDefaultDisplay"); 
		m_DefaultTexture = serializedObject.FindProperty("_defaultTexture");

        SetShowNativeSize(true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Movie);
		EditorGUILayout.PropertyField(m_DefaultTexture);
		EditorGUILayout.PropertyField(m_NoDefaultDisplay);
		AppearanceControlsGUI();
        EditorGUILayout.PropertyField(m_UVRect, m_UVRectContent);

		EditorGUILayout.PropertyField(m_SetNativeSize);
		EditorGUILayout.PropertyField(m_KeepAspectRatio);

        SetShowNativeSize(false);
        NativeSizeButtonGUI();

        serializedObject.ApplyModifiedProperties();

		AVProWindowsMediaUGUIComponent component = target as AVProWindowsMediaUGUIComponent;

		if (component.m_movie != null) 
		{
			if (component.m_movie.MovieInstance == null)
			{
				if (GUILayout.Button("Load"))
				{
					component.m_movie.LoadMovie(true);
				}
			}
			else
			{
				EditorUtility.SetDirty(component.m_movie);
				if (GUILayout.Button("Unload"))
				{
					component.m_movie.UnloadMovie();
				}
			}
		}
    }

    void SetShowNativeSize(bool instant)
    {
        base.SetShowNativeSize(m_Movie.objectReferenceValue != null, instant);
    }

    /// <summary>
    /// Allow the texture to be previewed.
    /// </summary>

    public override bool HasPreviewGUI()
    {
        AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;
        return rawImage != null;
    }

    /// <summary>
    /// Draw the Image preview.
    /// </summary>

	public override void OnPreviewGUI(Rect drawArea, GUIStyle background)
    {
        AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;
        Texture tex = rawImage.mainTexture;

        if (tex == null)
            return;

		// Create the texture rectangle that is centered inside rect.
		Rect outerRect = drawArea;

		Matrix4x4 m = GUI.matrix;
		// Flip the image vertically
		if (rawImage.HasValidTexture())
		{
			if (rawImage.m_movie.MovieInstance.RequiresFlipY)
			{
				GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, outerRect.y + (outerRect.height / 2)));
			}
		}

		EditorGUI.DrawTextureTransparent(outerRect, tex, ScaleMode.ScaleToFit);//, outer.width / outer.height);
        //SpriteDrawUtility.DrawSprite(tex, rect, outer, rawImage.uvRect, rawImage.canvasRenderer.GetColor());

		GUI.matrix = m;
    }

    /// <summary>
    /// Info String drawn at the bottom of the Preview
    /// </summary>

    public override string GetInfoString()
    {
        AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;

		string text = string.Empty;
		if (rawImage.HasValidTexture())
		{
			text += string.Format("Video Size: {0}x{1}\n",
			                        Mathf.RoundToInt(Mathf.Abs(rawImage.mainTexture.width)),
			                        Mathf.RoundToInt(Mathf.Abs(rawImage.mainTexture.height)));
		}

        // Image size Text
		text += string.Format("Display Size: {0}x{1}",
                Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)),
                Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));

        return text;
    }
}
#endif