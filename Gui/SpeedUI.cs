using UnityEngine;
using System.Collections;

public class SpeedUI : MonoBehaviour {
	public GameObject Speed;
	public GameObject speedF;
	public GameObject Energy;
	public GameObject EnergyF;
	private UISprite speedUi;
	private UISprite EnergyUi;
	private TweenAlpha tweenAlpha;
	private TweenAlpha tweenAlpha1;
	private bool faguangzhe;
	private bool HashowTishi=true;
	public static bool IsNotSubYouLiang = false;
	// Use this for initialization
	void Awake()
	{
		IsNotSubYouLiang = pcvr.IsTestGetInput;
		speedUi=Speed.GetComponent<UISprite>();
		EnergyUi=Energy.GetComponent<UISprite>();
		EnergyUi.fillAmount = 0.8f;

		GlobalScript.GetInstance().player.SpeedChange+=SpeedChange;
		GlobalScript.GetInstance().player.EnergyChange+=EnergyChange;
		GlobalScript.GetInstance().player.AddBufferEvent+=AddBufferEvent;
		tweenAlpha= EnergyF.GetComponent<TweenAlpha>();
		tweenAlpha1= speedF.GetComponent<TweenAlpha>();
	}

	public void SpeedChange()
	{
		if( (Application.loadedLevel == (int)GameLeve.Leve3 || Application.loadedLevel == (int)GameLeve.Leve4)
		   	&& Network.isServer )
		{
			return;
		}

		if (GlobalScript.GetInstance().player.Speed < 20 ) {
			if(!HashowTishi)
			{
				GlobalScript.GetInstance ().ShowTishi (TishiInfo.Sudu);
				HashowTishi = true;
			}
		}
		else 
		{
			HashowTishi=false;
		}
		float s=GlobalScript.GetInstance().player.Speed/60.0f;
		speedUi.fillAmount=s;
		//Debug.Log("faguag"+faguangzhe);
		if(s>0.75)
		{
			speedF.GetComponent<UISprite>().alpha=1;
			faguangzhe=true;
		}
		else if(faguangzhe)
		{
			tweenAlpha1.from=1;
			tweenAlpha1.to=0;
			tweenAlpha1.ResetToBeginning();
			tweenAlpha1.PlayForward();
			faguangzhe=false;
		}
	}
	private void AddBufferEvent(BufferKind kind)
	{
		if(kind==BufferKind.Dianchi)
		{
			tweenAlpha.from=0;
			tweenAlpha.to=1;
			tweenAlpha.ResetToBeginning();
			tweenAlpha.PlayForward();
			StartCoroutine("keep");
		}
	}
	public IEnumerator keep()
	{
		yield return new WaitForSeconds(1);
		tweenAlpha.from=1;
		tweenAlpha.to=0;
		tweenAlpha.ResetToBeginning();
		tweenAlpha.PlayForward();
		//	EnergyF.SetActive(false);

	}
	public void EnergyChange()
	{
		float s=((float)GlobalScript.GetInstance().player.Energy)/100.0f;
		EnergyUi.fillAmount=s*0.78f;
//		Debug.Log("ssssss"+s);
//		if(s==0.79)
//		{
//			EnergyF.SetActive(true);
//		}
//		else
//		{
//			EnergyF.SetActive(false);
//		}
	}
	// Update is called once per frame
//	void Update () {
//	
//	
//	}
}
