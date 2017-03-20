using UnityEngine;
using System.Collections;

public class wangQiuAmmoCtrl : MonoBehaviour
{
	bool isRemoved = false;
	public void DelayRemoveAmmo()
	{
		Invoke("removeAmmo", 3f);
	}

	void removeAmmo()
	{
		if(isRemoved){
			return;
		}
		isRemoved = true;
		//Debug.Log("removeAmmo ************** test");
		Destroy(gameObject, 0.1f);
	}
}