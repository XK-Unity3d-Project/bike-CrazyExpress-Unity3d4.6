using UnityEngine;
using System.Collections;
using System.Linq;
using Frederick.ProjectAircraft;
public class ChangeLeve : MonoBehaviour {
	public GameObject SelectObj;
	public GameObject Leve1;
	public GameObject Leve2;

	public Transform CartooCity = null;
	public Transform CartooOutdoor = null;
	private bool isInitCartoon = false;

	public GameObject Cam;
	public GameObject Starts;
	public AudioClip diaoluoAudio;
	public AudioClip BackgroundAudio;
	public AudioClip ChangeLeveAudio;
	public GameObject Loading;
//	private UISprite Leve1UI;
//	private UISprite Leve2UI;
	//Leve1Texture[0] -> diYiGuanHuiSe, Leve1Texture[1] -> diYiGuan, Leve1Texture[2] -> diYiGuanDaTu.
	public Texture[] Leve1Texture;
	//Leve2Texture[0] -> diErGuanHuiSe, Leve2Texture[1] -> diErGuan, Leve2Texture[2] -> diErGuanDaTu.
	public Texture[] Leve2Texture;
	public Texture[] Leve1TextureCh;
	public Texture[] Leve2TextureCh;
	public Texture[] Leve1TextureEn;
	public Texture[] Leve2TextureEn;
	private UITexture Leve1UITexture;
	private UITexture Leve2UITexture;
	//LinkTexture[0] -> LinkCity, LinkTexture[1] -> LinkOutdoor.
	public Texture[] LinkTexture;
	public Texture[] LinkTextureCh;
	public Texture[] LinkTextureEn;
	private TweenRotation Leve1Hover;
	private TweenRotation Leve1Unhover;
	private TweenRotation Leve2Hover;
	private TweenRotation Leve2UnHover;
	private AsyncOperation status;
	private TweenPosition[] tween;
	//private UISprite[] ui;
	private bool finished;
	// Use this for initialization
	void Start()
	{
		GameTextType gameTextVal = GlobalData.GetGameTextMode();
		switch (gameTextVal) {
		case GameTextType.Chinese:
			Leve1Texture = Leve1TextureCh;
			Leve2Texture = Leve2TextureCh;
			LinkTexture = LinkTextureCh;
			break;
			
		case GameTextType.English:
			Leve1Texture = Leve1TextureEn;
			Leve2Texture = Leve2TextureEn;
			LinkTexture = LinkTextureEn;
			break;
		}

		Leve1UITexture=Leve1.GetComponent<UITexture>();
		Leve2UITexture=Leve2.GetComponent<UITexture>();
//		Leve1UI=Leve1.GetComponent<UISprite>() as UISprite;
//		Leve2UI=Leve2.GetComponent<UISprite>() as UISprite;
		Leve1Hover=Leve1.GetComponents<TweenRotation>()[0]as TweenRotation;
		Leve1Unhover=Leve1.GetComponents<TweenRotation>()[1] as TweenRotation;
		Leve2Hover=Leve2.GetComponents<TweenRotation>()[0] as TweenRotation;
		Leve2UnHover=Leve2.GetComponents<TweenRotation>()[1] as TweenRotation;
		
		GlobalData.GetInstance().gameLeve = GameLeve.Leve1;
		Leve1UITexture.mainTexture = Leve1Texture[0];
		Leve2UITexture.mainTexture = Leve2Texture[0];
		//Leve1UI.spriteName="diYiGuanHuiSe";
		//Leve2UI.spriteName="diErGuanHuiSe";

		Leve1Hover.ResetToBeginning();
		Leve2UnHover.ResetToBeginning();

		Leve1Hover.PlayForward();
		Leve2UnHover.PlayForward();

		Starts.SetActive(false);
		pcvr.StartLightStateP1 = LedState.Mie;
		isInitCartoon = false;

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode && NetCtrlScript == null)
		{
			GameObject netCtrl = GameObject.Find(GlobalData.netCtrl);
			if(netCtrl != null)
			{
				NetCtrlScript = netCtrl.GetComponent<NetCtrl>();
			}
		}

		InputEventCtrl.GetInstance().ClickStartBtEvent += clickStartBtEvent;
	}

	public void Init()
	{
		if(isInitCartoon)
		{
			return;
		}
		isInitCartoon = true;

		if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
		{
			tween = CartooCity.GetComponentsInChildren<TweenPosition>();
		}
		else
		{
			tween = CartooOutdoor.GetComponentsInChildren<TweenPosition>();
		}

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			EventDelegate.Add(tween[0].onFinished, delegate{
				Invoke("startIntoGame", 6);
			});
		}
		else
		{
			EventDelegate.Add(tween[tween.Length-1].onFinished, delegate{
				Invoke("startIntoGame", 6);
			});
		}
	}

	void startIntoGame()
	{
		status.allowSceneActivation = true;
	}

	public IEnumerator PlayCartoon()
	{
		Init();

		int i=0;
		int maxNum = tween.Length;

		int [] interval = {1, 2, 3};
		if(GlobalData.GetInstance().gameLeve != GameLeve.Leve1)
		{
			interval[0] = 1;
			interval[1] = 4;
			interval[2] = 4;
		}

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			interval[0] = 3;
			UITexture cartoonSprite = tween[0].GetComponent<UITexture>();
			if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
			{
				cartoonSprite.mainTexture = LinkTexture[0];
			}
			else
			{
				cartoonSprite.mainTexture = LinkTexture[1];
			}
		}

		//ScreenLog.Log("maxNum " + maxNum);
		while(i < maxNum){
			//ScreenLog.Log("interval[i] " + interval[i]);
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode && i >= 1)
			{
				break;
			}
			yield return new WaitForSeconds(interval[i]);

			AudioManager.Instance.PlaySFX(diaoluoAudio);
			tween[i].ResetToBeginning();
			tween[i].PlayForward();
			i++;
			//ScreenLog.Log("i = " + i);
		}

		//ScreenLog.Log("play cartoon end");
		GlobalData.GetInstance().playCartoonEnd=true;
	}

	public void SelectLeve()
	{
		switch(GlobalData.GetInstance().gameLeve)
		{
		case GameLeve.Leve1:
			Leve1UITexture.mainTexture = Leve1Texture[1];
			Leve2UITexture.mainTexture = Leve2Texture[0];
			//Leve1UI.spriteName="diYiGuan";
			//Leve2UI.spriteName="diErGuanHuiSe";
			Leve1Hover.PlayForward();
			Leve2UnHover.PlayForward();
			break;
		case GameLeve.Leve2:
			Leve1UITexture.mainTexture = Leve1Texture[0];
			Leve2UITexture.mainTexture = Leve2Texture[1];
			//Leve1UI.spriteName="diYiGuanHuiSe";
			//Leve2UI.spriteName="diErGuan";
			Leve1Unhover.PlayForward();
			Leve2Hover.PlayForward();
			break;
		}
	}

	static public bool IsCanActiveSetPanel = true;
	public bool bIsSelectLeve = false;
	private AudioSource AudioSourceObj = null;
	
	void clickStartBtEvent(ButtonState val)
	{
		//ScreenLog.Log("ChangeLeve::clickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		
		if(!bIsClickStartBt)
		{
			return;
		}
		clickStartBt( 0 );

		InputEventCtrl.GetInstance().ClickStartBtEvent -= clickStartBtEvent;
	}

	float selectTime = 10.0f;
	public bool bIsClickStartBt = false;
	// Update is called once per frame
	void Update ()
	{
		if (bIsSelectLeve) {
			return;
		}

		selectTime += Time.deltaTime;
		if (StartSenceChangeUI.IsChangeGameLevel) {
			float hor = InputEventCtrl.PlayerFX[0];
			if (Mathf.Abs(hor) > 0.3f) {
				if (SelectObj.activeSelf) {
					SelectObj.SetActive(false);
					Starts.SetActive(true);
					pcvr.StartLightStateP1 = LedState.Shan;
				}
				bIsClickStartBt = true;

				if(hor > 0f)
				{
					if(GlobalData.GetInstance().gameLeve != GameLeve.Leve2 || selectTime > 2.0f)
					{
						selectTime = 0.0f;
						AudioManager.Instance.PlaySFX(ChangeLeveAudio);
					}
					GlobalData.GetInstance().gameLeve = GameLeve.Leve2;
					//ScreenLog.Log("1      " + GlobalData.GetInstance().gameLeve);
				}
				else if(hor < 0f)
				{
					if(GlobalData.GetInstance().gameLeve != GameLeve.Leve1 || selectTime > 2.0f)
					{
						selectTime = 0.0f;
						AudioManager.Instance.PlaySFX(ChangeLeveAudio);
					}
					GlobalData.GetInstance().gameLeve = GameLeve.Leve1;
					//ScreenLog.Log("2     " + GlobalData.GetInstance().gameLeve);
				}
				Leve1Hover.ResetToBeginning();
				Leve1Unhover.ResetToBeginning();
				Leve2Hover.ResetToBeginning();
				Leve2UnHover.ResetToBeginning();
				SelectLeve();
			}
		}

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode && NetCtrlScript.GetSelectGameLevel() != GameLeve.None)
		{
			timeDelay += Time.deltaTime; //make server first into game
			if(timeDelay >= 0.5f && !bIsSelectLeve)
			{
				ScreenLog.Log("OnlineMode -> clickStartBt...");
				bIsClickStartBt = true;
				clickStartBt( 1 );
			}
		}
	}

	float timeDelay = 0.0f;
	
	NetCtrl NetCtrlScript;

	void clickStartBt(int key)
	{
		if(!bIsClickStartBt)
		{
			return;
		}

		if(bIsSelectLeve)
		{
			return;
		}
		bIsClickStartBt = false;
		bIsSelectLeve = true;
		IsCanActiveSetPanel = false;

		//reset IsSelectGameMode
		StartSenceChangeUI.IsSelectGameMode = false;
		if( (Starts.activeSelf && key == 0) || key == 1 )
		{
			Starts.SetActive(false);
			if (GlobalData.GetInstance().LinkModeState != 0) {
				Toubi.GetInstance().subPlayerCoin();
			}

			pcvr.StartLightStateP1 = LedState.Mie;
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				LinkPlayerNameCtrl.GetInstance().HiddenPlayerInfo();
				if(key == 0)
				{
					NetCtrlScript.handleSelectLevel( (int)GlobalData.GetInstance().gameLeve );
				}
				else
				{
					SelectObj.SetActive(false);
					GlobalData.GetInstance().gameLeve = NetCtrlScript.GetSelectGameLevel();
					NetCtrlScript.handleResetLinkCount();
					//ScreenLog.Log("********** level " + GlobalData.GetInstance().gameLeve);
				}
			}
			
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode && key == 0)
			{
				NetCtrlScript.handleLoadLevel();
			}

			if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
			{
				Leve2.SetActive(false);
				StopAllCoroutines();
				Leve1.GetComponent<TweenPosition>().enabled=false;
				Leve1Hover.enabled=false;
				Leve1Unhover.enabled=false;
				//Leve1UI.spriteName="diYiGuanDaTu";
				//Leve1UITexture.mainTexture = Leve1Texture[2];
				Leve1.transform.position=Vector3.zero;
				Leve1.transform.eulerAngles=Vector3.zero;
				TweenScale scale = Leve1.GetComponent<TweenScale>();
				scale.PlayForward();
				EventDelegate.Add(scale.onFinished,delegate {
					//AudioManager.Instance.PlayBGM(BackgroundAudio,true);
					AudioSourceObj = AudioManager.Instance.audio;
					AudioSourceObj.clip = BackgroundAudio;
					AudioSourceObj.loop = true;
					AudioSourceObj.Play();
					
					InvokeRepeating("changeBackgroundSdVol", 0f, 1.5f);
					StartCoroutine("ColorCorrection");
				});
			}
			else if(GlobalData.GetInstance().gameLeve == GameLeve.Leve2)
			{
				Leve1.SetActive(false);
				StopAllCoroutines();
				Leve2.GetComponent<TweenPosition>().enabled=false;
				Leve2Hover.enabled=false;
				Leve2UnHover.enabled=false;
				//Leve2UITexture.mainTexture = Leve2Texture[2];
				//Leve2UI.spriteName="diErGuanDaTu";
				Leve2.transform.position=Vector3.zero;
				Leve2.transform.eulerAngles=Vector3.zero;
				TweenScale scale = Leve2.GetComponent<TweenScale>();
				scale.PlayForward();
				EventDelegate.Add(scale.onFinished,delegate {
					AudioSourceObj = AudioManager.Instance.audio;
					AudioSourceObj.clip = BackgroundAudio;
					AudioSourceObj.loop = true;
					AudioSourceObj.Play();
					
					InvokeRepeating("changeBackgroundSdVol", 0f, 1.5f);
					StartCoroutine("ColorCorrection");
				});
			}
		}

	}

	void changeBackgroundSdVol()
	{
		////ScreenLog.Log("AudioSourceObj.volum " + AudioSourceObj.volume);
		AudioSourceObj.volume -= 0.1f;
		if(AudioSourceObj.volume <= 0f)
		{
			CancelInvoke("changeBackgroundSdVol");
		}
	}

	public IEnumerator ColorCorrection()
	{
		AudioManager.Instance.PlaySFX(BackgroundAudio);
		ColorCorrectionCurves s=(ColorCorrectionCurves)Cam.GetComponent("ColorCorrectionCurves");

		while(s.saturation>0)
		{
			yield return 0;
			s.saturation-=Time.deltaTime;
		}
		s.saturation = 0;

		StartCoroutine("LoadLeve");
	}

	//int loadLevelCount = 0;

	public IEnumerator LoadLeve()
	{
		XkGameCtrl.IsLoadingLevel = true;
		if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
		{
			Leve1UITexture.mainTexture = Leve1Texture[0];
			//Leve1UI.spriteName = "diYiGuanHuiSe";
			Leve1.transform.localScale *= 1.00f;
		}
		else
		{
			Leve2UITexture.mainTexture = Leve2Texture[0];
			//Leve2UI.spriteName = "diErGuanHuiSe";
			Leve2.transform.localScale *= 1.00f;
		}

		ColorCorrectionCurves s=(ColorCorrectionCurves)Cam.GetComponent<ColorCorrectionCurves>();
		s.saturation = 1f;
		
		//System.GC.Collect();

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			//ScreenLog.Log("into linkMode game...");
			//NetCtrlScript.handleLoadLevel();
		
			yield return new WaitForSeconds(3f);
			
			if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
			{
				status = Application.LoadLevelAsync((int)GameLeve.Leve3);
			}
			else
			{
				status = Application.LoadLevelAsync((int)GameLeve.Leve4);
			}
		}
		else
		{
			yield return new WaitForSeconds(2f);
			status = Application.LoadLevelAsync((int)GlobalData.GetInstance().gameLeve);
		}
		status.allowSceneActivation = false;
		// Allow new scene to start.
		//ScreenLog.Log("Loading complete"+t+status.isDone);

		Loading.SetActive(true);

		yield return StartCoroutine("PlayCartoon");
		//status.allowSceneActivation = true;
		yield return new WaitForSeconds(3f);
		yield return status;
	}

	public IEnumerator PlayAnimation()
	{
		yield return new WaitForSeconds(3);
	}
}
