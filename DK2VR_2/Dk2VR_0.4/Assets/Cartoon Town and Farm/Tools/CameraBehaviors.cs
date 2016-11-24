using UnityEngine;
using System.Collections;

public class CameraBehaviors : MonoBehaviour {
	
	public float maxSpeed = 1;
	public float breakSpeed = 0.1f;
	public float BoundTop;
	public float BoundBottom;
	public float BoundLeft;
	public float BoundRight;
	public bool InvertX = false;
	public bool InvertZ = false;
	
	float dest_speedX;
	float dest_speedZ;
	float speedX;
	float speedZ;
	

	
	void UpdateInput()
	{
		dest_speedX = Input.GetAxis ("Horizontal");
		dest_speedZ = Input.GetAxis ("Vertical");
		
		speedX = Mathf.Lerp(speedX,dest_speedX,breakSpeed);
		speedZ = Mathf.Lerp(speedZ,dest_speedZ,breakSpeed);
		Mathf.Clamp (speedX,-maxSpeed,maxSpeed);
		Mathf.Clamp (speedZ,-maxSpeed,maxSpeed);
	}
	
	
	void UpdatePosition()
	{
		Vector3 tmpPosition;
			tmpPosition = this.transform.position;
			
		//X
			if (InvertX) {
				tmpPosition.x -= speedX;
				if(tmpPosition.x > BoundLeft) tmpPosition.x = BoundLeft;
				if(tmpPosition.x < BoundRight) tmpPosition.x = BoundRight;
			}
			else {
				tmpPosition.x += speedX;
				if(tmpPosition.x < BoundLeft) tmpPosition.x = BoundLeft;
				if(tmpPosition.x > BoundRight) tmpPosition.x = BoundRight;
			}
		//Z
			if (InvertZ) {
				tmpPosition.z -= speedZ;
				if(tmpPosition.z > BoundTop) tmpPosition.z = BoundTop;
				if(tmpPosition.z < BoundBottom) tmpPosition.z = BoundBottom;
			}
			else {
				tmpPosition.z += speedZ;
				if(tmpPosition.z < BoundTop) tmpPosition.z = BoundTop;
				if(tmpPosition.z > BoundBottom) tmpPosition.z = BoundBottom;
			}
			
		
		
		this.transform.position = tmpPosition;
	
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		UpdateInput();
		UpdatePosition();
	}
}
