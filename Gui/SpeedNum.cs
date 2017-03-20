using UnityEngine;
using System.Collections;

public class SpeedNum : MonoBehaviour {
	public GameObject ge;
	public GameObject shi;
	private UISprite geUi;
	private UISprite shiUi;
	// Use this for initialization
	void Start () {
		geUi=ge.GetComponent<UISprite>();
		shiUi=shi.GetComponent<UISprite>();
		GlobalScript.GetInstance().player.SpeedChange+=SpeedChange;
	}

	public void SpeedChange()
	{
		NumtoImg(GlobalScript.GetInstance().player.Speed);
	}
	public void NumtoImg(int num)
	{
		if(num<10)
		{
			//shi.SetActive(false);
			shiUi.spriteName = "sd" + 0;
			geUi.spriteName = "sd" + num%10;
		}
		else
		{
			//shi.SetActive(true);
			shiUi.spriteName = "sd" + (num / 10);
			geUi.spriteName = "sd" + (num % 10);
		}
	}

}
