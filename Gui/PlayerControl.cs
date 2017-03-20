using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerControl : MonoBehaviour {
	//public Player player;
	public int lifeTime=300;
	[Range(0,60)]
	public int speed=20;
	[Range(0,100)]
	public int Energy=100;
	public bool IsGameOver;
	public bool WaitingRequest;
	public bool IsRequested;
	public bool IsPass;
	public bool Isreverse;
	public int score;
	public bool CanSight;

	public Ranklist[] rank=new Ranklist[]{
		new Ranklist{Name="baobo",IsPlayer=true},
		new Ranklist{Name="fengxiaoyou",IsPlayer=false},
		new Ranklist{Name="hanmierdun",IsPlayer=false},
		new Ranklist{Name="hanmierdun",IsPlayer=false}};
	// Use this for initialization
	void Start () {
		GlobalScript.GetInstance().player.Life=lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
		GlobalScript.GetInstance().player.Speed=speed;
		GlobalScript.GetInstance().player.Energy=Energy;
		GlobalScript.GetInstance().player.Score=score;
		//GlobalScript.GetInstance().player.IsGameOver=IsGameOver;
		GlobalScript.GetInstance().player.WaitingRequest=WaitingRequest;
		GlobalScript.GetInstance().player.IsRequested=IsRequested;
		//GlobalScript.GetInstance().player.IsPass=IsPass;
		GlobalScript.GetInstance().player.Isreverse=Isreverse;
		//GlobalScript.GetInstance().player.CanSight=CanSight;
		//GlobalScript.GetInstance().player.CanFire=CanFire;
		//GlobalScript.GetInstance().player.RankList=new List<Ranklist>(rank);
	}
//	void  OnGUI()
//	{
//		if(GUI.Button(new Rect(20,150,80,50),"add"))
//		{
//			GlobalScript.GetInstance().player.AddBuffer(BufferKind.Shoubiao);
//		}
//	}
}
