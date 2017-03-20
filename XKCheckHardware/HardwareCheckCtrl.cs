using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HardwareCheckCtrl : MonoBehaviour
{
	public UILabel CoinInfo;
	public UILabel FangXiangInfo;
	public UILabel YouMenInfo;
	public UILabel ShaCheInfo_L;
	public UILabel ShaCheInfo_R;
	public UILabel TaBanInfo;
	public UILabel ZuLiValLabel;
	public UILabel ZuLiDengJiLabel;
	public UILabel ChuPiaoState;
	/**
	 * 风扇等级控制.
	 */
	public UILabel[] FengShanDJLable;
	public UILabel AnJianLable;
	public UILabel StartLed;
	public UILabel FireLed;
	public UILabel OutCardLed;
	public UILabel[] QiNangLabel;
	public GameObject JiaMiCeShiObj;
	public bool IsJiaMiCeShi;
	public static bool IsTestHardWare;
	public static HardwareCheckCtrl Instance;
	// Use this for initialization
	void Awake()
	{
		IsTestHardWare = true;
	}

	void Start()
	{
		Instance = this;
		pcvr.GetInstance();
		pcvr.OpenGameDongGan();
		
		JiaMiCeShiObj.SetActive(IsJiaMiCeShi);
		CoinInfo.text = "0";
		AnJianLable.text = "***";
		pcvr.CloseFangXiangPanPower();

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtEvent += ClickStartBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
//		InputEventCtrl.GetInstance().ClickOutCardBtEvent += ClickOutCardBtEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtEvent += ClickStopDongGanBtEvent;
		Screen.SetResolution(1360, 768, false);
	}
	
	// Update is called once per frame
	void Update()
	{
		CoinInfo.text = GlobalData.GetInstance().Icon.ToString();
		UpdateFangXiangInfo();
		UpdateYouMenInfo();
		UpdateShaCheLInfo();
		UpdateShaCheRInfo();
		UpdateTaBanInfo();
	}
	
	
	public void SubCoinPlayer()
	{
		if (GlobalData.GetInstance().Icon < 1) {
			return;
		}
		GlobalData.GetInstance().Icon--;
		pcvr.GetInstance().SubPlayerCoin(1);
	}

//	public static uint FangXiangVal;
	void UpdateFangXiangInfo()
	{
		FangXiangInfo.text = pcvr.SteerValCurAy[0].ToString();
	}
	
//	public static uint YouMenVal;
	void UpdateYouMenInfo()
	{
		YouMenInfo.text = pcvr.YouMenCurVal[0].ToString();
	}
	
//	public static uint ShaCheLVal;
	void UpdateShaCheLInfo()
	{
		ShaCheInfo_L.text = pcvr.ShaCheLCurVal[0].ToString();
	}

//	public static uint ShaCheRVal;
	void UpdateShaCheRInfo()
	{
		ShaCheInfo_R.text = pcvr.ShaCheRCurVal[0].ToString();
	}
	
//	public static uint TaBanVal;
	void UpdateTaBanInfo()
	{
		TaBanInfo.text = pcvr.BianMaQiCurVal[0].ToString();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "设置 Down";
		}
		else {
			AnJianLable.text = "设置 Up";
		}
	}
	
	void ClickSetMoveBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "移动 Down";
		}
		else {
			AnJianLable.text = "移动 Up";
		}
	}
	
	void ClickStartBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "开始 Down";
		}
		else {
			AnJianLable.text = "开始 Up";
		}
	}
	
	void ClickFireBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "发射 Down";
		}
		else {
			AnJianLable.text = "发射 Up";
		}
	}

//	void ClickOutCardBtEvent(ButtonState val)
//	{
//		if (val == ButtonState.DOWN) {
//			AnJianLable.text = "出票 Down";
//		}
//		else {
//			AnJianLable.text = "出票 Up";
//		}
//	}

	void ClickStopDongGanBtEvent(ButtonState val)
	{
		if (val == ButtonState.DOWN) {
			AnJianLable.text = "紧急 Down";
		}
		else {
			AnJianLable.text = "紧急 Up";
		}
	}

	int StartLedNum;
	public void StartLedCheck()
	{
		StartLedNum++;
		switch (StartLedNum) {
		case 1:
			StartLed.text = "开始灯闪";
			pcvr.StartLightStateP1 = LedState.Shan;
			break;
			
		case 2:
			StartLed.text = "开始灯灭";
			pcvr.StartLightStateP1 = LedState.Mie;
			StartLedNum = 0;
			break;
		}
	}
	
	int FireLedNum;
	public void FireLedCheck()
	{
		FireLedNum++;
		switch (FireLedNum) {
		case 1:
			FireLed.text = "发射灯闪";
			pcvr.FireLightState = LedState.Shan;
			break;
			
		case 2:
			FireLed.text = "发射灯灭";
			pcvr.FireLightState = LedState.Mie;
			FireLedNum = 1;
			break;
		}
	}

	int OutCardLedNum;
	public void OutCardLedCheck()
	{
		OutCardLedNum++;
		switch (OutCardLedNum) {
		case 1:
			OutCardLed.text = "出票灯闪";
			pcvr.OutCardLightState = LedState.Shan;
			break;
			
		case 2:
			OutCardLed.text = "出票灯灭";
			pcvr.OutCardLightState = LedState.Mie;
			OutCardLedNum = 1;
			break;
		}
	}
	
	public void SetFengShanValue_L()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		FengShanDJLable[0].text = valZD.ToString("X2");
		pcvr.FengShanVal_L = valZD;
	}

	public void SetFengShanValue_R()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		FengShanDJLable[1].text = valZD.ToString("X2");
		pcvr.FengShanVal_R = valZD;
	}

	public UILabel ZuLiLabel;
	public void SetZuLiValue()
	{
		if (UIProgressBar.current == null) {
			return;
		}
		int valZD = Mathf.RoundToInt(UIProgressBar.current.value * 255f);
		ZuLiLabel.text = valZD.ToString("X2");
		pcvr.mBikeZuLiInfo = valZD;
	}
	
	public void OnPrintCard()
	{
//		pcvr.GetInstance().setPrintCardNum(1);
	}

	public void OnClickQiNangBt_1()
	{
		QiNangLabel[0].text = QiNangLabel[0].text != "1气囊充气" ? "1气囊充气" : "1气囊放气";
		pcvr.QiNangArray[0] = (byte)(pcvr.QiNangArray[0] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_2()
	{
		QiNangLabel[1].text = QiNangLabel[1].text != "2气囊充气" ? "2气囊充气" : "2气囊放气";
		pcvr.QiNangArray[1] = (byte)(pcvr.QiNangArray[1] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_3()
	{
		QiNangLabel[2].text = QiNangLabel[2].text != "3气囊充气" ? "3气囊充气" : "3气囊放气";
		pcvr.QiNangArray[2] = (byte)(pcvr.QiNangArray[2] != 1 ? 1 : 0);
	}
	
	public void OnClickQiNangBt_4()
	{
		QiNangLabel[3].text = QiNangLabel[3].text != "4气囊充气" ? "4气囊充气" : "4气囊放气";
		pcvr.QiNangArray[3] = (byte)(pcvr.QiNangArray[3] != 1 ? 1 : 0);
	}

	public void OnQuitCheckHardware()
	{
		Application.Quit();
	}

	public void RestartCheckHardware()
	{
		Application.Quit();
		string cmd = "start ComTest.exe";
		RunCmd(cmd);
	}

	public static void OpenGameServer()
	{
		string cmd = "start GameServer.exe";
		RunCmd(cmd);
	}
	
	public static void OpenGameClient()
	{
		string cmd = "start GameClient.exe";
		RunCmd(cmd);
	}

	public static void OnRestartGame()
	{
		Application.Quit();
		string cmd = "start GameClient.exe";
		if (FreeModeCtrl.IsServer) {
			cmd = "start GameServer.exe";
		}
		RunCmd(cmd);
	}

	static void RunCmd(string command)
	{
		//實例一個Process類，啟動一個獨立進程.
		Process processObj = new Process();
		//設定程序名.  
		processObj.StartInfo.FileName = "cmd.exe";
		//設定程式執行參數.
		processObj.StartInfo.Arguments = "/c " + command;
		//關閉Shell的使用.
		processObj.StartInfo.UseShellExecute = false;
		
		//重定向標準輸入.
		//processObj.StartInfo.RedirectStandardInput = true;
		//重定向標準輸出.
		//processObj.StartInfo.RedirectStandardOutput = true;
		
		//重定向錯誤輸出.
		processObj.StartInfo.RedirectStandardError = true;
		//設置不顯示窗口.    
		processObj.StartInfo.CreateNoWindow = false;
		//啟動.
		processObj.Start();
	}

	public UILabel JiaMiJYLabel;
	public UILabel JiaMiJYMsg;
	public static bool IsOpenJiaMiJiaoYan;
	public void StartJiaoYanIO()
	{
		pcvr.GetInstance().StartJiaoYanIO();
	}

	void CloseJiaMiJiaoYanFailed()
	{
		if (!IsInvoking("JiaMiJiaoYanFailed")) {
			return;
		}
		CancelInvoke("JiaMiJiaoYanFailed");
	}
	
	public void JiaMiJiaoYanFailed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Failed);
	}
	
	public void JiaMiJiaoYanSucceed()
	{
		SetJiaMiJYMsg("", JiaMiJiaoYanEnum.Succeed);
	}

	public void SetJiaMiJYMsg(string msg, JiaMiJiaoYanEnum key)
	{
		switch (key) {
		case JiaMiJiaoYanEnum.Succeed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验成功";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验成功");
			break;
			
		case JiaMiJiaoYanEnum.Failed:
			CloseJiaMiJiaoYanFailed();
			JiaMiJYMsg.text = "校验失败";
			ResetJiaMiJYLabelInfo();
			ScreenLog.Log("校验失败");
			break;
			
		default:
			JiaMiJYMsg.text = msg;
			ScreenLog.Log(msg);
			break;
		}
	}
	
	public static void CloseJiaMiJiaoYan()
	{
		if (!IsOpenJiaMiJiaoYan) {
			return;
		}
		IsOpenJiaMiJiaoYan = false;
	}
	
	void ResetJiaMiJYLabelInfo()
	{
		CloseJiaMiJiaoYan();
		JiaMiJYLabel.text = "加密校验";
	}

	public UILabel FangXiangPanPowerLabel;
	public void OnClickFangXiangPanPowerBt()
	{
//		bool isLoopTestZD = false;
		switch (FangXiangPanPowerLabel.text) {
		case "方向盘力关闭":
			FangXiangPanPowerLabel.text = "方向盘力打开";
			pcvr.OpenFangXiangPanPower();
//			if (isLoopTestZD) {
//				CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
//				InvokeRepeating("TestLoopOpenFangXiangPanZhenDong", 0f, 0.2f);
//			}
			break;
			
		case "方向盘力打开":
			FangXiangPanPowerLabel.text = "方向盘力关闭";
			pcvr.CloseFangXiangPanPower();
//			if (isLoopTestZD) {
//				CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
//			}
			break;
		}
	}

	public UILabel FangXiangPanZDLabel;
	public void OnClickFangXiangPanZDBt()
	{
		switch (FangXiangPanZDLabel.text) {
		case "方向振动关闭":
			FangXiangPanZDLabel.text = "方向振动打开";
//			pcvr.OpenFangXiangPanPower();
			CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
			InvokeRepeating("TestLoopOpenFangXiangPanZhenDong", 0f, 0.2f);
			break;
			
		case "方向振动打开":
			FangXiangPanZDLabel.text = "方向振动关闭";
			//pcvr.CloseFangXiangPanPower();
			CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
			break;
		}
	}

	void TestLoopOpenFangXiangPanZhenDong()
	{
//		UnityEngine.Debug.Log("TestLoopOpenFangXiangPanZhenDong...");
		pcvr.GetInstance().OpenFangXiangPanZhenDong();
	}
}

public enum JiaMiJiaoYanEnum
{
	Null,
	Succeed,
	Failed,
}