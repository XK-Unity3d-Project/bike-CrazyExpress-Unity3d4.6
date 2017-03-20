using UnityEngine;
using System.Collections;

public class Rank : MonoBehaviour {
	public GameObject di1;
	public GameObject di2;
	public GameObject di3;
	public GameObject di4;
	public GameObject di5;
	public GameObject di6;
	public GameObject di7;
	public GameObject di8;

	public UISprite _1di;
	public UISprite _2di;
	public UISprite _3di;
	public UISprite _4di;
	public UISprite _5di;
	public UISprite _6di;
	public UISprite _7di;
	public UISprite _8di;

	public UISprite _1name;
	public UISprite _2name;
	public UISprite _3name;
	public UISprite _4name;
	public UISprite _5name;
	public UISprite _6name;
	public UISprite _7name;
	public UISprite _8name;

	private static Rank _Instance;
	public static Rank GetInstance()
	{
		return _Instance;
	}

	public void HiddenRankList()
	{
		gameObject.SetActive( false );
	}

	// Use this for initialization
	void Start () {
		_Instance = this;

		if(!FreeModeCtrl.IsServer && GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			RankHandleCtrl.GetInstance();
		}
		GlobalScript.GetInstance().player.RankListChange += RankListChange;

		//GlobalData.GetInstance().gameMode = GameMode.OnlineMode; //test
//		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
//		{
//			_1di=di1.transform.GetChild(0).GetComponent<UISprite>();
//			_2di=di2.transform.GetChild(0).GetComponent<UISprite>();
//			_1name=di1.transform.GetChild(2).GetComponent<UISprite>();
//			_2name=di2.transform.GetChild(1).GetComponent<UISprite>();
//
//			di3.SetActive(false);
//			di4.SetActive(false);
//			di5.SetActive(false);
//			di6.SetActive(false);
//			di7.SetActive(false);
//			di8.SetActive(false);
//		}
//		else
//		{
//			_1di=di1.transform.GetChild(0).GetComponent<UISprite>();
//			_2di=di2.transform.GetChild(0).GetComponent<UISprite>();
//			_3di=di3.transform.GetChild(0).GetComponent<UISprite>();
//			_4di=di4.transform.GetChild(1).GetComponent<UISprite>();
//			_5di=di5.transform.GetChild(1).GetComponent<UISprite>();
//			_6di=di6.transform.GetChild(1).GetComponent<UISprite>();
//			_7di=di7.transform.GetChild(1).GetComponent<UISprite>();
//			_8di=di8.transform.GetChild(1).GetComponent<UISprite>();
//
//			_1name=di1.transform.GetChild(2).GetComponent<UISprite>();
//			_2name=di2.transform.GetChild(1).GetComponent<UISprite>();
//			_3name=di3.transform.GetChild(1).GetComponent<UISprite>();
//			_4name=di4.transform.GetChild(0).GetComponent<UISprite>();
//			_5name=di5.transform.GetChild(0).GetComponent<UISprite>();
//			_6name=di6.transform.GetChild(0).GetComponent<UISprite>();
//			_7name=di7.transform.GetChild(0).GetComponent<UISprite>();
//			_8name=di8.transform.GetChild(0).GetComponent<UISprite>();
//		}
		
		di1.SetActive(true);
		di2.SetActive(true);
		di3.SetActive(true);
		di4.SetActive(true);
		di5.SetActive(true);
		di6.SetActive(true);
		di7.SetActive(true);
		di8.SetActive(true);
	}

	string getRankName(string str)
	{
		GameTextType gameTextVal = GlobalData.GetGameTextMode();
		if(GlobalData.GetInstance().gameMode != GameMode.OnlineMode)
		{
			if (gameTextVal == GameTextType.English) {
				str += "_En";
			}
			return str;
		}

		switch(str)
		{
		case "NPC_01NetUnity":
		case "AiNPC_01NetUnity":
			str = "NPC_01";
			break;
		case "NPC_02NetUnity":
		case "AiNPC_02NetUnity":
			str = "NPC_02";
			break;
		case "NPC_03NetUnity":
		case "AiNPC_03NetUnity":
			str = "NPC_03";
			break;
		case "NPC_04NetUnity":
		case "AiNPC_04NetUnity":
			str = "NPC_04";
			break;
		case "NPC_05NetUnity":
		case "AiNPC_05NetUnity":
			str = "NPC_05";
			break;
		case "NPC_06NetUnity":
		case "AiNPC_06NetUnity":
			str = "NPC_06";
			break;
		case "NPC_07NetUnity":
		case "AiNPC_07NetUnity":
			str = "NPC_07";
			break;
		case "NPC_08NetUnity":
		case "AiNPC_08NetUnity":
			str = "NPC_08";
			break;
		default:
			str = "NPC_08";
			break;
		}

		if (gameTextVal == GameTextType.English) {
			str += "_En";
		}
		//Debug.Log("getRankName -> name " + str);
		return str;
	}

	public void RankListChange()
	{
		_1name.spriteName = getRankName( GlobalScript.GetInstance().player.RankList[0].Name );
		if(GlobalScript.GetInstance().player.RankList[0].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 0;
			_1di.spriteName="faGuang";
		}
		else
		{
			_1di.spriteName="PaiHangDiTu";
		}

		_2name.spriteName = getRankName( GlobalScript.GetInstance().player.RankList[1].Name );
		if(GlobalScript.GetInstance().player.RankList[1].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 1;
			_2di.spriteName="faGuang";
		}
		else
		{
			_2di.spriteName="PaiHangDiTu";
		}

		_3name.spriteName= getRankName( GlobalScript.GetInstance().player.RankList[2].Name );
		if(GlobalScript.GetInstance().player.RankList[2].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 2;
			_3di.spriteName="faGuang";
		}
		else
		{
			_3di.spriteName="PaiHangDiTu";
		}

		_4name.spriteName= getRankName( GlobalScript.GetInstance().player.RankList[3].Name );
		if(GlobalScript.GetInstance().player.RankList[3].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 3;
			_4di.spriteName="faGuang";
		}
		else
		{
			_4di.spriteName="PaiHangDiTu";
		}

		_5name.spriteName= getRankName( GlobalScript.GetInstance().player.RankList[4].Name );
		if(GlobalScript.GetInstance().player.RankList[4].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 4;
			_5di.spriteName="faGuang";
		}
		else
		{
			_5di.spriteName="PaiHangDiTu";
		}
		
		_6name.spriteName= getRankName( GlobalScript.GetInstance().player.RankList[5].Name );
		if(GlobalScript.GetInstance().player.RankList[5].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 5;
			_6di.spriteName="faGuang";
		}
		else
		{
			_6di.spriteName="PaiHangDiTu";
		}
		
		_7name.spriteName= getRankName( GlobalScript.GetInstance().player.RankList[6].Name );
		if(GlobalScript.GetInstance().player.RankList[6].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 6;
			_7di.spriteName="faGuang";
		}
		else
		{
			_7di.spriteName="PaiHangDiTu";
		}
		
		_8name.spriteName= getRankName( GlobalScript.GetInstance().player.RankList[7].Name );
		if(GlobalScript.GetInstance().player.RankList[7].IsPlayer)
		{
			GlobalScript.GetInstance().player.FinalRank = 7;
			_8di.spriteName="faGuang";
		}
		else
		{
			_8di.spriteName="PaiHangDiTu";
		}
	}
}
