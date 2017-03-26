using UnityEngine;
using System.Collections;

public class DianLiangCtrl : MonoBehaviour {

	public GameObject DianLiangShan;
	public UISprite DianLiangBack;

	bool IsShowDianLiang = false;
	float dianLiangValMin = 20.0f;
	int Count = 0;
	
	bool IsHandleYouMen = false;
	float throttle = 0.0f;

	// Use this for initialization
	void Start () {
		DianLiangBack.enabled = false;
		DianLiangShan.SetActive(false);

		//InvokeRepeating("ShowDianLiangShan", 0.0f, 0.2f);
	}

	void ShowDianLiangShan()
	{
		Count++;
		if((IsShowDianLiang && Count >= 25) || (IsHandleYouMen && Count >= 12))
		{
			Count = 0;
			IsHandleYouMen = false;

			CancelInvoke("ShowDianLiangShan");
			DianLiangShan.SetActive(false);
			DianLiangBack.enabled = false;
			return;
		}

		if(Count % 2 == 0)
		{
			DianLiangShan.SetActive(false);
		}
		else
		{
			DianLiangShan.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (GlobalScript.GetInstance().player.Life <= 0) {
			if (IsInvoking("ShowDianLiangShan")) {
				CancelInvoke("ShowDianLiangShan");
				Count = 0;
				IsHandleYouMen = false;
				DianLiangShan.SetActive(false);
				DianLiangBack.enabled = false;
			}
			return;
		}

		if(GlobalScript.GetInstance().player.Energy > dianLiangValMin && IsInvoking("ShowDianLiangShan"))
		{
			Count = 50; //close ShowDianLiangShan
			IsHandleYouMen = true;
			if((IsShowDianLiang && Count >= 25) || (IsHandleYouMen && Count >= 12))
			{
				Count = 0;
				IsHandleYouMen = false;
				
				CancelInvoke("ShowDianLiangShan");
				DianLiangShan.SetActive(false);
				DianLiangBack.enabled = false;
			}
			return;
		}

		if(GlobalScript.GetInstance().player.Energy <= dianLiangValMin && !IsShowDianLiang)
		{
			IsShowDianLiang = true;
			DianLiangBack.enabled = true;
			CancelInvoke("ShowDianLiangShan");
			InvokeRepeating("ShowDianLiangShan", 0.0f, 0.2f);
		}
		else if(GlobalScript.GetInstance().player.Energy > dianLiangValMin && IsShowDianLiang)
		{
			IsShowDianLiang = false;
			DianLiangBack.enabled = false;
			DianLiangShan.SetActive(false);
		}
		
		throttle = InputEventCtrl.PlayerYM[0];
//		if(pcvr.bIsHardWare)
//		{
//			throttle = pcvr.mBikeThrottle;
//		}
//		else
//		{
//			throttle = Input.GetAxis("Vertical");
//		}

		if(GlobalScript.GetInstance().player.Energy <= 3.0f && !IsHandleYouMen && throttle >= 0.3f)
		{
			IsHandleYouMen = true;
			DianLiangBack.enabled = true;
			CancelInvoke("ShowDianLiangShan");
			InvokeRepeating("ShowDianLiangShan", 0.0f, 0.2f);
		}
	}
}
