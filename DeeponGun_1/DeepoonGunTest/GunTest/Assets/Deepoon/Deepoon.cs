using UnityEngine;
using System.Collections;
#if UNITY_EDITOR 
using UnityEditor;
#endif

public class Deepoon : MonoBehaviour
{
	public int expectedVersion = 200;
	public float fieldOfView = 120;
	public float interpupillaryDistance = 0.06f;
	public bool useGUI = true;
	public RenderTexture targetGuiLeft = null;
	public RenderTexture targetGuiRight = null;

	public static DeepoonImp imp = null;
	private static WaitForEndOfFrame _wait_for_end_of_frame = null;
	private static int has_initialized_display_change = 0;

	void Start()
	{
		Debug.Log ("Deepoon.Start() enter");
		if( null == targetGuiLeft )
			targetGuiLeft = new RenderTexture( 32 , 32 , 24 , RenderTextureFormat.Default );
		if( null == targetGuiRight )
			targetGuiRight = new RenderTexture( 32 , 32 , 24 , RenderTextureFormat.Default );
		imp = new DeepoonImp();
		RenderTexture [] tar_gui = new RenderTexture[2] { targetGuiLeft , targetGuiRight };
		imp.Init 
			( this.transform.gameObject
			 , expectedVersion
			 , fieldOfView , interpupillaryDistance
			 , useGUI
			 , tar_gui );
		
		//listen to _wait_for_end_of_frame
		_wait_for_end_of_frame = new WaitForEndOfFrame ();
		StartCoroutine ( EndFrame () );
		Debug.Log ("Deepoon.Start() leave");
	}

	void LateUpdate ()
	{
		//Debug.Log ("Deepoon.LateUpdate() enter");
		imp.Update();
		
		int msg;
		while( (int)DeepoonCommon.dpnhMESSAGE.dpnhMESSAGE_OK != ( msg = imp.CheckMessage() ) )
		{
			if( msg == (int)DeepoonCommon.dpnhMESSAGE.dpnhMESSAGE_DISPLAY_CHANGE )
			{
				if( has_initialized_display_change > 0 )
				{
					imp.Uninit();
					//todo: maybe the Deepoon is unpluged.
					Debug.Log ("WARNING: Deepoon Window Changed.");
#if UNITY_EDITOR 
					UnityEditor.EditorUtility.DisplayDialog("Deepoon", "WARNING: Deepoon Window Changed.", "OK");
#else
					Application.Quit();
#endif
				}
			}
		}
		//Debug.Log ("Deepoon.LateUpdate() leave");
	}
	
	public static IEnumerator EndFrame ()
	{
		while (true)
		{
			yield return _wait_for_end_of_frame;
			//Debug.Log ("Deepoon._wait_for_end_of_frame enter");
			imp.Compose();
			has_initialized_display_change = 1;
			//Debug.Log ("Deepoon._wait_for_end_of_frame leave");
		}
	}

	private void OnApplicationQuit()
	{
		Debug.Log ("Deepoon.OnApplicationQuit() enter");
		imp.Uninit();
		Debug.Log ("Deepoon.OnApplicationQuit() leave");
	}
}
