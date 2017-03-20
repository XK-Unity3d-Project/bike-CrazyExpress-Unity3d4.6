using UnityEngine;
using System.Collections;

public class Mutoudui : MonoBehaviour {
	//public GameObject trigger;

//	void Start()
//	{
//		MoveMuTou();
//	}

	void MoveMuTou()
	{
		Rigidbody rigObj;
		for(int i = 0; i < transform.childCount; i++)
		{
			rigObj = transform.GetChild(i).GetComponent<Rigidbody>();
			if(rigObj)
			{
				rigObj.isKinematic = false;
				rigObj.useGravity = true;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;
		string lay = LayerMask.LayerToName( obj.layer );
		if(lay == "Player" || lay == "NPC" || lay == "wuDiLayer")
		{
			MoveMuTou();
		}
	}
}
