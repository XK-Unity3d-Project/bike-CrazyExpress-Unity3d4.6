using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;
public class ChangeMode : MonoBehaviour {
	public GameObject SelectObj;
	public GameObject soloMode;
	public GameObject onlineMode;
	public UISprite waitSprite;
	public AudioClip ChangeModeMusic;

	private UISprite soloModeUI;
	private UISprite onlineModeUI;

	private TweenRotation soloHover;
	private TweenRotation soloUnhover;
	private TweenRotation onlineHover;
	private TweenRotation onlineUnHover;
	private bool isnew;

	bool isChangeWait = false;
	//private TweenRotation tween1;

	// Use this for initialization
	void Start () {
		IsClickModeStart = false;
		GameTextVal = GlobalData.GetGameTextMode();
		soloModeUI=soloMode.GetComponent<UISprite>() as UISprite;
		onlineModeUI=onlineMode.GetComponent<UISprite>() as UISprite;
		soloHover=soloMode.GetComponents<TweenRotation>()[0]as TweenRotation;
		soloUnhover=soloMode.GetComponents<TweenRotation>()[1] as TweenRotation;
		onlineHover=onlineMode.GetComponents<TweenRotation>()[0] as TweenRotation;
		onlineUnHover=onlineMode.GetComponents<TweenRotation>()[1] as TweenRotation;
		
		if (GameTextVal == GameTextType.English) {
			soloModeUI.spriteName="QuWeiModeHui_En";
			onlineModeUI.spriteName="LinkModeHui_En";
			waitSprite.spriteName = "wait_0";
		}
		else {
			soloModeUI.spriteName="QuWeiModeHui";
			onlineModeUI.spriteName="LinkModeHui";
			waitSprite.spriteName = "wait_0_En";
		}
		isnew =true;
		//SelectMode();
		InputEventCtrl.GetInstance().ClickStartBtEvent += ClickStartBtEvent;
	}
	
	GameTextType GameTextVal = GlobalData.GameTextVal;
	void SelectMode()
	{
		switch(GlobalData.GetInstance().gameMode)
		{
		case GameMode.SoloMode:
			if (GameTextVal == GameTextType.Chinese) {
				soloModeUI.spriteName="QuWeiMode";
				onlineModeUI.spriteName="LinkModeHui";
			}
			else {
				soloModeUI.spriteName="QuWeiMode_En";
				onlineModeUI.spriteName="LinkModeHui_En";
			}

			if(!isnew)
			{
				//soloUnhover.ResetToBeginning();
				soloHover.PlayForward();

				//soloUnhover.enabled=false;

				onlineUnHover.PlayForward();
				//onlineHover.enabled=false;
			}
			break;

		case GameMode.OnlineMode:
			if (GameTextVal == GameTextType.Chinese) {
				soloModeUI.spriteName="QuWeiModeHui";
				onlineModeUI.spriteName="LinkMode";
			}
			else {
				soloModeUI.spriteName="QuWeiModeHui_En";
				onlineModeUI.spriteName="LinkMode_En";
			}

			if(!isnew)
			{
				onlineHover.PlayForward();
				//onlineHover.Play();
				soloUnhover.PlayForward();
			}
			break;
		}
	}

	void InitChangeWait()
	{
		isChangeWait = true;
		waitSprite.enabled = true;
	}

	void resetChangeWait()
	{
		isChangeWait = false;
		waitSprite.enabled = false;
		if (GameTextVal == GameTextType.Chinese) {
			waitSprite.spriteName = "wait_1";
		}
		else {
			waitSprite.spriteName = "wait_1_En";
		}
	}

	void ChangeWaitSprit()
	{
		if(!isChangeWait)
		{
			CancelInvoke("ChangeWaitSprit");
			return;
		}

		if (GameTextVal == GameTextType.Chinese) {
			if(waitSprite.spriteName == "wait_1")
			{
				waitSprite.spriteName = "wait_0";
			}
			else
			{
				waitSprite.spriteName = "wait_1";
			}
		}
		else {
			if(waitSprite.spriteName == "wait_1_En")
			{
				waitSprite.spriteName = "wait_0_En";
			}
			else
			{
				waitSprite.spriteName = "wait_1_En";
			}
		}
	}

	public static bool IsClickModeStart;
	void ClickStartBtEvent(ButtonState val)
	{
		//ScreenLog.Log("ChangeMode::ClickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			if (Network.peerType != NetworkPeerType.Client) {
				NetworkServerNet.GetInstance().TryToLinkServer();
			}
			InitChangeWait();
			CancelInvoke("ChangeWaitSprit");
			InvokeRepeating("ChangeWaitSprit", 0.0f, 0.4f);
		}
		else {
			IsClickModeStart = true;
		}
	}

	float selectTime = 10.0f;
	// Update is called once per frame
	void Update ()
	{
		//ChangeWaitSprit();
		
		selectTime += Time.deltaTime;
		if (selectTime < 2.0f) {
			return;
		}
		selectTime = 2f;

		float hor = InputEventCtrl.PlayerFX[0];
//		if(((Input.GetButtonDown("Horizontal") && (!pcvr.bIsHardWare || pcvr.IsTestGetInput))
//		    || (pcvr.bIsHardWare && Mathf.Abs(pcvr.mBikeSteer) > 0.3f))
//		    && !StartSenceChangeUI.IsSelectGameMode)
		if(Mathf.Abs(hor) > 0.3f && !StartSenceChangeUI.IsSelectGameMode)
		{
			if(SelectObj.activeSelf)
			{
				SelectObj.SetActive(false);
				pcvr.StartLightStateP1 = LedState.Shan;
			}

//			float hor = Input.GetAxis("Horizontal");
//			if(pcvr.bIsHardWare && !pcvr.IsTestGetInput)
//			{
//				hor = pcvr.mBikeSteer;
//			}

			//AudioManager.Instance.PlaySFX(ChangeModeMusic);
			if(hor > 0f)
			{
				if(GlobalData.GetInstance().gameMode != GameMode.OnlineMode || selectTime > 2.0f)
				{
					selectTime = 0.0f;
					AudioManager.Instance.PlaySFX(ChangeModeMusic);
				}
				GlobalData.GetInstance().gameMode = GameMode.OnlineMode;
				IsClickModeStart = false;
			}
			else if(hor < 0f)
			{
				if(GlobalData.GetInstance().gameMode != GameMode.SoloMode || selectTime > 2.0f)
				{
					selectTime = 0.0f;
					AudioManager.Instance.PlaySFX(ChangeModeMusic);
				}
				GlobalData.GetInstance().gameMode = GameMode.SoloMode;
				resetChangeWait();
				IsClickModeStart = true;
			}

			isnew=false;
			soloHover.ResetToBeginning();
		    soloUnhover.ResetToBeginning();
			onlineHover.ResetToBeginning();
			onlineUnHover.ResetToBeginning();
			SelectMode();
		}
	}
}
