using UnityEngine;
using System.Collections;

public class PcvrInfoCtrl : MonoBehaviour {
	public UILabel BtInfoLB;
	public UILabel FangXiangInfoLB;
	public UILabel TaBanInfoLB;
	public UILabel YouMenInfoLB;
	public UILabel ShaCheLInfoLB;
	public UILabel ShaCheRInfoLB;
//	public static float FangXiangVal;
//	public static float TaBanVal;
//	public static float YouMenVal;
//	public static float ShaCheLVal;
//	public static float ShaCheRVal;
	public static bool IsActivePcvrInfo;
	// Use this for initialization
	void Start()
	{
		IsActivePcvrInfo = true;
		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickStartBtEvent += ClickStartBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtEvent += ClickFireBtEvent;
		InputEventCtrl.GetInstance().ClickStopDongGanBtEvent += ClickStopDongGanBtEvent;
	}
	
	// Update is called once per frame
	void Update()
	{
		FangXiangInfoLB.text = pcvr.SteerValCurAy[0].ToString();
		TaBanInfoLB.text = pcvr.BianMaQiCurVal[0].ToString();
		YouMenInfoLB.text = pcvr.YouMenCurVal[0].ToString();
		ShaCheLInfoLB.text = pcvr.ShaCheLCurVal[0].ToString();
		ShaCheRInfoLB.text = pcvr.ShaCheRCurVal[0].ToString();
	}
	
	void ClickStopDongGanBtEvent(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			BtInfoLB.text = "MovementBt";
			break;
		case ButtonState.UP:
			BtInfoLB.text = "xxxxxx";
			break;
		}
	}

	void ClickFireBtEvent(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			BtInfoLB.text = "FireBt";
			break;
		case ButtonState.UP:
			BtInfoLB.text = "xxxxxx";
			break;
		}
	}

	void ClickStartBtEvent(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			BtInfoLB.text = "StartBt";
			break;
		case ButtonState.UP:
			BtInfoLB.text = "xxxxxx";
			break;
		}
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			BtInfoLB.text = "SetEnterBt";
			break;
		case ButtonState.UP:
			BtInfoLB.text = "xxxxxx";
			break;
		}
	}
	
	void ClickSetMoveBtEvent(ButtonState val)
	{
		switch (val) {
		case ButtonState.DOWN:
			BtInfoLB.text = "SetMoveBt";
			break;
		case ButtonState.UP:
			BtInfoLB.text = "xxxxxx";
			break;
		}
	}
}