using UnityEngine;
using System.Collections;

public class DaojishiHaomiao : MonoBehaviour {
	public UISprite miaoUI;
	public UISprite hmshiUI;
	public UISprite hmbaiUI;

	private TweenPosition Tpos;
	private TweenAlpha[] Talps;
	private int mm = 1000;
	bool bIsOpen = false;

	// Use this for initialization
	void Start () {
		Tpos = GetComponent<TweenPosition>();
		Talps = GetComponentsInChildren<TweenAlpha>();
		InvokeRepeating("CheckLifeLoop", 0, 0.5f);
	}

	IEnumerator daojishi()
	{
		while(GlobalScript.GetInstance().player.Life <= 10 && GlobalScript.GetInstance().player.Life > 0)
		{
			if(GlobalScript.GetInstance().player.IsPlayHuanHu)
			{
				yield break;
			}
			yield return new WaitForSeconds(0.01f);
			
			mm -= 10;
			miaoUI.spriteName = "djs" + (GlobalScript.GetInstance().player.Life-1);
			hmbaiUI.spriteName = "djs" + mm/100;
			hmshiUI.spriteName = "djs" + (mm%100)/10;
			if(mm == 0)
			{
				mm = 1000;
			}
		}
		
		miaoUI.spriteName="djs"+0;
		hmbaiUI.spriteName="djs"+0;
		hmshiUI.spriteName="djs"+0;
		yield return new WaitForSeconds(0.01f);

		FadeOut();
	}

	public void FadeOut()
	{
		for(int i=0;i<Talps.Length;i++)
		{
			Talps[i].from=1;
			Talps[i].to=0;
			Talps[i].ResetToBeginning();
			Talps[i].PlayForward();
		}

		bIsOpen = false;
	}

	private void FadeIn()
	{
		if(bIsOpen)
		{
			return;
		}
		bIsOpen = true;

		StartCoroutine("daojishi");
		PositionFadeIn();
		alpFadeIn();
	}

	private void PositionFadeIn()
	{
		Tpos.ResetToBeginning();
		Tpos.PlayForward();
	}

	private void alpFadeIn()
	{
		for(int i=0;i<Talps.Length;i++)
		{
			Talps[i].from=0;
			Talps[i].to=1;
			Talps[i].ResetToBeginning();
			Talps[i].PlayForward();
		}
	}

	void CheckLifeLoop () {
		if(Network.isServer)
		{
			CancelInvoke("CheckLifeLoop");
			return;
		}

		if(GlobalScript.GetInstance().player.Life == 10)
		{
			FadeIn();
		}
	}
}
