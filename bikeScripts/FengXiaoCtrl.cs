using UnityEngine;
using System.Collections;

public class FengXiaoCtrl : MonoBehaviour {

	public static bool IsPlayFengXiao = false;
	UISprite FengSprite;
	// Use this for initialization
	void Start () {
		FengSprite = transform.GetComponent<UISprite>();

		InvokeRepeating("PlayFengXiao", 0.0f, 0.05f);
	}

	int count = 0;

	// Update is called once per frame
	void PlayFengXiao () {

		if(!IsPlayFengXiao)
		{
			if(FengSprite.enabled)
			{
				FengSprite.enabled = false;
			}
			return;
		}
		else
		{
			if(!FengSprite.enabled)
			{
				FengSprite.enabled = true;
			}
		}

		count++;
		switch(count)
		{
		case 1:
			FengSprite.spriteName = "texiao5";
			break;

		case 2:
			FengSprite.spriteName = "texiao6";
			break;

		case 3:
			FengSprite.spriteName = "texiao7";
			count = 0;
			break;
		}
	}
}
