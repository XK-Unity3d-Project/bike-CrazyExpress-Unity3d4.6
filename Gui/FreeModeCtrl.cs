using UnityEngine;
using System.Collections;

using System;
using System.Runtime.InteropServices;

public class FreeModeCtrl : MonoBehaviour {
	enum QualityLevelGame
	{
		Fastest,
		Fast,
		Simple,
		Good,
		Beautiful,
		Fantastic
	}

	public GameObject RootObj;
	public GameObject StartPageObj;

	public UISprite UiSpriteObj;
	public GameObject CointInfo;
	public GameObject StartBtObj;

	public bool IsServerPort = false;
	public static bool IsServer = false;
	public static bool IsRecordServerInfo = false;

	public Rect screenPosition;
	[DllImport("user32")]
	static extern IntPtr SetWindowLong (IntPtr hwnd, int _nIndex , int dwNewLong);  
	[DllImport("user32")]  
	static extern bool SetWindowPos (IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);  
	[DllImport("user32")]  
	static extern IntPtr GetForegroundWindow ();
	[DllImport("user32")]
	static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);
	[DllImport("user32")]
	static extern int GetWindowLong(IntPtr hWnd, int nIndex);
	[DllImport("user32")]
	static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
	[DllImport("user32")]
	static extern int GetSystemMetrics(int nIndex);
	[DllImport("user32")]
	static extern int SetForegroundWindow(IntPtr hwnd);
	[DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
	static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
	[DllImport("user32")]
	static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

	const int GWL_STYLE = -16;  
	const int WS_BORDER = 1;  
	const int WS_POPUP = 0x800000;
	const int WS_SYSMENU = 0x80000;


	const int SWP_NOSIZE = 0x0001;
	const int SWP_NOMOVE = 0x0002;
	const uint SW_SHOWNORMAL = 1;
	const int HWND_NOTOPMOST = 0xffffffe;

	const int WS_CAPTION = (int)0x00C00000; 
	const int WS_CHILD = (int)0x40000000; 
	
	const uint SWP_SHOWWINDOW = 0x0040;  
	const uint SWP_DRAWFRAME = 0x0020;
	const uint SWP_DEFERERASE = 0x2000;
	const uint SWP_FRAMECHANGED = 0x0020;
	//int HWND_TOP = 0;
	int SM_CXSCREEN = 0;
	int SM_CYSCREEN = 1;
	
	static bool IsChangePos = false;
	
	void ChangeWindowPos()  
	{
		if(!FreeModeCtrl.IsServer)
		{
			return;
		}

		if(IsChangePos)
		{
			return;
		}
		IsChangePos = true;
		
		Screen.fullScreen = false;
		Invoke("fixWindowPos", 3.0f); //move the game to child screen
		Invoke("fixWindowPos", 5.0f); //make the game full screen
	}

//	public static int ServerScreenW = 800;
//	public static int ServerScreenH = 600;
	public static int ClientScreenW = 1360;
	public static int ClientScreenH = 768;
	public static int ServerScreenW = 1360;
	public static int ServerScreenH = 768;
	static bool IsTestXiaoScreen = false;
	void FullScreenViewCtrl()
	{
		if(!IsServer)
		{
			//Screen.fullScreen = true;
			return;
		}

		if (IsTestXiaoScreen) {
			Screen.SetResolution(ServerScreenW / 2, ServerScreenH / 2, false);
			return;
		}

		IntPtr m_hWnd = GetForegroundWindow();
		
		//IntPtr pParentWndSave = new IntPtr(); //父窗口句柄
		//IntPtr pParentWndSaveTmp = new IntPtr(); //父窗口句柄
		int dwWindowStyleSave = 0; //窗口风格
		//Rect rcWndRectSave = new Rect(0, 0, 0, 0); //窗口位置
		
		int iScreenW = GetSystemMetrics(SM_CXSCREEN);
		//int iScreenH = GetSystemMetrics(SM_CYSCREEN);
		
		dwWindowStyleSave = GetWindowLong(m_hWnd, GWL_STYLE); //保存窗口风格
		
		//GetWindowRect(m_hWnd, &rcWndRectSave); //保存窗口位置
		
		//pParentWndSave = SetParent(m_hWnd, pParentWndSaveTmp); //保存父窗口句柄/设置父窗口
		
		SetWindowLong(m_hWnd, GWL_STYLE,
		              dwWindowStyleSave & (~WS_CHILD) & (~WS_CAPTION) & (~WS_BORDER));//使窗口不具有CAPTION风格
		
		uint SWP = SWP_DRAWFRAME | SWP_DEFERERASE | SWP_FRAMECHANGED | SWP_SHOWWINDOW;
		SWP = SWP_SHOWWINDOW;
		//SetWindowPos(m_hWnd, 0, iScreenW, 0, iScreenW, iScreenH, SWP); //修改窗口置全屏
		if (iScreenW != ServerScreenW) {
			ServerScreenW = iScreenW;
		}
		SetWindowPos(m_hWnd, 0, iScreenW, 0, ServerScreenW, ServerScreenH, SWP); //修改窗口置全屏
		//SetWindowPos(m_hWnd, 0, 0, 0, 800, 600, SWP); //修改窗口置全屏
		
//		IntPtr ptr = new IntPtr();
//		m_hWnd = new IntPtr();
//		SetMenu(m_hWnd, ptr); //取消边框
	}

	void CleanWindow()
	{
		if(IsServer) {
			return;
		}
		
		if (IsTestXiaoScreen) {
			Screen.SetResolution(680, 384, false);
			return;
		}
		
		IntPtr m_hWnd = GetForegroundWindow();
		
		//IntPtr pParentWndSave = new IntPtr(); //父窗口句柄
		//IntPtr pParentWndSaveTmp = new IntPtr(); //父窗口句柄
		int dwWindowStyleSave = 0; //窗口风格
		//Rect rcWndRectSave = new Rect(0, 0, 0, 0); //窗口位置
		
		int iScreenW = GetSystemMetrics(SM_CXSCREEN);
		int iScreenH = GetSystemMetrics(SM_CYSCREEN);
		
		dwWindowStyleSave = GetWindowLong(m_hWnd, GWL_STYLE); //保存窗口风格
		
		//GetWindowRect(m_hWnd, &rcWndRectSave); //保存窗口位置
		
		//pParentWndSave = SetParent(m_hWnd, pParentWndSaveTmp); //保存父窗口句柄/设置父窗口
		
		SetWindowLong(m_hWnd, GWL_STYLE,
		              dwWindowStyleSave & (~WS_CHILD) & (~WS_CAPTION) & (~WS_BORDER));//使窗口不具有CAPTION风格
		
		uint SWP = SWP_DRAWFRAME | SWP_DEFERERASE | SWP_FRAMECHANGED | SWP_SHOWWINDOW;
		SWP = SWP_SHOWWINDOW;
		//SetWindowPos(m_hWnd, 0, iScreenW, 0, iScreenW, iScreenH, SWP); //修改窗口置全屏
		SetWindowPos(m_hWnd, 0, 0, 0, iScreenW, iScreenH, SWP); //修改窗口置全屏
		
//		IntPtr ptr = new IntPtr();
//		m_hWnd = new IntPtr();
//		SetMenu(m_hWnd, ptr); //取消边框
	}

	void SetFullScreenTest()
	{
		//IntPtr ptr = new IntPtr();
		IntPtr m_hWnd = GetForegroundWindow();
		int iScreenW = GetSystemMetrics(SM_CXSCREEN);
		int iScreenH = GetSystemMetrics(SM_CYSCREEN);

		SetWindowLong( m_hWnd, GWL_STYLE, WS_POPUP|WS_SYSMENU );
		//SetWindowPos( m_hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED );

		uint SWP = SWP_DRAWFRAME | SWP_DEFERERASE | SWP_FRAMECHANGED | SWP_SHOWWINDOW;
		SWP = SWP_SHOWWINDOW;
		SetWindowPos(m_hWnd, 0, iScreenW, 0, iScreenW, iScreenH, SWP); //修改窗口置全屏

		ShowWindow( m_hWnd, SW_SHOWNORMAL );

		//SetMenu(m_hWnd, ptr);
	}

	void fixWindowPos()
	{
		FullScreenViewCtrl();
		//ScreenLog.Log("ChangeWindowPos..." + Screen.width + " " + Screen.height);
	}

	static bool IsCheckFullScreen = true;
	void CheckIsFullScreen()
	{
		if(!IsServer && IsCheckFullScreen) {
			if(Screen.width != ClientScreenW || !Screen.fullScreen) {
				IsCheckFullScreen = false;
				Screen.SetResolution(ClientScreenW, 768, true);
			}
		}
	}

	static int CountOpenServer;
	void OpenGameClientPort()
	{
		HardwareCheckCtrl.OpenGameClient();
	}

	void ChangeClientPortWindow()
	{
		Invoke("CleanWindow", 5f); //改变窗口位置.
		Invoke("CleanWindow", 7f); //去除窗口边框.
	}
	
	string ServerIpInfo = "";
	public static bool IsHavePlayerIp;
	static bool IsShowScreenInfo;
	// Use this for initialization
	void Awake ()
	{
		pcvr.ResetBikeZuLiInfo();
		if (!IsServerPort) {
			int screenW = GetSystemMetrics(SM_CXSCREEN);
			if (screenW != ClientScreenW) {
				ClientScreenW = screenW;
			}

			if (!IsShowScreenInfo) {
				IsShowScreenInfo = true;
				Debug.Log("ClientScreenW "+ClientScreenW);
			}
		}

		ChangeMode.IsClickModeStart = false;
		CheckIsHavePlayerIp();
		//pcvr.ResetPlayerBianMaQiMaxVal();
		CountOpenServer++;
//		TimeClientFullScreen = Time.realtimeSinceStartup;
		ServerIpInfo = NetworkServerNet.ServerPortIP;
		if (!pcvr.bIsHardWare) {
			ServerIpInfo = NetworkServerNet.GetServerPortIp();
		}

//		if (IsServerPort && CountOpenServer == 1) {
//			if (ServerIpInfo == Network.player.ipAddress) {
//				Invoke("OpenGameClientPort", 15f);
//			}
//			else {
//				Application.Quit();
//				OpenGameClientPort();
//			}
//		}

		//if (IsServerPort && (!pcvr.bIsHardWare || pcvr.IsTestGetInput)) {
			//IsTestXiaoScreen = true; //test
		//}

		pcvr.CloseGameDongGan();
		QualitySettings.SetQualityLevel((int)QualityLevelGame.Good);
		if(RootObj == null)
		{
			gameObject.SetActive(false);
		}

		if(Application.loadedLevel > (int)GameLeve.Movie && GlobalData.GetInstance().IsFreeMode)
		{
			gameObject.SetActive(true);
		}

		Screen.showCursor = false;
		if(!IsRecordServerInfo)
		{
			IsRecordServerInfo = true;
			IsServer = IsServerPort;
			if (IsServer) {
				pcvr.bIsHardWare = false;
			}

			if(!IsServer)
			{
				if(!IsTestXiaoScreen) {
					if ((CountOpenServer == 1
					    && GlobalData.GetInstance().LinkModeState == 0
					    && ServerIpInfo == Network.player.ipAddress)
					    || (CountOpenServer == 1 && GlobalData.GetInstance().LinkModeState == 1)) {

						if (IsHavePlayerIp) {
							Screen.SetResolution(ClientScreenW, 768, false);
							ChangeClientPortWindow();
						}
					}
				}
				else {
					Screen.SetResolution(680, 384, false);
				}

				if (pcvr.bIsHardWare) {
					pcvr.CloseFangXiangPanPower();
				}
			}
			else
			{
				//Screen.fullScreen = false;
				if(RootObj != null) {
					RootObj.SetActive(false);
				}
				ChangeWindowPos();
				DelayCheckServerIP();
			}
		}
		SetPanelCtrl.GetInstance();

		GameTextType gameTextVal = GlobalData.GetGameTextMode();
		//GlobalData.GetInstance().IsFreeMode = true; //test
		if(GlobalData.GetInstance().IsFreeMode)
		{			
			if(Application.loadedLevel > (int)GameLeve.Movie && GlobalData.GetInstance().IsFreeMode)
			{
				if (gameTextVal == GameTextType.Chinese) {
					UiSpriteObj.spriteName = "mianFei";
				}
				else {
					UiSpriteObj.spriteName = "mianFei_En";
				}
				gameObject.SetActive(true);
			}
			else
			{
				if (gameTextVal == GameTextType.Chinese) {
					UiSpriteObj.spriteName = "Free";
				}
				else {
					UiSpriteObj.spriteName = "Free_En";
				}
			}
			CointInfo.SetActive(false);
			
			if(Application.loadedLevel == (int)GameLeve.Movie) {
				StartBtObj.SetActive(true);
				pcvr.StartLightStateP1 = LedState.Shan;
			}
		}
		else
		{
			if(Application.loadedLevel > (int)GameLeve.Movie && GlobalData.GetInstance().IsFreeMode)
			{
				if (gameTextVal == GameTextType.Chinese) {
					UiSpriteObj.spriteName = "qingTouBi";
				}
				else {
					UiSpriteObj.spriteName = "qingTouBi_En";
				}
			}
			else
			{
				if (gameTextVal == GameTextType.Chinese) {
					UiSpriteObj.spriteName = "PushCoin";
				}
				else {
					UiSpriteObj.spriteName = "PushCoin_En";
				}
			}
			CointInfo.SetActive(true);
			StartBtObj.SetActive(false);
			pcvr.StartLightStateP1 = LedState.Mie;
			if(Application.loadedLevel != (int)GameLeve.Movie)
			{
				gameObject.SetActive(false);
			}
		}

		InputEventCtrl.GetInstance().ClickStartBtEvent += clickStartBtEvent;
	}

	void clickStartBtEvent(ButtonState val)
	{
		//ScreenLog.Log("FreeModeCtrl::clickStartBtEvent -> val " + val);
		if(val != ButtonState.UP)
		{
			return;
		}

		if(StartPageObj != null && StartPageObj.activeSelf)
		{
			return;
		}
		
		if(IsServer)
		{
			if(RootObj != null)
			{
				RootObj.SetActive(false);
			}
			return;
		}
		
		if(!GlobalData.GetInstance().IsFreeMode)
		{
			return;
		}

		if(Application.loadedLevel == (int)GameLeve.Movie)
		{
			if(RootObj != null)
			{
				MediaPlayer.stopPlayMove();
				RootObj.SetActive(false);
				StartPageObj.SetActive(true);
			}
		}
		
		InputEventCtrl.GetInstance().ClickStartBtEvent -= clickStartBtEvent;
	}
	
	public static NetCtrl NetCtrlScript;
//	float TimeClientFullScreen;
	// Update is called once per frame
	void Update()
	{
		if (!IsTestXiaoScreen && !IsServerPort && Time.frameCount % 300 == 0 && IsCheckFullScreen) {
			if ((IsHavePlayerIp && ServerIpInfo != Network.player.ipAddress) || !IsHavePlayerIp) {
				CheckIsFullScreen();
			}
		}

		if(IsServer)
		{
			GameObject root = transform.root.gameObject;
			root.SetActive(false);
			return;
		}
	}
	
	public void DelayCheckServerIP()
	{
		if (!IsHavePlayerIp
		    || !pcvr.bIsHardWare
		    || GlobalData.GetInstance().LinkModeState == 1) {
			return;
		}
		Invoke("CheckServerPortIP", 5f);
	}
	
	void CheckServerPortIP()
	{
		if (Network.player.ipAddress == NetworkServerNet.ServerPortIP) {
			return;
		}
		
		XKCheckGameServerIP.CheckServerIP();
	}
	
	public static void CheckIsHavePlayerIp()
	{
		if (GlobalData.GetInstance().LinkModeState != 0) {
			IsHavePlayerIp = false;
			return;
		}

		IsHavePlayerIp = Network.player.ipAddress == "0.0.0.0" ? false : true;
		if (!IsHavePlayerIp) {
			Debug.Log("IsHavePlayerIp *********** "+IsHavePlayerIp);
		}
	}
}