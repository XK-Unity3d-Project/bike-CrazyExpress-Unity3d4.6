using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class GlobalData  {
	public static GameTextType GameTextVal = GameTextType.Chinese;
	private static  GlobalData Instance;
	static string startCoinInfo = "";
	static string FilePath = "";
	public static string fileName = "../config/XKGameConfig.xml";
	public static HandleJson handleJsonObj = null;

	static public string bikeGameCtrl = "bikegamectrl";
	static public string netCtrl = "_NetCtrl";
	static public string NetworkServerNet = "_NetworkServerNet";

	private GlobalData()
	{
		gameLeve=GameLeve.None;
		gameMode=GameMode.None;
	}
	public static GlobalData GetInstance()
	{
		if(Instance==null)
		{
			Instance=new GlobalData();
			Instance.InitInfo();
			if (!Directory.Exists(FilePath)) {
				Directory.CreateDirectory(FilePath);
			}

			//init gameMode
			Instance.gameMode = GameMode.OnlineMode;
			if(Application.loadedLevel == (int)GameLeve.Leve1 || Application.loadedLevel == (int)GameLeve.Leve2)
			{
				Instance.gameMode = GameMode.SoloMode;
			}

			if(handleJsonObj == null)
			{
				handleJsonObj = HandleJson.GetInstance();
			}

			string gmText = handleJsonObj.ReadFromFileXml(fileName, "GameTextVal");
			if (gmText == null || gmText == "") {
				gmText = "0"; //中文版.
				handleJsonObj.WriteToFileXml(fileName, "GameTextVal", gmText);
			}
			GameTextVal = gmText == "0" ? GameTextType.Chinese : GameTextType.English;
			//GameTextVal = GameTextType.English; //test.

			//start coin info
			startCoinInfo = handleJsonObj.ReadFromFileXml(fileName, "START_COIN");
			if(startCoinInfo == null || startCoinInfo == "") {
				startCoinInfo = "1";
				handleJsonObj.WriteToFileXml(fileName, "START_COIN", startCoinInfo);
			}
			Instance.XUTOUBI = Convert.ToInt32( startCoinInfo );

			//free mode
			bool isFreeMode = false;
			string modeGame = handleJsonObj.ReadFromFileXml(fileName, "GAME_MODE");
			if(modeGame == null || modeGame == "") {
				modeGame = "1";
				handleJsonObj.WriteToFileXml(fileName, "GAME_MODE", modeGame);
			}

			if(modeGame == "0") {
				isFreeMode = true;
			}
			Instance.IsFreeMode = isFreeMode;

			//game diff
			string diffStr = handleJsonObj.ReadFromFileXml(fileName, "GAME_DIFFICULTY");
			if(diffStr == null || diffStr == "") {
				diffStr = "1";
				handleJsonObj.WriteToFileXml(fileName, "GAME_DIFFICULTY", diffStr);
			}
			Instance.GameDiff = diffStr;


			string readInfo = handleJsonObj.ReadFromFileXml(fileName, "GameAudioVolume");
			if (readInfo == null || readInfo == "") {
				readInfo = "7";
				handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", readInfo);
			}
			
			int value = Convert.ToInt32(readInfo);
			if (value < 0 || value > 10) {
				value = 7;
				handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", value.ToString());
			}
			GameAudioVolume = value;

			//开始设置联机游戏状态参数.
			readInfo = handleJsonObj.ReadFromFileXml(fileName, "LinkModeState");
			if (readInfo == null || readInfo == "") {
				readInfo = "0";
				handleJsonObj.WriteToFileXml(fileName, "LinkModeState", readInfo);
			}
			
			value = Convert.ToInt32(readInfo);
			if (value < 0 || value > 1) {
				value = 0;
				handleJsonObj.WriteToFileXml(fileName, "LinkModeState", value.ToString());
			}

			/**
			 * 此处代码必须放在这里,它是用来设定游戏是否为联机状态的控制接口.
			 */
			if (FreeModeCtrl.IsServer) {
				Instance.LinkModeState = 0;
			}
			else {
				Instance.LinkModeState = value;
			}
			//结束设置联机参数.

			readInfo = handleJsonObj.ReadFromFileXml(fileName, "BikeZuLiDengJi");
			if (readInfo == null || readInfo == "") {
				readInfo = "5";
				handleJsonObj.WriteToFileXml(fileName, "BikeZuLiDengJi", readInfo);
			}
			
			value = Convert.ToInt32(readInfo);
			if (value < 0 || value > 10) {
				value = 5;
				handleJsonObj.WriteToFileXml(fileName, "BikeZuLiDengJi", value.ToString());
			}
			Instance.BikeZuLiDengJi = value;
		}
		return Instance;
	}
	
	void InitInfo()
	{
		FilePath = Application.dataPath + "/../config";
	}

	static int GameAudioVolume = 7;
	public int ReadGameAudioVolume()
	{
		return GameAudioVolume;
	}
	
	public void WriteGameAudioVolume(int value)
	{
		handleJsonObj.WriteToFileXml(fileName, "GameAudioVolume", value.ToString());
		GameAudioVolume = value;
		AudioListener.volume = (float)value / 10f;
	}

	//public Player player;
	public int XUTOUBI=3;
	public bool IsFreeMode = false;
	/**
	 * LinkModeState == 0 -> 联机版游戏.
	 * LinkModeState == 1 -> 单机版游戏.
	 */
	public int LinkModeState = 0;
//	public bool IsOutputCaiPiao = true;
//	public string TicketRate = "1";
//	public float CointToTicket = 10f;
	public string GameDiff = "1";

	public readonly int GAMETIME=90;
	//public int xutoubi;
	public delegate void EventHandel();
	public GameMode gameMode;
	public GameLeve gameLeve;
	public bool playCartoonEnd;
	public event EventHandel IconCountChange;
	private int _icon;
	public int Icon
	{
		get
		{
			return _icon;
		}
		set
		{
			_icon=value;
			if(IconCountChange!=null)
			{
				IconCountChange();
			}
		}
	}

	public int BikeHeadSpeedState = 2;
	public int BikeZuLiDengJi = 0;

	public static void SetGameTextMode(GameTextType modeVal)
	{
		string gmText = modeVal == GameTextType.Chinese ? "0" : "1";
		//gmText == "0" -> 中文版,  gmText == "1" -> 英文版.
		handleJsonObj.WriteToFileXml(fileName, "GameTextVal", gmText);
		GameTextVal = modeVal;
	}
	
	public static GameTextType GetGameTextMode()
	{
		GetInstance();
		return GameTextVal;
	}
}
public enum GameMode
{
	None,
	SoloMode,
	OnlineMode
}

public enum GameLeve:int
{
	None       = -1,
	Movie       = 0,
	Leve1,
	Leve2,
	SetPanel,
	Leve3,
	Leve4,
}

public enum GameTextType
{
	Chinese,
	English,
}