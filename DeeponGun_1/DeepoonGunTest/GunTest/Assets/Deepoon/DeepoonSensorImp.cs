using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class DeepoonSensorImp
{
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static void dpnusInit();
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static void dpnusUninit();
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static IntPtr dpnusCreateSensor
		( int index , string product_name );
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static void dpnusDestroySensor
		( IntPtr sensor );
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static DeepoonCommon.dpnTransform dpnusGetTransform
		( IntPtr sensor );
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static DeepoonCommon.dpnSensorData dpnusGetSensorData
		( IntPtr sensor );
	[DllImport("deepoon_unity_sensor", CallingConvention = CallingConvention.Cdecl)]
	public extern static void dpnusRecenter
		( IntPtr sensor );

	IntPtr _sensor;

	public bool Init( int index , string product_name )
	{
		dpnusInit();
		_sensor = dpnusCreateSensor( index , product_name );
		if( IntPtr.Zero == _sensor )
		{
			dpnusUninit();
#if UNITY_EDITOR 
			UnityEditor.EditorUtility.DisplayDialog("DeepoonSensor", "WARNING: Failed to create sensor!", "OK");
#endif
			Debug.Log ("WARNING: Failed to create sensor!");
			return false;
		}
		return true;
	}

	public void Uninit()
	{
		if( IntPtr.Zero != _sensor ) dpnusDestroySensor( _sensor );
		dpnusUninit();
	}

	public Quaternion GetRotation()
	{
		if( IntPtr.Zero != _sensor ) 
		{
			//Quaternion init_rotation = Quaternion.AngleAxis (90, new Vector3 (0, 1, 0)) * Quaternion.AngleAxis (180, new Vector3 (0, 0, 1));

			DeepoonCommon.dpnTransform t = dpnusGetTransform( _sensor );
			return DeepoonCommon.ToQuaternion( t.q );
		}
		return Quaternion.identity;
	}
	public void Recenter()
	{
		if( IntPtr.Zero != _sensor ) 
		{
			dpnusRecenter( _sensor );
		}
	}
}
