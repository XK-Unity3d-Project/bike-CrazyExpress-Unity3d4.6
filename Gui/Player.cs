using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Player {
	public delegate void EventHandel();
//	public event EventHandel TriggerEnter;
//	public delegate void Eventhandel();
	public event EventHandel LifeTimeEnd;
	public event EventHandel ContinueGame;
	private bool _bIsGameStart;
	public bool bIsGameStart
	{
		get
		{
			return _bIsGameStart;
		}
		set
		{
			_bIsGameStart = value;
		}
	}

	private int _life;
	public int Life
	{
		get
		{
			return _life;
		}
		set
		{
		
			_life=value;

			if(value==0&&LifeTimeEnd!=null)
			{
				if(!GlobalScript.GetInstance().player.IsPlayHuanHu)
				{
					LifeTimeEnd();
				}
			}

		}
	}
	public event EventHandel SpeedChange;
	private int _speed;
	public int Speed
	{
		get
		{
			return _speed;
		}
		set
		{
			float temp=_speed;
			_speed=value;
			if(temp!=value&&SpeedChange!=null)
			{
				SpeedChange();
			}
				
		}
	}
	public event EventHandel EnergyChange;
	private float _energy;
	public float Energy
	{
		get
		{
			return _energy;
		}
		set
		{
			float temp=_energy;
			_energy=value;
			if(temp!=value&&EnergyChange!=null)
			{
				EnergyChange();
			}
		}
	}
	public event EventHandel ScoreChange;
	private int _score = 0;
	public int Score
	{
	  get
		{
			return _score;
		}
		set
		{
			_score=value;
			if(ScoreChange!=null)
			{
				ScoreChange();
			}
		}
	}
	public event EventHandel IsreverseChange;
	private bool _isreverse;
	public bool Isreverse
	{
		get
		{
			return _isreverse;
		}
		set
		{
			_isreverse=value;
			if(IsreverseChange!=null)
			{
				IsreverseChange();
			}
		}
	}
	public event EventHandel IsGameOverChange;
	private bool _isGameOver;
	public bool IsGameOver
	{
		get
		{
			return _isGameOver;
		}
		set
		{
			_isGameOver=value;
			if(IsGameOverChange!=null)
			{
				IsGameOverChange();
			}
		}
	}
	public event EventHandel IsPassChange;
	private bool _isPass;
	public bool IsPass
	{
		get
		{
			return _isPass;
		}
		set
		{
			_isPass=value;
			if(IsPassChange!=null)
			{
				IsPassChange();
			}
		}
	}

	public bool IsPlayHuanHu = false;

	public event EventHandel IsRequestedChange;
	private bool _isRequested;
	public bool IsRequested
	{
		get
		{
			return _isRequested;
		}
		set
		{
			_isRequested=value;
			if(IsRequestedChange!=null)
			{
				IsRequestedChange();
			}
		}
	}
	public event EventHandel WaitingRequestChange;
	private bool _waitingRequest;
	public bool WaitingRequest
	{
		get
		{
			return _waitingRequest;
		}
		set
		{
			_waitingRequest=value;
			if(WaitingRequestChange!=null)
			{
				WaitingRequestChange();
			}
		}
	}

	private Vector3 _AimPossion;
	public Vector3 AimPossion
	{
		get{return _AimPossion;}
		set
		{
			_AimPossion=value;
		}
	}
	public void setAimPosition(Vector3 v,bool b)
	{
		AimPossion=v;
		CanSight=b;
	}
	public event EventHandel CanSightChange;
	private bool _CanSight;
	public bool CanSight
	{
		get
		{
			return _CanSight;
		}
		set
		{
			bool temp=_CanSight;
			_CanSight=value;
			if(temp!=value&&CanSightChange!=null)
			{

				CanSightChange();

			}
		}
	}
	public event EventHandel CanFireChange;
	private bool _CanFire;
	public bool CanFire
	{
		get
		{
			return _CanFire;
		}
		set
		{
			bool temp =_CanFire;
			_CanFire=value;
			if(temp!=value&&CanFireChange!=null)
			{
				if(!hasShowWangqiuGuidance)
				{
					showWangqiuGuidanceEvent();
					hasShowWangqiuGuidance=true;
				}
				CanFireChange();
			}
		}
	}
	public event EventHandel RankListChange;
	private List<playerRank> _RankList;
	public List<playerRank> RankList
	{
		get
		{
			return _RankList;
		}
		set
		{
			//List<playerRank> temp =_RankList;
			_RankList=value;
		
			if(RankListChange!=null)
			{
				RankListChange();
			}
		}
	}

	//public event EventHandel AddTennisEvent;
	public event EventHandel RemoveTennisEvent;
	private int _TennisCount;
	public int TennisCount
	{
		get
		{
			return _TennisCount;
		}
		set
		{
			//List<Ranklist> temp =_RankList;
			_TennisCount=value;
			
//			if(RankListChange!=null)
//			{
//				RankListChange();
//			}
		}
	}

	public void Addtennis()
	{
		TennisCount++;
//		if(AddTennisEvent != null)
//		{
//			AddTennisEvent();
//		}
	}
	public void RemoveTennis()
	{
		TennisCount--;
		if(TennisCount<0)
			TennisCount=0;
		if(RemoveTennisEvent!=null)
		{
		RemoveTennisEvent();
		}
	}
	public event EventHandel lotteryCountChangeEvent;
	private int _lotteryCount;
	public int LotteryCount
	{
		get{
			return _lotteryCount;
		}
		set
		{
			_lotteryCount=value;
			if(lotteryCountChangeEvent!=null)
			{
				lotteryCountChangeEvent();
			}
		}
	}
	public bool SlowedDown;
	public void AddLife(int le)
	{
		//Debug.Log("sssss1111111ssddd"+le);
	
		if (_life == 0) {
						Life += le;
//						Debug.Log ("sssssssddd" + le);
						if (ContinueGame != null)
								ContinueGame ();
				} else
		{
			Life+=le;		
		}

	}
//	public event EventHandel Start;
	public void StartGame()
	{
		if(!bIsGameStart)
		{
			bIsGameStart = true;
			//GlobalData.GetInstance().gameMode = GameMode.OnlineMode; //test
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				bikeNetUnity.setIsGameStart(true);
			}
			else
			{
				bike.setIsGameStart(true);
			}
		}

		if(ContinueGame!=null)
		ContinueGame();
	}
	private bool hasShowWangqiuGuidance;
	//private bool hasShowDianchiGuidance;
	public EventHandel showWangqiuGuidanceEvent;
	public EventHandel showDianchiGuidanceEvent;
	public delegate void AddBufferEventhandel(BufferKind b);
	public event AddBufferEventhandel AddBufferEvent;
	public void  AddBuffer(BufferKind b)
	{
		if(AddBufferEvent!=null)
		{
			string bb=b.ToString();
			//Debug.Log("*******************test bb " + bb);
			switch(bb)
			{
				case "Dianchi": 
				this.Energy= (float)BufferKind.Dianchi;
				/*if(!hasShowDianchiGuidance)
				{
					showDianchiGuidanceEvent();
					hasShowDianchiGuidance=true;
				}*/
				showDianchiGuidanceEvent();
				AddBufferEvent(BufferKind.Dianchi);
				break;

				case "Wangqiu":
				//this.Addtennis();
				AddBufferEvent(BufferKind.Wangqiu);
				break;

				case "Hanbao":
				//this.Score+=(int)BufferKind.Hanbao;
				AddBufferEvent(BufferKind.Hanbao);
				break;

				case "Jitui":
				//this.Score+=(int)BufferKind.Jitui;
				AddBufferEvent(BufferKind.Jitui);
				break;

				case "Shoubiao":
				//Debug.Log("**********************test shouBiao");
				this.AddLife((int)BufferKind.Shoubiao);
				AddBufferEvent(BufferKind.Shoubiao);
				break;

				case "BaoGuo":
				AddBufferEvent(BufferKind.BaoGuo);
				break;

				case "Jiashidian":
				//this.AddLife((int)BufferKind.Jiashidian);
				AddBufferEvent(BufferKind.Jiashidian);
				break;
			}
	  	}
	}
	public int LostTime{ get; set;}
	public int FinalRank{ get; set;}
	public event EventHandel wangQiuTranEvent;
	private Transform _wangqiuTran;

	public Transform wangQiuTran
	{
		get
		{
			return _wangqiuTran;
		}
			
		set
		{
			_wangqiuTran=value;
			if(wangQiuTranEvent!=null)
			{
				wangQiuTranEvent();
			}
		}
	}
}
