using UnityEngine;
using System.Collections;


public class gameCollider : MonoBehaviour {

	public float slowWorldTime = 1f;

	void Start()
	{
		if(slowWorldTime < 0)
		{
			slowWorldTime = 1f;
		}
	}
}
