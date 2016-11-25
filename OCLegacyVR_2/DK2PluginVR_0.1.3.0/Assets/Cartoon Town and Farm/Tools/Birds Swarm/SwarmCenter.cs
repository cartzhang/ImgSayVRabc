using UnityEngine;
using System.Collections;

public class SwarmCenter : MonoBehaviour
{
	public float speed;
	
	// Use this for initialization
	void Start ()
	{
		transform.Rotate(new Vector3(0, Random.Range(0.0f, 360.0f), 0));
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
	}
}
