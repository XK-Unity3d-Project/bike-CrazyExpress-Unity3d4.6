using UnityEngine;
using System.Collections;

using System;

using Frederick.ProjectAircraft;

public class caipiao : MonoBehaviour {
	public GameObject shi;
	public GameObject ge;
	public float time;
	private UISprite shiUi;
	private UISprite geUi;
	public float y=10; //coin to card num
	public float z=1; //output card tickets

	public AudioClip CaiPiaoAudio = null;
	
	private TweenPosition tweenPosScript = null;
//	private pcvr pcvrScript = null;

	public UISprite buZu;
	public UISprite buZuGuang;
	GameObject buZuObj;

//	public UISprite jiXu;
//	public UISprite jiXuGuang;
	//GameObject jiXuObj;
	bool bIsInitBuZu = false;

	public void ResetCaiPiaoBuZu()
	{
		if(!bIsInitBuZu)
		{
			return;
		}
		
		bIsInitBuZu = false;
		CancelInvoke("PlayCaiBuZu");

		if(buZuObj == null)
		{
			buZuObj = buZu.gameObject;
		}
		
//		if(jiXuObj == null)
//		{
//			jiXuObj = jiXu.gameObject;
//		}
		
		buZuObj.SetActive(false);
		//jiXuObj.SetActive(false);

		buZuGuang.enabled = false;
		//jiXuGuang.enabled = false;
	}

	void PlayCaiBuZu()
	{
		if(!bIsInitBuZu)
		{
			CancelInvoke("PlayCaiBuZu");
			return;
		}
		buZuGuang.enabled = !buZuGuang.enabled;
		//jiXuGuang.enabled = !jiXuGuang.enabled;
	}

	public void InitCaiPiaoBuZu()
	{
		if(bIsInitBuZu)
		{
			return;
		}

		bIsInitBuZu = true;
		if(buZuObj == null)
		{
			buZuObj = buZu.gameObject;
		}

//		if(jiXuObj == null)
//		{
//			jiXuObj = jiXu.gameObject;
//		}

		buZuObj.SetActive(true);
		//jiXuObj.SetActive(true);
		InvokeRepeating("PlayCaiBuZu", 1.0f, 0.8f);
	}

	private static caipiao _Instance;
	public static caipiao GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start () {
		_Instance = this;
		PlayerCreatNet.IsBackMoveScence = false;

		tweenPosScript = GetComponent<TweenPosition>();
		//BanckToStartSence();
		GlobalScript.GetInstance().ShowCaipiaoEvent += ShowCaipiaoEvent;
	}

	private void ShowCaipiaoEvent()
	{
		GlobalScript.GetInstance().player.lotteryCountChangeEvent+=lotteryCountChangeEvent;

//		y = GlobalData.GetInstance().CointToTicket;
//		z = 1f;
//		switch(GlobalData.GetInstance().TicketRate)
//		{
//		case "0.8":
//			z = 0.8f;
//			break;
//
//		case "1.2":
//			z = 1.2f;
//			break;
//		}

//	 	int s = (int)(((float)GlobalScript.GetInstance().player.Score) * y * z / 4200.0f);
//		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
//		{
//			int rankNum = GlobalScript.GetInstance().player.FinalRank;
//			float tickRate = z;
//			float rankPer = 0.0f;
//			if(rankNum < 1)
//			{
//				rankPer = 0.8f;
//			}
//			else if(rankNum < 4)
//			{
//				rankPer = 0.5f;
//			}

//			s = (int) (rankPer * GlobalData.GetInstance().CointToTicket * tickRate);
//			ScreenLog.Log("rankNum " + rankNum + ", cardNum " + s);
//		}

//		if(!pcvr.bIsHardWare)
//		{
//			s = 0; //test
//		}
//		s = 0; //Remove CaiPiao
//		s = 10; //test
//		InitCaiPiaoBuZu(); //test

		StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo(0) );
		pcvr.GetInstance().setFengShanInfo(0, 0);
		pcvr.GetInstance().setFengShanInfo(0, 1);

//		if(!GlobalData.GetInstance().IsOutputCaiPiao || GlobalData.GetInstance().IsFreeMode)
//		{
//			s = 0;
//		}
//		AudioManager.Instance.PlaySFX( CaiPiaoAudio );

//		GlobalScript.GetInstance ().player.LotteryCount = s;
//		if(s > 0)
//		{
//			pcvr.GetInstance().setPrintCardNum(s);
//		}
	 	ShowLotteryCount ();
		tweenPosScript.PlayForward();
		tweenPosScript.duration = 0.2f;
		tweenPosScript.to = tweenPosScript.from;
		
		EventDelegate.Add(tweenPosScript.onFinished, onFinishedTweenPos);
	}

	void onFinishedTweenPos()
	{
		//ScreenLog.Log("onFinishedTweenPos -> caiPiao");
//		if(GlobalData.GetInstance().IsOutputCaiPiao)
//		{
//			if(pcvrScript == null)
//			{
//				GameObject pcvrObj = GameObject.Find("_PCVR");
//				pcvrScript = pcvrObj.GetComponent<pcvr>();
//				//ScreenLog.Log("caiPiao find pcvr");
//			}
////			pcvrScript.setPrintCardNum( GlobalScript.GetInstance ().player.LotteryCount );
//		}

		GlobalScript.GetInstance().Invoke("ShowFinalRank", 2f);
		EventDelegate.Remove(tweenPosScript.onFinished, onFinishedTweenPos);
	}

	void lotteryCountChangeEvent()
	{
		ShowLotteryCount ();
	}

	public void BanckToStartSence()
	{
		Invoke("Back",time);
	}

	public void Back()
	{
		if(GlobalScript.GetInstance().player.LotteryCount > 0)
		{
			//ScreenLog.Log("caiPiao Back test...");
			Invoke("Back", 2f);
			return;
		}

		if(GlobalData.GetInstance().gameMode != GameMode.OnlineMode)
		{
			bike.resetBikeStaticInfo();
		}
		else
		{
			bikeNetUnity.resetBikeStaticInfo();
		}
		Invoke("loadStartLevel", 3.0f);
	}

	public static void OnPlayerGameOver()
	{	
		PlayerCreatNet.IsDisconnected = true;
		PlayerCreatNet.IsBackMoveScence = true;
		NetworkServerNet netScript = NetworkServerNet.GetInstance();
		if(netScript != null) {
			netScript.SetIsDisconnect();
		}
	}

	void loadStartLevel()
	{
		XkGameCtrl.IsLoadingLevel = true;
		//System.GC.Collect();
		Application.LoadLevel( (int)GameLeve.Movie );
	}

	public void ShowLotteryCount()
	{
		shiUi=shi.GetComponent<UISprite>();
		geUi=ge.GetComponent<UISprite>();
		if(GlobalScript.GetInstance().player.LotteryCount>100)
		{
			shiUi.spriteName="c"+9;
			geUi.spriteName="c"+9;
		}
		else
		{
			shiUi.spriteName="c"+GlobalScript.GetInstance().player.LotteryCount/10;
			geUi.spriteName="c"+GlobalScript.GetInstance().player.LotteryCount%10;
		}
	}

	public void handlePrintCardState(string state)
	{
		switch(state)
		{
		case "2":
			GlobalScript.GetInstance ().player.LotteryCount--;
			ShowLotteryCount();
			break;
		}
	}
}
