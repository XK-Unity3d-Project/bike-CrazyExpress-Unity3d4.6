using UnityEngine;
using System.Collections;

public class CenterControl : MonoBehaviour {
	public GameObject GameOver;
	//public GameObject caipiao;
	public GameObject dengdailianji;
	public GameObject fangxiangcuowu;
	public GameObject jixuyouxi;
	public GameObject tongyilianji;
	//public GameObject zunshoujiaogui;
	public GameObject FireUi;
	//public GameObject camera1;
	//public GameObject camera2;
	//public GameObject player;
	public GameObject wangqiu;
	public GameObject AddTime;
	// Use this for initialization
	void Start () {
		GlobalScript.GetInstance().player.LifeTimeEnd+=LifeTimeEnd;
		GlobalScript.GetInstance().player.IsGameOverChange+=IsGameOverChange;
//		GlobalScript.GetInstance().player.IsPassChange+=IsPassChange;
		GlobalScript.GetInstance().player.IsreverseChange+=IsreverseChange;
		GlobalScript.GetInstance().player.IsRequestedChange+=IsRequestedChange;
		GlobalScript.GetInstance().player.WaitingRequestChange+=WaitingRequestChange;
		GlobalScript.GetInstance().player.ContinueGame+=ContinueGame;
		GlobalScript.GetInstance().player.CanSightChange+=CanSightChange;
		//GlobalScript.GetInstance().player.AddTennisEvent+=AddTennisEvent;
		GlobalScript.GetInstance().player.RemoveTennisEvent+=RemoveTennisEvent;
		GlobalScript.GetInstance().player.AddBufferEvent+=AddBufferEvent;
		//GlobalScript.GetInstance().player.SpeedChange+=SpeedChange;
	}
	public void AddBufferEvent(BufferKind kind)
	{
		if (kind == BufferKind.Wangqiu)
		{
		 wangqiu.SetActive (true);
	     wangqiu.GetComponent<wangqiu> ().AddBuffer (kind);
		}
	
	}
	public void RemoveTennisEvent()
	{
		//Debug.Log("removeevent");
		wangqiu.GetComponent<wangqiu>().updataTennisUI();
	}
	public void CanSightChange()
	{
		//Debug.Log("222222222222"+GlobalScript.GetInstance().player.CanSight);
		if(GlobalScript.GetInstance().player.CanSight)
		{
//			Vector3 v=	camera1.camera.WorldToScreenPoint(player.transform.position);
//			//Vector3 v1=camera2.camera.ScreenToWorldPoint(v);
//			FireUi.transform.localPosition=new Vector3(v.x-camera2.camera.pixelWidth/2.0f,v.y-camera2.camera.pixelHeight/2.0f,0);
			FireUi.SetActive(true);
			FireUi.GetComponent<Fire>().playTween();
		}
		else
		{
			GlobalScript.GetInstance().player.CanFire=false;
			FireUi.SetActive(false);

		}
	}
	public void ContinueGame()
	{
		if(jixuyouxi.activeSelf)
		{
			jixuyouxi.SetActive(false);
			jixuyouxi.GetComponent<Jixuyouxi>().StopAllCoroutines();
		}
	}
	public void LifeTimeEnd()
	{
		jixuyouxi.SetActive(true);
		//GlobalScript.GetInstance().player.Life=-1;
		//Debug.Log("wasaw");
		jixuyouxi.GetComponent<Jixuyouxi>().StartTimer();

	}
	public void IsGameOverChange()
	{
		if(GlobalScript.GetInstance().player.IsGameOver)
		{
			GameOver.SetActive(true);
		}
		else
		{
			GameOver.SetActive(false);
		}
	}
//	public void IsPassChange()
//	{
////		if(GlobalScript.GetInstance().player.IsPass)
////		{
////			caipiao.SetActive(true);
////			caipiao.GetComponent<caipiao>().ShowLotteryCount();
////		}
////		else
////		{
////			caipiao.SetActive(false);
////		}
//	}
	public void IsreverseChange()
	{
		if(GlobalScript.GetInstance().player.Isreverse)
		{
			fangxiangcuowu.SetActive(true);
		}
		else
		{
			fangxiangcuowu.SetActive(false);
		}
	}
	public void IsRequestedChange()
	{
		if(GlobalScript.GetInstance().player.IsRequested)
		{
			tongyilianji.SetActive(true);
		}
		else
		{
			tongyilianji.SetActive(false);
		}
	}
	public void WaitingRequestChange()
	{
		if(GlobalScript.GetInstance().player.WaitingRequest)
		{
			dengdailianji.SetActive(true);
		}
		else
		{
			dengdailianji.SetActive(false);
		}
		
	}
	// Update is called once per frame
	//	void Update () {
	//
	//	}
}

