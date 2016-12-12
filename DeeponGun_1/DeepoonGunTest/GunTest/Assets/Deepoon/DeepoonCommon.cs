using UnityEngine;
using System.Runtime.InteropServices;

public class DeepoonCommon
{
	public enum RENDER_EVENT
	{
		COMPOSE = 0,
	}
	
	public enum dpncAPPLY
	{
		dpncAPPLY_NONE ,
		dpncAPPLY_DISTORTION ,
		dpncAPPLY_TIMEWARP ,
		dpncAPPLY_DISTORTION_TIMEWARP ,
		dpncAPPLY_NUM ,
	}
	
	public enum dpncMSAA
	{
		dpncMSAA_1x ,
		dpncMSAA_4x ,
		dpncMSAA_16x ,
		dpncMSAA_NUM ,
	}
	
	public enum dpncEYE
	{
		dpncEYE_LEFT ,
		dpncEYE_RIGHT ,
		dpncEYE_SCREEN ,
		dpncEYE_NUM ,
	}
	
	public enum dpnhMESSAGE
	{
		dpnhMESSAGE_OK ,
		dpnhMESSAGE_KEY ,
		dpnhMESSAGE_MOUSE_BUTTON ,
		dpnhMESSAGE_MOUSE_MOVE ,
		dpnhMESSAGE_RESIZE ,
		dpnhMESSAGE_DISPLAY_CHANGE ,
		dpnhMESSAGE_NUM ,
	};
	
	[StructLayout(LayoutKind.Sequential)]
	public struct dpnRect
	{
		public int x;
		public int y;
		public int w;
		public int h;
	};
	
	[StructLayout(LayoutKind.Sequential)]
	public struct dpnVector3
	{
		public float x;
		public float y;
		public float z;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct dpnQuarterion
	{
		public float s;
		public float i;
		public float j;
		public float k;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct dpnTransform
	{
		public dpnQuarterion q;
		public dpnVector3 p;
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct dpnSensorData
	{
		public dpnVector3 angular_velocity;
		public dpnVector3 linear_acceleration;
		public dpnVector3 magnetometer;
	}
	
	public static Quaternion ToQuaternion( DeepoonCommon.dpnQuarterion q )
	{
		return new Quaternion( -q.i , -q.j , -q.k , q.s );
	}
}
