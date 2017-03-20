
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using SLAB_HID_DEVICE;

public class bikeNetUnity : MonoBehaviour {

	#region #bike net data start
	public NetworkPlayer ownerPlayer;
	private int playerRotate;
	private Vector3 playerTransform;

	public static bool isNetworkCheck = false;
	#endregion #bike net data end
	
	#region #bike data start
	public GameObject TouYingObj;
	private Projector TouYingPro;
	public Material TouYingMatZhong;
	public Material TouYingMatZuo;
	public Material TouYingMatYou;

	public GameObject FengXiaoObj;
	Transform kaCheTran;
	BikeAnimatorCtrl BikeAniScript;
	static public bool isIntoSuiDao = false;
	bool isIntoSuiDaoNPC = false;
	
	private GameObject playerSkinnedObj = null;
	
	public GameObject ParticleObj = null;
	public GameObject WaterParticleObj = null;

	private Vector3 mPosOld = Vector3.zero;
	static private int mDirWrongCount = 0;
	private bool bIsGameOver = false;
	
	static private Transform mLastAimNPC = null;
	
	static public bool bIsHitFall = false;
	
	public Transform wangQiuTran = null;
	
	bool bIsIntoFeiBan = false;
	private bool IsIntoXiaoFeiBan = false;
	
	int brakeState = 0;
	static public GameObject mBikeGameCtrl = null;
	private float mTimeSteerKey = 0f;
	
	private bool bIsRenderMat = true;

	private int mWuDiTimeCount = 0;
	private bool bIsWuDiState = false;
	
	private bool bIsPlayerBrakeBike = false;
	private bool bIsDoHurtAction = false;
	private bool bIsInitInfo = false;
	
	public static bool bIsGameStart = false;
	public static bool bIsSelectPlayer = false;
	
	public static float MaxMouseDownCount = 9500f;
	public static float MinMouseDownCount = 0f;
	
	float mMouseDownCount = 0;
	float mMaxMouseDownCount = 9500; // 7 * 60
//	float mMouseDownTime = 0f;
//	bool bIsMouseDown = false;
	private bool bIsDoTimeOver = false;
	
	public static int mGameTime = 1000;
	public Transform mBikeAimMark = null;
	
	private int mRankNum = 0;
	static public int mRankCount = 0;
	static public  playerRank [] mRankPlayer = null;
	public Transform mBikePlayer = null;
	
	static public float mMaxVelocity = 80f; //km/h
	static private float mMaxVelocityFoot = 65f;
	public GameObject mBike = null;
	public Transform centerOfMass = null;
	public Transform wheelForward = null;
	public Transform wheelBack = null;
	
	public Transform mAimPoint = null;
	public Transform mCamPoint_back = null; //back
	public Transform mCamPoint_left = null; //left

	public Transform []HangPaiTrans;
	public Transform []MiaoZhunGenSui;

	Transform backPointParent = null;
	Vector3 backLocalPos;

	static public bool isPlayPiaoYi = false;
	
	private Vector3 mBakeTimePointPos = Vector3.zero;
	private Vector3 mBakeTimePointRot = Vector3.zero;
	
	private GameObject mPlayer = null;
	static public BikeCameraNet mScriptCam = null;
	static public JiTui mJiTuiScript = null;

	private float throttle = 0f;
	private float mSteer = 0f;
	private float mStateSpeed = 1.0f;
	private float currentEnginePower = 0f;
	private float currentBrakeEnginePower = 0f;
	private float currentBrakeWaterPower = 0f;
	
	public bool bIsFall = false;
	
	float mSpeed = 0f;
	float mSpeedOld = 0f;

	private bool bIsTurnLeft = false;
	private bool bIsTurnRight = false;
	
	private bool isCloneObject = false;
	private int mRunState = -1;
	private int ROOT = 0;
	private const int STATE_RUN1 = 1; //run1 action
	private const int STATE_RUN2 = 2; //run2 action
	private const int STATE_RUN3 = 3; //run action
	private const int STATE_RUN4 = 4; //run3 action
	
	private bool bIsMixAction = false;
	private float mSteerTimeCur = 0f;
	private float maxSteerTime = 5f;
	
	private bool bIsDoFly = false;
	private string mRankingPlayer = "rankingPlayer";
	
	private bool bIsMoveUp = false;
	private bool bIsMoveDown = false;
	private float mResistanceForce = 0f;
	
	private float mThrottleForce = 0f;

	private bool bIsSpacePlayer = false;
	private float mSpaceTime = 0f;
	
	static public int mAiCtrlNum = 0;
	
	private bool isPlayCool2 = false;
	private bool isPlayCool3 = false;
	#endregion #end bike data
	
	#region #start AI
	private bool bIsAiNPC = false;
	
	private Transform mAiPathCtrl = null;

	static public Transform mPlayerObject = null;
	private bool bIsStopAi = false;
	private bool bIsSpaceAi = false;

	private Vector3 [] mAiPathMarkPos = null;
	private float [] aiMarkSpeed = null;
	#endregion #end AI

	#region #bike contrl
	public bool getIsAiNPC()
	{
		return bIsAiNPC;
	}
	
	public float GetBikeSpeed()
	{
		return mSpeed;
	}
	
	float getRandPer( int maxPer )
	{
		float per = (float)Random.Range(0, maxPer) / 100f;
		//ScreenLog.Log("getRandPer -> per: " + per);
		return per;
	}
	
	void initAiPlayerPathInfo()
	{
		if(bIsAiNPC)
		{
			if(mAiPathCtrl != null)
			{
				//mPathNode = 0;
				int count = mAiPathCtrl.childCount;
				if(count == 0)
				{
					return;
				}
				mAiPathMarkPos = new Vector3[count];
				aiMarkSpeed = new float[count];
				
				Vector3 posAiMark = Vector3.zero;
				Vector3 posAiMarkTmp = Vector3.zero;
				for(int index = 0; index < count; index++)
				{
					posAiMarkTmp = Vector3.zero;
					Transform tranChild = mAiPathCtrl.GetChild(index);
					
					posAiMark = tranChild.position;
					if(Random.Range(0, 2) == 0)
					{
						posAiMarkTmp += getRandPer(40) * tranChild.localScale.x * tranChild.right;
					}
					else
					{
						posAiMarkTmp -= getRandPer(40) * tranChild.localScale.x * tranChild.right;
					}
					
					if(Random.Range(0, 2) == 0)
					{
						posAiMarkTmp += getRandPer(40) * tranChild.localScale.z * tranChild.forward;
					}
					else
					{
						posAiMarkTmp -= getRandPer(40) * tranChild.localScale.z * tranChild.forward;
					}
					posAiMark += posAiMarkTmp;
					
					mAiPathMarkPos[index] = posAiMark;
					
					int markMaxSpeed = 62; //ai zuiDa suDu = markSpeed * 1.5f
					int markMinSpeed = 30; //ai suiJi zuiDi sudu
					int randSpeed = Random.Range(markMinSpeed, markMaxSpeed);
					aiMarkSpeed[index] = randSpeed;
				}
			}
		}
	}
	
	
	public GameObject getRankPlayer(int index)
	{
		return mRankPlayer[index].player;
	}

	public bool GetIsAiNPC()
	{
		return bIsAiNPC;
	}

	// Use this for initialization
	void Start()
	{
		XkGameCtrl.IsLoadingLevel = false;
		BikeAniScript = GetComponent<BikeAnimatorCtrl>();
		StartInitInfo();
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			if(Network.isClient)
			{
				handlePlayerCam();
			}
		}
	}
	
	public static void resetBikeStaticInfo()
	{
		//ScreenLog.Log("reset bike static info...");
		mRankCount = 0;
		mRankPlayer = null;
		mBikeGameCtrl = null;
		//bIsGameOver = false;
		isIntoSuiDao = false;
		bIsGameStart = false;
		BikeCamera.bIsAimPlayer = false;

		Screenshot.IsShootPlayer = false;
		Go.IsStartGame = false;

		bikeAiNetUnity.resetBikeStaticInfo();
	}

	public bool CheckObjIsPlayer()
	{
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			return true;
		}
		return false;
	}

	void StartInitInfo()
	{
		if(!bIsHitFall)
		{
			bIsHitFall = false;
			CancelInvoke("setIsHitFall");
			Invoke("setIsHitFall", 2.0f);
		}

		TouYingPro = TouYingObj.GetComponent<Projector>();
		if(backPointParent == null)
		{
			backPointParent = mCamPoint_back.parent;
			backLocalPos = mCamPoint_back.localPosition;
		}
		else
		{
			mCamPoint_back.parent = backPointParent;
			mCamPoint_back.localPosition = backLocalPos;
		}

		if(GlobalScript.GetInstance().player != null)
		{
			GlobalScript.GetInstance().player.Speed = 10;
			GlobalScript.GetInstance().player.Speed = 0;
		}

		if(GlobalData.GetInstance().gameMode != GameMode.SoloMode)
		{
			bIsAiNPC = false;
			
			mPlayerObject = transform;
			if(mScriptCam == null)
			{
				mScriptCam = Camera.main.GetComponent<BikeCameraNet>();
			}
		}
		
		if(GlobalScript.GetInstance().player == null)
		{
			return;
		}
		
		if(mMaxVelocity > 80f)
		{
			mMaxVelocity = 80f;
		}
		
		PlayerPrefs.SetInt(mRankingPlayer, 0);
		mSpeed = 0f;
		
		mPlayer = this.gameObject;
		if(mPlayer == null)
		{
			Debug.LogWarning("bike::start -> mPlayer is null!");
		}
		else
		{
			setPlayerMixAction( mPlayer );
			setPlayerChildRigibody( mPlayer.transform, false );
		}
		
		SetCenterOfMass();
		startCheckAiPosition();

		int maxRankPlayer = 2;
		if(!bIsInitInfo && mRankCount < maxRankPlayer)
		{
			bIsInitInfo = true;
			if(mAiPathCtrl == null)
			{
				if(mBikeGameCtrl == null)
				{
					mBikeGameCtrl = GameObject.Find(GlobalData.bikeGameCtrl);
				}
				BikeGameCtrl script = mBikeGameCtrl.GetComponent<BikeGameCtrl>();
				mAiPathCtrl = script.mAiPathCtrl;
			}

			if (mRankPlayer == null)
			{
				mRankPlayer = new playerRank[maxRankPlayer];
				for(int i = 0; i < maxRankPlayer; i++)
				{
					mRankPlayer[i] = new playerRank();
				}
			}
			
			if ( mRankCount < maxRankPlayer && mRankPlayer[mRankCount].player == null )
			{
				mRankNum = mRankCount;
				mRankPlayer[mRankCount].player = gameObject;
				//mRankPlayer[mRankCount].IsPlayer = !bIsAiNPC;
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
				{
					if(Network.isClient)
					{
						mRankPlayer[mRankCount].IsPlayer = true;
						RankHandleCtrl.GetInstance().SetRankPlayerArray(gameObject, true);
						//ScreenLog.Log("mRankPlayer.player " + this.name);
					}
				}
				else if(!FreeModeCtrl.IsServer)
				{
					RankHandleCtrl.GetInstance().SetRankPlayerArray(gameObject, false);
				}
				mRankCount++;
			}
			
//			if(GlobalScript.GetInstance().player != null)
//			{
//				GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
//			}
			
			if(mBakeTimePointPos == Vector3.zero && mAiPathCtrl != null)
			{
				if (mAiPathCtrl.childCount > 0)
				{
					Transform mark = mAiPathCtrl.GetChild(0);
					if(bIsAiNPC)
					{
						float key = 1f;
						if(Random.Range(0, 100) < 50)
						{
							key = -1f;
						}
						mBakeTimePointPos = mark.position + key * mark.forward * mark.localScale.z * 0.5f;
					}
					else
					{
						if(gameObject.name == "NPC_04NetUnity")
						{
							mBakeTimePointPos = mark.position + mark.forward * mark.localScale.z * 0.5f;
							//mBakeTimePointPos = mark.position;
						}
						else
						{
							mBakeTimePointPos = mark.position - mark.forward * mark.localScale.z * 0.5f;
						}
					}
					mBakeTimePointRot = mark.right;
					mBikeAimMark = mark;
					if(!FreeModeCtrl.IsServer && mBikeAimMark != null)
					{
						RankHandleCtrl.GetInstance().SetBikeAimMark(mBikeAimMark.parent.name,
						                                            GetAimMarkId(mBikeAimMark), gameObject.name);
					}
				}
			}
			//initAiPlayerPathInfo();
		}
		
		gameObject.layer = LayerMask.NameToLayer("Player");
	}

	void resetServerFollowPlayer()
	{
		BikeCameraNet.mBikePlayer = null;

		Invoke("resetIsMakeServerFollow", 1.0f);
	}

	void resetIsMakeServerFollow()
	{		
		IsMakeServerFollow = false;
	}

	public bool IsMakeServerFollow = false;
	//make server camera follow player
	void makeServerFollowPlayer()
	{
		if(IsMakeServerFollow)
		{
			return;
		}

		if(Network.isServer)
		{
			if(mScriptCam == null)
			{
				mScriptCam = Camera.main.GetComponent<BikeCameraNet>();
			}

			if(BikeCameraNet.mBikePlayer != null)
			{
				if(BikeCameraNet.mBikePlayer.name != gameObject.name)
				{
					return;
				}
			}
			IsMakeServerFollow = true;

			float randTime = Random.Range(6f, 12f);
			Invoke("resetServerFollowPlayer", randTime);

			mScriptCam.setBikePlayer( gameObject );

			Camera.main.transform.position = mCamPoint_back.position;
			Camera.main.transform.LookAt(mAimPoint);
		}
	}

	void activeChangeCamPosEvent(BikeCamEvent camEvent)
	{
		if(Network.isClient)
		{
			return;
		}
		//ScreenLog.Log("activeChangeCamPosEvent ****** camEvent " + camEvent);

		BikeCameraNet.mBikePlayer = null;
		mScriptCam.setBikePlayer( gameObject );

		mScriptCam.setBikeCamPosEvent( camEvent, eventScript );

		switch(camEvent)
		{
		case BikeCamEvent.JingZhi:
			Camera.main.transform.position = eventScript.JingZhiCamTrans.position;
			Camera.main.transform.rotation = eventScript.JingZhiCamTrans.rotation;
			break;
			
		case BikeCamEvent.HangPai:
			break;
			
		case BikeCamEvent.JingZhiMiaoZhun:
			Camera.main.transform.position = eventScript.JingZhiCamTrans.position;
			Camera.main.transform.LookAt(mAimPoint);
			break;
			
		case BikeCamEvent.MiaoZhunGenSui:
			break;
			
		case BikeCamEvent.TeDingDongZuo:
			break;
			
		case BikeCamEvent.GameOver:
			break;
			
		default:
			break;
		}
	}
	
	void setPlayerMixAction(GameObject player)
	{
		if(player == null)
		{
			Debug.LogWarning("bike::setPlayerMixAction -> player is null!");
			return;
		}
		
		if(bIsMixAction)
		{
			return;
		}
		bIsMixAction = true;
	}

	public float getMoveState()
	{
		if(mSpeed > 1f)
		{
			return 1f;
		}
		return 0f;
	}
	
//	public void setBikeMouseDown()
//	{
//		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
//		{
//			float taBanVal = InputEventCtrl.PlayerTB[0];
////			if( (!pcvr.bIsHardWare && Input.GetMouseButton(0))
////			   || (pcvr.bIsHardWare && pcvr.bPlayerHitTaBan))
//			if(taBanVal > 0f)
//			{
//				bIsMouseDown = true;
//				if(!pcvr.bIsHardWare)
//				{
//					float dTime = Time.time - mMouseDownTime;
//					mMouseDownCountCurrent = Mathf.FloorToInt((190f / dTime) + 0.5f);
//					mMouseDownTime = Time.time;
//				}
//				else
//				{
//					mMouseDownCountCurrent = pcvr.TanBanDownCount;
//				}
//				
//				if(mMouseDownCountCurrent < 3f)
//				{
//					mMouseDownCountCurrent = 0f;
//				}
//				
//				if(MaxMouseDownCount < mMouseDownCountCurrent)
//				{
//					MaxMouseDownCount = mMouseDownCountCurrent;
//				}
//			}
//			else
//			{
//				if(!pcvr.bIsHardWare)
//				{
//					if(bIsMouseDown)
//					{
//						bIsMouseDown = false;
//					}
//					else if (mMouseDownCountCurrent > 0f)
//					{
//						float dTime = Time.time - mMouseDownTime;
//						if(dTime > 0.1f)
//						{
//							mMouseDownCountCurrent = 0f;
//						}
//					}
//				}
//				else
//				{
//					if(bIsMouseDown)
//					{
//						bIsMouseDown = false;
//					}
//					mMouseDownCountCurrent = 0f;
//				}
//			}
//		}
//	}
	
	public float GetBikeSteer()
	{
		return mSteer;
	}

	float throttleCurrent = 0f;
	float throttleClient = 0f;

	float mSteerCurrent = 0f;
	float mSteerClient = 0f;

	float mMouseDownCountCurrent = 0f;
	float mMouseDownCountClient = 0f;

	[RPC]
	void SendBikeInputInfo(float throttleVal, float steerVal, float mMouseDownCountVal)
	{
		if(mPlayer == null)
		{
			return;
		}

		//ScreenLog.Log("throttleVal " + throttleVal + ", steerVal " + steerVal + ", mMouseDownCountVal " + mMouseDownCountVal);
		throttle = throttleVal;
		mSteer = steerVal;
		mMouseDownCount = mMouseDownCountVal;
		SetTurnValAni(steerVal);

//		AnimationState turnRight1 = mPlayer.animation["turnRight1"];
//		turnRight1.normalizedTime = Mathf.Abs( mSteer );

//		AnimationState turnLeft1 = mPlayer.animation["turnLeft1"];
//		turnLeft1.normalizedTime = Mathf.Abs( mSteer );
	}

	void GetInput()
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}

		if(mPlayer == null)
		{
			Start();
		}
		
		if(!bIsAiNPC)
		{
//			setBikeMouseDown();

			float steerTmp = InputEventCtrl.PlayerFX[0];
			if(mGameTime > 0)
			{
				throttleCurrent = GlobalScript.GetInstance().player.Energy > 0f ? InputEventCtrl.PlayerYM[0] : 0f;
//				if(pcvr.bIsHardWare)
//				{
////					steerTmp = pcvr.mBikeSteer;
//					if(GlobalScript.GetInstance().player.Energy > 0)
//					{
//						throttleCurrent = pcvr.mBikeThrottle;
//					}
//					else
//					{
//						throttleCurrent = 0f;
//					}
//				}
//				else
//				{
////					steerTmp = Input.GetAxis("Horizontal");
//					if(GlobalScript.GetInstance().player.Energy > 0)
//					{
//						throttleCurrent = Input.GetAxis("Vertical");
//					}
//					else
//					{
//						throttleCurrent = 0f;
//					}
//				}
				
				if(bIsDoFly || bIsIntoFeiBan)
				{
					steerTmp = 0f;
				}
			}
			else
			{
				mMouseDownCountCurrent = 0f;
				throttleCurrent = 0f;
				steerTmp = 0;
			}

			mSteerCurrent = steerTmp;
			SetTurnValAni(steerTmp);
			if(throttleCurrent != throttleClient || mSteerCurrent != mSteerClient
			   || mMouseDownCountCurrent != mMouseDownCountClient)
			{
				throttleClient = throttleCurrent;
				mSteerClient = mSteerCurrent;
				mMouseDownCountClient = mMouseDownCountCurrent;
				if(Network.isClient)
				{
					networkView.RPC("SendBikeInputInfo", RPCMode.OthersBuffered, throttleCurrent, mSteerCurrent, mMouseDownCountCurrent);
				}
			}
		}
		
		if(mSpeed > 2f && IsInvoking("resetBikeUseGravity"))
		{
			CancelInvoke("resetBikeUseGravity");
		}

		if (mGameTime > 0 && !bIsAiNPC && bIsDoTimeOver)
		{
			bIsDoTimeOver = false;
		}
		
		if (!bIsDoFly)
		{
			if(mSpeed >= 40f || (!bIsAiNPC && bIsMoveUp && currentBrakeWaterPower > 10f)
			   || (!bIsAiNPC && currentBrakeWaterPower < 0f && mSpeed > 5f) || mMouseDownCountClient > 0)
			{
				if(mSpeed > 50f)
				{
					//ScreenLog.Log("mMouseDownCount " + mMouseDownCount + ", mSpeed " + mSpeed);
					if( mRunState != STATE_RUN4 && mMouseDownCountClient > 1f )
					{
						mRunState = STATE_RUN4;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.run3);
							//Debug.Log("*******test run3");
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run3");
						}
					}
					else if(mRunState != STATE_RUN3 && mMouseDownCountClient < 1f)
					{
						mRunState = STATE_RUN3;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.run);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run");
						}
					}
				}
				else
				{
					if( mRunState != STATE_RUN2 && mMouseDownCountClient > 1f )
					{
						mRunState = STATE_RUN2;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.run2);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run2");
						}
					}
					else if(mRunState != STATE_RUN3 && mMouseDownCountClient < 1f)
					{
						mRunState = STATE_RUN3;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.run);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run");
						}
					}
				}
			}
			else
			{
				if(mSpeed < 1f)
				{
					if(mGameTime <= 0 && !bIsAiNPC)
					{
						if(!bIsDoTimeOver)
						{
							bIsDoTimeOver = true;
							mRunState = -1;
							PlayAnimation(PlayerAniEnum.root);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "root");
						}
					}
					else if(mRunState != ROOT)
					{
						bIsDoTimeOver = false;
						Invoke("resetBikeUseGravity", 0.5f);
						mRunState = ROOT;

						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.root);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "root");
						}
					}
					currentBrakeWaterPower = 0f;
				}
				else if (!bIsMoveUp)
				{
					if(mRunState != STATE_RUN1 && mMouseDownCountClient > 1f)
					{
						mRunState = STATE_RUN1;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.run1);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run1");
						}
					}
					else if(mRunState != STATE_RUN3 && mMouseDownCountClient < 1f)
					{
						mRunState = STATE_RUN3;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.run);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run");
						}
					}
				}
			}
		}
		
		if( (brakeState > 0 && !bIsAiNPC && mSpeed < 5f)
		   || (brakeState <= 0 && !bIsAiNPC && mSpeed < 2f) )
		{
			if(mRunState != ROOT)
			{
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
				bIsDoTimeOver = false;
				mRunState = ROOT;
				currentBrakeWaterPower = 0f;

				if(Network.isClient)
				{
					PlayAnimation(PlayerAniEnum.root);
					networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "root");
				}
			}
		}
		
		if(mSpeed < 2f && !bIsFall)
		{
			if(throttle > 0.011f || mMouseDownCountClient > 15f)
			{
				if((!bIsAiNPC && !bIsPlayerBrakeBike) || bIsAiNPC)
				{
					rigidbody.isKinematic = false;
					rigidbody.useGravity = true;
				}
			}
		}

		if(Network.isClient && checkHaveOwnerPlayer() && Network.player == ownerPlayer && Time.frameCount % 5 == 0)
		{
			if(mRunState == STATE_RUN1 || mRunState == STATE_RUN2 || mRunState == STATE_RUN4)
			{
				float minAnimationSpeed = 0.25f;
				float maxAnimationSpeed = 2.3f;
				float key = (maxAnimationSpeed - minAnimationSpeed) / MaxMouseDownCount;
				mStateSpeed = key * mMouseDownCountCurrent + minAnimationSpeed;
								
				if (mStateSpeed > maxAnimationSpeed)
				{
					mStateSpeed = maxAnimationSpeed;
				}
				else if(mStateSpeed < minAnimationSpeed)
				{
					mStateSpeed = minAnimationSpeed;
				}
				
				switch(mRunState)
				{
				case STATE_RUN1:
//					AnimationState AniRun1 = mPlayer.animation["run1"];
//					AniRun1.speed = mStateSpeed;
					break;
				case STATE_RUN2:
//					AnimationState AniRun2 = mPlayer.animation["run2"];
//					AniRun2.speed = mStateSpeed;
					break;
				case STATE_RUN4:
//					AnimationState AniRun4 = mPlayer.animation["run3"];
//					AniRun4.speed = mStateSpeed;
//					Debug.Log("**********layer "+ AniRun4.layer);
					break;
				}

				if(NetworkServerNet.GetInstance() != null)
				{
					networkView.RPC("setServerBikeActionSpeed", RPCMode.OthersBuffered, mStateSpeed, mRunState);
				}
			}
		}
		return;
	}
	
	void PlayAnimation(PlayerAniEnum ani)
	{
		if (BikeAniScript == null) {
			return;
		}
		
		if (ani != PlayerAniEnum.Cool2 && isPlayCool2) {
			isPlayCool2 = false;
		}
		
		if (ani != PlayerAniEnum.Cool3 && isPlayCool3) {
			isPlayCool3 = false;
		}
		//Debug.Log("PlayAnimation -> action "+ani);
		BikeAniScript.PlayAnimation(ani, 1f);
	}

	bool IsPlayTurn;
	void SetTurnValAni(float steer)
	{
		if (BikeAniScript == null) {
			return;
		}
		
		float val = (steer + 1) * 0.5f;
		if (val == 0.5f && IsPlayTurn) {
			IsPlayTurn = false;
			BikeAniScript.SetAniBool(PlayerAniEnum.IsTurn, false);
			BikeAniScript.SetAnimatorFloat(PlayerAniEnum.TurnVal, 0.5f);
		}
		else if (val != 0.5f && !IsPlayTurn){
			IsPlayTurn = true;
			BikeAniScript.SetAniBool(PlayerAniEnum.IsTurn, true);
		}
		
		if (val != 0.5f) {
			BikeAniScript.SetAnimatorFloat(PlayerAniEnum.TurnVal, val);
		}
	}

	[RPC]
	void setServerBikeActionSpeed( float actionSpeed, int ActionState )
	{
		if(mPlayer == null)
		{
			return;
		}

		switch(ActionState)
		{
		case STATE_RUN1:
//			AnimationState AniRun1 = mPlayer.animation["run1"];
//			AniRun1.speed = actionSpeed;
			break;
		case STATE_RUN2:
//			AnimationState AniRun2 = mPlayer.animation["run2"];
//			AniRun2.speed = actionSpeed;
			break;
		case STATE_RUN4:
//			AnimationState AniRun4 = mPlayer.animation["run3"];
//			AniRun4.speed = actionSpeed;
			break;
		}
	}

	void checkBikeEnergy()
	{
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			float energy = GlobalScript.GetInstance().player.Energy;
			if(energy > 0 && throttleClient > 0.15f)
			{
				float subEnergy = 0f;
				int level = Mathf.FloorToInt((throttleClient * 100) / 33);
				switch(level)
				{
				case 0:
					mMaxVelocity = 30;
					//subEnergy = 3.9f;
					break;
				case 1:
					mMaxVelocity = 45;
					//subEnergy = 5.5f;
					break;
				default:
					mMaxVelocity = 60;
					//subEnergy = 7.5f;
					break;
				}
				subEnergy = 100f / 15f; //15秒.
				
				subEnergy = subEnergy * Time.deltaTime;
				energy -= subEnergy;
				GlobalScript.GetInstance().player.Energy = energy;
			}
		}
	}

	void handleBikeRotation()
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}

		checkBikeEnergy();
		if(mSteerClient != 0f && !bIsAiNPC)// && mSpeed > 15f)
		{
			mTimeSteerKey += Time.deltaTime;
			if(mTimeSteerKey >= 0.1f)
			{
				if(mTimeSteerKey >= 0.2f)
				{
					mTimeSteerKey = 0f;
				}
			}
		}
	
		if(mSpeed > 2f && IsInvoking("resetBikeUseGravity"))
		{
			CancelInvoke("resetBikeUseGravity");
		}
		
		float rotSpeed = 50 * mSteerCurrent * Time.smoothDeltaTime;
		if(mSteerCurrent > 0f)
		{
			bool isCanTurn = true;
			if(Physics.Raycast(mAimPoint.position, transform.right, 1.3f) && mSpeed > 1)
			{
				isCanTurn = false;
			}
			else
			{
				if(mSpeed > 5f)
				{
					if(mSteerTimeCur < 0f)
					{
						mSteerTimeCur += 5f * Time.deltaTime;
					}
					
					if(mSteerTimeCur < maxSteerTime)
					{
						mSteerTimeCur += (0.5f * Time.deltaTime);
					}
				}
			}
			
			if(isCanTurn && mSteerClient > 0.1f)
			{
				transform.Rotate(0, rotSpeed, 0);
			}
			
			bIsTurnLeft = false;
			if(!bIsTurnRight)
			{
				if(!bIsAiNPC)
				{
					TouYingPro.material = TouYingMatYou;
				}
				bIsTurnRight = true;
				if(mSteerClient > 0.4f)
				{
					networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "turnRight1");
				}
			}

			if(Mathf.Abs( mSteerClient ) < 0.4f)
			{
				bIsTurnRight = false;
			}
		}
		else if(mSteerCurrent < 0f)
		{
			bool isCanTurn = true;
			if(Physics.Raycast(mAimPoint.position, -transform.right, 1.3f) && mSpeed > 1)
			{
				isCanTurn = false;
			}
			else
			{
				if(mSpeed > 5f)
				{
					if(mSteerTimeCur > 0f)
					{
						mSteerTimeCur -= 5f * Time.deltaTime;
					}
					
					if(mSteerTimeCur > -maxSteerTime)
					{
						mSteerTimeCur -= (0.5f * Time.deltaTime);
					}
				}
			}
			
			if(isCanTurn && mSteerClient < -0.1f)
			{
				transform.Rotate(0, rotSpeed, 0);
			}
			
			bIsTurnRight = false;
			if(!bIsTurnLeft)
			{
				if(!bIsAiNPC)
				{
					TouYingPro.material = TouYingMatZuo;
				}
				bIsTurnLeft = true;
				if(mSteerClient < -0.4f)
				{
					networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "turnLeft1");
				}
			}

			if(Mathf.Abs( mSteerClient ) < 0.4f)
			{
				bIsTurnLeft = false;
			}
		}
		else
		{
			if(!bIsAiNPC && TouYingMatZhong != null && TouYingPro.material != TouYingMatZhong)
			{
				TouYingPro.material = TouYingMatZhong;
			}

			if(Mathf.Abs(mSteerTimeCur) > 1.5f)
			{
				mSteerTimeCur -= 10f * Mathf.Sign(mSteerTimeCur) * Time.deltaTime;
			}
			else if(Mathf.Abs(mSteerTimeCur) > 0.5f)
			{
				mSteerTimeCur -= 3f * Mathf.Sign(mSteerTimeCur) * Time.deltaTime;
			}
			else if(Mathf.Abs(mSteerTimeCur) > 0.03f)
			{
				mSteerTimeCur -= Mathf.Sign(mSteerTimeCur) * Time.deltaTime;
			}
			else
			{
				mSteerTimeCur = 0f;
			}
			
			if(bIsTurnLeft || bIsTurnRight)
			{
//				turnRight1.normalizedTime = 0f;
//				turnLeft1.normalizedTime = 0f;
//				mPlayer.animation.Stop("turnRight1");
//				mPlayer.animation.Stop("turnLeft1");
//				turnRight1.wrapMode = WrapMode.Once;
//				turnLeft1.wrapMode = WrapMode.Once;
			}
			bIsTurnLeft = false;
			bIsTurnRight = false;
		}

		if(!bIsIntoFeiBan && !bIsDoFly)
		{
//			float maxAngle = 90f;
//			float angleZ = -(mSteerTimeCur * maxAngle) / maxSteerTime;
			float angleZ =  HidXKUnity_DLL.GetBikeRotationAZ( mSteerTimeCur );
			if(Mathf.Abs( angleZ ) > 45.0f)
			{
				if(angleZ > 0)
				{
					angleZ = 45.0f;
				}
				else
				{
					angleZ = -45.0f;
				}
			}

			Vector3 rotationA = mPlayer.transform.localEulerAngles;	
			rotationA.z = angleZ;
			if(Mathf.Abs(mSteerTimeCur) >= 0.05f && Time.frameCount % 3 == 0 && Network.isClient)
			{
				rotationA.x = 0;
			}

			mPlayer.transform.localEulerAngles = rotationA;
		}
	}
	
	void resetBikeUseGravity()
	{
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
	}
	
	public bool getIsDoFly()
	{
		return bIsDoFly;
	}
	
	public bool getIsPlayCool(int key = 0)
	{
		if (key != 0) {
			if (bIsDoFly || isPlayCool) {
				return true;
			}
			return false;
		}
		return isPlayCool;
	}
	
	public bool GetIsWuDiState()
	{
		return bIsWuDiState;
	}
	
	public bool GetIsIntoFeiBan()
	{
		return bIsIntoFeiBan;
	}

	bool isPlayCool = false;
	float timeActionCool3;
	static float TimeLastDoFly;
	void CheckResetIsDoFly()
	{
		if (!bIsDoFly || bIsAiNPC) {
			return;
		}
		
		if (Time.time - TimeLastDoFly < 4f) {
			return;
		}
		bIsDoFly = false;
	}

	void checkIsDoFlyAction()
	{
		string CoolAction = "Cool";
//		AnimationState Cool = mPlayer.animation[CoolAction];
		
		if(isPlayCool && !bIsAiNPC)
		{
			if(BikeCameraNet.mBikePlayer == null)
			{
				bIsDoFly = true;
				TimeLastDoFly = Time.time;
				return;
			}
			
			if(!bIsDoFly)
			{
				mRunState = -1;
				bIsDoFly = true;
				TimeLastDoFly = Time.time;
				if(Network.isClient)
				{
					ScreenLog.Log("checkIsDoFlyAction -> play cool action");
					timeActionCool3 = 0f;
//					mPlayer.animation.CrossFade(CoolAction, 0.0f);
					
					PlayAnimation(PlayerAniEnum.Cool);
					networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, CoolAction);

					activeChangeCamPosEvent( BikeCamEvent.TeDingDongZuo );
					if( pcvr.bIsHardWare )
					{
						if(IsHitJianSuDai)
						{
							ResetPlayerIntoJianSuDai();
						}
						//pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 0.0f);
					}
				}

				if(checkHitScript != null)
				{
					checkHitScript.BoxCol.enabled = true;
					if (transform.collider != null) {
						transform.collider.enabled = false;
					}
				}
			}
		}
		else
		{
			if(BikeCameraNet.mBikePlayer == null)
			{
				if(bIsDoFly)
				{
					makeBikeFall();
				}
				return;
			}
		}

		if(bIsIntoFeiBan)
		{
			timeActionCool3 += Time.deltaTime;
		}
	}

	public void HandleAnimationEventCool(int val)
	{
		//Debug.Log("HandleAnimationEventCool -> val "+val);
		if (val == 0) {
			mRunState = STATE_RUN2;
			if (Network.peerType == NetworkPeerType.Client) {
				PlayAnimation(PlayerAniEnum.run2);
				networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run2");
			}
			return;
		}

		if (!checkHaveOwnerPlayer() || Network.player != ownerPlayer) {
			if (val == 3) {
				bIsDoFly = false;
			}
			return;
		}
		
		switch (val) {
		case 1:
			if(!bIsAiNPC) {
				isOutFeiBan = true;
			}
			isPlayCool = false;
			break;

		case 3:
			if (mRunState != STATE_RUN2) {
				if (!bIsAiNPC) {
					isOutFeiBan = true;
					Invoke("resetIsOutFeiBan", 0.5f);
				}
				
				bIsDoFly = false;
				isPlayCool = false;
				isOutFeiBan = false;
				bIsIntoFeiBan = false;
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 10.0f);
				
				mRunState = STATE_RUN2;
				if (Network.isClient) {
					PlayAnimation(PlayerAniEnum.run2);
					networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "run2");
				}

				if (transform.collider != null) {
					transform.collider.enabled = true;
				}
			}
			break;
		}
	}

	void SetAnimatorSpeed(float val)
	{
		if (BikeAniScript == null) {
			return;
		}
		BikeAniScript.SetAnimatorSpeed(val);
	}
	
	bool isOutFeiBan = false;
	void resetIsOutFeiBan()
	{
		isOutFeiBan = false;
	}

	void ResetTouYing()
	{
		TouYingObj.SetActive(true);
	}

	void StopPlayFallAction()
	{
		if(PlayerCreatNet.IsDisconnected)
		{
			return;
		}

		networkView.RPC("ResetPlayFallAction", RPCMode.OthersBuffered);
	}

	void OnTriggerEnter(Collider other)
	{
		if(bIsGameOver)
		{
			return;
		}

		if(bIsFall)
		{
			return;
		}

		GameObject obj = other.gameObject;
		string lay = LayerMask.LayerToName( obj.layer );
		if(lay == "qiche" && !FreeModeCtrl.IsServer)
		{
			if(!bIsAiNPC)
			{
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
				{
					pcvr.GetInstance().HandlePlayerHitState();
					CameraShake.GetInstance().setCameraShakeImpulseValue();
				}
			}

			if(bIsWuDiState)
			{
				return;
			}
			
			TouYingObj.SetActive(false);
			Invoke("ResetTouYing", 2.0f);
			makeBikeFall();
			return;
		}
		
		string tagObj = obj.tag;
		bikeNetUnity bikeScript = null;
		if (lay == "NPC" || lay == "Player")
		{
			if(obj.name == gameObject.name)
			{
				return;
			}

			bikeScript = obj.GetComponent<bikeNetUnity>();
			if(!bIsAiNPC || !bikeScript.bIsAiNPC)
			{
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
				{
					pcvr.GetInstance().HandlePlayerHitState();
					CameraShake.GetInstance().setCameraShakeImpulseValue();
				}
			}
			
			if(bIsWuDiState)
			{
				return;
			}

			Vector3 vecA = transform.forward;
			Vector3 vecB = obj.transform.forward;
			Vector3 vecC = obj.transform.position - transform.position;
			vecA.y = 0.0f;
			vecB.y = 0.0f;
			vecC.y = 0.0f;
			
			float rand = Random.Range(0f, 100f);
			float dotAB = Vector3.Dot(vecA, vecB);
			float dotAC = Vector3.Dot(vecA, vecC);
			if(dotAB >= 0.0f && dotAC >= 0.0f)
			{
				if(rand < 20.0f)
				{
					if(!bIsFall)
					{
						OtherClientMakeBikeFall();
					}
				}
				else
				{
					if(bikeScript == null)
					{
						return;
					}
					
					if(!bikeScript.bIsFall)
					{
						bikeScript.OtherClientMakeBikeFall();
					}
				}
			}
			else
			{
				if(rand <= 80.0f)
				{
					if(!bIsFall)
					{
						OtherClientMakeBikeFall();
					}
				}
				else
				{
					if(bikeScript == null)
					{
						return;
					}
					
					if(!bikeScript.bIsFall)
					{
						bikeScript.OtherClientMakeBikeFall();
					}
				}
			}
			return;
		}
		
		if(tagObj == "rock"  && !FreeModeCtrl.IsServer)// && layBike == "Player")
		{
			//ScreenLog.Log("rock make bike fall...");
			if(!bIsAiNPC)
			{
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
				{
					pcvr.GetInstance().HandlePlayerHitState();
					CameraShake.GetInstance().setCameraShakeImpulseValue();
				}
			}

			if(bIsWuDiState)
			{
				return;
			}
			TouYingObj.SetActive(false);
			Invoke("ResetTouYing", 2.0f);
			makeBikeFall();
			return;
		}
		
		if(mGameTime <= 1)
		{
			return;
		}
		
		if(!bIsAiNPC && tagObj == "JianSuGang")
		{
			//if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
			if(Network.isClient)
			{
				pcvr.GetInstance().HandlePlayerHitState();
				return;
			}
		}

		StartCoroutine(OnCollisionObject( obj.transform, other, 0));
	}
	
	void OnTriggerExit(Collider other)
	{
		if(bIsGameOver)
		{
			return;
		}

		GameObject obj = other.gameObject;
		StartCoroutine(OnCollisionObject( obj.transform, other, 1));
	}
	
//	void OnTriggerStay(Collider other)
//	{
//		GameObject obj = other.gameObject;
//		StartCoroutine(OnCollisionObject( obj.transform, other, 2));
//	}

	BikeHeadMoveState StateMove = BikeHeadMoveState.NULL;
	public LayerMask TerrainLayer;
	public LayerMask DiMainSmokeLayer;
//	float lastPlaneTime = -1.0f;
	const float MinAngleGrade = 2.0f;
	float MaxBikeCosA = Mathf.Cos((90f - MinAngleGrade) * Mathf.Deg2Rad);
	void checkMoveUpDown()
	{
		float cosAngleFU = 0f;
		Vector3 vecFor = Vector3.zero;
		Vector3 vecPA = wheelBack.position;
		Vector3 vecPB = wheelForward.position;
		RaycastHit hit;
		if (Physics.Raycast(vecPA, Vector3.down, out hit, 20.0f, TerrainLayer.value))
		{
			vecPA = hit.distance * Vector3.down + vecPA;
		}
		else if(Physics.Raycast(vecPA, Vector3.down, out hit, 20.0f, DiMainSmokeLayer.value))
		{
			vecPA = hit.distance * Vector3.down + vecPA;
		}
		
		if (Physics.Raycast(vecPB, Vector3.down, out hit, 20.0f, TerrainLayer.value))
		{
			vecPB = hit.distance * Vector3.down + vecPB;
		}
		else if(Physics.Raycast(vecPB, Vector3.down, out hit, 20.0f, DiMainSmokeLayer.value))
		{
			vecPB = hit.distance * Vector3.down + vecPB;
		}
		vecFor = vecPB - vecPA;
		vecFor = vecFor.normalized;

		//change bike head
		if(Network.isServer)
		{
			transform.forward = vecFor;
			return;
		}
		cosAngleFU = Vector3.Dot(Vector3.up, vecFor);

		mResistanceForce = 0f;
		if(Mathf.Abs( cosAngleFU ) > MaxBikeCosA)
		{
			float angleFU = Mathf.Acos( Mathf.Abs(cosAngleFU) ) * Mathf.Rad2Deg;
			float grade = 90f - angleFU;
			if(cosAngleFU > 0f)
			{
				if(!bIsMoveUp)
				{
					bIsMoveUp = true;
					bIsMoveDown = false;
					//ScreenLog.Log(this.name + ": move up!");
				}
			}
			else
			{
				if(!bIsMoveDown)
				{
					bIsMoveUp = false;
					bIsMoveDown = true;
					//ScreenLog.Log(this.name + ": move down!");
				}
			}
			
			if(!bIsAiNPC)// && Time.frameCount % 2 == 0)
			{
				if(bIsMoveUp && StateMove != BikeHeadMoveState.UP)
				{
					if(!IsIntoXiaoFeiBan && !bIsIntoFeiBan)
					{
						StateMove = BikeHeadMoveState.UP;
						pcvr.GetInstance().HandleBikeHeadQiFu(StateMove, mSpeed, grade);
					}
				}
				
				if(!bIsMoveUp && StateMove != BikeHeadMoveState.DOWN)
				{
					if(!IsIntoXiaoFeiBan && !bIsIntoFeiBan)
					{
						StateMove = BikeHeadMoveState.DOWN;
						pcvr.GetInstance().HandleBikeHeadQiFu(StateMove, mSpeed, grade);
					}
				}
			}
			//ScreenLog.Log(this.name + ": grade " + grade + ", mResistanceForce " + mResistanceForce);
		}
		else
		{
			if(bIsMoveUp && !bIsWuDiState)
			{
				float subPower = mResistanceForce / rigidbody.mass;
				subPower *= 5f;
				currentEnginePower += subPower;
				if (currentEnginePower < 0f)
				{
					currentEnginePower = 0f;
				}
			}
			
			if(bIsMoveDown && !bIsWuDiState)
			{
				float subPower = mResistanceForce / rigidbody.mass;
				currentEnginePower -= subPower;
			}
			bIsMoveUp = false;
			bIsMoveDown = false;
		}
	}

	MeshRenderer []renderMesh;
	void setMeshRender(bool isRender)
	{
		if(bIsRenderMat == isRender)
		{
			return;
		}
		bIsRenderMat = isRender;
		
		if(playerSkinnedObj == null)
		{
			playerSkinnedObj = ParticleObj.transform.parent.gameObject;
		}

		if(playerSkinnedObj != null)
		{
			playerSkinnedObj.SetActive(isRender);
			if(renderMesh == null)
			{
				renderMesh = transform.GetComponentsInChildren<MeshRenderer>();
			}
			
			for(int i = 0; i < renderMesh.Length; i++)
			{
				renderMesh[i].enabled = isRender;
			}
		}
	}
	
	void setCameraEffect()
	{
		bIsAiNPC = false; //change npc to player
//		for(int i = 0; i < mRankPlayer.Length; i++)
//		{
//			if(mRankPlayer[i].Name == this.name)
//			{
//				if(GlobalScript.GetInstance().player.RankList == null)
//				{
//					GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
//				}
//				
//				mRankPlayer[i].IsPlayer = true;
//				GlobalScript.GetInstance().ChangeNPC();

//				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
//				{
//					FireScripts.wangQiuTranEvent( gameObject );
//				}
//				break;
//			}
//		}
		
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
		{
			FireScripts.wangQiuTranEvent( gameObject );
		}

		Transform camTranM = null;
		if(wangQiuTran != null)
		{
			GlobalScript.GetInstance().player.wangQiuTran = wangQiuTran;
		}
		
		GameObject mainCam = Camera.main.gameObject;
		if(mainCam != null)
		{
			camTranM = mainCam.transform;
			mScriptCam = camTranM.GetComponent<BikeCameraNet>();
			if(mScriptCam != null)
			{
				if(mScriptCam.mDaoJuCam != null)
				{
					mJiTuiScript = mScriptCam.mDaoJuCam.GetComponent<JiTui>();
				}
			}
		}
		
		gameObject.layer = LayerMask.NameToLayer("Player");
		
		BoxCollider colChild = null;
		BoxCollider [] colChilds = GetComponentsInChildren<BoxCollider>();
		int count = colChilds.Length;
		for(int i = 1; i < count; i++)
		{
			colChild = colChilds[i];
			if(colChild.enabled)
			{
				colChild.gameObject.layer = LayerMask.NameToLayer("pengzhuang");
			}
		}
		Invoke("setIsHitFall", 3f);
	}
	
	static public void setIsGameStart(bool isGameStart)
	{
		bIsGameStart = isGameStart;
		if(bIsGameStart && mPlayerObject != null)
		{
			bikeNetUnity bikeScript = mPlayerObject.GetComponent<bikeNetUnity>();
			bikeScript.setCameraEffect();
		}
	}
	
	void setIsHitFall()
	{
		bIsHitFall = true;
	}

	[RPC]
	void PlayerWuDiState()
	{
		//CancelInvoke("changePlayerMesh");
		CancelInvoke("resetPlayerColliderInfo");

		bIsWuDiState = true;
		mWuDiTimeCount = 0;

		InvokeRepeating("changePlayerMesh", 0f, 0.05f);
		Invoke("resetPlayerColliderInfo", 3f);
	}

	int maxWuDiTimeCount = (int)(6.0f / 0.05f);
	void changePlayerMesh()
	{
		int count = mWuDiTimeCount % 6;
		switch(count)
		{
		case 0:
			setMeshRender(false);
			break;
			
		case 3:
			setMeshRender(true);
			break;
		}
		
		mWuDiTimeCount++;
		if(mWuDiTimeCount >= maxWuDiTimeCount)
		{
			setMeshRender(true);
			CancelInvoke("changePlayerMesh");
		}
	}

	bool isSetStartGame = false;
	[RPC]
	void SendToSetIsGameStart()
	{
		GlobalScript.GetInstance().player.bIsGameStart = true;
	}

	// FixedUpdate is called once per frame
	void FixedUpdate()
	{
		if(PlayerCreatNet.IsDisconnected)
		{
			RemovePlayer();
			return;
		}

		if(!GlobalScript.GetInstance().player.bIsGameStart)
		{
			return;
		}
		else
		{
			if(!isSetStartGame)
			{
				isSetStartGame = true;
				networkView.RPC("SendToSetIsGameStart", RPCMode.OthersBuffered);
			}
		}

		if(mGameTime <= 0 || bIsFall)
		{
			return;
		}

		if( checkBikeState( 0 ) )
		{
			return;
		}
		
		checkBikeSpeed();
		if(Time.frameCount % 5 == 0 && Network.isClient)
		{
			if(!IsHitJianSuDai && !bIsIntoFeiBan && checkHaveOwnerPlayer() && Network.player == ownerPlayer)
			{
				checkMoveUpDown();
			}
		}

		if(mGameTime > 0)
		{
			GetInput();
			
			handleBikeRotation();
			
			CalculateEnginePower();
			
			checkObjTransform();

			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				return;
			}

			if(IsIntoXiaoFeiBan || bIsIntoFeiBan || isOutFeiBan)
			{
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
				{
					if(bIsIntoFeiBan || isOutFeiBan)
					{
						if(mSpeed < 75f && !bIsAiNPC)
						{
							rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 2400f, ForceMode.Acceleration);
						}
					}
					else
					{
						rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 2000f * rigidbody.mass);
					}
				}
			}
			else
			{
				if((!bIsAiNPC && (mThrottleForce > 1f || brakeState > 0 || Mathf.Abs(mResistanceForce) > 10f))
				   || throttleCurrent > 0.000001f || mMouseDownCountCurrent > 0)
				{
					if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
					{	
						if(mSpeed < 10f && !bIsPlayerBrakeBike)
						{
							rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 1000f, ForceMode.Acceleration);
						}
						ApplyThrottle();
					}
				}
			}
		}

		if(Network.isClient)
		{
			networkView.RPC("sendTranInfoToServer", RPCMode.OthersBuffered, transform.position, transform.rotation);
		}
	}
	
	void resetPlayCoolState()
	{
		if(!bIsAiNPC && (isPlayCool2 || isPlayCool3))
		{
//			AnimationState cool2 = mPlayer.animation["Cool2"];
//			AnimationState cool3 = mPlayer.animation["Cool3"];
			if(isPlayCool2)
			{
				isPlayCool2 = false;
//				cool2.wrapMode = WrapMode.Once;
//				mPlayer.animation.Stop("Cool2");
			}
			else if(isPlayCool3)
			{
				isPlayCool3 = false;
//				cool3.wrapMode = WrapMode.Once;
//				mPlayer.animation.Stop("Cool3");
			}
			mRunState = -1;
		}
	}

	void checkPlayerMouseFire()
	{
		if(bIsAiNPC)
		{
			return;
		}
		
		int tennisCount = GlobalScript.GetInstance().player.TennisCount;
		if(tennisCount < 1)
		{
			aimNPC(mLastAimNPC, false);
			return;
		}

		if(RankHandleCtrl.mRankPlayer == null)
		{
			return;
		}
		
		bikeNetUnity script = null;
		bikeAiNetUnity scriptAi = null;
		
		bool isCanFire = false;
		Transform child = null;
		int count = RankHandleCtrl.mRankPlayer.Length;
		
		for(int i = 0; i < count; i++)
		{
			if(RankHandleCtrl.mRankPlayer[i].player == null)
			{
				break;
			}

			if(RankHandleCtrl.mRankPlayer[i].Name != gameObject.name)
			{
				child = RankHandleCtrl.mRankPlayer[i].player.transform;
				scriptAi = RankHandleCtrl.mRankPlayer[i].AiNetScript;
				script = RankHandleCtrl.mRankPlayer[i].NetScript;
				if(scriptAi != null)
				{
					isCanFire = checkIsCanFire( child );
					if(isCanFire)
					{
						pcvr.IsOpenFireLight = true;
						return;
					}
				}
				else if(script != null && !script.bIsAiNPC)
				{
					isCanFire = checkIsCanFire( child );
					if(isCanFire)
					{
						pcvr.IsOpenFireLight = true;
						return;
					}
				}
			}
		}
		
		pcvr.IsOpenFireLight = false;
		aimNPC(null, false);
	}
	
	bool checkIsCanFire(Transform npcObj)
	{
		GameObject parentPlayer = npcObj.gameObject;
		
		float maxDis = 25f;
		float minDis = 3f;
		
		float maxAngle = 45f;
		Vector3 vecA = transform.forward;
		Vector3 vecB = parentPlayer.transform.position - transform.position;
		vecA.y = 0f;
		vecB.y = 0f;
		
		float lenB = Vector3.Distance(vecB, Vector3.zero);
		if(lenB > maxDis || lenB < minDis)
		{
			return false;
		}
		float lenA = Vector3.Distance(vecA, Vector3.zero);
		
		float cosAB = Vector3.Dot(vecA, vecB) / (lenA * lenB);
		if(cosAB < 0f)
		{
			return false;
		}
		
		float tanAB = Mathf.Tan( Mathf.Acos(cosAB) );
		float disTmp = lenB * Mathf.Abs( tanAB );
		if(disTmp > 10f)
		{
			//ScreenLog.Log("npc " + npcObj.name + ", disTmp " + disTmp);
			return false;
		}
		
		float cosTmp = Mathf.Cos(Mathf.Deg2Rad * maxAngle);
		if(cosAB > cosTmp)
		{
			aimNPC(npcObj, true);
			return true;
		}
		
		return false;
	}
	
	void aimNPC(Transform npcObj, bool isAim)
	{
		if(npcObj == null)
		{
			GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
			return;
		}
		
		if(mLastAimNPC != npcObj)
		{
			mLastAimNPC = npcObj;
		}
		Vector3 aimPos = npcObj.transform.position + Vector3.up;
		GlobalScript.GetInstance().player.setAimPosition(aimPos, isAim);
	}
	
	bool checkPlayerMoveDir()
	{
		if(mBikeAimMark == null)
		{
			return false;
		}
		
		Vector3 vecA = mBikeAimMark.right;
		Vector3 vecB = mBikeAimMark.position - transform.position;
		Vector3 vecC = transform.forward;
		vecA.y = 0f;
		vecB.y = 0f;
		vecC.y = 0f;
		float cosAC = Vector3.Dot(vecA, vecC);
		float cosAB = Vector3.Dot(vecA, vecB);
		float cosBC = Vector3.Dot(vecB, vecC);
		
		if(cosAC < 0f)
		{
			setIsDirWrong(true);
		}
		else
		{
			setIsDirWrong(false);
			if(cosBC <= 0f && cosAB <= 0f)
			{
				AiMark markScript = mBikeAimMark.GetComponent<AiMark>();
				if(markScript == null)
				{
					ScreenLog.Log(this.name + "::the mark not find AiMark scirpt! parent " + mBikeAimMark.parent.name);
					return false;
				}
				
				int conutTmp = mBikeAimMark.parent.childCount - 1;
				int markCount = markScript.getMarkCount();
				if(markCount == conutTmp)
				{
					RankHandleCtrl.GetInstance().SetBikePathCount( gameObject.name );
				}

				if(gameObject.name == "NPC_04NetUnity")
				{
					mBakeTimePointPos = mBikeAimMark.position + mBikeAimMark.forward * mBikeAimMark.localScale.z * 0.5f;
				}
				else
				{
					mBakeTimePointPos = mBikeAimMark.position - mBikeAimMark.forward * mBikeAimMark.localScale.z * 0.5f;
				}
				mBakeTimePointRot = mBikeAimMark.right;
				
				mBikeAimMark = markScript.mNextMark;
				RankHandleCtrl rankScript = RankHandleCtrl.GetInstance();
				if(rankScript != null && mBikeAimMark != null)
				{
					rankScript.SetBikeAimMark(mBikeAimMark.parent.name, GetAimMarkId(mBikeAimMark), gameObject.name);
				}

				networkView.RPC("updataServerBikeMarkCount", RPCMode.OthersBuffered);
				return true;
			}
		}
		
		return false;
	}

	[RPC]
	void updataServerBikeMarkCount()
	{
		if(Network.isServer)
		{
			return;
		}

		if(RankHandleCtrl.GetInstance() == null)
		{
			return;
		}

		if(mBikeAimMark == null)
		{
			return;
		}

		AiMark markScript = mBikeAimMark.GetComponent<AiMark>();
		if(markScript == null)
		{
			ScreenLog.Log(this.name + "::the mark not find AiMark scirpt! parent " + mBikeAimMark.parent.name);
			return;
		}
		
		int conutTmp = mBikeAimMark.parent.childCount - 1;
		int markCount = markScript.getMarkCount();
		if(markCount == conutTmp)
		{	
			RankHandleCtrl.GetInstance().SetBikePathCount( gameObject.name );
		}
		
		//mBakeTimePointPos = mBikeAimMark.position;
		if(gameObject.name == "NPC_04NetUnity")
		{
			mBakeTimePointPos = mBikeAimMark.position + mBikeAimMark.forward * mBikeAimMark.localScale.z * 0.5f;
		}
		else
		{
			mBakeTimePointPos = mBikeAimMark.position - mBikeAimMark.forward * mBikeAimMark.localScale.z * 0.5f;
		}
		mBakeTimePointRot = mBikeAimMark.right;
		
		mBikeAimMark = markScript.mNextMark;
		if(mBikeAimMark != null)
		{
			RankHandleCtrl.GetInstance().SetBikeAimMark(mBikeAimMark.parent.name, GetAimMarkId(mBikeAimMark), gameObject.name);
		}
	}

	void setIsDirWrong(bool isDirWrong)
	{
		if(isDirWrong)
		{
			if(!IsInvoking("spawnPlayerDirWrong"))
			{
				mDirWrongCount = 0;
				InvokeRepeating("spawnPlayerDirWrong", 0f, 0.05f);
			}
		}
		else
		{
			if(IsInvoking("spawnPlayerDirWrong"))
			{
				CancelInvoke("spawnPlayerDirWrong");
			}
		}
		GlobalScript.GetInstance().player.Isreverse = isDirWrong;
	}
	
	void spawnPlayerDirWrong()
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}

		int time = 40;
		if(mDirWrongCount > time)
		{
			//ScreenLog.Log("dir is wrong!");
			CancelInvoke("spawnPlayerDirWrong");

			bIsFall = true;
			cloneObject();
			return;
		}
		mDirWrongCount++;
		return;
	}

	void CheckCameraPlayer()
	{
//		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer & Time.frameCount % 5 == 0)
//		{
//			if(Network.isClient)
//			{
//				int maxNum = mRankPlayer.Length;
//				for(int i = 0; i < maxNum; i++)
//				{
//					if(mRankPlayer[i].player == null)
//					{
//						continue;
//					}
					
//					if(mRankPlayer[i].player.name != BikeCameraNet.mBikePlayer.name)
//					{
//						mRankPlayer[i].IsPlayer = false;
//						  new List<playerRank>(mRankPlayer);
//						
//						GlobalScript.GetInstance().ChangeNPC(); //handle small map
//					}
//					else
//					{
//						mRankPlayer[i].IsPlayer = true;
//						GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
//
//						GlobalScript.GetInstance().ChangeNPC(); //handle small map
//					}
//				}
//			}
//		}
	}

	void RemovePlayer()
	{
		if(FreeModeCtrl.IsServer)
		{
			return;
		}
		
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}
		enabled = false;

		Invoke("removeNetWorkPlayer", 2.0f);
		if(Network.peerType == NetworkPeerType.Connecting)
		{
			networkView.RPC("sendToOtherRemovePlayer", RPCMode.OthersBuffered);
		}
	}

	void removeNetWorkPlayer()
	{
		Network.RemoveRPCs(networkView.viewID);
		Network.Destroy(gameObject);
	}

	[RPC]
	void sendToOtherRemovePlayer()
	{
		enabled = false;
	}
	
	float lastTimeRender = 0.0f;
	void Update()
	{
		if (checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient) {
			mMouseDownCountCurrent = pcvr.mMouseDownCount;
		}

		if(PlayerCreatNet.IsDisconnected)
		{
			RemovePlayer();
			return;
		}

		float dTime = Time.realtimeSinceStartup - lastTimeRender;
		if(dTime < 0.03f)
		{
			return;
		}
		lastTimeRender = Time.realtimeSinceStartup;

		if(GlobalScript.GetInstance().player != null)
		{
			mGameTime = GlobalScript.GetInstance().player.Life;
		}

		if(Time.frameCount % 25 == 0)
		{
			if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
			{
				StopPlayFallAction();

				FireScripts.wangQiuTranEvent( gameObject );
			}
		}
		//CheckCameraPlayer();

		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			//ScreenLog.Log("test playerNet 2222222222222 name " + transform.name);
			if(Screenshot.IsShootPlayer)
			{
				RemovePlayer();
				return;
			}

			if(Network.isClient)
			{
				handlePlayerCam();

				if(bIsDoFly)
				{
					checkIsDoFlyAction();
					return;
				}
			}
		}

		if(bIsFall)
		{
			if(mGameTime > 0)
			{
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
				{
					isCloneObject = false;
					bIsFall = false;
					Invoke( "cloneObject", 1.0f);
				}
			}
			return;
		}

		if(!bIsAiNPC)
		{
			if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
			{
				checkPlayerMouseFire();

				checkPlayerMoveDir();
			}
		}

		//if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
		if(!FreeModeCtrl.IsServer)
		{
			checkIsDoFlyAction();
		}

		if(isSlowWorld)
		{
			resetIsSlowWorld();
		}
		moveBikeWheels();
	}

	void resetIsDoHurtAction()
	{
		bIsDoHurtAction = false;
	}

	[RPC]
	void SendMakeBikeFall()
	{
		if(Network.isServer)
		{
			return;
		}

		if(bIsFall)
		{
			return;
		}
		//ScreenLog.LogWarning("test SendMakeBikeFall**************** name " + gameObject.name);
		makeBikeFall();
	}

	public bool checkBikeState( int key )
	{
		if(FreeModeCtrl.IsServer)
		{
			return false;
		}

		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			//ScreenLog.LogWarning("test checkBikeState ********************* name " + gameObject.name);
			return false;
		}

		if(bIsFall)
		{
			return true;
		}

		bool isFall = false;
		if (key == (int)CheckBikeKey.HIT_CAR || key == (int)CheckBikeKey.HIT_CLIFF)
		{
			isFall = true;
		}
		else
		{
			float eaZ = mPlayer.transform.eulerAngles.z;
			eaZ = Mathf.Abs(eaZ);
			if(eaZ >= 180f)
			{
				eaZ = 360f - eaZ;
			}
			
			float angle = 90f - eaZ;
			if(angle <= 25.0f)
			{
				isFall = true;
			}
		}
		
		if (isFall && (bIsHitFall || bIsAiNPC))
		{
			bIsFall = true;
			if(!bIsAiNPC)
			{
				mSpeed = 0f;
				GlobalScript.GetInstance().ShowTishi( TishiInfo.Diedao );
				GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
				if(FengXiaoObj != null)
				{
					FengXiaoCtrl.IsPlayFengXiao = false;
					FengXiaoObj.SetActive(false);
					networkView.RPC("SetFengXiaoState", RPCMode.OthersBuffered, false);
				}
			}
			
			if(!isCloneObject)
			{
				isCloneObject = true;
				Invoke("cloneObject", 0.5f);
			}

			if(key != (int)CheckBikeKey.HIT_CLIFF)
			{
				BoxCollider boxCol = GetComponent<BoxCollider>();
				if(boxCol)
				{
					boxCol.enabled = false;
				}
				
//				animation.enabled = false;
				if(mPlayer != null)
				{
					setPlayerChildRigibody( mPlayer.transform, true );
				}
				
				Vector3 cm = rigidbody.centerOfMass;
				cm.y += 0.5f;
				rigidbody.centerOfMass = cm;
				rigidbody.isKinematic = true;
			}
		}
		
		return bIsFall;
	}

	[RPC]
	void sendToOtherSetHiddenPlayer(bool isHidden)
	{
		gameObject.SetActive(!isHidden);
	}

	enum CheckBikeKey
	{
		NORMAL,
		HIT_CAR,
		HIT_CLIFF
	}

	void OtherClientMakeBikeFall()
	{
		networkView.RPC("makeBikeFall", RPCMode.AllBuffered);
	}

	void BikePlayFallAction()
	{
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			return;
		}

		if(bIsFall)
		{
			return;
		}
		bIsFall = true;
		mBikePlayer.gameObject.SetActive(true);
//		animation.enabled = false;
		
		Invoke("StopPlayFallAction", 0.6f);
	}

	[RPC]
	void ResetPlayFallAction()
	{
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			return;
		}

//		if(animation.enabled)
//		{
//			return;
//		}
//		animation.enabled = true;
		mBikePlayer.gameObject.SetActive(false);
		bIsFall = false;
	}

	[RPC]
	void makeBikeFall()
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			if(bIsHitFall)
			{
				BikePlayFallAction();
			}
			return;
		}
		checkBikeState( (int)CheckBikeKey.HIT_CAR );
	}

	void OnBikeCollisionCliff()
	{
		checkBikeState( (int)CheckBikeKey.HIT_CLIFF );
	}
	
	void setPlayerChildRigibody(Transform tran, bool isRagdoll)
	{
		mBikePlayer.gameObject.SetActive(isRagdoll);
		if(!isRagdoll)
		{
			return;
		}
		
		Rigidbody [] rigids = tran.GetComponentsInChildren<Rigidbody>();
		int count = rigids.Length;
		for(int index = 1; index < count; index++)
		{
			if(rigids[index].transform.collider != null)
			{
				rigids[index].transform.collider.enabled = true;
			}
			rigids[index].mass = 70f;
			rigids[index].useGravity = true;
			rigids[index].isKinematic = false;
			rigids[index].detectCollisions = true;
		}
	}
	
	void ApplyThrottle()
	{	
		if(!bIsAiNPC)
		{
			if(bIsMoveUp && brakeState > 0 && rigidbody.useGravity && mSpeed < 10f)
			{
				rigidbody.isKinematic = true;
				rigidbody.useGravity = false;
			}

			float power = mThrottleForce + mResistanceForce;
			//ScreenLog.Log("power " + power + ", mResistanceForce " + mResistanceForce);
			if(power < 0f && mResistanceForce < 0f)
			{
				power = mThrottleForce;
			}

			if(power > 0)
			{
				rigidbody.isKinematic = false;
				rigidbody.useGravity = true;
			}
			rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * power);

//			if(FreeModeCtrl.IsServer)
//			{
//				networkView.RPC("sendTranInfoToClient", RPCMode.OthersBuffered, transform.position, transform.rotation);
//			}
		}
	}

	[RPC]
	void sendTranInfoToClient(Vector3 pos, Quaternion rot)
	{
		//ScreenLog.Log("pos " + pos + ", rotAngle " + rot.eulerAngles);
		transform.position = pos;
		transform.rotation = rot;
	}

	[RPC]
	void SendSpeedToServer(float val)
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			//ScreenLog.Log("val " + val);
			mSpeed = val;
		}
	}

	public bool GetIsGameOver()
	{
		return bIsGameOver;
	}

	void checkBikeSpeed()
	{
		if(PlayerCreatNet.IsDisconnected)
		{
			return;
		}

		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}

//		float speedTmp = 0f;
//		speedTmp = rigidbody.velocity.magnitude * 3.6f;
//		speedTmp *= 0.9f;
//		speedTmp = Mathf.FloorToInt(speedTmp);
//		mSpeed = speedTmp;
		mSpeed = HidXKUnity_DLL.GetBikeSpeed( rigidbody.velocity.magnitude );
		//ScreenLog.Log("speed " + mSpeed);

		if(Network.isClient)
		{
			networkView.RPC("SendSpeedToServer", RPCMode.OthersBuffered, mSpeed);
		}

		if (bIsAiNPC)
		{
			return;
		}
		
		float dVal = mSpeedOld - mSpeed;
		if(dVal >= 20.0f)
		{
			//ScreenLog.Log("checkBikeSpeed -> dVal " + dVal);
			pcvr.GetInstance().HandlePlayerHitState();
		}
		mSpeedOld = mSpeed;
		
		int fengVal = 0;
		if(mSpeed < 5 || bIsGameOver)
		{
			fengVal = 0;
		}
		else
		{
			fengVal = (int)((mSpeed / 65) * 253);
			if(fengVal < 50)
			{
				fengVal = 50;
			}
		}
		
		pcvr.GetInstance().setFengShanInfo(fengVal, 0);
		pcvr.GetInstance().setFengShanInfo(fengVal, 1);

		if(mSpeed >= 50)
		{
			FengXiaoCtrl.IsPlayFengXiao = true;
			FengXiaoObj.SetActive(true);
			networkView.RPC("SetFengXiaoState", RPCMode.OthersBuffered, true);
		}
		else
		{
			FengXiaoCtrl.IsPlayFengXiao = false;
			FengXiaoObj.SetActive(false);
			networkView.RPC("SetFengXiaoState", RPCMode.OthersBuffered, false);
		}

		if(isOutFeiBan && mSpeed < 20f)
		{
			mSpeed = 23f;
		}
		
		if(mSpeed > mMaxVelocity)
		{
			mAiCtrlNum = 0;
		}
		
		if(GlobalScript.GetInstance().player != null)
		{
			GlobalScript.GetInstance().player.Speed = Mathf.FloorToInt(mSpeed);
		}
	}

	[RPC]
	void SetFengXiaoState(bool IsShow)
	{
		if(IsShow)
		{
			FengXiaoObj.SetActive(true);
		}
		else
		{
			FengXiaoObj.SetActive(false);
		}
	}
	
	//-------------------------------------------------------------------------
	void moveBikeWheels()
	{
		if(mSpeed <= 0f)
		{
			return;
		}
		wheelForward.Rotate(Vector3.right * 15);
		wheelBack.Rotate(Vector3.right * 15);
	}

	//-------------------------------------------------------------------------
	[RPC]
	void CalculateEnginePower()
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}

		float timeAdd = 200f;
		float timeSub = 10f * timeAdd;
		float dTime = Time.deltaTime;
		
		bool isAddEnergy = false;
		if(!bIsAiNPC)
		{
			brakeState = 0;
			if (InputEventCtrl.PlayerSC[0] > 0f) {
				brakeState = 3;
				timeSub *= ( (float)brakeState / 8 );
			}
//			if(pcvr.bIsHardWare)
//			{
//				//get hid info
//				brakeState = pcvr.mBikeBrakeState;
//				//8 -> dengJiShu
//				timeSub *= ( (float)brakeState / 8 );
//			}
//			else
//			{
//				if(Input.GetKey(KeyCode.C))
//				{
//					brakeState = 3;
//					timeSub *= 0.85f;
//				}
//			}
			
			if(brakeState != 0)
			{
				bIsPlayerBrakeBike = true;
				if(bIsWuDiState && bIsMoveUp)
				{
					rigidbody.useGravity = false;
					rigidbody.isKinematic = true;
					return;
				}
			}
			else
			{
				bIsPlayerBrakeBike = false;
				bIsSpacePlayer = false;
				mSpaceTime = 0f;
			}
			
			if(throttleCurrent > 0f || mMouseDownCountCurrent > 10f)
			{
				isAddEnergy = true;
			}
		}
		
		if(bIsPlayerBrakeBike || bIsDoFly || (bIsSpaceAi && bIsAiNPC))
		{
			if (bIsDoFly)
			{
				return;
			}
			
			if(!bIsAiNPC)
			{
				if(brakeState > 0)
				{
					if(!bIsSpacePlayer)
					{
						bIsSpacePlayer = true;
						//mSpaceSpeed = mSpeed;
					}
					mSpaceTime += Time.deltaTime;
				}
			}

			if(mSpeed > 1f)
			{
				if(currentBrakeEnginePower + currentEnginePower < 0f)
				{
					currentBrakeEnginePower = -currentEnginePower;
				}
				else
				{
					if(!bIsMoveUp)
					{
						currentBrakeEnginePower -= (10f * dTime * timeSub);
					}
				}
			}
			else
			{
				//do root action
				if(!isAddEnergy)
				{
					currentEnginePower = 0f;
				}
			}
		}
		else
		{
			if(currentBrakeEnginePower != 0f)
			{
				currentEnginePower += currentBrakeEnginePower;
				currentBrakeEnginePower = 0f;
			}
			
			if(throttleCurrent > 0f || mMouseDownCountCurrent > 0f)
			{
				float maxVelocity = 0f;
				if(bIsIntoFeiBan)
				{
					maxVelocity = 70f;
				}
				else
				{
					if(mMaxMouseDownCount < mMouseDownCount)
					{
						mMaxMouseDownCount = mMouseDownCount;
					}
					
					float velocityTmp = (mMouseDownCountCurrent / mMaxMouseDownCount) * mMaxVelocityFoot;
					if(throttleCurrent > 0F && mMouseDownCountCurrent > 0)
					{
						maxVelocity = velocityTmp > mMaxVelocity ? velocityTmp : mMaxVelocity;
					}
					else if(mMouseDownCountCurrent > 0)
					{
						maxVelocity = velocityTmp;
					}
					else if(throttleCurrent > 0F)
					{
						maxVelocity = mMaxVelocity;
					}
					
					if(maxVelocity > 80f)
					{
						maxVelocity = 80f;
					}
				}
				
				if(mSpeed >= maxVelocity)
				{
					currentEnginePower -= dTime * timeAdd;
				}
				else
				{
					if(throttleCurrent > 0F)
					{
						currentEnginePower += (throttleCurrent + 50.0f ) * dTime * timeAdd;
						if(mSpeed < 50.0F && Time.frameCount % 10 == 0)
						{
							rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 24000f, ForceMode.Acceleration);
						}
					}
					else
					{
						currentEnginePower += dTime * timeAdd;
					}
				}
			}
			else if (throttleCurrent == 0f)
			{
				if(currentEnginePower < 0)
				{
					currentEnginePower = 0;
				}
				else if(currentEnginePower > 0)
				{
					currentEnginePower -= 0.1f * dTime * timeSub;
					
					if(currentEnginePower < 0)
					{
						currentEnginePower = 0;
					}
				}
			}
		}
		
		float maxPower = 1200f;
		if(currentEnginePower > maxPower)
		{
			currentEnginePower = maxPower;
		}

		//float power = currentEnginePower + currentBrakeEnginePower + currentBrakeWaterPower;
		float power = HidXKUnity_DLL.GetBikePower(currentEnginePower, currentBrakeEnginePower, currentBrakeWaterPower);
		if (currentBrakeWaterPower < 0f && power < 0f)
		{
			//power = currentEnginePower + currentBrakeEnginePower;
			power = 0f;
		}

		//mThrottleForce = Mathf.Sign(throttle) * power * rigidbody.mass;
		//mThrottleForce = power * rigidbody.mass;
		mThrottleForce = HidXKUnity_DLL.GetBikeThrottleForce(power, rigidbody.mass);
		if(throttleCurrent <= 0.15f && mMouseDownCountCurrent <= 0)
		{
			mThrottleForce = 0.0f;
		}

		if(bIsWuDiState && bIsMoveUp)
		{
			mThrottleForce *= 1.5f;
		}
	}
	
	void SetCenterOfMass()
	{
		if(centerOfMass != null)
		{
			//ScreenLog.Log("SetCenterOfMass");
			rigidbody.centerOfMass = centerOfMass.localPosition;
		}
	}
	
	void makeAiDoHurtAction()
	{
		if(bIsDoHurtAction)
		{
			return;
		}		
		bIsDoHurtAction = true;
		
		makeBikeFall();
		return;
	}
	
	public bool GetIsIntoSuiDao()
	{
		return isIntoSuiDao;
	}
	
	bool isSlowWorld = false;
	public bool GetIsSlowWorld()
	{
		return isSlowWorld;
	}
	
	float slowWorldRecordTime = 0f;
	float slowWorldTime = 1f;
	void resetIsSlowWorld()
	{
		if(isSlowWorld)
		{
			float dTime = Time.realtimeSinceStartup - slowWorldRecordTime;
			//ScreenLog.Log("dTime " + dTime);
			if(dTime < slowWorldTime)
			{
				return;
			}
		}
		
		Time.timeScale = 1f;
		isSlowWorld = false;
		isPlayCool = false;
	}


	CamPosEvent eventScript;
	GameObject eventCamObj;
	BikeCamEvent camEvent;
	public static bool IsHitJianSuDai = false;
	public static bool IsHitLuYan = false;
	int JianSuDaiNum = 0;
	void ResetPlayerIntoJianSuDai()
	{
		//Debug.Log("over HandlePlayerIntoJianSuDai...");
		CancelInvoke("HandlePlayerIntoJianSuDai");
		if(bIsMoveUp)
		{
			bIsMoveUp = false;
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 60.0f, 10.0f);
		}
		JianSuDaiNum = 0;
		IsHitJianSuDai = false;
		pcvr.IsHitJianSuDai = false;
	}

	void HandlePlayerIntoJianSuDai()
	{
		if(!IsHitJianSuDai || JianSuDaiNum >= 12)
		{
			IsHitJianSuDai = false;
			bIsMoveUp = false;
			JianSuDaiNum = 0;
			CancelInvoke("HandlePlayerIntoJianSuDai");
			return;
		}
		
		JianSuDaiNum++;
		pcvr.GetInstance().OpenFangXiangPanZhenDong();
		if(!bIsMoveUp) {
			bIsMoveUp = true;
			//ScreenLog.Log("HitJianSuDai ... move bike head up");
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, 60.0f, 10.0f);
		}
		else {
			bIsMoveUp = false;
			//ScreenLog.Log("HitJianSuDai ... move bike head down");
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.DOWN, 60.0f, 10.0f);
		}
	}

	int GetAimMarkId(Transform aimMark)
	{
		if(aimMark == null)
		{
			return -1;
		}
		
		AiMark markScript = aimMark.GetComponent<AiMark>();
		if(markScript == null)
		{
			return -1;
		}
		
		return markScript.getMarkCount();
	}

	int luYanPosY = -1;

	/**
	 * tag list:
	 * "cliff"      -> xuanYa
	 * "mark"       -> luJingDian
	 * "waterPath"  -> shuiLu
	 * "niPath"     -> niLu
	 * "shiZiPath"  -> shiZiLu
	 * "dianPing"   -> dianPing
	 * "tennisBall" -> wangQiu
	 * "timeBiao"   -> zhongBiao
	 * "timePoint"  -> jiaShiDian
	 * "hamburger"  -> hanBao
	 * "drumstick"  -> jiTui
	 * "tennisBallAmmo"		->		wangQiu Ammo
	 */
	private IEnumerator OnCollisionObject(Transform col, Collider other, int type)
	{
		if (col == null)
		{
			Debug.LogWarning(this.name + ": col is null!");
			yield break;
		}
		
		string colTag = col.tag;
		GameObject colObj = col.gameObject;
		string lay = LayerMask.LayerToName( col.gameObject.layer );
		//ScreenLog.Log(this.name + ": colTag is " + colTag);
		if(lay == "event")
		{
			eventScript = col.GetComponent<CamPosEvent>();
			if(eventScript != null)
			{
				if(eventScript.PosEvent == BikeCamEvent.HangPai && camEvent == BikeCamEvent.HangPai && eventCamObj == colObj)
				{
					yield break;
				}
				eventCamObj = colObj;
				camEvent = eventScript.PosEvent;

				switch(eventScript.PosEvent)
				{
				case BikeCamEvent.JingZhi:
					if(type == 1)
					{
						activeChangeCamPosEvent( BikeCamEvent.MiaoZhunGenSui );
						yield break;
					}
					break;
				}

				activeChangeCamPosEvent( eventScript.PosEvent );
				yield break;
			}
		}

		switch (colTag)
		{
		case "LuYan":
			if(bIsAiNPC)
			{
				yield break;
			}
			
			if(type == 1)
			{
				IsHitLuYan = false;
			}
			else
			{
				IsHitLuYan = true;
				luYanPosY = -luYanPosY;
				if(IsHitJianSuDai)
				{
					ResetPlayerIntoJianSuDai();
				}

				if(luYanPosY == -1)
				{
					pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 60.0f, 10.0f);
				}
				else
				{
					pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, 60.0f, 10.0f);
				}
			}
			break;

		case "JianSuDai":
			if(bIsAiNPC)
			{
				yield break;
			}

			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}
			
			luYanPosY = -1;
			if(type == 1)
			{
				if(IsHitJianSuDai)
				{
					ResetPlayerIntoJianSuDai();
					currentEnginePower += currentBrakeWaterPower;
					currentBrakeWaterPower = 0f;
					//ScreenLog.Log("out JianSuDai...");
				}
			}
			else
			{
				if(!IsHitJianSuDai)
				{
					currentBrakeWaterPower = -200;
					IsHitJianSuDai = true;
					pcvr.IsHitJianSuDai = true;
					bIsMoveUp = false;
					JianSuDaiNum = 0;

					//ScreenLog.Log("into JianSuDai...");
					CancelInvoke("HandlePlayerIntoJianSuDai");
					InvokeRepeating("HandlePlayerIntoJianSuDai", 0.0f, 0.3f);
				}
			}
			break;

		case "slowWorld":
			if(!bIsAiNPC)
			{
				luYanPosY = -1;
				if(!isSlowWorld)
				{
					float timeSlow = 1f;
					gameCollider colScript = col.GetComponent<gameCollider>();
					if(colScript != null)
					{
						timeSlow = colScript.slowWorldTime;
					}
					else
					{
						ScreenLog.Log("colScript is null");
					}
					
					slowWorldRecordTime = Time.realtimeSinceStartup;
					slowWorldTime = timeSlow;
				}
				isSlowWorld = true;
				Time.timeScale = 0.5f;
			}
			break;
			
		case "playCool":
			if(!isPlayCool)
			{
				if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
				{
					luYanPosY = -1;
					timeActionCool3 = 0f;
					//ScreenLog.Log("OnCollisionObject -> player hit playCool");
					isPlayCool = true;
				}
			}
			break;
			
		case "mark":
			if(type == 0 && !bIsAiNPC && col.parent != mAiPathCtrl)
			{
				luYanPosY = -1;
				mAiPathCtrl = col.parent;
				AiMark markScript = col.GetComponent<AiMark>();
				//mBakeTimePointPos = col.position;
				
				if(gameObject.name == "NPC_04NetUnity")
				{
					mBakeTimePointPos = col.position + col.forward * col.localScale.z * 0.5f;
				}
				else
				{
					mBakeTimePointPos = col.position - col.forward * col.localScale.z * 0.5f;
				}
				mBakeTimePointRot = col.right;
				
				mBikeAimMark = markScript.mNextMark;
				if(!FreeModeCtrl.IsServer && mBikeAimMark != null)
				{
					RankHandleCtrl.GetInstance().SetBikeAimMark(mBikeAimMark.parent.name,
					                                            GetAimMarkId(mBikeAimMark), gameObject.name);
				}
			}
			break;
			
		case "cliff":
			if(!bIsAiNPC)
			{
				luYanPosY = -1;
				BikeCamera.mBikePlayer = null;
				GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
			}
			
			if(!bIsFall)// && !isCloneObject)
			{
				//ScreenLog.Log("OnCollisionObject -> player hit cliff ... objName " + gameObject.name);
				if(!isCloneObject)
				{
					bIsFall = true;
					isCloneObject = true;
					Invoke("cloneObject", 0.5f);
				}
			}
			break;
			
		case "waterPath":
			if(bIsAiNPC)
			{
				yield break;
			}
			
			luYanPosY = -1;
			if(type == 1)
			{
				currentEnginePower += currentBrakeWaterPower;
				currentBrakeWaterPower = 0f;
				//ParticleObj.SetActive(true);
				WaterParticleObj.SetActive(false);
				//ScreenLog.Log("OutWater");
			}
			else if(type == 0)
			{
				ParticleObj.SetActive(false);
				if (!bIsAiNPC && checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient) {
					pcvr.GetInstance().PlayerMoveOutTuLu();
				}
				WaterParticleObj.SetActive(true);
				//ScreenLog.Log("IntoWater");
			}
			else
			{
				currentBrakeWaterPower = -500;
			}
			break;
			
		case "niPath":
			if(bIsAiNPC)
			{
				yield break;
			}
			
			luYanPosY = -1;
			if(type == 1)
			{
				currentEnginePower += currentBrakeWaterPower;
				currentBrakeWaterPower = 0f;
			}
			else
			{
				currentBrakeWaterPower = -800;
			}
			break;
			
		case "shiZiPath":
			if(bIsAiNPC)
			{
				yield break;
			}
			
			luYanPosY = -1;
			if(type == 1)
			{
				currentEnginePower += currentBrakeWaterPower;
				currentBrakeWaterPower = 0f;
			}
			else
			{
				currentBrakeWaterPower = -500;
			}
			break;
			
		case "dianPing":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}

			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				pcvr.GetInstance().HandlePlayerHitState(1);
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;
				
				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}
				
				//yield return new WaitForSeconds(delayTime);
				makeObjFly(col, BufferKind.Dianchi);
			}
			break;
			
		case "zhongDian":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}

			if(type == 0)
			{
				if(bIsAiNPC && bIsStopAi)
				{
					yield break;
				}
				
				if(!bIsAiNPC && bIsGameOver)
				{
					yield break;
				}
				
				//isIntoSuiDao = true; //test
				if(!isIntoSuiDao && !bIsAiNPC)
				{
					yield break;
				}
				
				if(!isIntoSuiDaoNPC && bIsAiNPC)
				{
					yield break;
				}
				
				int rankNum = PlayerPrefs.GetInt(mRankingPlayer) + 1;
				PlayerPrefs.SetInt(mRankingPlayer, rankNum);
				//ScreenLog.Log(this.name + ": go to end point! rankNum: " + rankNum);
				
				if(!bIsAiNPC)
				{
					isIntoSuiDao = false;
					bIsGameOver = true;
					RankHandleCtrl.IsStopCheckRank = true;
					Rank.GetInstance().HiddenRankList();
					StartCoroutine( callPlayerRunEnd(rankNum) );
				}
				else if(!bIsStopAi)
				{
					bIsGameOver = true;
					bIsStopAi = true;
					throttle = 0f;
				}
			}
			break;
			
		case "tennisBall":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}

			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				pcvr.GetInstance().HandlePlayerHitState(1);
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;
				MeshRenderer colMesh = colObj.GetComponent<MeshRenderer>();
				colMesh.enabled = false;
				
				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}
				GlobalScript.GetInstance().player.AddBuffer( BufferKind.Wangqiu );

				GameObject colParent = col.parent.gameObject;
				DaoJuNetCtrl DaoJuNetCtrlScript = colParent.GetComponent<DaoJuNetCtrl>();
				if(DaoJuNetCtrlScript != null)
				{
					DaoJuNetCtrlScript.Invoke("closeMeshRender", 0.5f);
				}
			}
			break;
			
		case "tennisBallAmmo":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				ScreenLog.LogWarning("ammo -> should hit real player client!");
				yield break;
			}
			
			wangQiuAmmoNet wangQiuAmmoNetScript = colObj.GetComponent<wangQiuAmmoNet>();
			if(colObj.activeSelf && wangQiuAmmoNetScript.firePlayerName != gameObject.name)
			{
				luYanPosY = -1;
				makeAiDoHurtAction();
			}
			break;
			
		case "timeBiao":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}

			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				pcvr.GetInstance().HandlePlayerHitState(1);
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;
				
				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}
				
				//yield return new WaitForSeconds(delayTime);
				makeObjFly(col, BufferKind.Shoubiao);
			}
			break;
			
		case "timePoint":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}

			timePointCtrl timeScript = colObj.GetComponent<timePointCtrl>();
			if(!bIsAiNPC && timeScript != null)
			{
				luYanPosY = -1;
				bool rv = timeScript.checkPlayerName(gameObject.name);
				if(rv)
				{
					//ScreenLog.Log(this.name + ": add time");
					//GlobalScript.GetInstance().player.AddBuffer( BufferKind.Shoubiao );
					GlobalScript.GetInstance().ShowTishi( TishiInfo.Jiashidian );
				}
			}
			break;

		case "luDuan":
			if(type == 0)
			{
				GlobalScript.GetInstance().ShowTishi( TishiInfo.Luduan );
			}
			break;
			
		case "feiBan":
			if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				if (!bIsAiNPC) {
					TimeLastDoFly = Time.time;
					bIsDoFly = true;
				}
				yield break;
			}

			if(FreeModeCtrl.IsServer)
			{
				yield break;
			}

			luYanPosY = -1;
			if(!bIsIntoFeiBan && (type == 0 || type == 2))
			{
				kaCheTran = col;
				if(kaCheTran != null)
				{
					transform.forward = kaCheTran.forward;
				}
				
				bIsIntoFeiBan = true;
				if(checkHitScript != null)
				{
					checkHitScript.BoxCol.enabled = false;
				}
				CancelInvoke("ResetIsIntoFeiBan");
				Invoke("ResetIsIntoFeiBan", 6f);
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, mSpeed, 10.0f);
			}
			break;
			
		case "xiaoFeiBan":
			if(Network.isServer || !checkHaveOwnerPlayer() || Network.player != ownerPlayer)
			{
				yield break;
			}
			
			luYanPosY = -1;
			if(!IsIntoXiaoFeiBan && (type == 0 || type == 2))
			{
				if(!IsIntoXiaoFeiBan)
				{
					BikeCameraNet.GetInstance().PlayFeiBanAudio();
				}
				IsIntoXiaoFeiBan = true;

				if(IsHitJianSuDai)
				{
					ResetPlayerIntoJianSuDai();
				}
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, mSpeed, 10.0f);
				Invoke("resetIsIntoXiaoFeiBan", 1f);
			}
			break;
			
		case "IntoSuidao":
			isIntoSuiDao = true;
			if(!bIsAiNPC)
			{
				if(mScriptCam == null)
				{
					mScriptCam = Camera.main.GetComponent<BikeCameraNet>();
				}
				mScriptCam.AudioZone.enabled = true;

				//isPlayerIntoSuiDao = true;
//				if(Application.loadedLevel == (int)GameLeve.Leve2 || Application.loadedLevel == (int)GameLeve.Leve4)
//				{
//					BikeLightObj.SetActive( true );
//				}
			}
			else
			{
				isIntoSuiDaoNPC = true;
			}
			break;
			
		case "OutSuidao":
			if(!bIsAiNPC)
			{
				if(mScriptCam == null)
				{
					mScriptCam = Camera.main.GetComponent<BikeCameraNet>();
				}
				mScriptCam.AudioZone.enabled = false;
			}
			break;
			
		default:
			if(lay != "Player" && lay != "Terrain" && lay != "trigger")
			{
				if(mSpeed < 3f && currentEnginePower > 160f)
				{
					currentEnginePower = 10f;
				}
			}
			break;
		}
	}
	
	void ResetIsIntoFeiBan()
	{
		bIsIntoFeiBan = false;
		pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 10.0f);
	}

	public void playFireAction()
	{
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
		{
			PlayAnimation(PlayerAniEnum.fire);
			networkView.RPC("sendToServerPlayFireAction", RPCMode.OthersBuffered);
		}
	}

	[RPC]
	void sendToServerPlayFireAction()
	{
		PlayAnimation(PlayerAniEnum.fire);
	}

	public bool GetIsIntoXiaoFeiBan()
	{
		return IsIntoXiaoFeiBan;
	}
	
	void resetIsIntoXiaoFeiBan()
	{
		IsIntoXiaoFeiBan = false;
		if(IsHitJianSuDai)
		{
			ResetPlayerIntoJianSuDai();
		}
		pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 0.0f);
	}
	
	IEnumerator callPlayerRunEnd(int rankNum)
	{
		ScreenLog.Log("player run to end point!");
		PlayAnimation(PlayerAniEnum.huanhu);
		networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "huanhu");

		GlobalScript.GetInstance().player.IsPlayHuanHu = true;
		yield return new WaitForSeconds( 2f );

		activeChangeCamPosEvent( BikeCamEvent.GameOver );
		
		GlobalScript.GetInstance().player.IsPass = true;
		GlobalScript.GetInstance().player.FinalRank = rankNum;
		if(IsHitJianSuDai)
		{
			ResetPlayerIntoJianSuDai();
		}
		pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 0.0f, 0.0f);
		yield break;
	}
	
	void makeObjFly(Transform col, BufferKind kind)
	{
		Transform parentCol = col.parent;
		if(mScriptCam != null)
		{
			parentCol.parent = mScriptCam.transform;
		}

		DaoJuNetCtrl DaoJuNetCtrlScript = parentCol.GetComponent<DaoJuNetCtrl>();
		if(DaoJuNetCtrlScript != null)
		{
			DaoJuNetCtrlScript.closeMeshRender();
		}

		if(mJiTuiScript == null)
		{
			mScriptCam = Camera.main.gameObject.GetComponent<BikeCameraNet>();
			if(mScriptCam != null)
			{
				if(mScriptCam.mDaoJuCam != null)
				{
					mJiTuiScript = mScriptCam.mDaoJuCam.GetComponent<JiTui>();
				}
			}
		}

		if(mJiTuiScript != null)
		{
			//ScreenLog.Log("test *********** mJiTuiScript.StartTransform");
			mJiTuiScript.StartTransform(parentCol.gameObject, kind);
		}
	}
	
	void resetPlayerColliderInfo()
	{
		setPlayerColliderInfo(transform, true);
		if(transform.collider != null)
		{
			//ScreenLog.Log("resetPlayerColliderInfo**************");
			transform.gameObject.layer = LayerMask.NameToLayer("Player");
		}
	}

	void setPlayerColliderInfo(Transform tran, bool isPlayerLayer)
	{
		if(isPlayerLayer && bIsWuDiState)
		{
			bIsWuDiState = false;
			if(playerSkinnedObj != null)
			{
				setMeshRender(true);
			}
			else
			{
				return;
			}
		}
		
		if(!isPlayerLayer)// && !bIsWuDiState)
		{
			mWuDiTimeCount = 0;
			bIsWuDiState = true;
			if(!IsInvoking("changePlayerMesh"))
			{
				InvokeRepeating("changePlayerMesh", 0f, 0.05f);
			}
		}
		
		BoxCollider colChild = null;
		BoxCollider [] colChilds = GetComponentsInChildren<BoxCollider>();
		int count = colChilds.Length;
		for(int i = 1; i < count; i++)
		{
			colChild = colChilds[i];
			if(colChild.enabled)
			{
				if(isPlayerLayer)
				{
					colChild.gameObject.layer = LayerMask.NameToLayer("pengzhuang");
				}
				else
				{
					colChild.gameObject.layer = LayerMask.NameToLayer("wuDiLayer");
				}
			}
		}
	}

	void cloneObject()
	{
		isSlowWorld = false;
		isPlayCool = false;

		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			bIsFall = false;
			isCloneObject = false;
			return;
		}

		if(mGameTime <= 0 && !bIsAiNPC)
		{
			return;
		}
		
		if(!gameObject.activeSelf)
		{
			return;
		}
		StopPlayFallAction();

		bIsFall = false;
		isCloneObject = false;

		currentEnginePower = 0f;

		BoxCollider boxCol = GetComponent<BoxCollider>();
		if(boxCol)
		{
			boxCol.enabled = true;
		}

		networkView.RPC("ResetPlayFallAction", RPCMode.OthersBuffered);

		if(mPlayer != null)
		{
			setPlayerChildRigibody( mPlayer.transform, false );
		}
		
//		if(mBike.rigidbody != null)
//		{
//			mBike.transform.localPosition = bikePosOffset;
//			mBike.transform.localEulerAngles = bikeAngOffset;
//			Destroy(mBike.rigidbody);
//		}
		
		Vector3 cm = rigidbody.centerOfMass;
		cm.y -= 0.5f;
		rigidbody.centerOfMass = cm;
//		animation.enabled = true;

		GameObject clone = gameObject;
		if(clone == null)
		{
			ScreenLog.Log(this.name + ":clone is null");
			return;
		}

		mRankPlayer[mRankNum].player = clone;
		mRankPlayer[mRankNum].IsPlayer = !bIsAiNPC;
		
		clone.transform.position = mBakeTimePointPos;
		clone.transform.forward = mBakeTimePointRot;
		networkView.RPC("sendTranInfoToOther", RPCMode.OthersBuffered, transform.position, transform.rotation);

		mSpeed = 0f;
		bIsIntoFeiBan = false;
		bIsDoFly = false;
		if (transform.collider != null) {
			transform.collider.enabled = true;
		}
		if(GlobalScript.GetInstance().player != null)
		{
			GlobalScript.GetInstance().player.Speed = 30;
			GlobalScript.GetInstance().player.Speed = 0;
		}
		
		if(!bIsAiNPC)
		{	
			clone.layer = LayerMask.NameToLayer("Player");
			
			Time.timeScale = 1f;
			bikeNetUnity.mPlayerObject = clone.transform;
			if(wangQiuTran != null)
			{
				GlobalScript.GetInstance().player.wangQiuTran = wangQiuTran;
			}
			
//			if(!isOutSuiDao && isPlayerIntoSuiDao)
//			{
//				if(Application.loadedLevel == (int)GameLeve.Leve2 || Application.loadedLevel == (int)GameLeve.Leve4)
//				{
//					BikeLightObj.SetActive(true);
//				}
//			}

			if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
			{
				FireScripts.wangQiuTranEvent( clone );
			}
			GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
		}
//		else if(!RankHandleCtrl.IsStopCheckRank)
//		{
//			GlobalScript.GetInstance().ChangeNPC();
//		}
		StartInitInfo();

		//ScreenLog.Log("clone player ************************");
		if(!bIsAiNPC)
		{
			CancelInvoke("resetPlayerColliderInfo");
			setPlayerColliderInfo(clone.transform, false);
			if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
			{
				networkView.RPC("PlayerWuDiState", RPCMode.OthersBuffered);
			}

			if(clone.transform.collider != null)
			{
				clone.layer = LayerMask.NameToLayer("wuDiLayer");
			}
			
			bIsWuDiState = true;
			Invoke("resetPlayerColliderInfo", 3f);
			if(IsHitJianSuDai)
			{
				ResetPlayerIntoJianSuDai();
			}
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 0.0f, 0.0f);
		}
		
		makeBikeFallPlane();
		
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			if (Network.isClient && mScriptCam != null && !bIsAiNPC)
			{
				//bikeScript.collider.isTrigger = false;
				BikeCameraNet.mBikePlayer = null;
				mScriptCam.setBikePlayer(clone);
			}
		}
		
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
	}
	
	void makeBikeFallPlane()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
	}

	//--------------------------------------------------------------------------------------
	//check npc position
	void startCheckAiPosition()
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer)
		{
			return;
		}

		mPosOld = transform.position;
		if(IsInvoking("checkAiPosition"))
		{
			CancelInvoke("checkAiPosition");
		}
		
		if(BikeCamera.bIsAimPlayer)
		{
			InvokeRepeating("checkAiPosition", 3f, 2f);
		}
		else
		{
			InvokeRepeating("checkAiPosition", 10f, 2f);
		}
	}
	
	public CheckHit checkHitScript;
	
	void checkAiPosition()
	{
		if(GlobalScript.GetInstance().player.IsPass || bIsStopAi || bIsGameOver)
		{
			CancelInvoke("checkAiPosition");
			return;
		}
		
		if(GlobalScript.GetInstance().player.Life <= 0)
		{
			return;
		}
		
		if(bIsFall)
		{
			return;
		}
		
		float dis = 0f;
		dis = Vector3.Distance(transform.position, mBakeTimePointPos);
		if(dis < 3.8f)
		{
			startCheckAiPosition();
			return;
		}
		
		Vector3 posCur = transform.position;
		dis = Vector3.Distance(mPosOld, posCur);
		mPosOld = posCur;
		if(dis < 1f && mSpeed < 0.5f)
		{
			if(checkHitScript != null && checkHitScript.GetIsHit())
			{
				bIsFall = true;
				cloneObject();
			}
		}
	}
	
	public void setParticleState(bool isOpen)
	{
		if(ParticleObj == null)
		{
			return;
		}
		
		if(mSpeed < 15f)
		{
			if (ParticleObj.activeSelf) {
				ParticleObj.SetActive(false);
				if (!bIsAiNPC && checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient) {
					pcvr.GetInstance().PlayerMoveOutTuLu();
				}
			}
			return;
		}
		
		if((isOpen && ParticleObj.activeSelf) || (!isOpen && !ParticleObj.activeSelf))
		{
			return;
		}
		
		//ScreenLog.Log("setParticleState -> isOpen " + isOpen);
		if(isOpen)
		{
			ParticleObj.SetActive(true);
			if (!bIsAiNPC && checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient) {
				pcvr.GetInstance().PlayerMoveIntoTuLu();
			}
		}
		else
		{
			ParticleObj.SetActive(false);
			if (!bIsAiNPC && checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient) {
				pcvr.GetInstance().PlayerMoveOutTuLu();
			}
		}
	}

	public void PlaySurpassAction(Transform tranPar)
	{
		if(!checkHaveOwnerPlayer() || Network.player != ownerPlayer || mPlayer == null)
		{
			return;
		}

		if(!bIsDoFly && !bIsIntoFeiBan && !isPlayCool)
		{
			if(!isPlayCool2 && !isPlayCool3)
			{
				Vector3 va = transform.right;
				Vector3 vb = tranPar.position - transform.position;
				va.y = 0f;
				vb.y = 0f;
				if(Vector3.Distance(vb, Vector3.zero) <= 10f)
				{
					float cosAB = Vector3.Dot(va, vb);
					if(cosAB >= 0)
					{
						isPlayCool2 = true;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.Cool2);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "Cool2");
						}
					}
					else
					{
						isPlayCool3 = true;
						if(Network.isClient)
						{
							PlayAnimation(PlayerAniEnum.Cool3);
							networkView.RPC("CrossFadeState", RPCMode.OthersBuffered, "Cool3");
						}
					}
					
					Invoke("resetPlayCoolState", 2f);
				}
			}
		}
	}

	public void SendAimInfoToServer(string AiNetName, bool isAddSpeed)
	{
		if(FreeModeCtrl.IsServer)
		{
			return;
		}
		int stateAddSpeed = isAddSpeed == true ? 1 : 0;
		networkView.RPC("SendAimInfo", RPCMode.OthersBuffered, AiNetName, stateAddSpeed);
	}

	[RPC]
	void SendAimInfo(string AiNetName, int stateAddSpeed)
	{
		if(!FreeModeCtrl.IsServer)
		{
			return;
		}

		GameObject aiObj = GameObject.Find(AiNetName);
		if (aiObj == null)
		{
			return;
		}

		bikeAiNetUnity netScript = aiObj.GetComponent<bikeAiNetUnity>();
		if(netScript != null)
		{
			netScript.SendAimPlayerToServer(gameObject.name, stateAddSpeed);
		}
	}
	#endregion
	
	#region #bike net start
	[RPC]
	void CrossFadeState(string stateName)
	{
		if (mPlayer == null) {
			return;
		}

		//Debug.Log("CrossFadeState -> stateName "+stateName);
		switch (stateName) {
		case "root":
			PlayAnimation(PlayerAniEnum.root);
			break;
			
		case "run":
			PlayAnimation(PlayerAniEnum.run);
			break;
			
		case "run1":
			PlayAnimation(PlayerAniEnum.run1);
			break;
			
		case "run2":
			PlayAnimation(PlayerAniEnum.run2);
			break;
			
		case "run3":
			PlayAnimation(PlayerAniEnum.run3);
			break;
			
		case "huanhu":
			PlayAnimation(PlayerAniEnum.huanhu);
			break;
			
		case "Cool":
			PlayAnimation(PlayerAniEnum.Cool);
			break;
			
		case "Cool2":
			PlayAnimation(PlayerAniEnum.Cool2);
			break;
			
		case "Cool3":
			PlayAnimation(PlayerAniEnum.Cool3);
			break;
		}

		switch(stateName)
		{
		case "turnRight1":
			TouYingPro.material = TouYingMatYou;
			break;

		case "turnLeft1":
			TouYingPro.material = TouYingMatZuo;
			break;

		default:
			TouYingPro.material = TouYingMatZhong;
			break;
		}
	}

	
	[RPC]
	//调用客户端（这里包括服务端）的SetPlayer函数
	void SetPlayer(NetworkPlayer player)
	{
		//ScreenLog.Log("SetPlayer ************* name " + transform.name);
		ownerPlayer = player;
		if(player == Network.player)
		{
			enabled = true;
		}
	}

	[RPC]
	void SetClientPlayerName(string namePlayer)
	{
		transform.name = namePlayer;
	}

	[RPC]
	void sendTranInfoToServer(Vector3 pos, Quaternion rot)
	{
		correctPlayerPos = pos;
		correctPlayerRot = rot;
	}

	[RPC]
	void sendTranInfoToOther(Vector3 pos, Quaternion rot)
	{
		correctPlayerPos = pos;
		correctPlayerRot = rot;

		transform.position = pos;
		transform.rotation = rot;
	}

	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	public bool GetIsServer()
	{
		return Network.isServer;
	}

	public bool CheckPlayerClient()
	{
		bool isClientPlayer = false;
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer && Network.isClient)
		{
			isClientPlayer = true;
		}
		return isClientPlayer;
	}

	bool checkHaveOwnerPlayer()
	{
		bool isHave = false;
		if(ownerPlayer != null)
		{
			isHave = true;
		}
		return isHave;
	}

	public void checkObjTransform()
	{
		if(checkHaveOwnerPlayer() && Network.player == ownerPlayer)
		{
			return;
		}

		if (Network.isServer || (Network.isClient && !networkView.isMine))
		{
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
			if(correctPlayerPos == Vector3.zero)
			{
				return;
			}

			float dis = Vector3.Distance(transform.position, correctPlayerPos);
			float angleDt = Vector3.Distance(transform.rotation.eulerAngles, correctPlayerRot.eulerAngles);
			//ScreenLog.Log("angleDt " + angleDt);
			if(dis < 0.5f && angleDt < 0.5f)
			{
				return;
			}
			//mRunState = STATE_RUN1;

			if(dis > 100f)
			{
				transform.position = correctPlayerPos;
				transform.rotation = correctPlayerRot;
				return;
			}
		
			if(dis >= 0.1f)
			{
				transform.position = Vector3.Lerp(transform.position, correctPlayerPos, 0.1f);
				moveBikeWheels();
			}
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, 30f);

			if (!bIsDoFly) {
				if(mMouseDownCount > 1.0f)
				{
					if(mSpeed > 50f)
					{
						if(mRunState != STATE_RUN4)
						{
							mRunState = STATE_RUN4;
							CrossFadeState("run3");
						}
					}
					else if(mSpeed > 40f)
					{
						if(mRunState != STATE_RUN2)
						{
							mRunState = STATE_RUN2;
							CrossFadeState("run2");
						}
					}
					else
					{
						if(mRunState != STATE_RUN1)
						{
							mRunState = STATE_RUN1;
							CrossFadeState("run1");
						}
					}
				}
				else
				{
					if(mSpeed > 2f)
					{
						if(mRunState != STATE_RUN3)
						{
							mRunState = STATE_RUN3;
							CrossFadeState("run");
						}
					}
					else if(mRunState != ROOT)
					{
						mRunState = ROOT;
						PlayAnimation(PlayerAniEnum.root);
					}
				}
			}
		}
	}

	Transform camTran = null;
	BikeCameraNet camScript = null;
	void handlePlayerCam()
	{
		if(camTran == null)
		{
			ScreenLog.Log("handlePlayerCam -> set bike camera info..");
			camTran = Camera.main.transform;

			camScript = camTran.GetComponent<BikeCameraNet>();
			camScript.setBikePlayer( gameObject );
		}
	}
	#endregion
}