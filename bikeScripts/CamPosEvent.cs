using UnityEngine;
using System.Collections;

public enum MiaoZhunGenSuiCtrl : int
{
	ONE = 0,
	TWO = 1,
	THREE = 2,
	FOURTH = 3,
	THIRD_PERSON = 4
}

public class CamPosEvent : MonoBehaviour {

	public BikeCamEvent PosEvent;

	public MiaoZhunGenSuiCtrl MiaoZhunGenSuiNum = MiaoZhunGenSuiCtrl.ONE;
	public Transform JingZhiCamTrans;

	public Transform NextCamEvent;
	
	// Use this for initialization
	void Start ()
	{
		if(PosEvent == BikeCamEvent.JingZhi && JingZhiCamTrans == null)
		{
			ScreenLog.LogWarning("CamPosEvent::Start -> JingZhiCamTrans is null, PosEvent = " + PosEvent);
		}

		if(PosEvent == BikeCamEvent.JingZhiMiaoZhun && JingZhiCamTrans == null)
		{
			ScreenLog.LogWarning("CamPosEvent::Start -> JingZhiCamTrans is null, PosEvent = " + PosEvent);
		}
	}
	
	// Update is called once per frame
//	void Update () {
//	
//	}
}
