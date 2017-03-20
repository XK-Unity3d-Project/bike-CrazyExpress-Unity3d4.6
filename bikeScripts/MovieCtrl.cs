using UnityEngine;
using System.Collections;

public class MovieCtrl : MonoBehaviour {
	public GameObject MovieObj;

	void OnTriggerEnter(Collider other)
	{
		bool IsRemove = false;
		switch(gameObject.name)
		{
		case "MovieStart":
			MovieObj.SetActive(true);
			break;
		case "MovieEnd":
			IsRemove = true;
			MovieObj.SetActive(false);
			break;
		}

		gameObject.SetActive(false);
		if(IsRemove)
		{
			GameObject objPar = transform.parent.gameObject;
			Destroy(objPar);
		}
	}
}
