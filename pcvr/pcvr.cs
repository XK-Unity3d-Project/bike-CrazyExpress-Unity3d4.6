using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using SLAB_HID_DEVICE;
using System;
using System.IO;

public class pcvr : MonoBehaviour {
	static public bool bIsHardWare = true;
	/**
	 * IsTestGetInput == true  -> 用键盘鼠标信息.
	 * IsTestGetInput == false -> 用硬件IO板信息.
	 */
	public static bool IsTestGetInput = false;
	public static bool IsTestBianMaQi = false;
	public static int mBikeBrakeState = 0;
	public static int mBikeZuLiState = 1;
	public static int mBikeZuLiInfo = 0x11;
	bool IsMovePlaneBikeHead = false;
	public static bool IsHitJianSuDai = false;
	public static bool IsJiaoYanHid;
	public static bool IsSlowLoopCom;
	private int HID_WRITE_BUF_LEN = 0;

	static public uint gOldCoinNum = 0;
	public static uint mOldCoinNum = 0;
	public int CoinNumCurrent = 0;
	static public bool bPlayerStartKeyDown = false;
	static public bool bPlayerStartKeyP2Down = false;
	static public bool bStopDongGanKeyDown = false;
	private bool bSetEnterKeyDown = false;
	static public bool bSetMoveKeyDown = false;
	static public bool bPlayerOnRocket = false;
	static public bool bPlayerOnFireBt = false;
	static private pcvr Instance = null;
	bool bIsRunBikeHead = false;

	static public pcvr GetInstance()
	{
		if(Instance == null)
		{
			GameObject obj = new GameObject("_PCVR");
			DontDestroyOnLoad(obj);
			Instance = obj.AddComponent<pcvr>();
			if (bIsHardWare) {
				obj.AddComponent<MyCOMDevice>();
			}
		}
		return Instance;
	}

	void Start()
	{
		InitHandleJsonInfo();
		HID_WRITE_BUF_LEN = MyCOMDevice.ComThreadClass.BufLenWrite;
		lastUpTime = Time.realtimeSinceStartup;
		InitFangXiangPowerOpen();
		InitSteerInfo();
		InitYouMenInfo();
		InitBianMaQiInfo();
		InitShaCheLInfo();
		InitShaCheRInfo();
	}

	public void StartMoveUpBikeHead(BikeHeadQFState state, BikeHeadMoveSpeed speedHead)
	{
		OpenQiNangQian();
		CloseQiNangHou();
	}

	public void StartMoveDownBikeHead(BikeHeadQFState state, BikeHeadMoveSpeed speedHead)
	{
		OpenQiNangHou();
		CloseQiNangQian();
	}

	public void StartMovePlaneBikeHead(BikeHeadQFState state, BikeHeadMoveSpeed speedHead)
	{
		CloseQiNangQian();
		CloseQiNangHou();
	}

	public static bool IsOpenFireLight = false;
	public static bool IsOpenStartLight = false;
	int subCoinNum = 0;
	public IEnumerator SetBikeZuLiInfo(int zuLiState)
	{
		if(zuLiState > 10)
		{
			zuLiState = 10;
		}

		if(zuLiState < 1)
		{
			zuLiState = 1;
		}

		int dZuLi = Mathf.Abs(zuLiState - mBikeZuLiState);
		while(dZuLi > 1)
		{
			////ScreenLog.Log("dZuLi " + dZuLi + ", zuLiState " + zuLiState + ", mBikeZuLiState " + mBikeZuLiState);
			if(zuLiState > mBikeZuLiState)
			{
				mBikeZuLiState++;
			}
			else if(zuLiState < mBikeZuLiState)
			{
				mBikeZuLiState--;
			}

			CheckBikeZuLiInfo( mBikeZuLiState );

			dZuLi = Mathf.Abs(zuLiState - mBikeZuLiState);
			yield return new WaitForSeconds(0.5f);
		}

		mBikeZuLiState = zuLiState;
		CheckBikeZuLiInfo( zuLiState );
	}

	void CheckBikeZuLiInfo( int zuLiState )
	{
		int min = 0x11;
		int max = 0x33;
		int baseVal = (max - min) / 10;
		
		int ZuLiDengJi = GlobalData.GetInstance().BikeZuLiDengJi;
		zuLiState += ZuLiDengJi;
		mBikeZuLiInfo = zuLiState * baseVal + min;
	}

	//ZuLiCeShi
	public void OpenBikeZuLi()
	{
		int randNum = UnityEngine.Random.Range(4, 8);
		StartCoroutine( SetBikeZuLiInfo( randNum ) );
	}

	public void CloseBikeZuLi()
	{
		StartCoroutine( SetBikeZuLiInfo( 1 ) );
	}

	//主角进入土路.
	public bool IsIntoTuLu;
	public void PlayerMoveIntoTuLu()
	{
		if (IsIntoTuLu) {
			return;
		}
		IsIntoTuLu = true;

		if (IsInvoking("HandlePlayerMoveIntoTuLu")) {
			CancelInvoke("HandlePlayerMoveIntoTuLu");
		}
		InvokeRepeating("HandlePlayerMoveIntoTuLu", 0f, TimeMoveTuLu);
	}

	float TimeMoveTuLu = 0.5f;
	float LastTimeMoveTuLu = 0f;
	int CountTuLuVal;
	void HandlePlayerMoveIntoTuLu()
	{
		if (!IsIntoTuLu) {
			CancelInvoke("HandlePlayerMoveIntoTuLu");
			return;
		}

		if (Time.realtimeSinceStartup - LastTimeMoveTuLu < TimeMoveTuLu) {
			return;
		}
		LastTimeMoveTuLu = Time.realtimeSinceStartup;
		
		if (!IsPlayerHit) {
			switch (CountTuLuVal % 2) {
			case 0:
				OpenQiNangZuo();
				OpenQiNangYou();
				break;
				
			case 1:
				CloseQiNangYou();
				CloseQiNangZuo();
				break;
			}
			
			CountTuLuVal++;
			if (CountTuLuVal > 1) {
				CountTuLuVal = 0;
			}
		}
	}

	public void PlayerMoveOutTuLu()
	{
		if (!IsIntoTuLu) {
			return;
		}
		IsIntoTuLu = false;

		if (IsInvoking("HandlePlayerMoveIntoTuLu")) {
			CancelInvoke("HandlePlayerMoveIntoTuLu");
		}
		CloseQiNangZuo();
		CloseQiNangYou();
	}

	bool IsPlayerHit = false;
	int MaxQiNangCount = 4;
	public void HandlePlayerHitState(int key = 0)
	{
		if (IsPlayerHit) {
			return;
		}
		IsPlayerHit = true;
		pcvr.GetInstance().OpenFangXiangPanZhenDong();

		QiNangCount = 0;
		MaxQiNangCount = key == 0 ? 4 : 2;
		if (IsInvoking("HandlePlayerHitObj")) {
			CancelInvoke("HandlePlayerHitObj");
		}
		InvokeRepeating("HandlePlayerHitObj", 0f, DTimePlayerHit);
	}

	int QiNangCount;
	float DTimePlayerHit = 0.1f;
	float LastTimePlayerHit;
	void HandlePlayerHitObj()
	{
		if (Time.realtimeSinceStartup - LastTimePlayerHit < DTimePlayerHit) {
			return;
		}
		LastTimePlayerHit = Time.realtimeSinceStartup;

		//ScreenLog.Log("HandlePlayerHitObj -> QiNangCount " + QiNangCount);
		CloseQiNangZuo();
		CloseQiNangYou();
		if (QiNangCount >= MaxQiNangCount) {
			ResetPlayerHitState();
			if (IsInvoking("HandlePlayerHitObj")) {
				CancelInvoke("HandlePlayerHitObj");
			}
			return;
		}

		switch (QiNangCount) {
		case 0:
		case 2:
			OpenQiNangZuo();
			OpenQiNangYou();
			break;
		}
		QiNangCount++;
	}

	BikeHeadQFState HitJianSuDaiDisQF = BikeHeadQFState.DIS_1;
	void ResetPlayerHitState()
	{
		if (!IsPlayerHit) {
			return;
		}
		IsPlayerHit = false;
	}

	float lastUpTime = 0.0f;
	// Update is called once per frame
	void Update()
	{
		if (XkGameCtrl.IsLoadingLevel) {
			return;
		}

		SetPcvrTaBanInfo();
		if (IsTestGetInput) {
			GetPcvrBianMaQiVal();
		}

		if (!bIsHardWare) {
			return;
		}

		float dTime = Time.realtimeSinceStartup - lastUpTime;
		if (IsJiaoYanHid) {
			if (dTime < 0.1f) {
				return;
			}
		}
		else {
			if (dTime < 0.03f) {
				return;
			}
		}
		lastUpTime = Time.realtimeSinceStartup;
		
		SendMessage();
		GetMessage();
	}

	public static int FengShanVal_L = 0;
	public static int FengShanVal_R = 0;
	public void setFengShanInfo(int num, int key)
	{
		if(num < 100 && num > 0)
		{
			num = 100;
		}

		if(key == 0)
		{
			//FengShan_L
			FengShanVal_L = num;
		}
		else
		{
			//FengShan_R
			FengShanVal_R = num;
		}
	}
	
	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	byte EndRead_1 = 0x41;
	byte EndRead_2 = 0x42;
	/**
****************.显示器.****************
			QiNangArray[0]
QiNangArray[3]				QiNangArray[1]
			QiNangArray[2]
**************************************
	 */
	public static byte[] QiNangArray = {0, 0, 0, 0};
	public static LedState StartLightStateP1 = LedState.Mie;
	public static LedState FireLightState = LedState.Mie;
	public static LedState OutCardLightState = LedState.Mie;
	public static bool IsOpenStartLightP1 = false;
	bool IsSubPlayerCoin = false;
	public static byte DongGanState = 0;
	enum JIAOYANENUM
	{
		NULL,
		SUCCEED,
		FAILED,
	}
	static int JiaoYanCheckCount;
	static JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
	static byte JiaoYanFailedMax = 0x01;
	static byte JiaoYanSucceedCount;
	static byte JiaoYanFailedCount;
	static byte[] JiaoYanDt = new byte[4];
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	byte JiOuJiaoYanCount;
	byte JiOuJiaoYanMax = 5;
	public static bool IsJiOuJiaoYanFailed;
	static bool IsOpenQiNangQian;
	static bool IsOpenQiNangHou;
	static bool IsOpenQiNangZuo;
	static bool IsOpenQiNangYou;
	static bool IsSiBianXing = false; //是否为四边形摆放气囊.
	/**
	 * ***********************
	 * *        qn1          *
	 * * qn4           qn2 *
	 * *        qn3          *
	 * ***********************
	 */
	public static void CloseAllQiNang()
	{
		CloseQiNangQian();
		CloseQiNangHou();
		CloseQiNangZuo();
		CloseQiNangYou();
	}

	public static void CloseGameDongGan()
	{
		DongGanState = 0;
	}
	
	public static void OpenGameDongGan()
	{
		DongGanState = 1;
	}

	public static void OpenQiNangQian()
	{
		if (IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = true;

		if (IsSiBianXing) {
			QiNangArray[0] = 1;
			QiNangArray[1] = 1;
		}
		else {
			QiNangArray[0] = 1;
		}
	}
	
	public static void CloseQiNangQian()
	{
		if (!IsOpenQiNangQian) {
			return;
		}
		IsOpenQiNangQian = false;

		if (IsSiBianXing) {
			QiNangArray[0] = 0;
			QiNangArray[1] = 0;
		}
		else {
			QiNangArray[0] = 0;
		}
	}
	
	public static void OpenQiNangHou()
	{
		if (IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = true;

		if (IsSiBianXing) {
			QiNangArray[2] = 1;
			QiNangArray[3] = 1;
		}
		else {
			QiNangArray[2] = 1;
		}
	}
	
	public static void CloseQiNangHou()
	{
		if (!IsOpenQiNangHou) {
			return;
		}
		IsOpenQiNangHou = false;

		if (IsSiBianXing) {
			QiNangArray[2] = 0;
			QiNangArray[3] = 0;
		}
		else {
			QiNangArray[2] = 0;
		}
	}
	
	public static void OpenQiNangZuo()
	{
		if (IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = true;

		if (IsSiBianXing) {
			QiNangArray[0] = 1;
			QiNangArray[3] = 1;
		}
		else {
			QiNangArray[3] = 1;
		}
	}
	
	public static void CloseQiNangZuo()
	{
		if (!IsOpenQiNangZuo) {
			return;
		}
		IsOpenQiNangZuo = false;

		if (IsSiBianXing) {
			QiNangArray[0] = 0;
			QiNangArray[3] = 0;
		}
		else {
			QiNangArray[3] = 0;
		}
	}
	
	public static void OpenQiNangYou()
	{
		if (IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = true;

		if (IsSiBianXing) {
			QiNangArray[1] = 1;
			QiNangArray[2] = 1;
		}
		else {
			QiNangArray[1] = 1;
		}
	}
	
	public static void CloseQiNangYou()
	{
		if (!IsOpenQiNangYou) {
			return;
		}
		IsOpenQiNangYou = false;

		if (IsSiBianXing) {
			QiNangArray[1] = 0;
			QiNangArray[2] = 0;
		}
		else {
			QiNangArray[1] = 0;
		}
	}

	void InitJiaoYanMiMa()
	{
		JiaoYanMiMa[1] = 0x8e; //0x8e
		JiaoYanMiMa[2] = 0xc3; //0xc3
		JiaoYanMiMa[3] = 0xd7; //0xd7
		JiaoYanMiMa[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
		}
	}
	
	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}
		
		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}
		
		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] = JiaoYanMiMaRand[0] == 0x00 ?
				(byte)UnityEngine.Random.Range(0x01, 0xff) : (byte)(JiaoYanMiMaRand[0] + UnityEngine.Random.Range(0x01, 0xff));
		}
	}
	
	public static bool IsZhenDongFangXiangPan;
	bool IsPlayFangXiangPanZhenDong;
	public void OpenFangXiangPanZhenDong()
	{
		if (IsPlayFangXiangPanZhenDong) {
			return;
		}
		IsPlayFangXiangPanZhenDong = true;
		StartCoroutine(PlayFangXiangPanZhenDong());
	}

	IEnumerator PlayFangXiangPanZhenDong()
	{
		int count = UnityEngine.Random.Range(1, 4);
		count = 1; //test
		do {
			IsZhenDongFangXiangPan = !IsZhenDongFangXiangPan;
			count--;
			yield return new WaitForSeconds(0.05f);
		} while (count > -1);
		IsZhenDongFangXiangPan = false;
		//IsZhenDongFangXiangPan = true; //test
		IsPlayFangXiangPanZhenDong = false;
	}
	
	public static bool IsOpenFangXiangPanPower = true;
	public static void OpenFangXiangPanPower()
	{
		IsOpenFangXiangPanPower = true;
	}
	
	static bool IsInitFangXiangPower;
	void InitFangXiangPowerOpen()
	{
		if (HardwareCheckCtrl.IsTestHardWare) {
			return;
		}
		
		if (IsInitFangXiangPower) {
			return;
		}
		IsInitFangXiangPower = true;
		OpenFangXiangPanPower();
		Invoke("DelayCloseFangXiangPanPower", 300f);
	}
	
	void DelayCloseFangXiangPanPower()
	{
		IsInitFangXiangPower = false;
		if (Application.loadedLevel == (int)GameLeve.SetPanel
		    || Application.loadedLevel == (int)GameLeve.SetPanel) {
			CloseFangXiangPanPower();
		}
	}

	public static void CloseFangXiangPanPower()
	{
		if (IsInitFangXiangPower) {
			return;
		}
		IsOpenFangXiangPanPower = false;
	}

	void SendMessage()
	{
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}
		
		byte[] buffer = new byte[HID_WRITE_BUF_LEN];
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[HID_WRITE_BUF_LEN - 2] = WriteEnd_1;
		buffer[HID_WRITE_BUF_LEN - 1] = WriteEnd_2;
		for (int i = 4; i < HID_WRITE_BUF_LEN - 2; i++) {
			buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
		}
		
		if (IsSubPlayerCoin) {
			buffer[2] = 0xaa;
			buffer[3] = (byte)subCoinNum;
		}

		buffer[5] = 0x00;
		switch (StartLightStateP1) {
		case LedState.Shan:
			buffer[5] |= 0x20;
			break;
			
		case LedState.Mie:
			buffer[5] &= 0xdf;
			break;
		}

		if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {

			byte qnTmp = (byte)(QiNangArray[0]
			                   + (QiNangArray[1] << 1)
			                   + (QiNangArray[2] << 2)
			                   + (QiNangArray[3] << 3));
			buffer[5] |= qnTmp;
			//ScreenLog.Log("buffer[5] " + Convert.ToString(buffer[5], 2));
		}
		else {
			buffer[5] &= 0xf0;
		}

		if (IsJiaoYanHid) {
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMa[i];
			}
			
			for (int i = 0; i < 4; i++) {
				buffer[i + 14] = JiaoYanDt[i];
			}
		}
		else {
			RandomJiaoYanMiMaVal();
			for (int i = 0; i < 4; i++) {
				buffer[i + 10] = JiaoYanMiMaRand[i];
			}
			
			//0x41 0x42 0x43 0x44
			for (int i = 15; i < 18; i++) {
				buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
			}
			buffer[14] = 0x00;
			
			for (int i = 15; i < 18; i++) {
				buffer[14] ^= buffer[i];
			}
		}
		
		
		if (IsZhenDongFangXiangPan) {
			if (DongGanState == 1) {
				buffer[8] = 0x55;
			}
			else {
				if (IsOpenFangXiangPanPower) {
					buffer[8] = 0xaa;
				}
				else {
					buffer[8] = 0x00;
				}
			}
		}
		else {
			if (IsOpenFangXiangPanPower) {
				buffer[8] = 0xaa;
			}
			else {
				buffer[8] = 0x00;
			}
		}

		buffer[4] = (byte)FengShanVal_L;
		//buffer[9] = (byte)FengShanVal_R;
		
		//ZuLiCtrl
		if (HardwareCheckCtrl.IsTestHardWare) {
			buffer[9] = (byte)mBikeZuLiInfo;
		}
		else {
			if (Application.loadedLevel == (int)GameLeve.Movie) {
				buffer[9] = 0x11;
			}
			else {
				buffer[9] = (byte)mBikeZuLiInfo;
			}
		}

		buffer[6] = 0x00;
		for (int i = 2; i <= 11; i++) {
			if (i == 6) {
				continue;
			}
			buffer[6] ^= buffer[i];
		}
		
		buffer[19] = 0x00;
		for (int i = 0; i < HID_WRITE_BUF_LEN; i++) {
			if (i == 19) {
				continue;
			}
			buffer[19] ^= buffer[i];
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
		
//		byte[] testBuf = new byte[]{	0x02, 0x55, 0x00, 0x00, 0x94, 0x00, 0x79, 0xd1,
//										0x09, 0x09, 0x4d, 0x71, 0xbc, 0x7d, 0x19, 0x07,
//										0x22, 0x3c, 0xc2, 0xee, 0xbd, 0x0d, 0x0a		};
//		MyCOMDevice.ComThreadClass.WriteByteMsg = testBuf;
	}

	static void RandomJiaoYanDt()
	{	
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[i] = (byte)UnityEngine.Random.Range(0x00, 0x7b);
		}
		JiaoYanDt[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[0] ^= JiaoYanDt[i];
		}
	}
	
	public void StartJiaoYanIO()
	{
		if (IsJiaoYanHid) {
			return;
		}
		InitJiaoYanMiMa();
		
		if (!HardwareCheckCtrl.IsTestHardWare) {
			if (JiaoYanSucceedCount >= JiaoYanFailedMax) {
				return;
			}
			
			if (JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) {
				return;
			}
		}
		else {
			HardwareCheckCtrl.Instance.SetJiaMiJYMsg("校验中...", JiaMiJiaoYanEnum.Null);
		}
		RandomJiaoYanDt();
		JiaoYanCheckCount = 0;
		IsJiaoYanHid = true;
		CancelInvoke("CloseJiaoYanIO");
		Invoke("CloseJiaoYanIO", 3f);
		//ScreenLog.Log("开始校验...");
	}
	
	void CloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		IsJiaoYanHid = false;
		OnEndJiaoYanIO(JIAOYANENUM.FAILED);
	}
	
	void OnEndJiaoYanIO(JIAOYANENUM val)
	{
		IsJiaoYanHid = false;
		if (IsInvoking("CloseJiaoYanIO")) {
			CancelInvoke("CloseJiaoYanIO");
		}
		
		switch (val) {
		case JIAOYANENUM.FAILED:
			JiaoYanFailedCount++;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanFailed();
			}
			break;
			
		case JIAOYANENUM.SUCCEED:
			JiaoYanSucceedCount++;
			JiaoYanFailedCount = 0;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanSucceed();
			}
			break;
		}
		JiaoYanState = val;
		//Debug.Log("*****JiaoYanState "+JiaoYanState);
		
		if (JiaoYanFailedCount >= JiaoYanFailedMax || IsJiOuJiaoYanFailed) {
			//JiaoYanFailed
			if (IsJiOuJiaoYanFailed) {
				//JiOuJiaoYanFailed
				//Debug.Log("JOJYSB...");
			}
			else {
				//JiaMiXinPianJiaoYanFailed
				//Debug.Log("JMXPJYSB...");
				IsJiaMiJiaoYanFailed = true;
			}
		}
	}
	
	public static bool IsJiaMiJiaoYanFailed;
	static float TimeJiOuVal;
	void GetMessage()
	{
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.ReadByteMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
			//Debug.Log("ReadBufLen was wrong! len is "+MyCOMDevice.ComThreadClass.ReadByteMsg.Length);
			return;
		}
		
		if (IsJiOuJiaoYanFailed) {
			if (Time.time - TimeJiOuVal < 5f) {
				return;
			}
			IsJiOuJiaoYanFailed = false;
			JiOuJiaoYanCount = 0;
			//return;
		}
		
		if ((MyCOMDevice.ComThreadClass.ReadByteMsg[22]&0x01) == 0x01) {
			JiOuJiaoYanCount++;
			if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
				IsJiOuJiaoYanFailed = true;
				TimeJiOuVal = Time.time;
				//JiOuJiaoYanFailed
				Debug.LogWarning("JiOuJiaoYan failed!");
			}
		}
//		//IsJiOuJiaoYanFailed = true; //test

		byte tmpVal = 0x00;
		string testA = "";
		int max = MyCOMDevice.ComThreadClass.BufLenRead - 4;
		if (MyCOMDevice.ComThreadClass.BufLenRead == 29) {
			max = MyCOMDevice.ComThreadClass.BufLenRead - 6;
		}

		for (int i = 2; i < max; i++) {
			if (i == 18 || i == 21) {
				continue;
			}
			testA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		tmpVal ^= EndRead_1;
		tmpVal ^= EndRead_2;
		testA += EndRead_1.ToString("X2") + " ";
		testA += EndRead_2.ToString("X2") + " ";
		
		if (tmpVal != MyCOMDevice.ComThreadClass.ReadByteMsg[21]) {
			if (MyCOMDevice.ComThreadClass.IsStopComTX) {
				return;
			}
			MyCOMDevice.ComThreadClass.IsStopComTX = true;
//			ScreenLog.Log("testA: "+testA);
//			ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[21] "+MyCOMDevice.ComThreadClass.ReadByteMsg[21].ToString("X2"));
//			ScreenLog.Log("byte21 was wrong!");
			return;
		}
		
		if (IsJiaoYanHid) {
			tmpVal = 0x00;
//string testStrA = MyCOMDevice.ComThreadClass.ReadByteMsg[30].ToString("X2") + " ";
//			string testStrB = "";
//			string testStrA = "";
//			for (int i = 0; i < MyCOMDevice.ComThreadClass.ReadByteMsg.Length; i++) {
//				testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("readStr: "+testStrA);
//
//			for (int i = 0; i < JiaoYanDt.Length; i++) {
//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("GameSendDt: "+testStrB);
//
//			string testStrC = "";
//			for (int i = 0; i < JiaoYanDt.Length; i++) {
//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
//			}
//			ScreenLog.Log("GameSendMiMa: "+testStrC);

			for (int i = 11; i < 14; i++) {
				tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				//testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
			}
			
			if (tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[10]) {
				bool isJiaoYanDtSucceed = false;
				tmpVal = 0x00;
				for (int i = 15; i < 18; i++) {
					tmpVal ^= MyCOMDevice.ComThreadClass.ReadByteMsg[i];
				}
				
				//校验2...
				if ( tmpVal == MyCOMDevice.ComThreadClass.ReadByteMsg[14]
				    && (JiaoYanDt[1]&0xef) == MyCOMDevice.ComThreadClass.ReadByteMsg[15]
				    && (JiaoYanDt[2]&0xfe) == MyCOMDevice.ComThreadClass.ReadByteMsg[16]
				    && (JiaoYanDt[3]|0x28) == MyCOMDevice.ComThreadClass.ReadByteMsg[17] ) {
					isJiaoYanDtSucceed = true;
				}
				
				JiaoYanCheckCount++;
				if (isJiaoYanDtSucceed) {
					//JiaMiJiaoYanSucceed
					OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
					//ScreenLog.Log("JMJYCG...");
				}
				else {
					if (JiaoYanCheckCount > 0) {
						OnEndJiaoYanIO(JIAOYANENUM.FAILED);
						//ScreenLog.Log("JMJY Failed...");
					}
//					testStrA = "";
//					for (int i = 0; i < 23; i++) {
//						testStrA += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//					}
//
//					testStrB = "";
//					for (int i = 14; i < 18; i++) {
//						testStrB += MyCOMDevice.ComThreadClass.ReadByteMsg[i].ToString("X2") + " ";
//					}
//					
//					string testStrD = "";
//					for (int i = 0; i < 4; i++) {
//						testStrD += JiaoYanDt[i].ToString("X2") + " ";
//					}
//					ScreenLog.Log("ReadByte[0 - 22] "+testStrA);
//					ScreenLog.Log("ReadByte[14 - 17] "+testStrB);
//					ScreenLog.Log("SendByte[14 - 17] "+testStrD);
//					ScreenLog.LogError("校验数据错误!");
				}
			}
//			else {
//				testStrB = "byte[10] "+MyCOMDevice.ComThreadClass.ReadByteMsg[10].ToString("X2")+" "
//					+", tmpVal "+tmpVal.ToString("X2");
//				ScreenLog.Log("ReadByte[10 - 13] "+testStrA);
//				ScreenLog.Log(testStrB);
//				ScreenLog.LogError("ReadByte[10] was wrong!");
//			}
		}
		CheckIsPlayerActivePcvr();

		int len = MyCOMDevice.ComThreadClass.ReadByteMsg.Length;
		uint[] readMsg = new uint[len];
		for (int i = 0; i < len; i++) {
			readMsg[i] = MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		KeyProcess(readMsg);

		if (!IsTestGetInput) {
			GetPcvrSteerVal();
			CheckYouMenValInfo();
			GetPcvrYouMenVal();
			GetPcvrBianMaQiVal();
			
			CheckShaCheLValInfo();
			CheckShaCheRValInfo();
			GetPcvrShaCheLVal();
			GetPcvrShaCheRVal();
			CheckPcvrShaCheInput();
		}
		CheckPlayerBianMaQiMaxVal();
	}

	float SubTaBanCount = 2.0f;
	public static float TanBanDownCount = 0;
	void SubTaBanCountInfo()
	{
		if (TanBanDownCount > 0) {
			TanBanDownCount -= SubTaBanCount;
			if (TanBanDownCount < 0) {
				TanBanDownCount = 0;
			}
		}
	}

	public static float mMouseDownCount;
	static float MaxMouseDownCount = 9500f;
	public static void SetPcvrTaBanInfo()
	{
		float taBanVal = InputEventCtrl.PlayerTB[0];
		if (IsTestGetInput) {
			taBanVal = taBanVal <= 0f ? InputEventCtrl.PlayerYM[0] : taBanVal;
		}

		if (taBanVal > 0f) {
			if(!bIsHardWare || (IsTestGetInput && !IsTestBianMaQi)) {
				mMouseDownCount = MaxMouseDownCount;
			}
			else {
				mMouseDownCount = TanBanDownCount;
			}
			mMouseDownCount = Mathf.Clamp(mMouseDownCount, 0, MaxMouseDownCount);
		}
		else {
			mMouseDownCount = 0f;
		}
		//ScreenLog.Log("GetInput -> mMouseDownCount " + mMouseDownCount);
	}

	public static bool IsIntoXiaoFeiBan = false;
	//handle bike head move up or down
	public void HandleBikeHeadQiFu(BikeHeadMoveState moveState, float bikeSpeed, float bikeGrade)
	{
		if(IsPlayerHit || bIsRunBikeHead || IsMovePlaneBikeHead)
		{
			return;
		}
		BikeHeadQFState stateQF = BikeHeadQFState.DIS_1;
		BikeHeadMoveSpeed mvSpeed = BikeHeadMoveSpeed.SPEED_0;

		int zuLiState = 4;
		float maxGrade = 45.0f;
		float tmpGrade = 0.6f * maxGrade;
		if(bikeGrade > tmpGrade)
		{
			bikeGrade = tmpGrade;
		}
		float tmpVal = (bikeGrade / maxGrade) * 100.0f;
		tmpVal = ((int)(tmpVal / 11.0f) + 1); //tmpVal -> 1 - 10

		stateQF = (BikeHeadQFState)tmpVal;
		mvSpeed = BikeHeadMoveSpeed.SPEED_8;
		int BikeHeadSpeedState = GlobalData.GetInstance().BikeHeadSpeedState;
		switch(BikeHeadSpeedState)
		{
		case 1:
			mvSpeed = BikeHeadMoveSpeed.SPEED_4;
			break;

		case 2:
			mvSpeed = BikeHeadMoveSpeed.SPEED_7;
			break;

		case 3:
			mvSpeed = BikeHeadMoveSpeed.SPEED_10;
			break;
		}

		if(IsIntoXiaoFeiBan)
		{
			IsIntoXiaoFeiBan = false;
			mvSpeed = BikeHeadMoveSpeed.SPEED_10;
		}

		if(moveState == BikeHeadMoveState.UP)
		{
			tmpVal = (bikeGrade / maxGrade) * 100.0f;
			tmpVal = ((int)(tmpVal / 45.0f) + zuLiState); //tmpVal -> 4 - 6
			if(tmpVal > 6)
			{
				tmpVal = 6;
			}

			if(tmpVal > zuLiState)
			{
				zuLiState = (int)tmpVal;
			}
		}
//		ScreenLog.Log("stateQF " + stateQF + ", mvSpeed " + mvSpeed
//		              + ", zuLiState " + zuLiState + ", moveState " + moveState);

		if( ((bike.IsHitLuYan || bike.IsHitJianSuDai) && GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		   || ((bikeNetUnity.IsHitLuYan || bikeNetUnity.IsHitJianSuDai) && GlobalData.GetInstance().gameMode == GameMode.OnlineMode) )
		{
			stateQF = HitJianSuDaiDisQF;
			mvSpeed = BikeHeadMoveSpeed.SPEED_15;
		}

		switch(moveState)
		{
		case BikeHeadMoveState.UP:
			StartMoveUpBikeHead( stateQF, mvSpeed );
			break;

		case BikeHeadMoveState.PLANE:
			StartMovePlaneBikeHead( stateQF, mvSpeed );
			break;

		case BikeHeadMoveState.DOWN:
			StartMoveDownBikeHead( stateQF, mvSpeed);
			break;
		}

		if((!bike.IsStartQiBuZuLi && GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		   || GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			StartCoroutine( SetBikeZuLiInfo( zuLiState ) );
		}
	}

	public static uint CoinCurPcvr;
	void SubPcvrCoin(int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		IsSubPlayerCoin = true;
		subCoinNum = subNum;
	}

	public void SubPlayerCoin(int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		
		if(gOldCoinNum >= subNum) {
			gOldCoinNum = (uint)(gOldCoinNum - subNum);
		}
		else {
			if (mOldCoinNum == 0) {
				return;
			}
			
			int subTmpVal = (int)(subNum - gOldCoinNum);
			mOldCoinNum -= (uint)subTmpVal;
			gOldCoinNum = 0;
		}
		//CoinNumCurrent = gOldCoinNum + mOldCoinNum;
	}

	void KeyProcess(uint []buffer)
	{
		YouMenCurVal[0] = ( (buffer[2] & 0x0f) << 8 ) + buffer[3]; //youMen.
		ShaCheLCurVal[0] = ( (buffer[4] & 0x0f) << 8 ) + buffer[5]; //shaCheL.
		SteerValCurAy[0] = ( (buffer[6] & 0x0f) << 8 ) + buffer[7]; //fangXiang.
		ShaCheRCurVal[0] = ( (buffer[23] & 0x0f) << 8 ) + buffer[24]; //shaCheR.
		BianMaQiCurVal[0] = (buffer[8] << 8) + buffer[9]; //bianMaQi.

		CoinCurPcvr = buffer[18];
		if (IsSubPlayerCoin) {
			if (CoinCurPcvr == 0) {
				IsSubPlayerCoin = false;
			}
		}
		else {
			if (CoinCurPcvr > 0 && CoinCurPcvr < 35) {
				mOldCoinNum += CoinCurPcvr;
				Toubi.PlayerPushCoin( (int)mOldCoinNum );
				SubPcvrCoin((int)CoinCurPcvr);
			}
		}
		
		if( !bPlayerOnFireBt && (buffer[19]&0x40) == 0x40 ) {
			bPlayerOnFireBt = true;
			//ScreenLog.Log("game fireBt down!");
			InputEventCtrl.GetInstance().ClickFireBt( ButtonState.DOWN );
		}
		else if( bPlayerOnFireBt && (buffer[19]&0x40) == 0x00 ) {
			bPlayerOnFireBt = false;
			//ScreenLog.Log("game fireBt up!");
			InputEventCtrl.GetInstance().ClickFireBt( ButtonState.UP );
		}

		//game startBt, fireNpcBt or jiaoZhunBt
		if( !bPlayerStartKeyDown && (buffer[19]&0x04) == 0x04 ) {
			bPlayerStartKeyDown = true;
			//ScreenLog.Log("game startBt down!");
			InputEventCtrl.GetInstance().ClickStartBt( ButtonState.DOWN );
		}
		else if ( bPlayerStartKeyDown && (buffer[19]&0x04) == 0x00 ) {
			//ScreenLog.Log("game startBt up!");
			bPlayerStartKeyDown = false;
			InputEventCtrl.GetInstance().ClickStartBt( ButtonState.UP );
		}

		if( !bPlayerStartKeyP2Down && (buffer[19]&0x08) == 0x08 ) {
			bPlayerStartKeyP2Down = true;
			//ScreenLog.Log("game startBt down!");
			InputEventCtrl.GetInstance().ClickStartBt( ButtonState.DOWN );
		}
		else if ( bPlayerStartKeyP2Down && (buffer[19]&0x08) == 0x00 ) {
			//ScreenLog.Log("game startBt up!");
			bPlayerStartKeyP2Down = false;
			InputEventCtrl.GetInstance().ClickStartBt( ButtonState.UP );
		}

		if( !bStopDongGanKeyDown && (buffer[19]&0x10) == 0x10 ) {
			bStopDongGanKeyDown = true;
			InputEventCtrl.GetInstance().ClickStopDongGanBt( ButtonState.DOWN );
		}
		else if ( bStopDongGanKeyDown && (buffer[19]&0x10) == 0x00 ) {
			bStopDongGanKeyDown = false;
			InputEventCtrl.GetInstance().ClickStopDongGanBt( ButtonState.UP );
		}

		//setPanel selectBt
		if( !bSetEnterKeyDown && (buffer[19]&0x01) == 0x01 ) {
			bSetEnterKeyDown = true;
			//ScreenLog.Log("game setEnterBt down!");
			InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
		}
		else if ( bSetEnterKeyDown && (buffer[19]&0x01) == 0x00 ) {
			bSetEnterKeyDown = false;
			//ScreenLog.Log("game setEnterBt up!");
			InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
		}

		//setPanel moveBt
		if ( !bSetMoveKeyDown && (buffer[19]&0x02) == 0x02 ) {
			bSetMoveKeyDown = true;
			//ScreenLog.Log("game setMoveBt down!");
			if (!HardwareCheckCtrl.IsTestHardWare) {
				FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
			}
			InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
		}
		else if( bSetMoveKeyDown && (buffer[19]&0x02) == 0x00 ) {
			bSetMoveKeyDown = false;
			//ScreenLog.Log("game setMoveBt up!");
			if (!HardwareCheckCtrl.IsTestHardWare) {
				FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
			}
			InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
		}
	}
	
	public static bool IsPlayerActivePcvr = true;
	public static float TimeLastActivePcvr;
	void CheckIsPlayerActivePcvr()
	{
		if (Application.loadedLevel == 1 || Application.loadedLevel == 2) {
			return;
		}

		if (!IsPlayerActivePcvr) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeLastActivePcvr > 60f) {
			IsPlayerActivePcvr = false;
		}
	}
	
	public static void SetIsPlayerActivePcvr()
	{
		if (!bIsHardWare) {
			return;
		}
		IsPlayerActivePcvr = true;
		TimeLastActivePcvr = Time.realtimeSinceStartup;
	}

	static string FileName = "";
	static HandleJson HandleJsonObj;
	public static void InitHandleJsonInfo()
	{
		GlobalData.GetInstance();
		FileName = GlobalData.fileName;
		HandleJsonObj = GlobalData.handleJsonObj;
	}
	
	public static uint[] SteerValMaxAy = new uint[4]{999999, 999999, 999999, 999999};
	public static uint[] SteerValCenAy = new uint[4]{1765, 1765, 1765, 1765};
	public static uint[] SteerValMinAy = new uint[4];
	public static uint[] SteerValCurAy = new uint[4];
	void InitSteerInfo()
	{
		int indexVal = 0;
		string strVal = "";
		int maxVal = 1;
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMaxP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "SteerValMaxP"+indexVal, strVal);
			}
			SteerValMaxAy[i] = Convert.ToUInt32( strVal );
		}
		
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "SteerValCenP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "1";
				HandleJsonObj.WriteToFileXml(FileName, "SteerValCenP"+indexVal, strVal);
			}
			SteerValCenAy[i] = Convert.ToUInt32( strVal );
		}
		
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMinP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "0";
				HandleJsonObj.WriteToFileXml(FileName, "SteerValMinP"+indexVal, strVal);
			}
			SteerValMinAy[i] = Convert.ToUInt32( strVal );
		}
	}
	
	public static void SaveSteerVal(PcvrValState key, PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		Debug.Log("key "+key);
		switch (key) {
		case PcvrValState.ValMin:
			SteerValMinAy[indexVal] = SteerValCurAy[indexVal];
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMinP"+playerNum, SteerValCurAy[indexVal].ToString());
			break;
			
		case PcvrValState.ValCenter:
			SteerValCenAy[indexVal] = SteerValCurAy[indexVal];
			HandleJsonObj.WriteToFileXml(FileName, "SteerValCenP"+playerNum, SteerValCurAy[indexVal].ToString());
			break;
			
		case PcvrValState.ValMax:
			SteerValMaxAy[indexVal] = SteerValCurAy[indexVal];
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMaxP"+playerNum, SteerValCurAy[indexVal].ToString());
			break;
		}
	}
	
	void GetPcvrSteerVal()
	{
		if (!bIsHardWare) {
			return;
		}
		
		uint steerMaxVal = 0;
		uint steerMinVal = 0;
		uint steerCenVal = 0;
		uint fangXiangVal = 0;
		float fangXiangValTmp = 0f;
		int maxVal = 1;
		for (int i = 0; i < maxVal; i++) {
			steerMaxVal = SteerValMaxAy[i];
			steerMinVal = SteerValMinAy[i];
			steerCenVal = SteerValCenAy[i];
			fangXiangVal = SteerValCurAy[i];
			fangXiangValTmp = 0f;
			if (fangXiangVal >= steerCenVal) {
				if (steerMaxVal > steerMinVal) {
					fangXiangValTmp = ((float)fangXiangVal - steerCenVal) / (steerMaxVal - steerCenVal);
				}
				else {
					fangXiangValTmp = ((float)steerCenVal - fangXiangVal) / (steerMinVal - steerCenVal);
				}
			}
			else {
				if (steerMaxVal > steerMinVal) {
					fangXiangValTmp = ((float)fangXiangVal - steerCenVal) / (steerCenVal - steerMinVal);
				}
				else {
					fangXiangValTmp = ((float)steerCenVal - fangXiangVal) / (steerCenVal - steerMaxVal);
				}
			}
			
			fangXiangValTmp = Mathf.Clamp(fangXiangValTmp, -1f, 1f);
			fangXiangValTmp = Mathf.Abs(fangXiangValTmp) <= 0.05f ? 0f : fangXiangValTmp;
			InputEventCtrl.PlayerFX[i] = fangXiangValTmp;
		}
	}

	public static uint[] YouMenCurVal = new uint[4];
	static uint[] YouMenMaxVal = new uint[4];
	static uint[] YouMenMinVal = new uint[4];
	static int CountYM = 100;
	float TimeYM;
	public static void OpenCheckYouMenValInfo()
	{
		for (int i = 0; i < YouMenMinVal.Length; i++) {
			YouMenMinVal[i] = 0;
		}
		CountYM = 0;
	}
	
	void CheckYouMenValInfo()
	{
		if (!bIsHardWare) {
			return;
		}
		
		if (CountYM >= 10) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeYM < 0.1f) {
			return;
		}
		TimeYM = Time.realtimeSinceStartup;
		
		CountYM++;
		int playerNum = 0;
		for (int i = 0; i < 4; i++) {
			YouMenMinVal[i] += YouMenCurVal[i];
			//Debug.Log("YouMenMinVal["+i+"] "+YouMenMinVal[i]);
			if (CountYM >= 10) {
				playerNum = i+1;
				YouMenMinVal[i] = (uint)((float)YouMenMinVal[i] / 10f);
				//Debug.Log("***YouMenMinVal["+i+"] "+YouMenMinVal[i]);
				HandleJsonObj.WriteToFileXml(FileName, "YouMenMinValP"+playerNum, YouMenMinVal[i].ToString());
			}
		}
	}
	
	void InitYouMenInfo()
	{
		int indexVal = 0;
		string strVal = "";
		int maxVal = 1;
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "YouMenMaxValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "YouMenMaxValP"+indexVal, strVal);
			}
			YouMenMaxVal[i] = Convert.ToUInt32( strVal );
		}
		
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "YouMenMinValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "0";
				HandleJsonObj.WriteToFileXml(FileName, "YouMenMinValP"+indexVal, strVal);
			}
			YouMenMinVal[i] = Convert.ToUInt32( strVal );
		}
	}
	
	public static void SaveYouMenVal(PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		YouMenMaxVal[indexVal] = YouMenCurVal[indexVal];
		HandleJsonObj.WriteToFileXml(FileName, "YouMenMaxValP"+playerNum, YouMenCurVal[indexVal].ToString());
	}
	
	void GetPcvrYouMenVal()
	{
		if (!bIsHardWare) {
			return;
		}
		
		uint youMenCurVal = 0;
		float youMenInput = 0f;
		int maxVal = 1;
		for (int i = 0; i < maxVal; i++) {
			youMenCurVal = YouMenCurVal[i];
			if (YouMenMinVal[i] < YouMenMaxVal[i]) {
				//油门正接.
				if (youMenCurVal < YouMenMinVal[i]) {
					youMenCurVal = YouMenMinVal[i];
				}
				youMenInput = ((float)youMenCurVal - YouMenMinVal[i]) / (YouMenMaxVal[i] - YouMenMinVal[i]);
			}
			else {
				//油门反接.
				if (youMenCurVal > YouMenMinVal[i]) {
					youMenCurVal = YouMenMinVal[i];
				}
				youMenInput = ((float)YouMenMinVal[i] - youMenCurVal) / (YouMenMinVal[i] - YouMenMaxVal[i]);
			}
			youMenInput = Mathf.Clamp01(youMenInput);
			youMenInput = youMenInput >= 0.05f ? 1f : 0f;
			InputEventCtrl.PlayerYM[i] = youMenInput;
		}
	}
	
//	public static float[] BianMaQiVal = new float[4];
	/**
	 * 编码器（脚踏板）信息校准.
	 */
	public static uint[] BianMaQiCurVal = new uint[4];
	static uint[] BianMaQiMaxVal = new uint[4];
	static uint[] BianMaQiMaxValTmp = new uint[4];
	uint[] BianMaQiMinVal = new uint[4];
	void InitBianMaQiInfo()
	{
		int indexVal = 0;
		int maxVal = 1;
		string strVal = "";
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "BianMaQiMaxValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "BianMaQiMaxValP"+indexVal, strVal);
			}
			BianMaQiMaxVal[i] = Convert.ToUInt32( strVal );
			BianMaQiMaxValTmp[indexVal] = BianMaQiMaxVal[i];
		}
		
		for (int i = 0; i < maxVal; i++) {
			BianMaQiMinVal[i] = 30000;
		}
	}
	
	public static void SaveBianMaQiVal(PlayerEnum indexPlayer)
	{
		if (!bIsHardWare) {
			return;
		}
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		BianMaQiMaxVal[indexVal] = BianMaQiCurVal[indexVal];
		BianMaQiMaxValTmp[indexVal] = BianMaQiCurVal[indexVal];
		HandleJsonObj.WriteToFileXml(FileName, "BianMaQiMaxValP"+playerNum, BianMaQiCurVal[indexVal].ToString());
	}

	static float TimeLastMaxBMQ;
	void CheckPlayerBianMaQiMaxVal()
	{
		if (Application.loadedLevel == (int)GameLeve.Movie
		    || Application.loadedLevel == (int)GameLeve.SetPanel) {
			return;
		}

		if (Time.time - TimeLastMaxBMQ < 1f) {
			return;
		}
		TimeLastMaxBMQ = Time.time;

		for (int i = 0; i < BianMaQiMaxVal.Length; i++) {
			if (BianMaQiMaxValTmp[i] > BianMaQiMinVal[i]) {
				if (BianMaQiCurVal[i] > BianMaQiMaxValTmp[i] && BianMaQiCurVal[i] < BianMaQiMaxVal[i]) {
					BianMaQiMaxVal[i] = BianMaQiCurVal[i];
				}
			}
			else {
				if (BianMaQiCurVal[i] < BianMaQiMaxValTmp[i] && BianMaQiCurVal[i] > BianMaQiMaxVal[i]) {
					BianMaQiMaxVal[i] = BianMaQiCurVal[i];
				}
			}
		}
	}

	public static void ResetPlayerBianMaQiMaxVal()
	{
		for (int i = 0; i < BianMaQiMaxVal.Length; i++) {
			BianMaQiMaxVal[i] = BianMaQiMaxValTmp[i];
		}
	}

	void GetPcvrBianMaQiVal()
	{
		if (!bIsHardWare) {
			return;
		}
		
		int maxVal = 1;
		uint bianMaQiCurVal = 0;
		float bianMaQiInput = 0f;
		for (int i = 0; i < maxVal; i++) {
			bianMaQiCurVal = BianMaQiCurVal[i];
			if (BianMaQiMinVal[i] < BianMaQiMaxVal[i]) {
				//编码器（脚踏板）正接.
				if (BianMaQiMaxVal[i] < bianMaQiCurVal) {
					BianMaQiMaxVal[i] = bianMaQiCurVal;
				}
				if (bianMaQiCurVal < BianMaQiMinVal[i]) {
					bianMaQiCurVal = BianMaQiMinVal[i];
				}
				bianMaQiInput = ((float)bianMaQiCurVal - BianMaQiMinVal[i]) / (BianMaQiMaxVal[i] - BianMaQiMinVal[i]);
			}
			else {
				//编码器（脚踏板）反接.
				if (BianMaQiMaxVal[i] > bianMaQiCurVal) {
					BianMaQiMaxVal[i] = bianMaQiCurVal;
				}
				if (bianMaQiCurVal > BianMaQiMinVal[i]) {
					bianMaQiCurVal = BianMaQiMinVal[i];
				}
				bianMaQiInput = ((float)BianMaQiMinVal[i] - bianMaQiCurVal) / (BianMaQiMinVal[i] - BianMaQiMaxVal[i]);
			}
			bianMaQiInput = Mathf.Clamp01(bianMaQiInput);
			bianMaQiInput = bianMaQiInput < 0.01f ? 0f : bianMaQiInput;
//			BianMaQiVal[i] = bianMaQiInput;
			InputEventCtrl.PlayerTB[i] = bianMaQiInput;
		}

		//InputEventCtrl.PlayerTB[0] = TestTBVal;
		if (InputEventCtrl.PlayerTB[0] > 0f) {
			TanBanDownCount = 1.8f * 9500f * InputEventCtrl.PlayerTB[0];
		}
		else {
			SubTaBanCountInfo();
		}
	}

	//[Range(0f, 1f)]public float TestTBVal = 0f;
//	void OnGUI()
//	{
//		if (!IsTestBianMaQi) {
//			return;
//		}
//		string strA = "playerTB "+InputEventCtrl.PlayerTB[0]+", TanBanDownCount "+TanBanDownCount;
//		GUI.Box(new Rect(0f, Screen.height - 30f, 400f, 30f), strA);
//	}

	public static float[] ShaCheLVal = new float[4];
	public static uint[] ShaCheLCurVal = new uint[4];
	static uint[] ShaCheLMaxVal = new uint[4];
	static uint[] ShaCheLMinVal = new uint[4];
	static int CountShaCheL = 100;
	float TimeShaCheL;
	public static void OpenCheckShaCheLValInfo()
	{
		CountShaCheL = 0;
		for (int i = 0; i < 4; i++) {
			ShaCheLMinVal[i] = 0;
		}
	}
	
	void CheckShaCheLValInfo()
	{
		if (!bIsHardWare) {
			return;
		}
		
		if (CountShaCheL >= 10) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeShaCheL < 0.1f) {
			return;
		}
		TimeShaCheL = Time.realtimeSinceStartup;
		
		CountShaCheL++;
		int playerNum = 0;
		int maxVal = 1;
		for (int i = 0; i < maxVal; i++) {
			ShaCheLMinVal[i] += ShaCheLCurVal[i];
			//Debug.Log("ShaCheLMinVal["+i+"] "+ShaCheLMinVal[i]);
			if (CountShaCheL >= 10) {
				playerNum = i+1;
				ShaCheLMinVal[i] = (uint)((float)ShaCheLMinVal[i] / 10f);
				//Debug.Log("***ShaCheLMinVal["+i+"] "+ShaCheLMinVal[i]);
				HandleJsonObj.WriteToFileXml(FileName, "ShaCheLMinValP"+playerNum, ShaCheLMinVal[i].ToString());
			}
		}
	}
	
	void InitShaCheLInfo()
	{
		int indexVal = 0;
		int maxVal = 1;
		string strVal = "";
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "ShaCheLMaxValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "ShaCheLMaxValP"+indexVal, strVal);
			}
			ShaCheLMaxVal[i] = Convert.ToUInt32( strVal );
		}
		
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "ShaCheLMinValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "0";
				HandleJsonObj.WriteToFileXml(FileName, "ShaCheLMinValP"+indexVal, strVal);
			}
			ShaCheLMinVal[i] = Convert.ToUInt32( strVal );
		}
	}
	
	public static void SaveShaCheLVal(PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		ShaCheLMaxVal[indexVal] = ShaCheLCurVal[indexVal];
		HandleJsonObj.WriteToFileXml(FileName, "ShaCheLMaxValP"+playerNum, ShaCheLCurVal[indexVal].ToString());
	}
	
	void GetPcvrShaCheLVal()
	{
		if (!bIsHardWare) {
			return;
		}
		
		int maxVal = 1;
		uint shaCheCurVal = 0;
		float shaCheInput = 0f;
		for (int i = 0; i < maxVal; i++) {
			shaCheCurVal = ShaCheLCurVal[i];
			if (ShaCheLMinVal[i] < ShaCheLMaxVal[i]) {
				//刹车正接.
				if (ShaCheLMinVal[i] > shaCheCurVal) {
					shaCheCurVal = ShaCheLMinVal[i];
				}
				shaCheInput = ((float)shaCheCurVal - ShaCheLMinVal[i]) / (ShaCheLMaxVal[i] - ShaCheLMinVal[i]);
			}
			else {
				//刹车反接.
				if (ShaCheLMinVal[i] < shaCheCurVal) {
					shaCheCurVal = ShaCheLMinVal[i];
				}
				shaCheInput = ((float)ShaCheLMinVal[i] - shaCheCurVal) / (ShaCheLMinVal[i] - ShaCheLMaxVal[i]);
			}
			shaCheInput = Mathf.Clamp01(shaCheInput);
			//shaCheInput = shaCheInput >= 0.05f ? 1f : 0f;
			ShaCheLVal[i] = shaCheInput;
			InputEventCtrl.PlayerSC[i] = ShaCheLVal[i] > ShaCheRVal[i] ? ShaCheLVal[i] : ShaCheRVal[i];
		}
	}
	
	public static float[] ShaCheRVal = new float[4];
	public static uint[] ShaCheRCurVal = new uint[4];
	static uint[] ShaCheRMaxVal = new uint[4];
	static uint[] ShaCheRMinVal = new uint[4];
	static int CountShaCheR = 100;
	float TimeShaCheR;
	public static void OpenCheckShaCheRValInfo()
	{
		CountShaCheR = 0;
		for (int i = 0; i < 4; i++) {
			ShaCheRMinVal[i] = 0;
		}
	}
	
	void CheckShaCheRValInfo()
	{
		if (!bIsHardWare) {
			return;
		}
		
		if (CountShaCheR >= 10) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeShaCheR < 0.1f) {
			return;
		}
		TimeShaCheR = Time.realtimeSinceStartup;
		
		CountShaCheR++;
		int playerNum = 0;
		int maxVal = 1;
		for (int i = 0; i < maxVal; i++) {
			ShaCheRMinVal[i] += ShaCheRCurVal[i];
			//Debug.Log("ShaCheRMinVal["+i+"] "+ShaCheRMinVal[i]);
			if (CountShaCheR >= 10) {
				playerNum = i+1;
				ShaCheRMinVal[i] = (uint)((float)ShaCheRMinVal[i] / 10f);
				//Debug.Log("***ShaCheRMinVal["+i+"] "+ShaCheRMinVal[i]);
				HandleJsonObj.WriteToFileXml(FileName, "ShaCheRMinValP"+playerNum, ShaCheRMinVal[i].ToString());
			}
		}
	}
	
	void InitShaCheRInfo()
	{
		int indexVal = 0;
		int maxVal = 1;
		string strVal = "";
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "ShaCheRMaxValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "ShaCheRMaxValP"+indexVal, strVal);
			}
			ShaCheRMaxVal[i] = Convert.ToUInt32( strVal );
		}
		
		for (int i = 0; i < maxVal; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "ShaCheRMinValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "0";
				HandleJsonObj.WriteToFileXml(FileName, "ShaCheRMinValP"+indexVal, strVal);
			}
			ShaCheRMinVal[i] = Convert.ToUInt32( strVal );
		}
	}
	
	public static void SaveShaCheRVal(PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		ShaCheRMaxVal[indexVal] = ShaCheRCurVal[indexVal];
		HandleJsonObj.WriteToFileXml(FileName, "ShaCheRMaxValP"+playerNum, ShaCheRCurVal[indexVal].ToString());
	}
	
	void GetPcvrShaCheRVal()
	{
		if (!bIsHardWare) {
			return;
		}
		
		int maxVal = 1;
		uint shaCheCurVal = 0;
		float shaCheInput = 0f;
		for (int i = 0; i < maxVal; i++) {
			shaCheCurVal = ShaCheRCurVal[i];
			if (ShaCheRMinVal[i] < ShaCheRMaxVal[i]) {
				//刹车正接.
				if (ShaCheRMinVal[i] > shaCheCurVal) {
					shaCheCurVal = ShaCheRMinVal[i];
				}
				shaCheInput = ((float)shaCheCurVal - ShaCheRMinVal[i]) / (ShaCheRMaxVal[i] - ShaCheRMinVal[i]);
			}
			else {
				//刹车反接.
				if (ShaCheRMinVal[i] < shaCheCurVal) {
					shaCheCurVal = ShaCheRMinVal[i];
				}
				shaCheInput = ((float)ShaCheRMinVal[i] - shaCheCurVal) / (ShaCheRMinVal[i] - ShaCheRMaxVal[i]);
			}
			shaCheInput = Mathf.Clamp01(shaCheInput);
			shaCheInput = shaCheInput >= 0.05f ? 1f : 0f;
			ShaCheRVal[i] = shaCheInput;
			InputEventCtrl.PlayerSC[i] = ShaCheLVal[i] > ShaCheRVal[i] ? ShaCheLVal[i] : ShaCheRVal[i];
		}
	}

	void CheckPcvrShaCheInput()
	{
		float bikeBrakePer = InputEventCtrl.PlayerSC[0];
		if(bikeBrakePer < 0.5f) {
			bikeBrakePer = 0.0f;
			mBikeBrakeState = 0;
		}
		else {
			if(bikeBrakePer > 0.75f) {
				mBikeBrakeState = 7;
			}
			else if(bikeBrakePer > 0.65f) {
				mBikeBrakeState = 6;
			}
			else if(bikeBrakePer > 0.55f) {
				mBikeBrakeState = 5;
			}
			
			if(mBikeBrakeState > 6) {
				StartCoroutine( SetBikeZuLiInfo( 6 ) );
			}
			else {
				StartCoroutine( SetBikeZuLiInfo( mBikeBrakeState ) );
			}
		}
	}
}

public enum PlayerEnum
{
	Null,
	PlayerOne,
	PlayerTwo,
	PlayerThree,
	PlayerFour,
}

public enum BikeLightState : int
{
	TING,
	MAN,
	JIAO_KUAI,
	KUAI,
	TE_KUAI,
	QUAN_MIE,
	QUAN_HONG,
	QUAN_LAN,
	QUAN_LV,
	ZI_YOU
}

public enum BikeHeadQFState : int
{
	DIS_1,
	DIS_2,
	DIS_3,
	DIS_4,
	DIS_5,
	DIS_6,
	DIS_7,
	DIS_8,
	DIS_9,
	DIS_10
}

public enum BikeHeadMoveSpeed : int
{
	SPEED_0,
	SPEED_1,
	SPEED_2,
	SPEED_3,
	SPEED_4,
	SPEED_5,
	SPEED_6,
	SPEED_7,
	SPEED_8,
	SPEED_9,
	SPEED_10,
	SPEED_11,
	SPEED_12,
	SPEED_13,
	SPEED_14,
	SPEED_15
}

public enum BikeHeadMoveState : int
{
	NULL = -2,
	DOWN = -1,
	PLANE,
	UP
}

public enum LedState
{
	Liang,
	Shan,
	Mie
}

public enum PcvrValState
{
	ValMin,
	ValCenter,
	ValMax,
}