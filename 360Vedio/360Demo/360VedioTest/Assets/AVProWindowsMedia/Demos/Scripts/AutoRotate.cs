using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2015-2016 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProVideo.Demos
{
	[RequireComponent(typeof(Transform))]
	public class AutoRotate : MonoBehaviour
	{
		private float x, y, z;

		void Awake()
		{
			float s = 32f;
			x = Random.Range(-s, s);
			y = Random.Range(-s, s);
			z = Random.Range(-s, s);
		}
		void Update()
		{
			this.transform.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
		}
	}
}