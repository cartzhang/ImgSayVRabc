using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{
	public int animCount = 2;
	public float speedX;
	public float speedY;
	public float speedZ;
	public float amplitudeX;
	public float amplitudeY;
	public float amplitudeZ;
	
	private Animator anim;
	private bool     canChangeAnim;
	private float    angleX;
	private float    angleY;
	private float    angleZ;
	private Vector3  lastPosition;
	
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
		
		angleX = Random.Range(0, 360);
		angleY = Random.Range(0, 360);
		angleZ = Random.Range(0, 360);
		
		lastPosition = GetNewPos();
	}
	
	// Update is called once per frame
	void OnAnimatorMove ()
	{
		AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
	
		if(state.IsTag("NewAnim"))
		{
			if(canChangeAnim)
			{
				anim.SetInteger("AnimNum", Random.Range(0, animCount+1));
				canChangeAnim = false;
				Debug.Log("Bird anim: " + anim.GetInteger("AnimNum"));
			}
		}
		else
		{
			canChangeAnim = true;
		}
		
		Vector3 newPosition = GetNewPos();
		
		transform.position += (newPosition - lastPosition);
	
		lastPosition = newPosition;
		
		angleX = Mathf.MoveTowardsAngle(angleX, angleX + speedX * Time.deltaTime, speedX * Time.deltaTime);
		angleY = Mathf.MoveTowardsAngle(angleY, angleY + speedY * Time.deltaTime, speedY * Time.deltaTime);
		angleZ = Mathf.MoveTowardsAngle(angleZ, angleZ + speedZ * Time.deltaTime, speedZ * Time.deltaTime);
	}
	
	Vector3 GetNewPos()
	{
		Vector3 newPosition;
	
		newPosition.x = Mathf.Sin(angleX * Mathf.Deg2Rad) * amplitudeX;
		newPosition.y = Mathf.Sin(angleY * Mathf.Deg2Rad) * amplitudeY;
		newPosition.z = Mathf.Sin(angleZ * Mathf.Deg2Rad) * amplitudeZ;
		
		return newPosition;
	}
	
}
