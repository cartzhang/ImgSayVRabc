using UnityEngine;
using System.Collections;

public class DeepoonSensor : MonoBehaviour
{
	public enum DEEPOON_PRODUCTS
	{
		DeepoonE2TrackingDevice ,
		DeepoonG1SensorModuleDevice ,
	};

	public enum PERIPHERAL_TYPE
	{
		Custom ,
		XRoverTest ,
		XRover1 ,
		DeePoonE2 ,
	};

	private string[] DEEPOON_PRODUCT_NAMES = 
	{
		"DeePoon Tracker Device" ,
		"DeePoon Sensor Module Device" ,
	};

	public DEEPOON_PRODUCTS product = DEEPOON_PRODUCTS.DeepoonG1SensorModuleDevice;
	public int index = 0;

	public PERIPHERAL_TYPE peripheral = PERIPHERAL_TYPE.Custom;
	public Vector3 [] peripheralsRotation = 
	{
		new Vector3( 0 , 0 , 0 ) ,
		new Vector3( 180 , -90 , 0 ) ,
		new Vector3( 0 , 90 , 0 ) ,
		new Vector3( 0 , 0 , 0 ) ,
	};

	private DeepoonSensorImp imp = null;

	public GameObject objectToAlign = null;
	public KeyCode keyToAlign = KeyCode.Alpha1;
	private Quaternion alignment_rotation = Quaternion.identity;
	public KeyCode keyToRecenter = KeyCode.X;

	// Use this for initialization
	void Start ()
	{
		imp = new DeepoonSensorImp();
		imp.Init( index , DEEPOON_PRODUCT_NAMES[(int)product] );
	}

	Quaternion Alignment( Quaternion cur_rotation , Quaternion object_to_align )
	{
		Vector3 ecur = cur_rotation.eulerAngles;
		Vector3 eobj = object_to_align.eulerAngles;
		return Quaternion.AngleAxis( eobj.y - ecur.y , new Vector3( 0 , 1 , 0 ) );
	}

	// Update is called once per frame
	void Update ()
	{
		Quaternion q = imp.GetRotation();
		q = q * Quaternion.Euler( peripheralsRotation[(int)peripheral] );
		if ( Input.GetKeyDown (keyToAlign) )
		{
			if( null != objectToAlign )
			{
				alignment_rotation = Alignment( q , objectToAlign.transform.rotation );
			}
			else
			{
				alignment_rotation = Alignment( q , Quaternion.identity );
			}
		}
		if ( Input.GetKeyDown (keyToRecenter) )
		{
			imp.Recenter();
		}

		if ( Input.GetKeyDown (KeyCode.T) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].x += -90;
		}
		if ( Input.GetKeyDown (KeyCode.Y) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].x += 90;
		}
		if ( Input.GetKeyDown (KeyCode.U) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].y += -90;
		}
		if ( Input.GetKeyDown (KeyCode.I) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].y += 90;
		}
		if ( Input.GetKeyDown (KeyCode.O) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].z += -90;
		}
		if ( Input.GetKeyDown (KeyCode.P) )
		{
			peripheralsRotation[(int)PERIPHERAL_TYPE.Custom].z += 90;
		}

		transform.localRotation = alignment_rotation * q;
	}

	void OnApplicationQuit()
	{
		imp.Uninit();
	}
}
