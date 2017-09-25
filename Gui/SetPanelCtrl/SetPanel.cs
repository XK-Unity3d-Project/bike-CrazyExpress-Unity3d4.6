using UnityEngine;
using System.Collections;
using System;

public class SetPanel : MonoBehaviour {

	#region #setPanel value
	static SetPanel _Instance;
	public static SetPanel GetInstance()
	{
		return _Instance;
	}
	private int moveCount = -1;
	public Transform StarTran = null;

	private string fileName = "";
	private HandleJson handleJsonObj = null;

	public UILabel StartCoinLabel = null;
	private int startCoin = 0;
	public GameObject MianFeiDuiHao = null;
	public GameObject YunYingDuiHao = null;

	public GameObject LianJiDuiHao = null;
	public GameObject DanJiDuiHao = null;
	
	public GameObject GameDiffLowDuiHao = null;
	public GameObject GameDiffMiddleDuiHao = null;
	public GameObject GameDiffHighDuiHao = null;
	public GameObject JiaoZhunObj;
	#endregion

	#region #hardware check value
	public GameObject HardwareObj = null;
	public GameObject DirCheckObj = null;
	/**
	 * DirCheckObjArray[0] -> Zuo.
	 * DirCheckObjArray[1] -> You.
	 */
	public GameObject[] DirCheckObjArray;
	private float steerInfo = 0f;
	public GameObject SuDuCheck = null;
	public UILabel suDuNumLabel = null;
	public GameObject ZuLiOpenObj;
	public GameObject ZuLiCloseObj;

	public GameObject BrakeCheck = null;
	public GameObject ShaCheActiveObj;

	public GameObject YouMenCheck = null;
	public GameObject YouMenActiveObj;

	public GameObject KaiShiDengObj;
	public GameObject KaiShiDengMie;
	public GameObject FengShanCeShi;
	public GameObject CheTouDouDongCeShi;
	public GameObject QNCQCeShi_1;
	public GameObject QNCQCeShi_2;
	public GameObject QNCQCeShi_3;
	public GameObject QNCQCeShi_4;
	public GameObject BtCheck = null;
	public GameObject BtActiveObj;
	private bool isShowZuLi = false;
	private bool isCanClickMoveBt = true;
	#endregion

	#region #show setPanel info
	void showGameModeDuiHao(string mode)
	{
		Debug.Log("showGameModeDuiHao -> mode "+mode);
		bool isFreeMode = false;
		switch(mode)
		{
		case "0":
			isFreeMode = true;
			MianFeiDuiHao.SetActive(true);
			YunYingDuiHao.SetActive(false);
			break;
		case "1":
			MianFeiDuiHao.SetActive(false);
			YunYingDuiHao.SetActive(true);
			break;
		default:
			mode = "1";
			MianFeiDuiHao.SetActive(false);
			YunYingDuiHao.SetActive(true);
			break;
		}
		handleJsonObj.WriteToFileXml(fileName, "GAME_MODE", mode);
		GlobalData.GetInstance().IsFreeMode = isFreeMode;
	}
	
	void showGameLinkModeDuiHao(string mode)
	{
		Debug.Log("showGameLinkModeDuiHao -> mode "+mode);
		int linnkMode = 0;
		switch(mode)
		{
		case "0":
			linnkMode = 0;
			LianJiDuiHao.SetActive(true);
			DanJiDuiHao.SetActive(false);
			break;
		case "1":
			linnkMode = 1;
			LianJiDuiHao.SetActive(false);
			DanJiDuiHao.SetActive(true);
			break;
		default:
			linnkMode = 0;
			LianJiDuiHao.SetActive(true);
			DanJiDuiHao.SetActive(false);
			break;
		}
		handleJsonObj.WriteToFileXml(fileName, "LinkModeState", linnkMode.ToString());
		GlobalData.GetInstance().LinkModeState = linnkMode;
	}

	void showDiffDuiHao(string diffStr)
	{
		switch(diffStr)
		{
		case "0":
			GameDiffLowDuiHao.SetActive(true);
			GameDiffMiddleDuiHao.SetActive(false);
			GameDiffHighDuiHao.SetActive(false);
			break;
		case "1":
			GameDiffLowDuiHao.SetActive(false);
			GameDiffMiddleDuiHao.SetActive(true);
			GameDiffHighDuiHao.SetActive(false);
			break;
		case "2":
			GameDiffLowDuiHao.SetActive(false);
			GameDiffMiddleDuiHao.SetActive(false);
			GameDiffHighDuiHao.SetActive(true);
			break;
		default:
			diffStr = "1";
			GameDiffLowDuiHao.SetActive(false);
			GameDiffMiddleDuiHao.SetActive(true);
			GameDiffHighDuiHao.SetActive(false);
			break;
		}
		handleJsonObj.WriteToFileXml(fileName, "GAME_DIFFICULTY", diffStr);
		GlobalData.GetInstance().GameDiff = diffStr;
	}

	public GameObject GameTextChDuiHao;
	public GameObject GameTextEnDuiHao;
	void showGameTextDuiHao(GameTextType textVal)
	{
		switch(textVal)
		{
		case GameTextType.Chinese:
			GameTextChDuiHao.SetActive(true);
			GameTextEnDuiHao.SetActive(false);
			break;
		case GameTextType.English:
			GameTextChDuiHao.SetActive(false);
			GameTextEnDuiHao.SetActive(true);
			break;
		default:
			textVal = GameTextType.Chinese;
			GameTextChDuiHao.SetActive(true);
			GameTextEnDuiHao.SetActive(false);
			break;
		}
		GlobalData.SetGameTextMode(textVal);
	}
	#endregion 

	#region #set setPanel info
	void setStartCoinInfo()
	{
		if(startCoin >= 9) {
			startCoin = 0;
		}

		startCoin++;
		string StartCoinBuf = startCoin.ToString();
		if(StartCoinLabel != null) {
			string buf = StartCoinBuf.ToString();
			StartCoinLabel.text = buf;
		}
		handleJsonObj.WriteToFileXml(fileName, "START_COIN", StartCoinBuf);
		GlobalData.GetInstance().XUTOUBI = startCoin;
	}

	public UILabel CurrentCoinLB;
	int CoinNumPlayer = -1;
	void UpdatePlayerCoin()
	{
		int coinVal = GlobalData.GetInstance().Icon;
		if (CoinNumPlayer == coinVal) {
			return;
		}
		CoinNumPlayer = coinVal;

		if(CurrentCoinLB != null) {
			string buf = "CurrentCoin: " + coinVal.ToString();
			CurrentCoinLB.text = buf;
		}
	}

	public UILabel ZuLiInfo;
	public UILabel ZuLiJiZhi;
	int zuLiDengJi = 3;
	void InitBikeZuLiDengJi()
	{
		zuLiDengJi = GlobalData.GetInstance().BikeZuLiDengJi - 1;
		int max = GlobalData.GetInstance().ZuLiDengJiMax;
		if(zuLiDengJi > max) {
			zuLiDengJi = max - 1;
		}
		setBikeZuLiDengJi();
	}

	void ResetBikeZuLiDengJi()
	{
		zuLiDengJi = 1;
		setBikeZuLiDengJi();
	}

	void setBikeZuLiDengJi()
	{
		zuLiDengJi++;
		int max = GlobalData.GetInstance().ZuLiDengJiMax;
		if(zuLiDengJi > max) {
			zuLiDengJi = 0;
		}

		ZuLiJiZhi.text = "(0-"+max.ToString()+")";
		ZuLiInfo.text = zuLiDengJi.ToString();
		GlobalData.GetInstance().BikeZuLiDengJi = zuLiDengJi;
		handleJsonObj.WriteToFileXml(fileName, "BikeZuLiDengJi", zuLiDengJi.ToString());
	}

	int GameDiffVal;
	void ChangeGameDiff()
	{
		if (GameDiffVal < 0 || GameDiffVal > 1) {
			GameDiffVal = -1;
		}
		GameDiffVal++;
		setGameDiff(GameDiffVal.ToString());
	}

	void setGameDiff(string diff)
	{
		GameDiffVal = Convert.ToInt32(diff);
		showDiffDuiHao(diff);
	}
	
	GameTextType GameTextVal;
	void ChangeGameText()
	{
		GameTextVal = GameTextVal == GameTextType.Chinese ? GameTextType.English : GameTextType.Chinese;
		setGameText(GameTextVal);
	}
	
	void setGameText(GameTextType textVal)
	{
		GameTextVal = textVal;
		showGameTextDuiHao(textVal);
	}

	int GameModeVal;
	void ChangeGameMode()
	{
		GameModeVal = GameModeVal == 0 ? 1 : 0;
		setGameMode(GameModeVal.ToString());
	}

	void ChangeGameLinkMode()
	{
		SetGameLinkMode(LianJiDuiHao.activeSelf == true ? "1" : "0");
	}

	void setGameMode(string mode)
	{
		GameModeVal = mode == "0" ? 0 : 1;
		showGameModeDuiHao(mode);
	}

	void SetGameLinkMode(string mode)
	{
		showGameLinkModeDuiHao(mode);
	}

	void setSuDuNum()
	{
		if(!SuDuCheck.activeSelf)  {
			return;
		}

		if(Time.frameCount % 20 != 0) {
			return;
		}

		PlayerTaBanSpeed = (int) (75 * (pcvr.mMouseDownCount / 1500f));
		suDuNumLabel.text = PlayerTaBanSpeed.ToString();
		if(!isShowZuLi && PlayerTaBanSpeed > 10) {
			isShowZuLi = true;
			if (!IsOpenZuLiCheck) {
				IsOpenZuLiCheck = true;
				StartCoroutine( openZuLiCheck() );
			}
		}
	}

	int PlayerTaBanSpeed;
	bool IsOpenZuLiCheck;
	IEnumerator openZuLiCheck()
	{
		if(!isShowZuLi) {
			pcvr.GetInstance().CloseBikeZuLi();
			//Debug.Log("stop zuli check");
			yield break;
		}

		if (PlayerTaBanSpeed > 10) {
			pcvr.GetInstance().OpenBikeZuLi();
			ZuLiOpenObj.SetActive(true);
			ZuLiCloseObj.SetActive(false);
			yield return new WaitForSeconds( 1f );
		}
		else {
			pcvr.GetInstance().CloseBikeZuLi();
			ZuLiOpenObj.SetActive(false);
			ZuLiCloseObj.SetActive(true);
			yield return new WaitForSeconds( 1f );
		}
		yield return StartCoroutine( openZuLiCheck() );
	}


	int KaiShiDengState;
	float TimeLastKaiShiDeng;
	void SetKaiShiDengState()
	{
		if(!KaiShiDengObj.activeSelf) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeLastKaiShiDeng < 3f) {
			return;
		}
		TimeLastKaiShiDeng = Time.realtimeSinceStartup;

		if (KaiShiDengState % 2 == 0) {
			KaiShiDengMie.SetActive(true);
			pcvr.StartLightStateP1 = LedState.Mie;
		}
		else {
			KaiShiDengMie.SetActive(false);
			pcvr.StartLightStateP1 = LedState.Shan;
		}

		KaiShiDengState++;
		if (KaiShiDengState > 1) {
			KaiShiDengState = 0;
		}
	}
	
	void OpenKaiShiDengCeShi()
	{
		TimeLastKaiShiDeng = Time.realtimeSinceStartup;
		KaiShiDengState = 0;
		KaiShiDengObj.SetActive(true);
		pcvr.StartLightStateP1 = LedState.Shan;
	}

	void CloseKaiShiDengCeShi()
	{
		KaiShiDengState = 0;
		KaiShiDengObj.SetActive(false);
		pcvr.StartLightStateP1 = LedState.Mie;
	}
	
	void TestLoopOpenFangXiangPanZhenDong()
	{
//		UnityEngine.Debug.Log("TestLoopOpenFangXiangPanZhenDong...");
		pcvr.GetInstance().OpenFangXiangPanZhenDong();
	}

	void setShaCheInfo()
	{
		if(!BrakeCheck.activeSelf) {
			return;
		}

		bool isShaChe = false;
		if(pcvr.bIsHardWare) {
			int brakeState = pcvr.mBikeBrakeState;
			if(brakeState > 0) {
				isShaChe = true;
			}
		}
		else {
			if(Input.GetKey( KeyCode.C )) {
				isShaChe = true;
			}
		}
		ShaCheActiveObj.SetActive(isShaChe);
	}

	void setYouMenInfo()
	{
		if(!YouMenCheck.activeSelf)
		{
			return;
		}

		bool isShowInfo = false;
		float throttle = InputEventCtrl.PlayerYM[0];

		if(throttle > 0.3f)
		{
			isShowInfo = true;
		}
		YouMenActiveObj.SetActive(isShowInfo);
	}

	void ClickStartBtEvent(ButtonState val)
	{
		//ScreenLog.Log("SetPanel::ClickStartBtEvent -> val " + val);
		setBtInfo(val);
		if(val == ButtonState.DOWN) {
			CheckJiaoZhunPcvr();
		}
	}

	void ClickFireBtEvent(ButtonState val)
	{
		//ScreenLog.Log("SetPanel::ClickFireBtEvent -> val " + val);
		setBtInfo(val);
		if(val == ButtonState.DOWN) {
			CheckJiaoZhunPcvr();
		}
	}

	void CheckJiaoZhunPcvr()
	{
		JiaoZhunShaChe();
		CheckFangXiangJiaoZhun();
		ResetYouMenJiaoZhun();
		ResetJiaoTaBanJiaoZhun();
	}
	
	void ClickStopDongGanBtEvent(ButtonState val)
	{
		//ScreenLog.Log("SetPanel::ClickStopDongGanBtEvent -> val " + val);
		setBtInfo(val);
	}

	void setBtInfo(ButtonState val)
	{
		if(!BtCheck.activeSelf) {
			return;
		}

		bool isShowInfo = val == ButtonState.DOWN ? true : false;
		if(isShowInfo) {
			BtActiveObj.SetActive(true);
		}
		else {
			BtActiveObj.SetActive(false);
		}
	}
	
	void checkBikeDir()
	{
		if(!DirCheckObj.activeSelf)
		{
			return;
		}

		if(steerInfo < 0f) {
			DirCheckObjArray[0].SetActive(true);
			DirCheckObjArray[1].SetActive(false);
			//dirCheckSprite.spriteName = "left";
		}

		if(steerInfo > 0f) {
			DirCheckObjArray[0].SetActive(false);
			DirCheckObjArray[1].SetActive(true);
			//dirCheckSprite.spriteName = "right";
		}

		if (steerInfo == 0f) {
			DirCheckObjArray[0].SetActive(false);
			DirCheckObjArray[1].SetActive(false);
			//dirCheckSprite.enabled = false;
		}
	}
	
	void InitCeShiInfo()
	{
		if(DirCheckObj != null) {
			DirCheckObj.SetActive(false);
		}
		
		if(SuDuCheck != null) {
			ZuLiOpenObj.SetActive(false);
			ZuLiCloseObj.SetActive(false);
			SuDuCheck.SetActive(false);
		}
		
		if(BrakeCheck != null) {
			BrakeCheck.SetActive(false);
		}
		
		if(YouMenCheck != null) {
			YouMenCheck.SetActive(false);
		}
		
		if(BtCheck != null) {
			BtActiveObj.SetActive(false);
			BtCheck.SetActive(false);
		}
		KaiShiDengObj.SetActive(false);
		KaiShiDengMie.SetActive(false);
		FengShanCeShi.SetActive(false);
		CheTouDouDongCeShi.SetActive(false);
		QNCQCeShi_1.SetActive(false);
		QNCQCeShi_2.SetActive(false);
		QNCQCeShi_3.SetActive(false);
		QNCQCeShi_4.SetActive(false);
	}
	#endregion

	#region #reset setPanel info
	void resetGameStartCoin()
	{
		startCoin = 0;
		if (pcvr.bIsHardWare) {
			pcvr.GetInstance().SubPlayerCoin( GlobalData.GetInstance().Icon );
		}
		GlobalData.GetInstance().Icon = 0;
		setStartCoinInfo();
	}

	void resetGameConfig()
	{
		resetGameStartCoin();
		setGameDiff("1");
		setGameMode("1");
		SetGameLinkMode("0");
		setGameText(GameTextType.Chinese);
		ResetBikeZuLiDengJi();
		
		GameAudioVolume = 7;
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
		GlobalData.GetInstance().WriteGameAudioVolume(GameAudioVolume);

		GlobalData.GetInstance().Icon = 0;
		pcvr.GetInstance().SubPlayerCoin(CoinNumPlayer);
	}
	#endregion
	
	void moveBtUp()
	{	
		if(!isCanClickMoveBt)
		{
			return;
		}

		if (moveCount >= (int)GameSetEnum.QNCQ_4) {
			moveCount = -1;
		}
        if (GameSetEnum.Exit == (GameSetEnum)moveCount)
        {
			moveCount++; //跳过方向校准,由硬件告诉游戏方向极值信息.
        }
		moveCount++;

		GameSetEnum valEnum = (GameSetEnum)moveCount; 
		switch(valEnum)
		{
		case GameSetEnum.BiZhi:
			StarTran.localPosition = new Vector3(-565f, 235f, 0f);
			break;
		case GameSetEnum.GameMode:
			StarTran.localPosition = new Vector3(-565f, 180f, 0f);
			break;
		case GameSetEnum.GameDiff:
			StarTran.localPosition = new Vector3(-565f, 125f, 0f);
			break;
		case GameSetEnum.ZuLiSet:
			StarTran.localPosition = new Vector3(-565f, 65f, 0f);
			break;
		case GameSetEnum.GameReset:
			StarTran.localPosition = new Vector3(-565f, 15f, 0f);
			break;
		case GameSetEnum.GameLanguage:
			StarTran.localPosition = new Vector3(-565f, -40f, 0f);
			break;
		case GameSetEnum.GameAudioSet:
			StarTran.localPosition = new Vector3(-498f, -128f, 0f);
			break;
		case GameSetEnum.GameAudioReset:
			StarTran.localPosition = new Vector3(-295f, -128f, 0f);
			break;
		case GameSetEnum.GameLinkMode:
			StarTran.localPosition = new Vector3(-565f, -155f, 0f);
			break;
		case GameSetEnum.Exit:
			StarTran.localPosition = new Vector3(-565f, -210f, 0f);
			break;
		case GameSetEnum.FangXiangJZ:
			StarTran.localPosition = new Vector3(35f, 235f, 0f);
			break;
		case GameSetEnum.ShaCheJZ:
			StarTran.localPosition = new Vector3(35f, 200f, 0f);
			break;
		case GameSetEnum.JiaoTaBanJZ:
			StarTran.localPosition = new Vector3(35f, 170f, 0f);
			break;
		case GameSetEnum.YouMenJZ:
			StarTran.localPosition = new Vector3(35f, 140f, 0f);
			break;
		case GameSetEnum.AnJianCS:
			CloseJiaoZhun();
			StarTran.localPosition = new Vector3(35f, 110f, 0f);
			break;
		case GameSetEnum.FangXiangCS:
			StarTran.localPosition = new Vector3(35f, 75f, 0f);
			break;
		case GameSetEnum.ShaCheCS:
			StarTran.localPosition = new Vector3(35f, 45f, 0f);
			break;
		case GameSetEnum.SuDuCS:
			StarTran.localPosition = new Vector3(35f, 10f, 0f);
			break;
		case GameSetEnum.YouMenCS:
			pcvr.ResetBikeZuLiInfo();
			StarTran.localPosition = new Vector3(35f, -20f, 0f);
			break;
		case GameSetEnum.KaiShiLEDCS:
			StarTran.localPosition = new Vector3(35f, -50f, 0f);
			break;
		case GameSetEnum.FengShangCS:
			StarTran.localPosition = new Vector3(35f, -85f, 0f);
			break;
		case GameSetEnum.CheTouDouDongCS:
			StarTran.localPosition = new Vector3(35f, -115f, 0f);
			break;
		case GameSetEnum.QNCQ_1:
			StarTran.localPosition = new Vector3(35f, -145f, 0f);
			break;
		case GameSetEnum.QNCQ_2:
			StarTran.localPosition = new Vector3(35f, -180f, 0f);
			break;
		case GameSetEnum.QNCQ_3:
			StarTran.localPosition = new Vector3(35f, -210f, 0f);
			break;
		case GameSetEnum.QNCQ_4:
			StarTran.localPosition = new Vector3(35f, -245f, 0f);
			break;
		}
	}
	
	void InitJiaoZhun()
	{
		if(JiaoZhunObj.activeSelf) {
			return;
		}
		JiaoZhunObj.SetActive(true);
	}

	void CloseJiaoZhun()
	{
		if(!JiaoZhunObj.activeSelf) {
			return;
		}
		JiaoZhunObj.SetActive(false);
	}

	void CheckJiaoZhunInfo()
	{
		InitJiaoZhun();
		if(ShaCheJiaoZhunObj.activeSelf)
		{
			//ScreenLog.LogWarning("ShaCheJiaoZhunObj is active!");
			return;
		}
		
		if(FangXiangJiaoZhunObj.activeSelf)
		{
			//ScreenLog.LogWarning("FangXiangJiaoZhunObj is active!");
			return;
		}
		
		if(YouMenJiaoZhunObj.activeSelf)
		{
			//ScreenLog.LogWarning("YouMenJiaoZhunObj is active!");
			return;
		}
		
		if(JiaoTaBanObj.activeSelf)
		{
			//ScreenLog.LogWarning("JiaoTaBanObj is active!");
			return;
		}

		GameSetEnum enumVal = (GameSetEnum)moveCount;
		switch(enumVal)
		{
		case GameSetEnum.FangXiangJZ:
			//FangXiangJiaoZhun
			InitFangXiangJiaoZhun();
			break;
			
		case GameSetEnum.YouMenJZ:
			//YouMenJiaoZhun
			InitYouMenJiaoZhun();
			break;
			
		case GameSetEnum.ShaCheJZ:
			//ShaCheJiaoZhun
			InitShaCheJiaoZhun();
			break;
			
		case GameSetEnum.JiaoTaBanJZ:
			//TaBanJiaoZhun
			InitJiaoTaBanJiaoZhun();
			break;
		}
	}

	//FangXiangJiaoZhun
	public GameObject FangXiangJiaoZhunObj;
	UITexture FangXiangJZUITexture;
	/**
	 * FangXiangJZTextureArray[0] -> ZuoFangXiang.
	 * FangXiangJZTextureArray[1] -> ZhongFangXiang.
	 * FangXiangJZTextureArray[2] -> YouFangXiang.
	 * 
	 */
	public Texture[] FangXiangJZTextureArray;
	int DirJiaoZhunCount = 0;
	void InitFangXiangJiaoZhun()
	{
		if(FangXiangJiaoZhunObj.activeSelf)
		{
			return;
		}
		//ScreenLog.Log("SetPanel -> InitFangXiangJiaoZhun...");
		DirJiaoZhunCount = 0;
		if (FangXiangJZUITexture == null) {
			FangXiangJZUITexture = FangXiangJiaoZhunObj.GetComponent<UITexture>();
		}
		FangXiangJZUITexture.mainTexture = FangXiangJZTextureArray[0];
		FangXiangJiaoZhunObj.SetActive(true);
		isCanClickMoveBt = false;
	}

	void ResetFangXiangJiaoZhun()
	{
		if(!FangXiangJiaoZhunObj.activeSelf)
		{
			return;
		}
		//ScreenLog.Log("SetPanel -> ResetFangXiangJiaoZhun...");

		FangXiangJiaoZhunObj.SetActive(false);
		DirJiaoZhunCount = 0;
		if (FangXiangJZUITexture == null) {
			FangXiangJZUITexture = FangXiangJiaoZhunObj.GetComponent<UITexture>();
		}
		FangXiangJZUITexture.mainTexture = FangXiangJZTextureArray[0];
		isCanClickMoveBt = true;
	}

	void CheckFangXiangJiaoZhun()
	{
		if(!FangXiangJiaoZhunObj.activeSelf) {
			return;
		}

		PcvrValState jiaoZhunSt = (PcvrValState)DirJiaoZhunCount;
		pcvr.SaveSteerVal(jiaoZhunSt, PlayerEnum.PlayerOne);
		if(DirJiaoZhunCount >= 2) {
			ResetFangXiangJiaoZhun();
			return;
		}

		DirJiaoZhunCount++;
		FangXiangJZUITexture.mainTexture = FangXiangJZTextureArray[DirJiaoZhunCount];
	}

	//YouMenJiaoYan
	public GameObject YouMenJiaoZhunObj;
	void InitYouMenJiaoZhun()
	{
		if (YouMenJiaoZhunObj.activeSelf) {
			return;
		}
		//ScreenLog.Log("SetPanel -> InitYouMenJiaoZhun...");
		pcvr.OpenCheckYouMenValInfo();

		YouMenJiaoZhunObj.SetActive(true);
		isCanClickMoveBt = false;
	}

	void ResetYouMenJiaoZhun(int key = 0)
	{
		if(!YouMenJiaoZhunObj.activeSelf) {
			return;
		}
		//ScreenLog.Log("SetPanel -> ResetYouMenJiaoZhun...");
		if (key == 0) {
			pcvr.SaveYouMenVal(PlayerEnum.PlayerOne);
		}
		YouMenJiaoZhunObj.SetActive(false);
		isCanClickMoveBt = true;
	}

	//ShaCheJiaoZhun
	public GameObject ShaCheJiaoZhunObj;
	UITexture ShaCheJZUITexture;
	/**
	 * ShaCheJZTextureArray[0] -> zuoShaChe.
	 * ShaCheJZTextureArray[1] -> youShaChe.
	 */
	public Texture[] ShaCheJZTextureArray;
	void InitShaCheJiaoZhun()
	{
		if(ShaCheJiaoZhunObj.activeSelf)
		{
			return;
		}
		//ScreenLog.Log("SetPanel -> InitShaCheJiaoZhun...");
		pcvr.OpenCheckShaCheLValInfo();
		pcvr.OpenCheckShaCheRValInfo();

		ShaCheJZUITexture.mainTexture = ShaCheJZTextureArray[0];
		ShaCheJiaoZhunObj.SetActive(true);
		isCanClickMoveBt = false;
	}

	void ResetShaCheJiaoZhun()
	{
		if(!ShaCheJiaoZhunObj.activeSelf){
			return;
		}
		//ScreenLog.Log("SetPanel -> ResetShaCheJiaoZhun...");

		if (ShaCheJZUITexture == null) {
			ShaCheJZUITexture = ShaCheJiaoZhunObj.GetComponent<UITexture>();
		}
		ShaCheJZUITexture.mainTexture = ShaCheJZTextureArray[0];
		ShaCheJiaoZhunObj.SetActive(false);
		isCanClickMoveBt = true;
	}

	void JiaoZhunShaChe()
	{
		if (!ShaCheJiaoZhunObj.activeSelf) {
			return;
		}

		if(ShaCheJZUITexture.mainTexture.name == ShaCheJZTextureArray[0].name) {
			ShaCheJZUITexture.mainTexture = ShaCheJZTextureArray[1];
			pcvr.SaveShaCheLVal(PlayerEnum.PlayerOne);
		}
		else {
			ResetShaCheJiaoZhun();
			pcvr.SaveShaCheRVal(PlayerEnum.PlayerOne);
		}
	}

	//JiaoTaBanJiaoZhun
	public GameObject JiaoTaBanObj;
	void InitJiaoTaBanJiaoZhun()
	{
		if(JiaoTaBanObj.activeSelf)
		{
			return;
		}
		//ScreenLog.Log("SetPanel -> InitJiaoTaBanJiaoZhun...");
		JiaoTaBanObj.SetActive(true);
		isCanClickMoveBt = false;
	}

	void ResetJiaoTaBanJiaoZhun(int key = 0)
	{
		if(!JiaoTaBanObj.activeSelf) {
			return;
		}
		//ScreenLog.Log("SetPanel -> ResetJiaoTaBanJiaoZhun...");
		if (key == 0) {
			pcvr.SaveBianMaQiVal(PlayerEnum.PlayerOne);
		}
		JiaoTaBanObj.SetActive(false);
		isCanClickMoveBt = true;
	}

	public UILabel GameAudioVolumeLB;
	int GameAudioVolume;
	void handleSelectBtUp()
	{
		GameSetEnum enumVal = (GameSetEnum)moveCount;
		switch(enumVal) {
		case GameSetEnum.BiZhi:
			setStartCoinInfo();
			break;
		case GameSetEnum.GameMode:
			ChangeGameMode();
			break;
		case GameSetEnum.GameDiff:
			ChangeGameDiff();
			break;
		case GameSetEnum.ZuLiSet:
			setBikeZuLiDengJi();
			break;
		case GameSetEnum.GameReset:
			resetGameConfig();
			break;
		case GameSetEnum.GameLanguage:
			ChangeGameText();
			break;
		case GameSetEnum.GameAudioSet:
			GameAudioVolume++;
			if (GameAudioVolume > 10) {
				GameAudioVolume = 0;
			}
			GameAudioVolumeLB.text = GameAudioVolume.ToString();
			GlobalData.GetInstance().WriteGameAudioVolume(GameAudioVolume);
			break;
		case GameSetEnum.GameAudioReset:
			GameAudioVolume = 7;
			GameAudioVolumeLB.text = GameAudioVolume.ToString();
			GlobalData.GetInstance().WriteGameAudioVolume(GameAudioVolume);
			break;
		case GameSetEnum.GameLinkMode:
			ChangeGameLinkMode();
			break;
		case GameSetEnum.Exit:
			PcvrInfoCtrl.IsActivePcvrInfo = false;
			SetPanelCtrl.IsIntoSetPanel = false;
			XkGameCtrl.IsLoadingLevel = true;
			System.GC.Collect();
			Application.LoadLevel( (int)GameLeve.Movie );
			break;
		case GameSetEnum.FangXiangJZ:
		case GameSetEnum.ShaCheJZ:
		case GameSetEnum.JiaoTaBanJZ:
		case GameSetEnum.YouMenJZ:
			CheckJiaoZhunInfo();
			break;
		case GameSetEnum.AnJianCS:
			if(BtCheck.activeSelf) {
				isCanClickMoveBt = true;
				BtCheck.SetActive(false);
			}
			else {
				isCanClickMoveBt = false;
				BtCheck.SetActive(true);
			}
			break;
		case GameSetEnum.FangXiangCS:
			if(DirCheckObj.activeSelf) {
				isCanClickMoveBt = true;
				DirCheckObj.SetActive(false);
				//dirCheckSprite.enabled = false;
			}
			else {
				isCanClickMoveBt = false;
				DirCheckObj.SetActive(true);
			}
			break;
		case GameSetEnum.ShaCheCS:
			if(BrakeCheck.activeSelf) {
				isCanClickMoveBt = true;
				BrakeCheck.SetActive(false);
			}
			else {
				isCanClickMoveBt = false;
				BrakeCheck.SetActive(true);
			}
			break;
		case GameSetEnum.SuDuCS:
			if(SuDuCheck.activeSelf) {
				isShowZuLi = false;
				isCanClickMoveBt = true;
				SuDuCheck.SetActive(false);
				StopCoroutine( openZuLiCheck() );
				pcvr.ResetBikeZuLiInfo();
				IsOpenZuLiCheck = false;
			}
			else {
				isCanClickMoveBt = false;
				SuDuCheck.SetActive(true);
			}
			break;
		case GameSetEnum.YouMenCS:
			if(YouMenCheck.activeSelf) {
				isCanClickMoveBt = true;
				YouMenCheck.SetActive(false);
			}
			else {
				isCanClickMoveBt = false;
				YouMenCheck.SetActive(true);
			}
			break;
		case GameSetEnum.KaiShiLEDCS:
			if(KaiShiDengObj.activeSelf) {
				isCanClickMoveBt = true;
				CloseKaiShiDengCeShi();
			}
			else {
				isCanClickMoveBt = false;
				OpenKaiShiDengCeShi();
			}
			break;
		case GameSetEnum.FengShangCS:
			if(FengShanCeShi.activeSelf) {
				isCanClickMoveBt = true;
				FengShanCeShi.SetActive(false);
				pcvr.FengShanVal_L = 0x00;
				pcvr.FengShanVal_R = 0x00;
			}
			else {
				isCanClickMoveBt = false;
				FengShanCeShi.SetActive(true);
				pcvr.FengShanVal_L = 0x99;
				pcvr.FengShanVal_R = 0x99;
			}
			break;
		case GameSetEnum.CheTouDouDongCS:
			if(CheTouDouDongCeShi.activeSelf) {
				isCanClickMoveBt = true;
				CheTouDouDongCeShi.SetActive(false);
				CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
			}
			else {
				isCanClickMoveBt = false;
				CheTouDouDongCeShi.SetActive(true);
				CancelInvoke("TestLoopOpenFangXiangPanZhenDong");
				InvokeRepeating("TestLoopOpenFangXiangPanZhenDong", 0f, 0.2f);
			}
			break;
		case GameSetEnum.QNCQ_1:
			if(QNCQCeShi_1.activeSelf) {
				isCanClickMoveBt = true;
				QNCQCeShi_1.SetActive(false);
				pcvr.QiNangArray[0] = 0;
			}
			else {
				isCanClickMoveBt = false;
				QNCQCeShi_1.SetActive(true);
				pcvr.QiNangArray[0] = 1;
			}
			break;
		case GameSetEnum.QNCQ_2:
			if(QNCQCeShi_2.activeSelf) {
				isCanClickMoveBt = true;
				QNCQCeShi_2.SetActive(false);
				pcvr.QiNangArray[1] = 0;
			}
			else {
				isCanClickMoveBt = false;
				QNCQCeShi_2.SetActive(true);
				pcvr.QiNangArray[1] = 1;
			}
			break;
		case GameSetEnum.QNCQ_3:
			if(QNCQCeShi_3.activeSelf) {
				isCanClickMoveBt = true;
				QNCQCeShi_3.SetActive(false);
				pcvr.QiNangArray[2] = 0;
			}
			else {
				isCanClickMoveBt = false;
				QNCQCeShi_3.SetActive(true);
				pcvr.QiNangArray[2] = 1;
			}
			break;
		case GameSetEnum.QNCQ_4:
			if(QNCQCeShi_4.activeSelf) {
				isCanClickMoveBt = true;
				QNCQCeShi_4.SetActive(false);
				pcvr.QiNangArray[3] = 0;
			}
			else {
				isCanClickMoveBt = false;
				QNCQCeShi_4.SetActive(true);
				pcvr.QiNangArray[3] = 1;
			}
			break;
		}
	}

	// Use this for initialization
	void Start ()
	{
		_Instance = this;
		pcvr.ResetBikeZuLiInfo();
		fileName = GlobalData.fileName;
		XkGameCtrl.IsLoadingLevel = false;
		pcvr.OpenGameDongGan();
		if (pcvr.bIsHardWare) {
			pcvr.GetInstance();
			pcvr.CloseFangXiangPanPower();
		}
		InitCeShiInfo();

		//reset IsSelectGameMode
		StartSenceChangeUI.IsSelectGameMode = false;
		if(JiaoZhunObj != null) {
			JiaoZhunObj.SetActive(false);
		}

		if(handleJsonObj == null) {
			handleJsonObj = HandleJson.GetInstance();
		}
		
		GameAudioVolume = GlobalData.GetInstance().ReadGameAudioVolume();
		GameAudioVolumeLB.text = GameAudioVolume.ToString();

		//init start coin
		string startCoinInfo = handleJsonObj.ReadFromFileXml(fileName, "START_COIN");
		if(startCoinInfo == null || startCoinInfo == "")
		{
			startCoinInfo = "1";
			handleJsonObj.WriteToFileXml(fileName, "START_COIN", startCoinInfo);
		}

		startCoin = Convert.ToInt32( startCoinInfo );
		if(startCoin <= 0) {
			startCoin = 1;
		}
		startCoin--;
		setStartCoinInfo();
		
		//game mode init
		string lianJiStateStr = handleJsonObj.ReadFromFileXml(fileName, "LinkModeState");
		if(lianJiStateStr == null || lianJiStateStr == "") {
			lianJiStateStr = "0";
		}
		SetGameLinkMode(lianJiStateStr);

		string mianFeiStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_MODE");
		if(mianFeiStr == null || mianFeiStr == "") {
			mianFeiStr = "0";
		}
		setGameMode(mianFeiStr);
		
		//game diff init
		string gameDiffStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_DIFFICULTY");
		if(gameDiffStr == null || gameDiffStr == "")
		{
			gameDiffStr = "1";
		}
		setGameDiff(gameDiffStr);

		//game language init
		GameTextType gameTextTmp = GlobalData.GetGameTextMode();
		setGameText(gameTextTmp);

		ResetFangXiangJiaoZhun();
		ResetShaCheJiaoZhun();
		ResetJiaoTaBanJiaoZhun(1);
		ResetYouMenJiaoZhun(1);
		InitBikeZuLiDengJi();
		moveBtUp(); //init

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;

		InputEventCtrl.GetInstance().ClickStartBtEvent += ClickStartBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtEvent += ClickStopDongGanBtEvent;
	}

	// Update is called once per frame
	void Update()
	{
		if (SetBtSt == ButtonState.DOWN && Time.time - TimeSetMoveBt > 1f && Time.frameCount % 200 == 0) {
			moveBtUp();
		}

		UpdatePlayerCoin();
		steerInfo = InputEventCtrl.PlayerFX[0];
		if(Mathf.Abs(steerInfo) < 0.3f) {
			steerInfo = 0.0f;
		}

		setSuDuNum();
		checkBikeDir();
		setShaCheInfo();
		setYouMenInfo();
		SetKaiShiDengState();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		//ScreenLog.Log("ClickSetEnterBtEvent -> val " + val);
		if(val == ButtonState.DOWN) {
			return;
		}
		handleSelectBtUp();
	}
	
	float TimeSetMoveBt;
	ButtonState SetBtSt = ButtonState.UP;
	void ClickSetMoveBtEvent(ButtonState val)
	{
		//ScreenLog.Log("SetPanel::ClickSetMoveBtEvent -> val " + val);
		if (BtCheck.activeSelf) {
			setBtInfo(val);
			return;
		}
		
		SetBtSt = val;
		if (val == ButtonState.DOWN) {
			TimeSetMoveBt = Time.time;
			return;
		}
		
		if (Time.time - TimeSetMoveBt > 1f) {
			return;
		}
		moveBtUp();
	}
}

public enum GameSetEnum
{
	BiZhi,
	GameMode,
	GameDiff,
	ZuLiSet,
	GameReset,
	GameLanguage,
	GameAudioSet,
	GameAudioReset,
	GameLinkMode,
	Exit,
	FangXiangJZ,
	ShaCheJZ,
	JiaoTaBanJZ,
	YouMenJZ,
	AnJianCS,
	FangXiangCS,
	ShaCheCS,
	SuDuCS,
	YouMenCS,
	KaiShiLEDCS,
	FengShangCS,
	CheTouDouDongCS,
	QNCQ_1,
	QNCQ_2,
	QNCQ_3,
	QNCQ_4,
}