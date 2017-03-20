using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class MediaPlayer : MonoBehaviour {
	public GameObject ServerLoad;
	public GameObject LevelCity;
	public GameObject LevelOutdoor;
	static bool isStopMove = false;
	public GameObject MoviePrefab;
	public GameObject ServerMvUI;

	void Awake()
	{
		AudioManager.Instance.StopBGM();
		XkGameCtrl.IsLoadingLevel = false;
		if(FreeModeCtrl.IsServer && Application.loadedLevel == (int)GameLeve.Movie)
		{
			ServerMvUI.SetActive(true);
			if (FreeModeCtrl.ServerScreenW == 800) {
				transform.localPosition = new Vector3(0, 0, 5.2f);
				transform.localEulerAngles = new Vector3(0, 0, 0);
				transform.localScale = new Vector3(8, 6, 1);
			}
			else {
				transform.localPosition = new Vector3(0, 0, 6.6f);
				transform.localEulerAngles = new Vector3(0, 0, 0);
				transform.localScale = new Vector3(13.6f, 7.2f, 1);
			}
		}

		if(!FreeModeCtrl.IsServer)
		{
			transform.localPosition = new Vector3(0, 0, 6.6f);
			transform.localEulerAngles = new Vector3(0, 0, 0);
			transform.localScale = new Vector3(13.6f, 7.2f, 1);
		}
	}

	// Use this for initialization
	void Start()
	{
		if (FreeModeCtrl.IsServer) {
			AudioListener.volume = 0f;
		}
		else {
			AudioListener.volume = (float)GlobalData.GetInstance().ReadGameAudioVolume() / 10f;
		}

		TimeLast = Time.realtimeSinceStartup;
		if(GameMovieCtrl.GetInstance() == null)
		{
			GameObject obj = (GameObject)Instantiate(MoviePrefab);
			Transform tran = obj.transform;
			tran.parent = transform;
			tran.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			tran.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			tran.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
		else
		{
			GameMovieCtrl.GetInstance().PlayMovie();
			Transform tran = GameMovieCtrl.GetInstance().transform;
			tran.parent = transform;
			tran.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
			tran.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			tran.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if(isStopMove)
		{
			if(ServerLoad != null && GlobalData.GetInstance().gameMode == GameMode.OnlineMode && FreeModeCtrl.IsServer)
			{
				//ScreenLog.Log("stop movie -> gameLeve " + GlobalData.GetInstance().gameLeve);
				if(GlobalData.GetInstance().gameLeve == GameLeve.Leve1)
				{
					LevelCity.SetActive(true);
					LevelOutdoor.SetActive(false);
				}
				else
				{
					LevelCity.SetActive(false);
					LevelOutdoor.SetActive(true);
				}
				ServerLoad.SetActive(true);
			}

			isStopMove = false;
			enabled = false;
		}
	}

	public static void stopPlayMove()
	{
		isStopMove = true;
		GameMovieCtrl.stopPlayMovie();
	}
	
	float TimeLast;
	public static string PlayerIPInfo = "192.168.0.2";
	bool IsFixServerPortIP;
	bool IsRestartGame;
	void OnGUI()
	{
		if (FreeModeCtrl.IsServer) {
			if (!pcvr.bIsHardWare) {
				return;
			}
			
			if (FreeModeCtrl.IsHavePlayerIp && Time.frameCount % 200 == 0) {
				if (Network.player != null) {
					PlayerIPInfo = Network.player.ipAddress;
				}
			}
			
			if (Time.realtimeSinceStartup - TimeLast < 20f) {
				if (PlayerIPInfo == NetworkServerNet.ServerPortIP) {
					return;
				}
				IsFixServerPortIP = true;
				string infoA = "The pc IP is "+PlayerIPInfo+", ServerPortIP was wrong! Fixing IP...";
				GUI.Box(new Rect(0, 0, Screen.width, Screen.height), infoA);
				return;
			}
			
			if (PlayerIPInfo == NetworkServerNet.ServerPortIP) {
				if (IsFixServerPortIP) {
					string infoA = "ServerPortIP is fixed, restart the game...";
					GUI.Box(new Rect(0, 0, Screen.width, Screen.height), infoA);
					if (!IsRestartGame) {
						IsRestartGame = true;
						//XKCheckGameServerIP.CloseCmd();
						XKCheckGameServerIP.RestartGame();
					}
				}
				return;
			}
			
			string infoB = "Set ServerPortIp(192.168.0.2) is failed! The ip(192.168.0.2) has been used!";
			GUI.Box(new Rect(0, 0, Screen.width, 30), infoB);
			string infoC = "Please restart the pc after change the pcIP(192.168.0.2) to otherIP!";
			GUI.Box(new Rect(0, 30, Screen.width, 30), infoC);
			return;
		}
	}
}
