using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour {
	private static  GlobalScript Instance;
	public static GlobalScript GetInstance()
	{
		if (!Instance) {
			Instance=(GlobalScript)GameObject.FindObjectOfType(typeof(GlobalScript));

			if(!Instance)
				Debug.LogError("There needs to be one active MyClass script on a GameObject in your scene.");

			Instance.InitPlayer();
		}

		return Instance;
	}
//	void Awake()
//	{
//		InitPlayer();
//	}
	public Player player;
	public void InitPlayer()
	{
		player=new Player();
		player.Energy = 100f;
		player.Speed = 0;
		player.Score = 0;

		//GlobalData.GetInstance().GameDiff = "0"; //test
		switch(GlobalData.GetInstance().GameDiff)
		{
		case "0":
			player.Life = 80; //game diff low
			break;
		case "1":
			player.Life = 60; //game diff middle
			break;
		case "2":
			player.Life = 40; //game diff high
			break;
		default:
			player.Life = 60;
			break;
		}
		//player.Life = 3600; //test

		player.LotteryCount = 0;
		player.FinalRank = 8;
		player.IsPlayHuanHu = false;
	}
	public delegate void EventHandel();
	public event EventHandel ShowLosttimeEvent;
	public void ShowLostTime()
	{
		if(ShowLosttimeEvent!=null)
		ShowLosttimeEvent ();
	}
	public event EventHandel ShowEndPageEvent;
	public void ShowEndPage()
	{
		if(ShowEndPageEvent!=null)
		ShowEndPageEvent ();
	}
	public event EventHandel ShowFinalScoreEvent;
	public void ShowFinalScore()
	{
		if(ShowFinalScoreEvent!=null)
			ShowFinalScoreEvent ();
	}
	public event EventHandel ShowCaipiaoEvent;
	public void ShowCaipiao()
	{
		if(ShowCaipiaoEvent!=null)
			ShowCaipiaoEvent ();
	}
	public event EventHandel ShowFinalRankEvent;
	public void ShowFinalRank()
	{
		if(ShowFinalRankEvent!=null)
			ShowFinalRankEvent ();
	}
	public delegate void EventHandel1(TishiInfo tishi);
	public event EventHandel1 ShowTishiEvent;
	public void ShowTishi(TishiInfo tishi)
	{
		if(ShowTishiEvent!=null)
			ShowTishiEvent (tishi);
	}
	public event EventHandel AddTimeEvent;
	public void AddTime()
	{
		if(AddTimeEvent!=null)
			AddTimeEvent ();
	}
	public event EventHandel ChangeNPCEvent;
	public void ChangeNPC()
	{
		if(ChangeNPCEvent!=null)
			ChangeNPCEvent ();
	}
}
public enum TishiInfo
{
	Diedao,
	Jiashidian,
	Baoguo,
	Luduan,
	Daojishi,
	Sudu

}
