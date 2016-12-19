using UnityEngine;
using System.Collections;

public class SphereDemo : MonoBehaviour 
{
	private float _spinX;
	private float _spinY;

	void Update () 
	{
		// Also rotate from mouse / touch input
		if (Input.GetMouseButton(0))
		{
			float h = 40.0f * -Input.GetAxis("Mouse X") * Time.deltaTime;
			float v = 40.0f * Input.GetAxis("Mouse Y") * Time.deltaTime;
			_spinX += h;
			_spinY += v;
		}
		if (!Mathf.Approximately(_spinX, 0f) || !Mathf.Approximately(_spinY, 0f))
		{
			this.transform.Rotate(Vector3.up, _spinX);
			this.transform.Rotate(Vector3.right, _spinY);

			_spinX = Mathf.MoveTowards(_spinX, 0f, 5f * Time.deltaTime);
			_spinY = Mathf.MoveTowards(_spinY, 0f, 5f * Time.deltaTime);
		}
	}
}
