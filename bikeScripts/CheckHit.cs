using UnityEngine;
using System.Collections;

public class CheckHit : MonoBehaviour {
	
	public BoxCollider BoxCol;
	private bool isHit = false;

	public bool GetIsHit()
	{
		return isHit;
	}
	// Use this for initialization
//	void Start () {
//	
//	}

	void OnTriggerEnter(Collider other)
	{
		isHit = true;
	}

	void OnTriggerExit(Collider other)
	{
		isHit = false;
	}

//	void OnTriggerStay(Collider other)
//	{
//		isHit = true;
//	}
}
