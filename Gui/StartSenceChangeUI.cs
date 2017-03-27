using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Frederick.ProjectAircraft;
public class StartSenceChangeUI : MonoBehaviour {
	public GameObject TouBiObj;
	private Toubi TouBiScript;

	public GameObject SelectObj;
	public GameObject backCam = null;
	public GameObject Mode;
	public GameObject Leve;
	public GameObject Starts;
	public int timer;
	public GameObject lable;
	public AudioClip startAudio;
	public AudioClip modeFallAudio;
	public AudioClip modeSelectAudio;
	//private float t;
	private bool CanAddMusic=true;
	//private bool CanStartAct=true;
	private bool isLoading;
	private AsyncOperation status;
	public float Sfrom;
	public float STo;
	public float Duation;
	public GameObject backgroud_1;
	public GameObject backgroud_2;
	static StartSenceChangeUI _Instance;
	public static StartSenceChangeUI GetInstance()
	{
		return _Instance;
	}
	// Use this for initialization
	void Start()
	{
		_Instance = this;
		IsChangeGameMode = true;
		IsChangeGameLevel = true;
		GlobalData.GetInstance().gameMode = GameMode.None;
		if(TouBiObj != null)
		{
			TouBiScript = TouBiObj.GetComponent<Toubi>();
		}
		SelectObj.SetActive(false);
		ChangeLeve.SetInstance(Leve);

		if(GlobalData.GetInstance().IsFreeMode)
		{
			timer = 1;
			Starts.SetActive(false);
			pcvr.StartLightStateP1 = LedState.Mie;
		}
		else
		{
			Starts.SetActive(true);
			pcvr.StartLightStateP1 = LedState.Shan;
		}

		if(!GlobalData.GetInstance().IsFreeMode)
		{
			Mode.SetActive(false);
		}
		else
		{
			SelectObj.SetActive(true);
			Mode.SetActive(true);
		}
		Leve.SetActive(false);
		StartCoroutine("Timer");

		if(NetCtrlScript == null)
		{
			GameObject netCtrl = GameObject.Find(GlobalData.netCtrl);
			if(netCtrl != null)
			{
				NetCtrlScript = netCtrl.GetComponent<NetCtrl>();
			}
		}

		//单机游戏,跳过联机或单机选择界面.
		if (GlobalData.GetInstance().LinkModeState == 1) {
			Starts.SetActive(true);
			Mode.SetActive(true);
			GlobalData.GetInstance().gameMode = GameMode.SoloMode;
			ClickStartBtEvent( ButtonState.UP );
		}

		InputEventCtrl.GetInstance().ClickStartBtEvent += ClickStartBtEvent;
	}
	/// <summary>
	/// zi dong jin ru mo shi xuan ze
	/// </summary>
	private IEnumerator Timer()
	{
		while(timer>0)
		{

			yield return new WaitForSeconds(1);
			timer--;
			if (!Starts.activeSelf && (Mode.activeSelf || Leve.activeSelf)) {
				yield break;
			}

			if(timer == 0 && Starts.activeSelf && !Mode.activeSelf && !Leve.activeSelf)
			{
				//auto an kai shi jian
				TouBiScript.subPlayerCoin();
				SelectObj.SetActive(true);
				Starts.SetActive(false);
				Mode.SetActive(true);
				pcvr.StartLightStateP1 = LedState.Mie;
			}
		}
	}
	/// <summary>
	/// bei jing tui se 
	/// </summary>
	IEnumerator Tuise()
	{
		Starts.SetActive (false);
		Mode.SetActive (true);
		pcvr.StartLightStateP1 = LedState.Mie;

		Invoke("playModeFallAudio", 0.5f);
		yield return 0;
	}

	void playModeFallAudio()
	{
		// mo shi xuan ze diao luo shi de yin xiao
		AudioManager.Instance.PlaySFX (modeFallAudio);
	}

	public static bool IsSelectGameMode = false;
	bool isHandleNetLink = false;
	NetCtrl NetCtrlScript;
	void resetIsSelectGameMode()
	{
		IsSelectGameMode = false;
	}

	void ClickStartBtEvent(ButtonState val)
	{
		//ScreenLog.Log("StartSenceChangeUI::ClickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		
		if(Starts.activeSelf && !Leve.activeSelf)
		{
			AudioManager.Instance.PlaySFX(startAudio);
		}
		
		if(Starts.activeSelf && !Mode.activeSelf && !Leve.activeSelf)
		{
			//an kai shi jian
			TouBiScript.subPlayerCoin();
			
			StartCoroutine( ColorCorrection(0) );
			
			Starts.SetActive(false);
			pcvr.StartLightStateP1 = LedState.Mie;
			StopCoroutine("Timer");
			StartCoroutine("Tuise");
		}
		else if(Starts.activeSelf&&Mode.activeSelf)
		{
			if(!IsSelectGameMode)
			{
				Invoke("resetIsSelectGameMode", 3.0f);
			}
			IsSelectGameMode = true;
			
			//mo shi xuan ze
			if(GlobalData.GetInstance().gameMode==GameMode.SoloMode)
			{
				if(CanAddMusic)
				{
					// guan ka xuan ze diao luo shi de yin xiao
					AudioManager.Instance.PlaySFX(modeSelectAudio);
					CanAddMusic=false;
				}
				
				//ScreenLog.Log("single player mode");
				StopAllCoroutines();
				Mode.SetActive(false); 
				Leve.SetActive(true);
				SelectObj.SetActive(true);
				LinkPlayerNameCtrl.GetInstance().HiddenPlayerInfo();
				if(NetworkServerNet.GetInstance() != null) {
					NetworkServerNet.GetInstance().SetIsDisconnect();
				}
			}
			else
			{
				if(NetCtrlScript == null)
				{
					//ScreenLog.LogWarning("NetCtrlScript is null");
					return;
				}

				NetworkServerNet NetworkServerScript = NetworkServerNet.GetInstance();
				if(NetworkServerScript != null && !NetworkServerScript.CheckIsLinkedServerPort())
				{
					//ScreenLog.LogWarning("the client have not linked server...");
					return;
				}
				
				if(!isHandleNetLink)
				{
					//ScreenLog.Log("link server mode");
					isHandleNetLink = true;
					
					NetCtrlScript.ClientCallHandleSelectLinkCount( true );
				}
			}
		}
	}

	public void HanldeClientSelectLink()
	{
		if (NetCtrlScript == null) {
			NetCtrlScript = NetCtrl.GetInstance();
			if (NetCtrlScript == null) {
				return;
			}
		}
		
		NetworkServerNet NetworkServerScript = NetworkServerNet.GetInstance();
		if(NetworkServerScript != null && !NetworkServerScript.CheckIsLinkedServerPort())
		{
			//ScreenLog.LogWarning("the client have not linked server...");
			return;
		}
		
		if(!isHandleNetLink)
		{
			//ScreenLog.Log("link server mode");
			isHandleNetLink = true;
			NetCtrlScript.ClientCallHandleSelectLinkCount( true );
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		float hor = InputEventCtrl.PlayerFX[0];
		if( Mathf.Abs(hor) > 0.3f && NetCtrlScript != null && !IsSelectGameMode) {
			if(hor < 0f && Mode.activeSelf && !Leve.activeSelf) {
				if(isHandleNetLink) {
					isHandleNetLink = false;
					NetCtrlScript.ClientCallHandleSelectLinkCount( false );
				}
			}
		}

		if(Mode.activeSelf)
		{
			if(GlobalData.GetInstance().gameMode==GameMode.None)
			{
				Starts.SetActive(false);
				pcvr.StartLightStateP1 = LedState.Mie;
			}
			else
			{
				Starts.SetActive(true);
				pcvr.StartLightStateP1 = LedState.Shan;
			}
		}
	}

	public void ActivePlayerLinkGame()
	{
		if (!Mode.activeSelf) {
			return;
		}

		if (CanAddMusic) {
			// guan ka xuan ze diao luo shi de yin xiao
			AudioManager.Instance.PlaySFX(modeSelectAudio);
			CanAddMusic = false;
		}
		
		//ScreenLog.Log("single player mode");
		StopAllCoroutines();
		IsChangeGameLevel = false;
		Mode.SetActive(false);
		Starts.SetActive(false);
		pcvr.StartLightStateP1 = LedState.Mie;
		Leve.SetActive(true);
//		SelectObj.SetActive(true);
		LinkPlayerNameCtrl.GetInstance().ActivePlayerInfo();
	}

	public static bool IsChangeGameMode;
	public static bool IsChangeGameLevel;
	/**
	 * ipInfo -> 第一个选择联机游戏的玩家IP信息.
	 * port   -> 第一个选择联机游戏的玩家port信息.
	 */
	public void ActiveGameSelectObj(string ipInfo, int port)
	{
		if (!FreeModeCtrl.IsHavePlayerIp) {
			return;
		}
		GlobalData.GetInstance().gameMode = GameMode.OnlineMode;//强制设置为联机模式游戏.
		IsChangeGameMode = false;

		ScreenLog.Log("ActiveGameSelectObj -> ipInfo "+ipInfo+", ipAddress "+Network.player.ipAddress
		              +", port "+port+", playerPort "+Network.player.port);
		if (ipInfo != Network.player.ipAddress || Network.player.port != port) {
			//该玩家不是第一个选择联机游戏的,不允许进行游戏关卡的选择.
			IsChangeGameLevel = false;
			ChangeLeve.ActiveSelectCityLevel(-1);
			return;
		}
		
		if (SelectObj.activeSelf) {
			return;
		}
		//该玩家是第一个选择联机游戏的,允许进行游戏关卡的选择.
		IsChangeGameLevel = true;
		SelectObj.SetActive(true);
		ChangeLeve.ActiveSelectCityLevel();
	}

	IEnumerator ColorCorrection(int key)
	{
		if(backCam == null)
		{
			yield break;
		}

		ColorCorrectionCurves s=(ColorCorrectionCurves)backCam.GetComponent<ColorCorrectionCurves>();
		if(key == 0)
		{
			float minSat = 0.27f;
			while(s.saturation > minSat)
			{
				yield return 0;
				s.saturation -= 1.5f * Time.deltaTime;
//				//ScreenLog.Log("s.saturation " + s.saturation);
				if(s.saturation < minSat)
				{
					backgroud_1.SetActive(false);
					backgroud_2.SetActive(true);
					s.saturation = 1f;
					SelectObj.SetActive(true);
					break;
				}
			}
		}
		else
		{
			while(s.saturation < 1)
			{
				yield return 0;
				s.saturation += Time.deltaTime;
			}
		}
	}
#if UNITY_EDITOR
	void OnGUI()
	{
		ChangeLeve.GetInstance().TestInfoChangeLevel();
	}
#endif
}