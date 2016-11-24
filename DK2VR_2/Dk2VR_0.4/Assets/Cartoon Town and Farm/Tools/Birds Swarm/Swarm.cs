using UnityEngine;
using System.Collections;

public class Swarm : MonoBehaviour
{
	public GameObject bird;
	public int        birdsCount;
	public float      swarmRadius;
	public float      birdsDistance;
	public float      amplitude;
	public float      speed;
	
	private float     angle;
	private Vector3   lastPosition;
	
	// Use this for initialization
	void Start ()
	{
		angle = Random.Range(0.0f, 360.0f);
		lastPosition = GetNewPos();
		
		float places = swarmRadius / birdsDistance;
		for(int i = 0; i < birdsCount; i ++)
		{
			Vector3 pos = new Vector3(Random.Range(0.0f, places) * birdsDistance, Random.Range(0.0f, places) * birdsDistance, Random.Range(0.0f, places) * birdsDistance);
			pos += transform.position;
			GameObject obj = (GameObject)Instantiate(bird, pos, transform.rotation);
			obj.transform.parent = transform;
		}
	
	}
	
	void FixedUpdate ()
	{
		Vector3 newPosition = GetNewPos();
		
		transform.position += (newPosition - lastPosition);
	
		lastPosition = newPosition;
		
		angle = Mathf.MoveTowardsAngle(angle, angle + speed * Time.deltaTime, speed * Time.deltaTime);
	}
	
	Vector3 GetNewPos()
	{
		Vector3 newPosition;
	
		newPosition.x = 0;
		newPosition.y = Mathf.Sin(angle * Mathf.Deg2Rad) * amplitude;
		newPosition.z = 0;
		
		return newPosition;
	}
	
}
