using UnityEngine;
using System.Collections;

public class speedEffect : MonoBehaviour {

	void Awake()
	{
		GlobalScript.GetInstance().player.SpeedChange+=SpeedChange;
		//Debug.Log("cscsscscscscscs");
	}
	private void SpeedChange()
	{
		if(GlobalScript.GetInstance().player.Speed>35)
		{

			gameObject.GetComponent<UISprite>().enabled=true;
		}
		else
		{
			gameObject.GetComponent<UISprite>().enabled=false;
		}
	}
}
