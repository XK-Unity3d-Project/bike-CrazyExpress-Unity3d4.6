using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class HidCheckBtCtrl : MonoBehaviour {

	public HidBtType HidClickType;
	
	public enum Trigger
	{
		OnClick,
	}
	
	public Trigger trigger = Trigger.OnClick;
	static int ZuLiVal = 0;
	int HeadQFNum = 0;

	void OnClick ()
	{
//		InitLoopClickBikeHeadQF(); //up down head
//		return;

		int rVal = 0;
		//Debug.Log ("HidClickType **** " + HidClickType);
		switch(HidClickType)
		{
		case HidBtType.QF_UP:
			pcvr.GetInstance().StartMoveUpBikeHead( BikeHeadQFState.DIS_3, BikeHeadMoveSpeed.SPEED_9 );
			break;

		case HidBtType.QF_DOWN:
			pcvr.GetInstance().StartMoveDownBikeHead( BikeHeadQFState.DIS_3, BikeHeadMoveSpeed.SPEED_9 );
			break;

		case HidBtType.QF_PLANE:
			pcvr.GetInstance().StartMovePlaneBikeHead( BikeHeadQFState.DIS_3, BikeHeadMoveSpeed.SPEED_9 );
			break;

		case HidBtType.QF_CHECK:
//			pcvr.GetInstance().InitReordBikeHeadData();
			break;

		case HidBtType.ZULI_OPEN:
			ZuLiVal++;
			if(ZuLiVal > 9)
			{
				ZuLiVal = 0;
			}
			StopCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( ZuLiVal ) );
			StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( ZuLiVal ) );
			break;

		case HidBtType.ZULI_CLOSE:
			ZuLiVal = 0;
//			pcvr.ZuLiDengJi++;
//			if(pcvr.ZuLiDengJi > 9)
//			{
//				pcvr.ZuLiDengJi = 0;
//			}
			StopCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( ZuLiVal ) );
			StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( ZuLiVal ) );
			break;

		case HidBtType.LED1_LIANG:
//			pcvr.LEDState_1 = 1;
			break;

		case HidBtType.LED1_SHAN:
//			pcvr.LEDState_1 = 0;
			break;

		case HidBtType.LED1_MIE:
//			pcvr.LEDState_1 = 2;
			break;

		case HidBtType.LED2_LIANG:
//			pcvr.LEDState_2 = 1;
			break;

		case HidBtType.LED2_SHAN:
//			pcvr.LEDState_2 = 0;
			break;

		case HidBtType.LED2_MIE:
//			pcvr.LEDState_2 = 2;
			break;

		case HidBtType.LED3_LIANG:
//			pcvr.LEDState_3 = 1;
			break;

		case HidBtType.LED3_SHAN:
//			pcvr.LEDState_3 = 0;
			break;

		case HidBtType.LED3_MIE:
//			pcvr.LEDState_3 = 2;
			break;

		case HidBtType.START_LED_LIANG:
//			pcvr.StartLightState = 1;
			break;
			
		case HidBtType.START_LED_SHAN:
//			pcvr.StartLightState = 0;
			break;
			
		case HidBtType.START_LED_MIE:
//			pcvr.StartLightState = 2;
			break;
			
		case HidBtType.FIRE_LED_LIANG:
//			pcvr.FireLightState = 1;
			break;
			
		case HidBtType.FIRE_LED_SHAN:
//			pcvr.FireLightState = 0;
			break;

		case HidBtType.FIRE_LED_MIE:
//			pcvr.FireLightState = 2;
			break;

		case HidBtType.OUT_CARD_LED_LIANG:
//			pcvr.OutCardLightState = 1;
			break;
			
		case HidBtType.OUT_CARD_LED_SHAN:
//			pcvr.OutCardLightState = 0;
			break;
			
		case HidBtType.OUT_CARD_LED_MIE:
//			pcvr.OutCardLightState = 2;
			break;

		case HidBtType.CAIPIAO_DAYIN:
//			pcvr.GetInstance().setPrintCardNum(1);
			break;

		case HidBtType.OPEN_FENGSHAN_L:
			rVal = Random.Range(125, 249);
			pcvr.GetInstance().setFengShanInfo(rVal, 0);
			break;

		case HidBtType.OPEN_FENGSHAN_R:
			rVal = Random.Range(125, 249);
			pcvr.GetInstance().setFengShanInfo(rVal, 1);
			break;

		case HidBtType.CLOSE_FENGSHAN_L:
			pcvr.GetInstance().setFengShanInfo(0, 0);
			break;

		case HidBtType.CLOSE_FENGSHAN_R:
			pcvr.GetInstance().setFengShanInfo(0, 1);
			break;
			
		case HidBtType.QUIT_BT:
			Application.Quit();
			break;

		case HidBtType.RESTART_BT:
			RestartCheckHardware();
			break;
		}
	}

	void RestartCheckHardware()
	{
		Application.Quit();
		string cmd = "start CheckHid.exe";
		RunCmd(cmd);
	}

	void RunCmd(string command)
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

	void InitLoopClickBikeHeadQF()
	{
		HeadQFNum = 0;
		CancelInvoke("LoopClickBikeHeadQF");
		InvokeRepeating("LoopClickBikeHeadQF", 1f, 0.5f);
	}

	void LoopClickBikeHeadQF()
	{
		int val = HeadQFNum % 2;
		if(val == 0)
		{
			pcvr.GetInstance().StartMoveUpBikeHead( BikeHeadQFState.DIS_3, BikeHeadMoveSpeed.SPEED_15 );
		}
		else
		{
			pcvr.GetInstance().StartMoveDownBikeHead( BikeHeadQFState.DIS_3, BikeHeadMoveSpeed.SPEED_15 );
		}
		HeadQFNum++;
	}
}


public enum HidBtType : int
{
	QF_UP,
	QF_DOWN,
	
	ZULI_OPEN,
	ZULI_CLOSE,

	LED1_LIANG,
	LED1_SHAN,
	LED1_MIE,

	LED2_LIANG,
	LED2_SHAN,
	LED2_MIE,
	
	LED3_LIANG,
	LED3_SHAN,
	LED3_MIE,

	START_LED_LIANG,
	START_LED_SHAN,
	START_LED_MIE,

	FIRE_LED_LIANG,
	FIRE_LED_SHAN,
	FIRE_LED_MIE,

	CAIPIAO_DAYIN,

	QF_PLANE,
	QF_CHECK,

	OUT_CARD_LED_LIANG,
	OUT_CARD_LED_SHAN,
	OUT_CARD_LED_MIE,

	OPEN_FENGSHAN_L,
	OPEN_FENGSHAN_R,
	CLOSE_FENGSHAN_L,
	CLOSE_FENGSHAN_R,

	QUIT_BT,
	RESTART_BT
}