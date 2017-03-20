using UnityEngine;
using System.Collections;

public class LostTime : Timer {
	public string NamePrefix;
	// Use this for initialization
	void Awake()
	{

			fenshiUI=fenshi.GetComponent<UISprite>();
			fengeUI=fenge.GetComponent<UISprite>();
			miaoshiUI=miaoshi.GetComponent<UISprite>();
			miaogeUI=miaoge.GetComponent<UISprite>();
		
		GlobalScript.GetInstance ().ShowLosttimeEvent += ShowLosttimeEvent;
	}
	void ShowLosttimeEvent()
	{
		//Debug.Log("++++++++++++"+GlobalScript.GetInstance().player.LostTime);
		PR=NamePrefix;
		intToTimerImage(GlobalScript.GetInstance().player.LostTime);
	
	}

//	// Update is called once per frame
//	void Update () {
//	
//	}
}
