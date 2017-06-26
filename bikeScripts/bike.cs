using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using SLAB_HID_DEVICE;

public class bike : MonoBehaviour {

	#region #bike data start
	public GameObject TouYingObj;
	private Projector TouYingPro;
	public Material TouYingMatZhong;
	public Material TouYingMatZuo;
	public Material TouYingMatYou;
	public GameObject FengXiaoObj;
	bool IsBackBaoGuo = false;
	float BaoGuoTime = 0f;
	FunEvent baoGuoScript = null;
	Score scoreScript = null;
	static public bool isIntoSuiDao = false;
	bool isIntoSuiDaoNPC = false;
//	bool isOutSuiDao = false;
	bool isPlayerIntoSuiDao = false;
	private GameObject playerSkinnedObj = null;
	public GameObject ParticleObj = null;
	public GameObject WaterParticleObj = null;
	//player object
	private float mAddAiSpeed = 0f;
	private Vector3 mPosOld = Vector3.zero;
	private Transform mClonePrefab = null;
	static private int mDirWrongCount = 0;
	static private bool bIsGameOver = false;
	static private Transform mLastAimNPC = null;
	static public bool bIsHitFall = false;
	public Transform wangQiuTran = null;
	bool bIsIntoFeiBan = false;
	private bool IsIntoXiaoFeiBan = false;
	int brakeState = 0;
	short mAiAddSpeed = 1;
	short mAiSubSpeed = -1;
	private short addAiSpeedState = 0;
	static public GameObject mBikeGameCtrl = null;
	private BikeGameCtrl bikeGameScript = null;
	private bool bIsTurnAiBike = false;
	private float mTimeSteerKey = 0f;
	private bool bIsRenderMat = true;
	static private short mWuDiTimeCount = 0;
	private bool bIsWuDiState = false;
	private bool bIsPlayerBrakeBike = false;
	private bool bIsDoHurtAction = false;
	private bool bIsInitInfo = false;
	public static bool bIsGameStart = false;
	public static bool bIsSelectPlayer = false;	
	public static float MaxMouseDownCount = 9500.0f;
	public static float MinMouseDownCount = 0f;
	public static float mMouseDownCount = 0;
	private float mMaxMouseDownCount = 9500;
	public static float mMouseDownTime = 0f;
	private float mBakeBikeTime = 0f;
	private bool bIsDoTimeOver = false;
	private static int mGameTime = 1000;
	public Transform mBikeAimMark = null;
	private int mBikePathCount = 0;
	private int mRankNum = 0;
	static public int mRankCount = 0;
	static public  playerRank [] mRankPlayer = null;
	public Transform mBikePlayer = null;
	static public float mMaxVelocity = 80f; //km/h
	const float mMaxVelocityFoot = 65f;
	public GameObject mBike = null;
	public Transform centerOfMass = null;
	public Transform wheelForward = null;
	public Transform wheelBack = null;
	public Transform mAimPoint = null;
	public Transform mCamPoint_back = null; //back
	public Transform mCamPoint_left = null; //left
	public Transform mCamPoint_right = null; //right
	private Vector3 mBakeTimePointPos = Vector3.zero;
	private Vector3 mBakeTimePointRot = Vector3.zero;
	private GameObject mPlayer = null;
	static public BikeCamera mScriptCam = null;
	static public JiTui mJiTuiScript = null;
	private bool bIsFallBake = false;
	private float throttle = 0f;
	private float mSteer = 0f;
	private float mStateSpeed = 1.0f;
	private float currentEnginePower = 0f;
	private float currentBrakeEnginePower = 0f;
	private float currentBrakeWaterPower = 0f;
	private bool bIsFall = false;
	public static bool IsStartQiBuZuLi = false;
	int QiBuZuLi = 10;
	float mSpeed = 0f;
	float mSpeedOld = 0.0f;
	private float mFallTime = 0f;
	private bool bIsTurnLeft = false;
	private bool bIsTurnRight = false;
	private bool isCloneObject = false;
	private int mRunState = -1;
	private int ROOT = 0;
	private const int STATE_RUN1 = 1; //run1 action
	private const int STATE_RUN2 = 2; //run2 action
	private const int STATE_RUN3 = 3; //run action
	private const int STATE_RUN4 = 4; //run3 action
	private float mSteerTimeCur = 0f;
	private const float maxSteerTime = 1f;
	private bool bIsDoFly = false;
	private static int mRankingPlayer = 0;
	private bool bIsMoveUp = false;
	private bool bIsMoveDown = false;
	static private	float mGravity = 9.8f;
	private float mResistanceForce = 0f;
	private float mThrottleForce = 0f;
	static int mResistanceUpKey = 50;
	static int mResistanceDownKey = 50;
	private bool bIsSpacePlayer = false;
	private float mSpaceTime = 0f;
	static public int mAiCtrlNum = 0;
	private bool isHiddeBike = false;
	private bool isPlayCool2 = false;
	private bool isPlayCool3 = false;
	BikeAnimatorCtrl BikeAniScript;
	#endregion #end bike data

	#region #start AI
	private bool bIsTurnLeftAiBike = false;
	private bool bIsTurnRightAiBike = false;
	private bool bIsAiNPC = true;
	public int mPathNode = 0;
	float mDelayTimeAI = 0f;
	public Transform mAiPathCtrl = null;
	public Transform mAiRayStartTran = null;
	static public Transform mPlayerObject = null;
	bool bIsStopAi = false;
	private bool bIsSpaceAi = false;
	private float mStopAiTime = 0f;
	public Vector3 [] mAiPathMarkPos = null;
	private float [] aiMarkSpeed = null;
	private bool bIsToEndPointAi = false;
	private float mToEndPointAiTime = 0f;
	Rigidbody rigObj;
	#endregion #end AI

	#region #bike contrl

	public float GetBikeSpeed()
	{
		return mSpeed;
	}

	float getRandPer( int maxPer )
	{
		return (float)Random.Range(0, maxPer) / 100f;
	}

	void initAiPlayerPathInfo()
	{
		if(bIsAiNPC)
		{
			if(mAiPathCtrl != null)
			{
				mPathNode = 0;
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

					int markMaxSpeed = 65; //ai zuiDa suDu = markSpeed * 1.5f
					int markMinSpeed = 45; //ai suiJi zuiDi sudu
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

	static private string mPlayerName = "";
	static public void spawnRandPlayer()
	{
		mPlayerName = "NPC_04";
		GameObject playerObj = GameObject.Find( mPlayerName );
		if(playerObj != null)
		{
			Transform playerTran = null;
			playerTran = playerObj.transform;
			bike script = playerTran.GetComponent<bike>();
			script.bIsAiNPC = false;
			
			mPlayerObject = playerTran;
			if(mScriptCam == null)
			{
				mScriptCam = Camera.main.GetComponent<BikeCamera>();
			}
			mScriptCam.setBikePlayer( playerObj );
		}
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
		transform.parent = null;
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
		BikeAniScript.PlayAnimation(ani, 1f);
	}

	void SetAnimatorSpeed(float val)
	{
		if (BikeAniScript == null) {
			return;
		}
		BikeAniScript.SetAnimatorSpeed(val);
	}

	public static void resetBikeStaticInfo()
	{
		//ScreenLog.Log("reset bike static info...");
		mRankCount = 0;
		mRankingPlayer = 0;
		mBikeGameCtrl = null;
		bIsGameOver = false;
		isIntoSuiDao = false;
		BikeCamera.bIsAimPlayer = false;
	}

	void StartInitInfo()
	{
		rigObj = GetComponent<Rigidbody>();
		TouYingPro = TouYingObj.GetComponent<Projector>();
		if(GlobalScript.GetInstance().player == null)
		{
			return;
		}

		if(mMaxVelocity > 80f)
		{
			mMaxVelocity = 80f;
		}

		mSpeed = 0f;
//		if(!bIsAiNPC)
//		{
//			IsStartQiBuZuLi = true;
//			StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( QiBuZuLi ) );
//		}
		
		mPlayer = this.gameObject;
		setPlayerChildRigibody( mPlayer.transform, false );

		SetCenterOfMass();
		startCheckAiPosition();
		
		int maxRankPlayer = 8;
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
				mRankPlayer = new playerRank[8];
				for(int i = 0; i < maxRankPlayer; i++)
				{
					mRankPlayer[i] = new playerRank();
				}
			}

			if ( mRankCount < maxRankPlayer && mRankPlayer[mRankCount].player == null )
			{
				mRankNum = mRankCount;
				mRankPlayer[mRankCount].player = gameObject;
				mRankCount++;
			}

			if(GlobalScript.GetInstance().player != null)
			{
				GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
			}

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
						mBakeTimePointPos = mark.position;
					}
					mBakeTimePointRot = mark.right;
					mBikeAimMark = mark;
				}
			}

			initAiPlayerPathInfo();
		}
	}

//	public static void setBikeMouseDown()
//	{
//		float taBanVal = InputEventCtrl.PlayerTB[0];
//		if (taBanVal > 0f) {
//			if(!pcvr.bIsHardWare || pcvr.IsTestGetInput) {
//				float dTime = Time.time - mMouseDownTime;
//				mMouseDownCount = Mathf.FloorToInt((190f / dTime) + 0.5f);
//				mMouseDownTime = Time.time;
//			}
//			else {
//				mMouseDownCount = pcvr.TanBanDownCount;
//			}
//			
//			if(mMouseDownCount < 3f) {
//				mMouseDownCount = 0f;
//			}
//			else if(MaxMouseDownCount < mMouseDownCount) {
//				MaxMouseDownCount = mMouseDownCount;
//			}
//		}
//		else {
//			if(!pcvr.bIsHardWare || pcvr.IsTestGetInput) {
//				if (mMouseDownCount > 0f) {
//					float dTime = Time.time - mMouseDownTime;
//					if(dTime > 0.1f) {
//						mMouseDownCount = 0f;
//					}
//				}
//			}
//			else {
//				mMouseDownCount = 0f;
//			}
//		}
//		//ScreenLog.Log("GetInput -> mMouseDownCount " + mMouseDownCount);
//	}

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

	void GetInput()
	{
		if(mPlayer == null) {
			Start();
		}

		if(!bIsAiNPC)
		{
			if (BikeCamera.mBikePlayer == null)
			{
				mFallTime += Time.deltaTime;
				if(mFallTime > 1f)
				{
					bIsFall = true;
					cloneObject();
					return;
				}
				
				if(mFallTime > 0.5f)
				{
					throttle = 0f;
					mSteer = 0f;
					return;
				}
			}

			float steerTmp = InputEventCtrl.PlayerFX[0];
			throttle = 0f;
			if(mGameTime > 0)
			{
				throttle = GlobalScript.GetInstance().player.Energy > 0f ? InputEventCtrl.PlayerYM[0] : 0f;
//				if(pcvr.bIsHardWare && !pcvr.IsTestGetInput)
//				{
////					steerTmp = pcvr.mBikeSteer;
//					if(GlobalScript.GetInstance().player.Energy > 0)
//					{
//						throttle = pcvr.mBikeThrottle;
//					}
//				}
//				else
//				{
////					steerTmp = Input.GetAxis("Horizontal");
//					if(GlobalScript.GetInstance().player.Energy > 0)
//					{
//						throttle = Input.GetAxis("Vertical");
//					}
//				}

				if(bIsDoFly || bIsIntoFeiBan)
				{
					steerTmp = 0f;
				}
			}
			else
			{
				mMouseDownCount = 0f;
			}

			mSteer = steerTmp;

			if(Mathf.Abs(mSteer) < 0.1f)
			{
				mSteer = 0f;
			}

			if(mSteer != 0f && !bIsAiNPC)// && mSpeed > 15f)
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

			float energy = GlobalScript.GetInstance().player.Energy;
			if(energy > 0 && throttle > 0.15f)
			{
				float subEnergy = 0f;
				int level = Mathf.FloorToInt((throttle * 100) / 33);
				//ScreenLog.Log("level " + level);
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
				subEnergy = 100f / 20f; //15秒.

				subEnergy = subEnergy * Time.deltaTime;
				energy -= subEnergy;
				if (!SpeedUI.IsNotSubYouLiang) {
					GlobalScript.GetInstance().player.Energy = energy;
				}
			}
		}

		if(mSpeed > 2f && IsInvoking("resetBikeUseGravity"))
		{
			CancelInvoke("resetBikeUseGravity");
		}

		float rotSpeed = 50 * mSteer * Time.smoothDeltaTime;
		if (!bIsAiNPC) {
			SetTurnValAni(mSteer);
		}

		if(mSteer > 0f && !bIsAiNPC)
		{
			bool isCanTurn = true;
			if(Physics.Raycast(mAimPoint.position, transform.right, 1.3f, RoadColLayer.value) && mSpeed > 0)
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

			if(isCanTurn)
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
			}

			if(Mathf.Abs( mSteer ) < 0.4f)
			{
				bIsTurnRight = false;
			}
		}
		else if(mSteer < 0f && !bIsAiNPC)
		{
			bool isCanTurn = true;
			if(Physics.Raycast(mAimPoint.position, -transform.right, 1.3f, RoadColLayer.value) && mSpeed > 0)
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
			
			if(isCanTurn)
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
			}

			if(Mathf.Abs( mSteer ) < 0.4f)
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
			bIsTurnLeft = false;
			bIsTurnRight = false;
		}

		if (mGameTime > 0 && !bIsAiNPC && bIsDoTimeOver)
		{
			bIsDoTimeOver = false;
		}

		if (!bIsDoFly)
		{
			if(mSpeed >= 40f
			   || (!bIsAiNPC && bIsMoveUp && currentBrakeWaterPower > 10f)
			   || (!bIsAiNPC && currentBrakeWaterPower < 0f && mSpeed > 5f)
			   || (!bIsAiNPC && mMouseDownCount > 0))
			{
				if(bIsAiNPC)
				{
					if (mSpeed > 50f) {
						if (mRunState != STATE_RUN4) {
							mRunState = STATE_RUN4;
							PlayAnimation(PlayerAniEnum.run3);
						}
					}
					else {
						if (mRunState != STATE_RUN2) {
							mRunState = STATE_RUN2;
							PlayAnimation(PlayerAniEnum.run2);
						}
					}
				}
				else
				{
					if(mSpeed > 50f)
					{
						if( mRunState != STATE_RUN4 && mMouseDownCount > 1f )
						{
							mRunState = STATE_RUN4;
							PlayAnimation(PlayerAniEnum.run3);
						}
						else if(mRunState != STATE_RUN3 && mMouseDownCount < 1f)
						{
							mRunState = STATE_RUN3;
							PlayAnimation(PlayerAniEnum.run);
						}
					}
					else
					{
						if( mRunState != STATE_RUN2 && mMouseDownCount > 1f )
						{
							mRunState = STATE_RUN2;
							PlayAnimation(PlayerAniEnum.run2);
						}
						else if(mRunState != STATE_RUN3 && mMouseDownCount < 1f)
						{
							mRunState = STATE_RUN3;
							PlayAnimation(PlayerAniEnum.run);
						}
					}
				}
			}
			else
			{
				if(mSpeed < 1f)
				{
					if (mGameTime <= 0 && !bIsAiNPC) {
					}
					else if(mRunState != ROOT)
					{
						bIsDoTimeOver = false;
						Invoke("resetBikeUseGravity", 0.5f);
						mRunState = ROOT;
						PlayAnimation(PlayerAniEnum.root);
//						if(!bIsAiNPC && !IsStartQiBuZuLi)
//						{
//							IsStartQiBuZuLi = true;
//							StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( QiBuZuLi ) );
//						}
					}
					currentBrakeWaterPower = 0f;
				}
				else if (!bIsMoveUp)
				{
					if (bIsAiNPC) {
						if(mRunState != STATE_RUN1)
						{
							mRunState = STATE_RUN1;
							PlayAnimation(PlayerAniEnum.run1);
						}
					}
					else
					{
						if(mRunState != STATE_RUN1 && mMouseDownCount > 1f)
						{
							mRunState = STATE_RUN1;
							PlayAnimation(PlayerAniEnum.run1);
						}
						else if(mRunState != STATE_RUN3 && mMouseDownCount < 1f)
						{
							mRunState = STATE_RUN3;
							PlayAnimation(PlayerAniEnum.run);
						}
					}
				}
			}
			
			if(!bIsIntoFeiBan)
			{
				float maxAngle = 20f;
				float angleZ = -(mSteerTimeCur * maxAngle) / maxSteerTime;
				if (angleZ > maxAngle) {
					angleZ = maxAngle;
				}
				if (angleZ < -maxAngle) {
					angleZ = -maxAngle;
				}
				//float angleZ =  HidXKUnity_DLL.GetBikeRotationAZ( mSteerTimeCur );
				Vector3 rotationA = mPlayer.transform.localEulerAngles;	
				rotationA.z = angleZ;
				if(Mathf.Abs(mSteerTimeCur) >= 0.05f && Time.frameCount % 3 == 0)
				{
					rotationA.x = 0;
				}
				mPlayer.transform.localEulerAngles = rotationA;
			}
		}

		if(mSpeed < 5f)
		{
			if(bIsAiNPC)
			{
				if(mRunState != ROOT && bIsStopAi)
				{
					rigObj.isKinematic = true;
					rigObj.useGravity = false;
					mRunState = ROOT;
					PlayAnimation(PlayerAniEnum.root);
				}
			}
			else
			{
				if(mRunState != ROOT && brakeState > 0)
				{
					rigObj.isKinematic = true;
					rigObj.useGravity = false;
					bIsDoTimeOver = false;
					mRunState = ROOT;
					PlayAnimation(PlayerAniEnum.root);
					currentBrakeWaterPower = 0f;
//					if(!IsStartQiBuZuLi)
//					{
//						IsStartQiBuZuLi = true;
//						StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( QiBuZuLi ) );
//					}
				}
			}

			if(!bIsFall && mSpeed < 2f)
			{
				if(throttle > 0.011f || mMouseDownCount > 15f)
				{
					if((!bIsAiNPC && !bIsPlayerBrakeBike) || (bIsAiNPC && throttle > 0.011f))
					{
						rigObj.isKinematic = false;
						rigObj.useGravity = true;
					}
				}
			}
		}
		
		if(mRunState == STATE_RUN1 || mRunState == STATE_RUN2 || mRunState == STATE_RUN4)
		{
			float minAnimationSpeed = 0.25f;
			float maxAnimationSpeed = 2.3f;
			float maxSpeed = 50f;
			float key =  0f;

			if(bIsAiNPC)
			{
				key = (maxAnimationSpeed - minAnimationSpeed) / maxSpeed;
				mStateSpeed = key * mSpeed + minAnimationSpeed;
			}
			else
			{
				key = (maxAnimationSpeed - minAnimationSpeed) / MaxMouseDownCount;
				mStateSpeed = key * mMouseDownCount + minAnimationSpeed;
			}
			
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
			case STATE_RUN2:
			case STATE_RUN4:
				SetAnimatorSpeed(mStateSpeed);
				break;
			}
		}
		return;
	}
	
	void resetBikeUseGravity()
	{
		rigObj.useGravity = false;
		rigObj.isKinematic = true;
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
	
	float timeActionCool3;
	bool isPlayCool = false;
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
		if (isPlayCool && !bIsAiNPC) {
			if (BikeCamera.mBikePlayer == null) {
				bIsDoFly = true;
				TimeLastDoFly = Time.time;
				return;
			}

			if (!bIsDoFly) {
				timeActionCool3 = 0f;
				mRunState = -1;
				bIsDoFly = true;
				TimeLastDoFly = Time.time;
				PlayAnimation(PlayerAniEnum.Cool);

				if (checkHitScript != null) {
					checkHitScript.BoxCol.enabled = true;
					if (transform.collider != null) {
						transform.collider.enabled = false;
					}
				}

				if (!GlobalScript.GetInstance().player.IsPass) {
					if (IsHitJianSuDai) {
						ResetPlayerIntoJianSuDai();
					}
					//pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 0.0f);
				}
			}
		}
		else {
			if (BikeCamera.mBikePlayer == null) {
				if (bIsDoFly) {
					makeBikeFall();
				}
				return;
			}
		}
		
		if (bIsIntoFeiBan) {
			timeActionCool3 += Time.deltaTime;
		}
	}

	bool isOutFeiBan = false;
	void resetIsOutFeiBan()
	{
		isOutFeiBan = false;
	}

	void OnTriggerEnter(Collider other)
	{
		GameObject obj = other.gameObject;
		string lay = LayerMask.LayerToName( obj.layer );
		string tagObj = obj.tag;
		if(bIsGameOver || mGameTime <= 1)
		{
			if(tagObj == "cliff")
			{
				bIsFall = true;
			}
			return;
		}
		bike bikeScript = null;

		if(lay == "qiche")
		{
			TouYingObj.SetActive(false);
			if(!bIsAiNPC)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState();
				}
				CameraShake.GetInstance().setCameraShakeImpulseValue();
			}
			makeBikeFall();
			return;
		}
		else if (lay == "NPC" || lay == "Player")
		{
			bikeScript = obj.GetComponent<bike>();
			if(!bIsAiNPC || !bikeScript.bIsAiNPC)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState();
				}
				CameraShake.GetInstance().setCameraShakeImpulseValue();
			}

			if(mSpeed > 25f || (bikeScript && bikeScript.mSpeed > 25))
			{
				if(bikeScript == null)
				{
					return;
				}
				
				float rand = Random.Range(0f, 10000f) % 100;
				if(rand < 35)
				{
					if(!bikeScript.bIsFall && !bikeScript.bIsAiNPC)
					{
						bikeScript.makeBikeFall();
					}
					
					if(!bIsFall && !bIsAiNPC)
					{
						makeBikeFall();
					}
				}
				else
				{
					if(!bikeScript.bIsFall && bikeScript.bIsAiNPC)
					{
						bikeScript.makeBikeFall();
					}

					if(!bIsFall && bIsAiNPC)
					{
						makeBikeFall();
					}
				}
				return;
			}
		}

		if(tagObj == "rock")
		{
			TouYingObj.SetActive(false);
			////ScreenLog.Log("rock make bike fall...");
			if(!bIsAiNPC)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState();
				}
				CameraShake.GetInstance().setCameraShakeImpulseValue();
			}
			makeBikeFall();
			return;
		}

		if(!bIsAiNPC && tagObj == "JianSuGang")
		{
			//ScreenLog.Log("**************************** test jianSuGang");
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().HandlePlayerHitState();
			}
			return;
		}

		StartCoroutine(OnCollisionObject( obj.transform, other, 0));
	}

	void OnTriggerExit(Collider other)
	{
		if(bIsGameOver || mGameTime <= 1)
		{
			return;
		}
		GameObject obj = other.gameObject;
		StartCoroutine(OnCollisionObject( obj.transform, other, 1));
	}

	void checkAiForwardHit()
	{
		if(!bIsAiNPC)
		{
			return;
		}
		
		RaycastHit hit;
		Vector3 startPos = transform.position;
		BoxCollider boxCol = GetComponent<BoxCollider>();
		if(boxCol)
		{
			startPos = boxCol.center;
			startPos.z += (boxCol.size.z - 0.1f);
			startPos.x += (boxCol.size.x - 0.05f) * getRandPer(100) * Random.Range(-1, 2);
			startPos.y += (boxCol.size.y - 0.05f) * getRandPer(100) * Random.Range(-1, 2);
		}
		
		if(mAiRayStartTran)
		{
			mAiRayStartTran.localPosition = startPos;
			mAiRayStartTran.forward = transform.forward;
			startPos = mAiRayStartTran.position;
		}
		
		if (Physics.Raycast(startPos, transform.forward, out hit, 20))
		{
			Transform hitTran = hit.transform;
			Vector3 vecA = hitTran.right;
			Vector3 vecB = transform.position - hitTran.position;
			vecA.y = 0f;
			vecB.y = 0f;
			float cosAB = Vector3.Dot(vecA, vecB);

			bool isTurnRight = false;
			if(cosAB < 0f)
			{
				isTurnRight = true;
			}

			bool isHandleTurn = false;
			string layHit = LayerMask.LayerToName( hitTran.gameObject.layer );
			if(layHit == "qiche")
			{
				isHandleTurn = true;
			}
			else
			{
				string tagHit = hitTran.tag;
				switch(tagHit)
				{
				case "feiBan":
					isHandleTurn = true;
					break;

				case "car":
					isHandleTurn = true;
					break;

				case "Player":
					bike bikeScript = hitTran.GetComponent<bike>();
					if(!bikeScript.bIsAiNPC)
					{
						isHandleTurn = true;
					}
					break;
				}
			}

			if(!bIsTurnAiBike && isHandleTurn)
			{
				if(!isTurnRight)
				{
					//						//ScreenLog.Log("make Ai turn left!");
					bIsTurnLeftAiBike = true;
					bIsTurnRightAiBike = false;
				}
				else
				{
					//						//ScreenLog.Log("make Ai turn right!");
					bIsTurnLeftAiBike = false;
					bIsTurnRightAiBike = true;
				}
				
				bIsTurnAiBike = true;
				makeAiTurn();
			}
		}
	}

	BikeHeadMoveState StateMove = BikeHeadMoveState.NULL;
	public LayerMask RoadColLayer;
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

		cosAngleFU = Vector3.Dot(Vector3.up, vecFor);

		bool isCheckUpDown = true;
		if(brakeState != 0)
		{
			isCheckUpDown = false;
		}

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
					////ScreenLog.Log(this.name + ": move up!");
				}

				if(isCheckUpDown)
				{
					if(!bIsWuDiState)
					{
						mResistanceForce = - mGravity * rigObj.mass * Mathf.Sin( Mathf.Deg2Rad * grade );
						mResistanceForce *= mResistanceUpKey;
					}
				}
			}
			else
			{
				if(!bIsMoveDown)
				{
					bIsMoveUp = false;
					bIsMoveDown = true;
					////ScreenLog.Log(this.name + ": move down!");
				}

				if(isCheckUpDown && !bIsWuDiState)
				{
					mResistanceForce = mGravity * rigObj.mass * Mathf.Sin( Mathf.Deg2Rad * grade );
					mResistanceForce *= mResistanceDownKey;
				}
			}

			if(!bIsAiNPC)// && Time.frameCount % 2 == 0)
			{
				//bool isOutput = false;
				//string strInfo = bIsMoveUp == true ? ", +++++++++++" : ", ------------";
				if(bIsMoveUp && StateMove != BikeHeadMoveState.UP)
				{
					//isOutput = true;
					if(!IsIntoXiaoFeiBan && !bIsIntoFeiBan)
					{
						StateMove = BikeHeadMoveState.UP;
						if(!GlobalScript.GetInstance().player.IsPass)
						{
							if (pcvr.GetInstance() != null) {
								pcvr.GetInstance().HandleBikeHeadQiFu(StateMove, mSpeed, grade);
							}
						}
					}
				}

				if(!bIsMoveUp && StateMove != BikeHeadMoveState.DOWN)
				{
					//isOutput = true;
					if(!IsIntoXiaoFeiBan && !bIsIntoFeiBan)
					{
						StateMove = BikeHeadMoveState.DOWN;
						if(!GlobalScript.GetInstance().player.IsPass)
						{
							if (pcvr.GetInstance() != null) {
								pcvr.GetInstance().HandleBikeHeadQiFu(StateMove, mSpeed, grade);
							}
						}
					}
				}
			}
			//ScreenLog.Log(this.name + ": grade " + grade + ", mResistanceForce " + mResistanceForce);
		}
		else
		{
			if (!bIsAiNPC) {
				if (StateMove != BikeHeadMoveState.PLANE) {
					if (!IsIntoXiaoFeiBan && !bIsIntoFeiBan) {
						StateMove = BikeHeadMoveState.PLANE;
						if (!GlobalScript.GetInstance().player.IsPass) {
							if (pcvr.GetInstance() != null) {
								pcvr.GetInstance().HandleBikeHeadQiFu(StateMove, mSpeed, 0f);
							}
						}
					}
				}
			}

			if(bIsMoveUp && !bIsWuDiState)
			{
				float subPower = mResistanceForce / rigObj.mass;
				subPower *= 5f;
				currentEnginePower += subPower;
				if (currentEnginePower < 0f)
				{
					currentEnginePower = 0f;
				}
			}
			
			if(bIsMoveDown && !bIsWuDiState)
			{
				float subPower = mResistanceForce / rigObj.mass;
				currentEnginePower -= subPower;
			}
			bIsMoveUp = false;
			bIsMoveDown = false;
		}

		Vector3 vecA = rigObj.velocity;
		Vector3 vecB = transform.forward;
		vecA.y = 0f;
		vecB.y = 0f;
		float cosAB = Vector3.Dot(vecA, vecB);
		if(cosAB < 0f)
		{
			mBakeBikeTime += Time.deltaTime;
			if(mBakeBikeTime > 5f)
			{
				bIsFallBake = true;
				makeBikeFall();
			}
		}
		else
		{
			mBakeBikeTime = 0f;
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
		for(int i = 0; i < mRankPlayer.Length; i++)
		{
			if(mRankPlayer[i].Name == this.name)
			{
				if(GlobalScript.GetInstance().player.RankList == null)
				{
					GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
				}

				mRankPlayer[i].IsPlayer = true;
				GlobalScript.GetInstance().ChangeNPC();
				FireScripts.wangQiuTranEvent( gameObject );
				break;
			}
		}

		Transform camTranM = null;
		if(wangQiuTran != null)
		{
			GlobalScript.GetInstance().player.wangQiuTran = wangQiuTran;
			//ScreenLog.Log("set wangQiu info");
		}

		GameObject mainCam = GameObject.Find("Main_Camera");
		if(mainCam != null)
		{
			camTranM = mainCam.transform;
			mScriptCam = camTranM.GetComponent<BikeCamera>();
			if(mScriptCam != null)
			{
				//mScriptCam.setBikePlayer(gameObject);
				if(mScriptCam.mDaoJuCam != null)
				{
					//camTranC = mScriptCam.mDaoJuCam;
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

		startPlayerCheckAi();

		Invoke("setIsHitFall", 3f);
	}

	void startPlayerCheckAi()
	{
		if(IsInvoking("playerCheckAi"))
		{
			CancelInvoke("playerCheckAi");
		}
		InvokeRepeating("playerCheckAi", 5f, 10f);
	}

	static public void setIsGameStart(bool isGameStart)
	{
		bIsGameStart = isGameStart;
		if(bIsGameStart && mPlayerObject != null)
		{
			bike bikeScript = mPlayerObject.GetComponent<bike>();
			bikeScript.setCameraEffect();
		}
	}

	void setIsHitFall()
	{
		bIsHitFall = true;
	}

	void changePlayerMesh()
	{
		if(!bIsWuDiState)
		{
			return;
		}

		short maxNum = 6;
		int count = mWuDiTimeCount % maxNum;
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
	}

	// FixedUpdate is called once per frame
	void FixedUpdate()
	{
		if( Time.frameCount % 5 == 0 &&
		   (!bIsInitInfo || GlobalScript.GetInstance().player.IsPass || GlobalScript.GetInstance().player.IsGameOver) )
		{
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().setFengShanInfo(0, 0);
				pcvr.GetInstance().setFengShanInfo(0, 1);
			}
			return;
		}
		else if( mRunState == ROOT && bIsAiNPC && bIsStopAi)
		{
			return;
		}

		if(bIsDoFly || bIsGameOver)
		{
			Vector3 rotationA = mPlayer.transform.eulerAngles;
			rotationA.z = 0f;
			mPlayer.transform.eulerAngles = rotationA;
			return;
		}
		else if(bIsFall)
		{
			if(mBikePlayer.rigidbody != null)
			{
				mBikePlayer.rigidbody.useGravity = true;
				mBikePlayer.rigidbody.isKinematic = false;
				if(bIsFallBake)
				{
					mBikePlayer.rigidbody.AddForce(-transform.forward * rigObj.mass * 25f * Time.deltaTime);
					mBikePlayer.rigidbody.AddTorque(-transform.forward * rigObj.mass * 2f);
				}
				else
				{
					mBikePlayer.rigidbody.AddForce(transform.forward * rigObj.mass * 25f * Time.deltaTime);
					mBikePlayer.rigidbody.AddTorque(transform.forward * rigObj.mass * 2f);
				}
			}
			
			if(mBike.rigidbody != null)
			{
				mBike.rigidbody.useGravity = true;
				mBike.rigidbody.isKinematic = false;
				if(bIsFallBake)
				{
					mBike.rigidbody.AddForce(-transform.forward * rigObj.mass * 50f * Time.deltaTime);
					mBike.rigidbody.AddTorque(-transform.forward * rigObj.mass * 2f);
				}
				else
				{
					mBike.rigidbody.AddForce(transform.forward * rigObj.mass * 50f * Time.deltaTime);
					mBike.rigidbody.AddTorque(transform.forward * rigObj.mass * 2f);
				}
			}
			return;
		}
		else if( checkBikeState( 0 ) )
		{
			return;
		}

		if(Time.frameCount % 5 == 0)
		{
			checkBikeSpeed();
			if(!bIsAiNPC && !bIsIntoFeiBan && !IsHitJianSuDai)
			{
				checkMoveUpDown();
			}
		}

		if(mGameTime > 0)
		{
			GetInput();
			
			CalculateEnginePower();

			if(IsIntoXiaoFeiBan || bIsIntoFeiBan || isOutFeiBan)
			{
				if(bIsIntoFeiBan || isOutFeiBan)
				{
					if(mSpeed < 75f && !bIsAiNPC)
					{
						rigObj.AddForce(mBike.transform.forward * Time.deltaTime * 2400f, ForceMode.Acceleration);
					}
					
					if(mSpeed < 60f && bIsAiNPC)
					{
						rigObj.AddForce(mBike.transform.forward * Time.deltaTime * 2400f, ForceMode.Acceleration);
					}
				}
				else
				{
					rigObj.AddForce(mBike.transform.forward * Time.deltaTime * 2000f * rigObj.mass);
				}
			}
			else
			{
				if(throttle > 0.000001f || mMouseDownCount > 0 ||
				   (!bIsAiNPC && (mThrottleForce > 1f || brakeState > 0 || Mathf.Abs(mResistanceForce) > 10f)) )
				{
					// The rigidbody velocity is always given in world space,
					//but in order to work in local space of the car model we need to transform it first.
					Vector3 relativeVelocity = mBike.transform.InverseTransformDirection(rigObj.velocity);
					ApplyThrottle( relativeVelocity );

					if(mSpeed < 10f && !bIsPlayerBrakeBike)
					{
						rigObj.AddForce(mBike.transform.forward * Time.deltaTime * 1000f, ForceMode.Acceleration);
					}
				}
			}
		}
	}

	public void resetPlayCoolState(int val)
	{
		if (bIsAiNPC) {
			return;
		}

		switch (val) {
		case 1:
			SetAnimatorSpeed(0.3f);
			break;

		case 2:
			if (!bIsAiNPC) {
				isOutFeiBan = true;
			}
			isPlayCool = false;
			SetAnimatorSpeed(1f);
			break;

		case 3:
			if (!bIsAiNPC) {
				isOutFeiBan = true;
				Invoke("resetIsOutFeiBan", 0.5f);
			}
			bIsIntoFeiBan = false;
			bIsDoFly = false;
			
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 10.0f);
			}
			mRunState = STATE_RUN2;
			PlayAnimation(PlayerAniEnum.run2);
			if (transform.collider != null) {
				transform.collider.enabled = true;
			}
			break;

		default:
			mRunState = STATE_RUN2;
			PlayAnimation(PlayerAniEnum.run2);
			break;
		}
	}

	void updatePlayerRank()
	{
		if(mRankNum < 1 || mRankNum > 7)
		{
			return;
		}

		GameObject objParent = mRankPlayer[mRankNum - 1].player;
		if(objParent == null)
		{
			return;
		}

		bike bikeScript = objParent.GetComponent<bike>();
		bikeScript.mRankNum++;

		mRankPlayer[mRankNum].player = objParent;
		mRankPlayer[mRankNum - 1].player = gameObject;
		mRankNum--;
		if(!bIsAiNPC)
		{
			if(!bIsDoFly && !bIsIntoFeiBan && !isPlayCool)
			{
				if(!isPlayCool2 && !isPlayCool3)
				{
					Vector3 va = transform.right;
					Vector3 vb = objParent.transform.position - transform.position;
					va.y = 0f;
					vb.y = 0f;
					if(Vector3.Distance(vb, Vector3.zero) <= 10f)
					{
						float cosAB = Vector3.Dot(va, vb);
						if(cosAB >= 0)
						{
							isPlayCool2 = true;
							PlayAnimation(PlayerAniEnum.Cool2);
						}
						else
						{
							isPlayCool3 = true;
							PlayAnimation(PlayerAniEnum.Cool3);
						}
					}
				}
			}
		}

		if(GlobalScript.GetInstance().player != null)
		{
			GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
		}
	}

	void checkPlayerRank()
	{
		if(bIsGameOver)
		{
			return;
		}

		if(mRankNum < 1)
		{
			return;
		}

		if(mRankPlayer[mRankNum].player != null)
		{
			GameObject parentPlayer = mRankPlayer[mRankNum - 1].player;
			if(parentPlayer == null)
			{
				return;
			}
			//ScreenLog.Log("parentPlayer " + parentPlayer.name + ", player " + this.name);
			
			bike bikeScript = parentPlayer.GetComponent<bike>();
			if(bikeScript.mBikeAimMark == null || bikeScript.bIsFall || bIsFall)
			{
				return;
			}

			if(bikeScript.bIsStopAi)
			{
				return;
			}
			//ScreenLog.Log("" + bikeScript.mBikeAimMark);

			int AiPathIdP = bikeScript.mBikeAimMark.parent.GetInstanceID();
			if(mBikeAimMark == null)
			{
				//ScreenLog.LogWarning("mBikeAimMark is null");
				return;
			}

			int pathId = mBikeAimMark.parent.GetInstanceID();
			if(AiPathIdP == pathId)
			{
				AiMark markScript = mBikeAimMark.GetComponent<AiMark>();
				AiMark markScriptP = bikeScript.mBikeAimMark.GetComponent<AiMark>();
				int markCount = markScript.getMarkCount();
				int markCountP = markScriptP.getMarkCount();
				if(markCount < markCountP)
				{
					return;
				}
			}
			else if(mBikePathCount < bikeScript.mBikePathCount)
			{
				return;
			}

			Vector3 vecA = parentPlayer.transform.position - gameObject.transform.position;
			Vector3 vecB = bikeScript.mBikeAimMark.position - parentPlayer.transform.position;
			Vector3 vecC = gameObject.transform.forward;
			Vector3 vecD = parentPlayer.transform.forward;
			vecA.y = 0f;
			vecB.y = 0f;
			vecC.y = 0f;
			vecD.y = 0f;
			
			float lenA = Vector3.Distance(vecA, Vector3.zero);
			float lenB = Vector3.Distance(vecB, Vector3.zero);
			float lenC = Vector3.Distance(vecC, Vector3.zero);
			float lenD = Vector3.Distance(vecD, Vector3.zero);
			if(lenA == 0f || lenB == 0f || lenC == 0f || lenD == 0f)
			{
				return;
			}
			
			float cosDB = Vector3.Dot(vecD, vecB) / (lenD * lenB);
			float cosDC = Vector3.Dot(vecD, vecC) / (lenD * lenC);
			float cosAC = Vector3.Dot(vecA, vecC) / (lenA * lenC);
			if( (cosDB > 0f && cosDC > 0f && cosAC < 0f)
			   || (cosDB <= 0f && cosDC > 0f && cosAC > 0f)
			   || (cosDB <= 0f && cosDC <= 0f && cosAC < 0f) )
			{
				updatePlayerRank();
				return;
			}
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

		bike script = null;

		bool isCanFire = false;
		Transform child = null;
		int count = mRankPlayer.Length;

		for(int i = 0; i < count; i++)
		{
			child = mRankPlayer[i].player.transform;
			if(child != null)
			{
				script = child.GetComponent<bike>();
				if(script != null && script.bIsAiNPC)
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
					//ScreenLog.Log(this.name + "::the mark not find AiMark scirpt! parent " + mBikeAimMark.parent.name);
					return false;
				}
				
				int conutTmp = mBikeAimMark.parent.childCount - 1;
				int markCount = markScript.getMarkCount();
				if(markCount == conutTmp)
				{
					mBikePathCount++;
					checkPlayerRank();
				}
				
				mBakeTimePointPos = mBikeAimMark.position;
				mBakeTimePointRot = mBikeAimMark.right;
				
				mBikeAimMark = markScript.mNextMark;

				/*//ScreenLog.Log(this.name + "::The player update AimMark! mBikePathCount = " + mBikePathCount
				          + ", markCount " + markCount);*/
				return true;
			}
		}

		return false;
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
		int time = 40;
		if(mDirWrongCount > time)
		{
			////ScreenLog.Log("dir is wrong!");
			CancelInvoke("spawnPlayerDirWrong");
			cloneObject();
			return;
		}
		mDirWrongCount++;
		return;
	}

	void hiddenBikeObj()
	{
		gameObject.SetActive(false);
	}

	void HandleCamDouDong()
	{
		rigObj.useGravity = false;
		rigObj.isKinematic = true;
	}

	float lastTimeRender = 0.0f;
	void Update()
	{
		mMouseDownCount = pcvr.mMouseDownCount;
		float dTime = Time.realtimeSinceStartup - lastTimeRender;
		if(dTime < 0.03f)
		{
			return;
		}
		lastTimeRender = Time.realtimeSinceStartup;
		CheckResetIsDoFly();
		
		if(GlobalScript.GetInstance().player != null)
		{
			mGameTime = GlobalScript.GetInstance().player.Life;
			if (mGameTime <= 0) {
				if ((!bIsAiNPC && !bIsFall) || bIsAiNPC) {
					if (!rigObj.isKinematic) {
						PlayAnimation(PlayerAniEnum.root);
					}
					rigObj.isKinematic = true;
					rigObj.useGravity = false;
					return;
				}
			}
			
			if (mGameTime <= 1 && !bIsAiNPC && IsInvoking("HandlePlayerIntoJianSuDai")) {
				ResetPlayerIntoJianSuDai();
			}
		}

		if( mRunState == ROOT && bIsAiNPC && bIsStopAi)
		{
			return;
		}

		if(GlobalScript.GetInstance().player.IsPass && !isHiddeBike)
		{
			isHiddeBike = true;
			Invoke("hiddenBikeObj", 3f);
			return;
		}
		
		if(isHiddeBike)
		{
			return;
		}

		if(!bIsAiNPC)
		{
			if(Time.timeScale == 0)
			{
				if(!IsBackBaoGuo)
				{
					IsBackBaoGuo = true;
					BaoGuoTime = Time.realtimeSinceStartup;
					
					if(mBikeGameCtrl == null)
					{
						mBikeGameCtrl = GameObject.Find(GlobalData.bikeGameCtrl);
					}
					if(bikeGameScript == null)
					{
						bikeGameScript = mBikeGameCtrl.GetComponent<BikeGameCtrl>();
					}
					baoGuoScript = bikeGameScript.getBaoGuoScript();
					scoreScript = bikeGameScript.getScoreScript();
				}

				if(Time.realtimeSinceStartup - BaoGuoTime > 3f)
				{
					if(!baoGuoScript.IsBackInfo)
					{
						baoGuoScript.BackInfo();
					}
					
					if(!scoreScript.IsAddBaoGuoScore)
					{
						scoreScript.IsAddBaoGuoScore = true;
					}
				}
			}
			else if(IsBackBaoGuo)
			{
				IsBackBaoGuo = false;
			}
			//ScreenLog.Log("realtimeSinceStartup " + Time.realtimeSinceStartup + ", dTime " + Time.deltaTime);
		}

		if(!bIsInitInfo)
		{
			return;
		}

		if(bIsGameOver)
		{
			return;
		}

		if(bIsFall)
		{
			if(!bIsAiNPC)
			{
				if(mGameTime > 0)
				{
					isCloneObject = false;
					bIsFall = false;
					Invoke( "cloneObject", 1.0f);
				}
			}
			return;
		}

		if(bIsDoFly)
		{
			checkIsDoFlyAction();
			return;
		}

		if(bIsAiNPC && !bIsStopAi)
		{
			checkAiForwardHit();
			
			if(mDelayTimeAI > 0f)
			{
				mDelayTimeAI -= Time.deltaTime;
				return;
			}
		}

		if(!bIsAiNPC)
		{
			checkPlayerMouseFire();
			checkPlayerMoveDir();
		}
		else if(!checkIsMoveNPC())
		{
			return;
		}

		if(Time.frameCount % 5 == 0)
		{
			checkPlayerRank();

			checkIsDoFlyAction();

			if(isSlowWorld)
			{
				resetIsSlowWorld();
			}
		}
		moveBikeWheels();
	}

	bool checkIsMoveNPC()
	{
		if(bIsStopAi || bIsFall)
		{
			return false;
		}

		AiPathCtrl aiPath = mAiPathCtrl.GetComponent<AiPathCtrl>();

		Vector3 vMarkPos = mAiPathMarkPos[mPathNode];
		Vector3 posCur = transform.position;
		//vMarkPos.y = posCur.y;
		posCur.y = vMarkPos.y;

		float dis = Vector3.Distance(transform.position, vMarkPos);
		float minDis = 2f;
		float toEndMinTime = 1.5f;
		if(bIsToEndPointAi &&  mToEndPointAiTime < toEndMinTime)
		{
			mToEndPointAiTime += Time.deltaTime;
		}

		if(dis <= minDis || (bIsToEndPointAi && mToEndPointAiTime >= toEndMinTime))
		{
			bool isNextPath = false;
			int markCount = mAiPathCtrl.childCount;
			int endCount = markCount - 1;
			float key = 1f;
			
			Transform aimMark = mAiPathCtrl.GetChild(mPathNode);
			AiMark markScript = aimMark.GetComponent<AiMark>();
			mBikeAimMark = markScript.mNextMark;
			if(Random.Range(0, 100) < 50)
			{
				key = -1f;
			}

			if(mToEndPointAiTime >= toEndMinTime)
			{
				//Debug.Log("test***********************111");
				aimMark = mAiPathCtrl.GetChild(0);
				mBakeTimePointPos = aimMark.position + key * aimMark.forward * aimMark.localScale.z * 0.5f;
				mBakeTimePointRot = aimMark.right;
			}
			else
			{
				mBakeTimePointPos = aimMark.position + key * aimMark.forward * aimMark.localScale.z * 0.5f;
				mBakeTimePointRot = aimMark.right;
			}

			if(mPathNode >= endCount)
			{
				mBikePathCount++;
				checkPlayerRank();
			}
			//ScreenLog.Log(this.name + ":ai to next mark, mPathNode " + mPathNode + ", mBikePathCount = " + mBikePathCount);

			if((mPathNode >= endCount && !bIsStopAi) || (bIsToEndPointAi && mToEndPointAiTime >= toEndMinTime))
			{
				if(aiPath != null)
				{
					Transform nextPath1 = aiPath.mNextPath1;
					Transform nextPath2 = aiPath.mNextPath2;
					if(nextPath1 || nextPath2)
					{
						if(nextPath1 && nextPath2)
						{
							if(Random.Range(0, 2) == 0)
							{
								mAiPathCtrl = nextPath1;
							}
							else
							{
								mAiPathCtrl = nextPath2;
							}
						}
						else if(nextPath1)
						{
							mAiPathCtrl = nextPath1;
						}
						else if(nextPath2)
						{
							mAiPathCtrl = nextPath2;
						}
						//ScreenLog.Log(gameObject.name + ": AI move to next path, mAiPathCtrl " + mAiPathCtrl.name);
						
						initAiPlayerPathInfo();
						isNextPath = true;
					}
					else
					{
						bIsStopAi = true;
						throttle = 0.0f;
					}
				}
			}

			if(mPathNode < endCount && !isNextPath)
			{
				mPathNode++;
			}
			
			mSteer = 0f;
			return false;
		}
		
		throttle = 0f;
		if(mGameTime > 0)
		{
			throttle = 1f;
		}
		
		if(!bIsTurnAiBike)
		{
			Vector3 vecTmp = vMarkPos - transform.position;
			vecTmp.y = 0f;
			vecTmp = vecTmp.normalized;
			transform.forward = vecTmp;
		}
		return true;
	}

	void makeAiTurn()
	{
		if(!bIsTurnAiBike)
		{
			return;
		}

		float dis = 3f;
		float delayTime = 0.5f;
		if(mSpeed > 1f)
		{
			delayTime = (dis * 3.6f) / mSpeed;
		}

		if(delayTime > 1f)
		{
			delayTime = 1f;
		}

		if(bIsTurnLeftAiBike)
		{
			Invoke("resetTurnInofNPC", delayTime);
			transform.eulerAngles += new Vector3(0f, -25f, 0f);
			bIsTurnLeftAiBike = false;
		}
		else if(bIsTurnRightAiBike)
		{
			Invoke("resetTurnInofNPC", delayTime);
			transform.eulerAngles += new Vector3(0f, 25f, 0f);
			bIsTurnRightAiBike = false;
		}
		else
		{
			resetTurnInofNPC();
		}
	}
	
	private void resetTurnInofNPC()
	{
		if(!bIsAiNPC)
		{
			return;
		}
		bIsTurnAiBike = false;
	}
	
	bool checkBikeForwardAngle()
	{
		Vector3 vA = transform.forward;
		Vector3 vB = Vector3.up;
		float lenA = Vector3.Distance(vA, Vector3.zero);
		float lenB = Vector3.Distance(vB, Vector3.zero);
		float cosAB = Vector3.Dot(vA, vB) / (lenA * lenB);
		cosAB = Mathf.Abs( cosAB );
		
		float maxAngle = 30f;
		float angleTmp = (90f - maxAngle) * Mathf.Deg2Rad;
		float minCos = Mathf.Cos(angleTmp);

		if(cosAB >= minCos)
		{
			float angleAB = Mathf.Acos(cosAB) * Mathf.Rad2Deg;
			angleAB = 90f - angleAB;
			//ScreenLog.Log(this.name + ": bike should be fall! angleForward: " + angleAB);

			bIsHitFall = true;
			return true;
		}
		return false;
	}

	void CloseAnimator()
	{
		if (BikeAniScript == null) {
			return;
		}
		BikeAniScript.CloseAnimator();
	}
	
	public bool checkBikeState( int key )
	{
		bool isFall = false;
		if (key == 1)
		{
			isFall = true;
		}
		else
		{
			isFall = checkBikeForwardAngle();
			if(!isFall)
			{
				float eaZ = mPlayer.transform.eulerAngles.z;
				isFall = HidXKUnity_DLL.CheckBikeAngleLR( eaZ );
			}
		}
		
		if (isFall && (bIsHitFall || bIsAiNPC))
		{
			bIsFall = true;
			if(!bIsAiNPC)
			{
				if(IsInvoking("changePlayerMesh"))
				{
					CancelInvoke("changePlayerMesh");
				}

				mSpeed = 0f;
				GlobalScript.GetInstance().ShowTishi( TishiInfo.Diedao );
				GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
				if(FengXiaoObj != null)
				{
					FengXiaoCtrl.IsPlayFengXiao = false;
					FengXiaoObj.SetActive(false);
				}
			}

			if(!isCloneObject)
			{
				isCloneObject = true;
				Invoke("cloneObject", 0.5f);
			}

			BoxCollider boxCol = GetComponent<BoxCollider>();
			if(boxCol)
			{
				boxCol.enabled = false;
				Destroy(boxCol);
			}
			
			if(mPlayer != null)
			{
				CloseAnimator();
				setPlayerChildRigibody( mPlayer.transform, true );
			}

			if(mBike.rigidbody == null)
			{
				mBike.AddComponent("Rigidbody");
				mBike.rigidbody.mass = 75f;
				mBike.rigidbody.isKinematic = false;
				//mBike.rigidbody.detectCollisions = true;
			}

			Vector3 cm = rigObj.centerOfMass;
			cm.y += 0.5f;
			rigObj.centerOfMass = cm;
			rigObj.isKinematic = true;
			
			CloseAnimator();
		}
		
		return bIsFall;
	}
	
	void makeBikeFall()
	{
		checkBikeState( 1 );
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
			if (rigids[index].transform.collider != null) {
				rigids[index].transform.collider.enabled = true;
			}
			rigids[index].mass = 70f;
			rigids[index].useGravity = true;
			rigids[index].isKinematic = false;
			rigids[index].detectCollisions = true;
		}
	}
	
	void ApplyThrottle(Vector3 relativeVelocity)
	{	
		if(!bIsAiNPC)
		{
			if(bIsMoveUp && brakeState > 0 && rigObj.useGravity && mSpeed < 10f)
			{
				rigObj.isKinematic = true;
				rigObj.useGravity = false;
			}

			float power = mThrottleForce + mResistanceForce;
			//ScreenLog.Log("power " + power + ", mResistanceForce " + mResistanceForce);
			if(power < 0f && mResistanceForce < 0f)
			{
				power = mThrottleForce;
			}
			rigObj.AddForce(mBike.transform.forward * Time.deltaTime * power);
		}
		else
		{
			rigObj.AddForce(mBike.transform.forward * Time.deltaTime * mThrottleForce);
		}
	}

	void checkBikeSpeed()
	{
		mSpeed = HidXKUnity_DLL.GetBikeSpeed( rigObj.velocity.magnitude );
		if (ParticleObj.activeSelf && mSpeed < 15f) {
			ParticleObj.SetActive(false);
			if (!bIsAiNPC) {
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().PlayerMoveOutTuLu();
				}
			}
			return;
		}

		if (bIsAiNPC) {
			return;
		}

		int fengVal = 0;
		if(mSpeed < 10 || bIsGameOver || Time.timeScale == 0)
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
		
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().setFengShanInfo(fengVal, 0);
			pcvr.GetInstance().setFengShanInfo(fengVal, 1);
		}

		float dVal = mSpeedOld - mSpeed;
		if(dVal >= 20.0f)
		{
			//ScreenLog.Log("checkBikeSpeed -> dVal " + dVal);
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().HandlePlayerHitState();
			}
		}
		mSpeedOld = mSpeed;

		if(mSpeed >= 50)
		{
			FengXiaoCtrl.IsPlayFengXiao = true;
			FengXiaoObj.SetActive(true);
		}
		else
		{
			FengXiaoCtrl.IsPlayFengXiao = false;
			FengXiaoObj.SetActive(false);
		}

//		if(IsStartQiBuZuLi)
//		{
//			int zuLiVal = HidXKUnity_DLL.GetQiBuZuLi(mSpeed);
//			if(zuLiVal <= 3)
//			{
//				zuLiVal = 3;
//				IsStartQiBuZuLi = false;
//			}
//			
//			//ScreenLog.Log("zuLiVal " + zuLiVal);
//			if(pcvr.mBikeZuLiState != zuLiVal)
//			{
//				StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( zuLiVal ) );
//			}
//		}

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
			int val = Mathf.FloorToInt(mSpeed);
			if(val >= 100)
			{
				val = 99;
			}

			if(val <= 0)
			{
				GlobalScript.GetInstance().player.Speed = 10;
			}
			GlobalScript.GetInstance().player.Speed = val;
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

	void resetAddAiSpeedState()
	{
		mAiCtrlNum--;
		addAiSpeedState = 0;
	}

	//-------------------------------------------------------------------------
	void CalculateEnginePower()
	{
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
//				//brakeState -= 1;
//				//8 -> dengJiShu
//				timeSub *= ( (float)brakeState / 8 );
//			}
//			else
//			{
//				if(Input.GetKey(KeyCode.C))
//				{
//					brakeState = 3;
//					timeSub *= 0.85f;
//					//timeSub *= (0.85f + 0.15f); // maxShaChe
//				}
//			}
			//brakeState = 0; //test

			if(brakeState != 0)
			{
				bIsPlayerBrakeBike = true;
				if(bIsWuDiState && bIsMoveUp)
				{
					rigObj.useGravity = false;
					rigObj.isKinematic = true;
					return;
				}
			}
			else
			{
				bIsPlayerBrakeBike = false;
				bIsSpacePlayer = false;
				mSpaceTime = 0f;
				//mSpaceSpeed = 0f;
			}
			
			if(throttle > 0f || mMouseDownCount > 10f)
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
					if(!bIsMoveUp) {
						currentBrakeEnginePower -= (10f * dTime * timeSub);
						if(currentEnginePower < 0) {
							currentEnginePower = 0;
						}
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

			if(throttle > 0F || mMouseDownCount > 0)
			{
				if(bIsAiNPC)
				{
					if(bIsStopAi)
					{
						if(mStopAiTime >= 3f)
						{
							throttle = 0f;
							return;
						}
						mStopAiTime += Time.deltaTime;
					}
					
					float markSpeed = 20f; //ai zhiNeng zuiDi suDu
					int playerRankNum = 7;
					float playerSpeed = 0f;
					bike bikeScript = null;
					if(mPlayerObject != null)
					{
						bikeScript = mPlayerObject.GetComponent<bike>();
						playerRankNum = bikeScript.mRankNum;
						playerSpeed = bikeScript.mSpeed;
					}

					float maxAiSpeed = 85f;
					switch(addAiSpeedState)
					{
					case 1:
						if(mRankNum < playerRankNum)
						{
							Invoke("resetAddAiSpeedState", 5f);
						}
						markSpeed = mAddAiSpeed;
						break;

					case -1:
						if(mRankNum > playerRankNum)
						{
							Invoke("resetAddAiSpeedState", 5f);
						}
						markSpeed = mAddAiSpeed;
						break;

					default:
						int markCount = mAiPathCtrl.childCount;
						if(mPathNode < markCount && bIsGameStart)
						{
							Transform mark = mAiPathCtrl.GetChild(mPathNode);
							if(mark == null)
							{
								return;
							}

							markSpeed = aiMarkSpeed[mPathNode];
						}
						
						if(mPlayerObject)
						{
							if(bikeScript)
							{
								playerSpeed = bikeScript.mSpeed;
								if(playerSpeed > markSpeed)
								{
									markSpeed = playerSpeed;
								}
							}
						}
						break;
					}

					if( mSpeed > markSpeed || mSpeed >= maxAiSpeed )
					{
						throttle = 0;
						currentEnginePower -= dTime * timeAdd;
					}
					else
					{
						currentEnginePower += dTime * timeAdd;
					}
				}
				else
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

						float velocityTmp = (mMouseDownCount / mMaxMouseDownCount) * mMaxVelocityFoot;
						if(throttle > 0F && mMouseDownCount > 0)
						{
							maxVelocity = velocityTmp > mMaxVelocity ? velocityTmp : mMaxVelocity;
						}
						else if(mMouseDownCount > 0)
						{
							maxVelocity = velocityTmp;
						}
						else if(throttle > 0F)
						{
							maxVelocity = mMaxVelocity;
						}
						
						if(maxVelocity > 80f)
						{
							maxVelocity = 80f;
						}
					}

					//ScreenLog.Log("mSpeed " + mSpeed + ", maxVelocity " + maxVelocity);
					if(mSpeed >= maxVelocity)
					{
						currentEnginePower -= dTime * timeAdd;
					}
					else
					{
						if(throttle > 0F)
						{
							currentEnginePower += (throttle + 50.0f ) * dTime * timeAdd;
							if(mSpeed < 40.0F && Time.frameCount % 10 == 0)
							{
								rigObj.AddForce(mBike.transform.forward * Time.deltaTime * 1000f, ForceMode.Acceleration);
							}
						}
						else
						{
							//currentEnginePower += dTime * timeAdd;
							currentEnginePower += dTime * pcvr.TanBanDownCount;
						}
					}
				}
			}
			else if (throttle == 0f)
			{
				if(bIsAiNPC)
				{
					int markCount = mAiPathCtrl.childCount;
					if(mPathNode >= markCount)
					{
						return;
					}
							
					float markSpeed = aiMarkSpeed[mPathNode];
					if(mSpeed < markSpeed && !bIsStopAi)
					{
						if(mGameTime > 0 && !bIsDoHurtAction)
						{
							throttle = 1f;
						}
						else
						{
							throttle = 0f;
						}
					}
				}
				else
				{
					if(currentEnginePower < 0)
					{
						currentEnginePower = 0;
					}
					else if(currentEnginePower > 0)
					{
						currentEnginePower -= 0.1f * dTime * timeSub;
						if(currentEnginePower < 0) {
							currentEnginePower = 0;
						}
					}
				}
			}
		}
		
		float maxPower = 1000f;
		if(bIsAiNPC)
		{
			maxPower = 2000f;
		}
		else
		{
			maxPower = 1200f;
		}

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

		if(bIsAiNPC && bIsStopAi)
		{
			throttle = 0.0f;
		}
			
		//mThrottleForce = power * rigidbody.mass;
		mThrottleForce = HidXKUnity_DLL.GetBikeThrottleForce(power, rigObj.mass);
		if(throttle <= 0.15f && mMouseDownCount <= 0)
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
			rigObj.centerOfMass = centerOfMass.localPosition;
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
			////ScreenLog.Log("dTime " + dTime);
			if(dTime < slowWorldTime)
			{
				return;
			}
		}

		Time.timeScale = 1f;
		isSlowWorld = false;
	}

	public static bool IsHitLuYan = false;
	public static bool IsHitJianSuDai = false;
	int JianSuDaiNum = 0;
	void ResetPlayerIntoJianSuDai()
	{
		//Debug.Log("over HandlePlayerIntoJianSuDai...");
		CancelInvoke("HandlePlayerIntoJianSuDai");
		bIsMoveUp = false;
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 60.0f, 10.0f);
		}
		JianSuDaiNum = 0;
		IsHitJianSuDai = false;
		pcvr.IsHitJianSuDai = false;
	}

	void HandlePlayerIntoJianSuDai()
	{
		if(!IsHitJianSuDai || JianSuDaiNum >= 12 || mGameTime <= 1)
		{
			ResetPlayerIntoJianSuDai();
			return;
		}

		JianSuDaiNum++;
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().OpenFangXiangPanZhenDong();
		}
		if(!bIsMoveUp) {
			bIsMoveUp = true;
			//ScreenLog.Log("HitJianSuDai ... move bike head up");
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, 60.0f, 10.0f);
			}
		}
//		else {
//			bIsMoveUp = false;
//			//ScreenLog.Log("HitJianSuDai ... move bike head down");
//			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.DOWN, 60.0f, 10.0f);
//		}
	}

	int luYanPosY = -1; //down state

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
			//ScreenLog.LogWarning(this.name + ": col is null!");
			yield break;
		}

		float delayTime = 0.1f;
		string colTag = col.tag;
		GameObject colObj = col.gameObject;
		string lay = LayerMask.LayerToName( col.gameObject.layer );
		////ScreenLog.Log(this.name + ": colTag is " + colTag);
		switch (colTag)
		{
		case "JianSuDai":
			if(bIsAiNPC)
			{
				yield break;
			}

			if(mGameTime <= 1)
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
					InvokeRepeating("HandlePlayerIntoJianSuDai", 0.04f, 0.25f);
				}
			}
			break;

		case "LuYan":
			if(bIsAiNPC)
			{
				yield break;
			}

			if(type == 1)
			{
				IsHitLuYan = false;
			}
			else if(!IsHitLuYan)
			{
				luYanPosY = -luYanPosY;
				IsHitLuYan = true;
				
				if(IsHitJianSuDai)
				{
					ResetPlayerIntoJianSuDai();
				}

				if(luYanPosY == -1)
				{
					if (pcvr.GetInstance() != null) {
						pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 60.0f, 0f);
					}
				}
				else
				{
					if (pcvr.GetInstance() != null) {
						pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, 60.0f, 10.0f);
					}
				}
			}
			break;

		case "slowWorld":
			if(!bIsAiNPC)
			{
				if(!isSlowWorld)
				{
					float timeSlow = 1f;
					gameCollider colScript = col.GetComponent<gameCollider>();
					if(colScript != null)
					{
						timeSlow = colScript.slowWorldTime;
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
				timeActionCool3 = 0f;
				isPlayCool = true;
				luYanPosY = -1;
			}
			break;

		case "mark":
			if(type == 0 && !bIsAiNPC && col.parent != mAiPathCtrl)
			{
				luYanPosY = -1;
				mAiPathCtrl = col.parent;
				AiMark markScript = col.GetComponent<AiMark>();
				mBakeTimePointPos = col.position;
				mBakeTimePointRot = col.right;
				
				mBikeAimMark = markScript.mNextMark;
			}
			break;

		case "cliff":
			if(!bIsAiNPC)
			{
				BikeCamera.mBikePlayer = null;
				GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
			}

			if(!bIsFall && !isCloneObject)
			{
				isCloneObject = true;
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState(1);
				}
				Invoke("cloneObject", 0.3f);
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
				if (!bIsAiNPC) {
					if (pcvr.GetInstance() != null) {
						pcvr.GetInstance().PlayerMoveOutTuLu();
					}
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
			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState(1);
				}
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;
				
				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}

				yield return new WaitForSeconds(delayTime);
				makeObjFly(col, BufferKind.Dianchi);
			}
			break;
			
		case "zhongDian":
			if(type == 0)
			{
				if(bIsAiNPC && bIsStopAi)
				{
					throttle = 0.0f;
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

				mRankingPlayer++;
				checkPlayerRank();

				if(!bIsAiNPC)
				{
					isIntoSuiDao = false;
					bIsStopAi = true;
					bIsGameOver = true;
					Rank.GetInstance().HiddenRankList();
					StartCoroutine( callPlayerRunEnd( mRankingPlayer ) );
				}
				else if(!bIsStopAi)
				{
					bIsStopAi = true;
					throttle = 0.0f;
				}
			}
			break;

		case "tennisBall":
			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState(1);
				}
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
				Destroy(colObj, 3f);
			}
			break;

		case "tennisBallAmmo":
			if(bIsAiNPC && colObj.activeSelf && transform == mLastAimNPC)
			{
				colObj.SetActive(false);
				Destroy(colObj);
				makeAiDoHurtAction();
			}
			break;

		case "timeBiao":
			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState(1);
				}
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;
				
				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}

				yield return new WaitForSeconds(delayTime);
				makeObjFly(col, BufferKind.Shoubiao);
			}
			break;

		case "timePoint":
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

		case "hamburger":
			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState(1);
				}
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;
				
				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}

				yield return new WaitForSeconds(delayTime);
				makeObjFly(col, BufferKind.Hanbao);
			}
			break;

		case "drumstick":
			if(!bIsAiNPC && colObj.collider.enabled && col.childCount > 0)
			{
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandlePlayerHitState(1);
				}
				luYanPosY = -1;
				colObj.animation.Stop();
				colObj.collider.enabled = false;

				Transform particleObj = col.GetChild(0);
				if(!particleObj.gameObject.activeSelf)
				{
					particleObj.gameObject.SetActive(true);
				}

				yield return new WaitForSeconds(delayTime);
				makeObjFly(col, BufferKind.Jitui);
			}
			break;

		case "baoGuo":
			if(!bIsAiNPC && colObj.collider.enabled)
			{
				colObj.collider.enabled = false;
				GlobalScript.GetInstance().ShowTishi( TishiInfo.Baoguo );

				Time.timeScale = 0;
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().setFengShanInfo(0, 0);
					pcvr.GetInstance().setFengShanInfo(0, 1);
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
			if(!bIsIntoFeiBan && (type == 0 || type == 2))
			{
				luYanPosY = -1;
				transform.forward = col.forward;
				bIsIntoFeiBan = true;
				if(checkHitScript != null)
				{
					checkHitScript.BoxCol.enabled = false;
				}

				if(!bIsAiNPC && mSpeed < 60.0f)
				{
					rigObj.AddForce(mBike.transform.forward * Time.deltaTime * 50000f, ForceMode.Acceleration);
				}
				CancelInvoke("ResetIsIntoFeiBan");
				Invoke("ResetIsIntoFeiBan", 6f);
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, mSpeed, 10.0f);
				}
			}
			break;
			
		case "xiaoFeiBan":
			if(!IsIntoXiaoFeiBan && (type == 0 || type == 2))
			{
				luYanPosY = -1;
				IsIntoXiaoFeiBan = true;
				//CancelInvoke("resetIsIntoXiaoFeiBan");
				if(!bIsAiNPC)
				{
					if(!GlobalScript.GetInstance().player.IsPass)
					{
						pcvr.IsIntoXiaoFeiBan = true;
						if(IsHitJianSuDai)
						{
							ResetPlayerIntoJianSuDai();
						}
						if (pcvr.GetInstance() != null) {
							pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, mSpeed, 10.0f);
						}
					}
				}
				Invoke("resetIsIntoXiaoFeiBan", 0.6f);
			}
			break;

		case "IntoSuidao":
			if(!bIsAiNPC)
			{
				isIntoSuiDao = true;
				BikeGameCtrl.SetPlayerLedCheTou(true, BikeAniScript.LedCheTou);
				if(mScriptCam == null)
				{
					mScriptCam = Camera.main.GetComponent<BikeCamera>();
				}
				mScriptCam.AudioZone.enabled = true;
				isPlayerIntoSuiDao = true;
			}
			else
			{
				isIntoSuiDaoNPC = true;
			}
			break;

		case "OutSuidao":
//			isOutSuiDao = true;
			if(!bIsAiNPC)
			{
				BikeGameCtrl.SetPlayerLedCheTou(false, BikeAniScript.LedCheTou);
				if(mScriptCam == null)
				{
					mScriptCam = Camera.main.GetComponent<BikeCamera>();
				}
				mScriptCam.AudioZone.enabled = false;
				//BikeLightObj.SetActive( false );
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
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 10.0f);
		}
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
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 0.0f);
		}
	}

	IEnumerator callPlayerRunEnd(int rankNum)
	{
		////ScreenLog.Log("player run to end point!");
		PlayAnimation(PlayerAniEnum.huanhu);
		GlobalScript.GetInstance().player.IsPlayHuanHu = true;
		yield return new WaitForSeconds( 2f );

		GlobalScript.GetInstance().player.IsPass = true;
		pcvr.ResetBikeZuLiInfo();
		GlobalScript.GetInstance().player.FinalRank = rankNum;
		if(IsHitJianSuDai)
		{
			ResetPlayerIntoJianSuDai();
		}
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 0.0f, 0.0f);
		}
		yield break;
	}

	void makeObjFly(Transform col, BufferKind kind)
	{
		GameObject colObj = col.gameObject;
		Transform parentCol = col.parent;
		if(mScriptCam != null)
		{
			parentCol.parent = mScriptCam.transform;
		}

		colObj.layer = LayerMask.NameToLayer("DaoJuFly");
		colObj.animation.Play("fly");
		
		if(mJiTuiScript != null)
		{
			mJiTuiScript.StartTransform(parentCol.gameObject, kind);
		}
	}

	void resetPlayerColliderInfo()
	{
		setPlayerColliderInfo(transform, true);
		if(transform.collider != null)
		{
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
		
		if(!isPlayerLayer && !bIsWuDiState)
		{
			mWuDiTimeCount = 0;
			bIsWuDiState = true;
			InvokeRepeating("changePlayerMesh", 0f, 0.05f);
		}

		BoxCollider colChild = null;
		BoxCollider [] colChilds = GetComponentsInChildren<BoxCollider>();
		int count = colChilds.Length;
		//ScreenLog.Log ("count ************** " + count);
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

	public void ResetRunActionInfo()
	{
		mRunState = -1;
	}

	void cloneObject()
	{
		if(mGameTime <= 0 && !bIsAiNPC)
		{
			return;
		}

		if(!gameObject.activeSelf)
		{
			return;
		}
		gameObject.SetActive(false);

		GameObject clone = null;
		if(mClonePrefab != null)
		{
			clone = (GameObject) Instantiate(mClonePrefab.gameObject);
			clone.name = mClonePrefab.name;
		}
		else
		{
			if(mBikeGameCtrl == null)
			{
				mBikeGameCtrl = GameObject.Find(GlobalData.bikeGameCtrl);
			}
			
			bikeGameScript = mBikeGameCtrl.GetComponent<BikeGameCtrl>();
			GameObject [] mClonePlayerGroup = bikeGameScript.mClonePlayerGroup;
			if(mClonePlayerGroup == null)
			{
				//ScreenLog.LogWarning("mClonePlayerGroup is null");
				return;
			}
			
			int count = mClonePlayerGroup.Length;
			Transform player = null;
			//Transform playerGroup = mClonePlayerGroup.transform;
			for(int i = 0; i < count; i++)
			{
				player = mClonePlayerGroup[ i ].transform;
				if(player.name == this.name)
				{
					mClonePrefab = player;
					clone = (GameObject) Instantiate(mClonePrefab.gameObject);
					clone.name = player.name;
				}
			}
		}

		if(clone == null)
		{
			//ScreenLog.Log(this.name + ":clone is null");
			return;
		}
		isCloneObject = true;
		clone.transform.position = mBakeTimePointPos;
		clone.transform.forward = mBakeTimePointRot;

		//ScreenLog.Log(this.name + "::cloneObject -> clone bike! mRankNum " + mRankNum);
		if(mPlayerName == this.name)
		{
			bIsAiNPC = false;
		}
		
		if(!bIsAiNPC)
		{
			if(IsInvoking("changePlayerMesh"))
			{
				CancelInvoke("changePlayerMesh");
			}
		}

		bike bikeScript = clone.GetComponent<bike>();
		mRankPlayer[mRankNum].player = clone;
		mRankPlayer[mRankNum].IsPlayer = !bIsAiNPC;

		if(!bIsAiNPC)
		{
			IsIntoXiaoFeiBan = false;
			Camera.main.transform.position = bikeScript.mCamPoint_back.position;
			bikeScript.Invoke("HandleCamDouDong", 0.5f);

			if(!GlobalScript.GetInstance().player.IsPass)
			{
				if(IsHitJianSuDai)
				{
					ResetPlayerIntoJianSuDai();
				}
				
				if (pcvr.GetInstance() != null) {
					pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, 0.0f, 0.0f);
				}
			}
		}

		//init info...
		bikeScript.mClonePrefab = mClonePrefab;
		bikeScript.mBakeTimePointPos = mBakeTimePointPos;
		bikeScript.mBakeTimePointRot = mBakeTimePointRot;
		bikeScript.bIsInitInfo = bIsInitInfo;
		bikeScript.bIsDoTimeOver = false;
		bikeScript.mRankNum = mRankNum;
		bikeScript.mPathNode = mPathNode;
		bikeScript.mBikeAimMark = mBikeAimMark;
		bikeScript.mAiPathCtrl = mAiPathCtrl;
		bikeScript.mAiPathMarkPos = mAiPathMarkPos;
		bikeScript.mBikePathCount = mBikePathCount;

		bikeScript.aiMarkSpeed = aiMarkSpeed;
		bikeScript.bIsAiNPC = bIsAiNPC;
		bikeScript.isIntoSuiDaoNPC = isIntoSuiDaoNPC;
//		bikeScript.isOutSuiDao = isOutSuiDao;
		bikeScript.isPlayerIntoSuiDao = isPlayerIntoSuiDao;
		bikeScript.mSpeed = 0f;

		if(!bIsAiNPC)
		{
//			IsStartQiBuZuLi = true;
//			if(pcvr.bIsHardWare)
//			{
//				bikeScript.StartCoroutine( pcvr.GetInstance().SetBikeZuLiInfo( QiBuZuLi ) );
//			}

			mMouseDownCount = 0;
			clone.layer = LayerMask.NameToLayer("Player");

			Time.timeScale = 1f;
			bike.mPlayerObject = clone.transform;
			if(wangQiuTran != null)
			{
				GlobalScript.GetInstance().player.wangQiuTran = bikeScript.wangQiuTran;
			}

			bikeScript.startPlayerCheckAi();
			FireScripts.wangQiuTranEvent( clone );
			GlobalScript.GetInstance().player.setAimPosition(Vector3.zero, false);
		}
		else if(!bIsGameOver)
		{
			GlobalScript.GetInstance().ChangeNPC();
		}
		bikeScript.StartInitInfo();
		
		if(!bIsAiNPC)
		{
			bikeScript.setPlayerColliderInfo(clone.transform, false);
			if(clone.transform.collider != null)
			{
				clone.layer = LayerMask.NameToLayer("wuDiLayer");
			}

			bikeScript.bIsWuDiState = true;
			bikeScript.Invoke("resetPlayerColliderInfo", 3.0f);
		}

		bikeScript.makeBikeFallPlane();

		if (mScriptCam != null && !bIsAiNPC)
		{
			//bikeScript.collider.isTrigger = false;
			BikeCamera.mBikePlayer = null;
			mScriptCam.setBikePlayer(clone);
		}

		if(mPlayer != null)
		{
			Destroy(mCamPoint_back.gameObject);
			Destroy(mPlayer, 1f);
		}
	}

	void makeBikeFallPlane()
	{
		rigObj.isKinematic = false;
		rigObj.useGravity = true;
	}

	void playerCheckAi()
	{
		if(mAiCtrlNum > 0)
		{
			//ScreenLog.Log("ai have not over! mAiCtrlNum " + mAiCtrlNum);
			return;
		}

		int rand = 0;
		int frontCountMax = Mathf.FloorToInt( mRankNum * 0.5f + 0.5f );
		int backCountMax = Mathf.FloorToInt( (7 - mRankNum) * 0.5f + 0.5f );
		int frontCount = 0;
		int backCount = 0;
		int [] randNum = {-1, -1, -1, -1};

		int count = 0;
		bool isContinue = false;
		while(true)
		{
			isContinue = false;
			if(frontCount < frontCountMax)
			{
				rand = Random.Range(0, mRankNum);
			}
			else
			{
				rand = Random.Range(mRankNum + 1, 8);
			}

			if(rand != mRankNum)
			{
				for(int i = 0; i < 4; i++)
				{
					if(randNum[i] == rand)
					{
						isContinue = true;
						break;
					}
				}

				if(isContinue)
				{
					continue;
				}

				randNum[count] = rand;
				if(frontCount < frontCountMax)
				{
					frontCount++;
				}
				else if(backCount < backCountMax)
				{
					backCount++;
				}

				count++;
				if(count > 2)
				{
					break;
				}
			}
		}
		mAiCtrlNum = count;
		//ScreenLog.Log(this.name + ", rankNum " + mRankNum);

		for(int i = 0; i < count; i++)
		{
			//ScreenLog.Log("mRankPlayer.name " + mRankPlayer[ randNum[i] ].Name + ", rankNum " + randNum[i]);
			if(randNum[i] > mRankNum)
			{
				changeAiBikeSpeed(mRankPlayer[ randNum[i] ].player, true);
			}
			else
			{
				changeAiBikeSpeed(mRankPlayer[ randNum[i] ].player, false);
			}
		}
	}

	void changeAiBikeSpeed(GameObject aiBike, bool isAddSpeed)
	{
		if(aiBike == null)
		{
			return;
		}

		bike bikeScript = aiBike.GetComponent<bike>();
		if(!bikeScript.bIsAiNPC)
		{
			return;
		}

		if(isAddSpeed)
		{
			bikeScript.addAiSpeedState = mAiAddSpeed;
			bikeScript.mAddAiSpeed = Random.Range(50f, 61f);
		}
		else
		{
			bikeScript.addAiSpeedState = mAiSubSpeed;
			bikeScript.mAddAiSpeed = Random.Range(20f, 38f);
		}
		//ScreenLog.Log(this.name + " -> mAddAiSpeed " + bikeScript.mAddAiSpeed);
	}

	//--------------------------------------------------------------------------------------
	//check npc position
	void startCheckAiPosition()
	{
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

		float dis = Vector3.Distance(transform.position, mBakeTimePointPos);
		if(dis < 3.8f)
		{
			startCheckAiPosition();
			return;
		}

		Vector3 posCur = transform.position;
		dis = Vector3.Distance(mPosOld, posCur);
		mPosOld = posCur;
		if((!bIsAiNPC && dis < 1.0f && mSpeed < 5.0f) || (bIsAiNPC && dis < 2.0f))
		{
			if(bIsAiNPC)
			{
				bIsFall = true;
				cloneObject();
			}
			else
			{
				if(checkHitScript != null && checkHitScript.GetIsHit())
				{
					bIsFall = true;
					cloneObject();
				}
			}
		}
	}

	public void setParticleState(bool isOpen)
	{
		if(ParticleObj == null) {
			return;
		}

		if (mSpeed < 15f && isOpen) {
			isOpen = false;
		}

		if((isOpen && ParticleObj.activeSelf) || (!isOpen && !ParticleObj.activeSelf)) {
			return;
		}

		//ScreenLog.Log("setParticleState -> isOpen " + isOpen);
		ParticleObj.SetActive(isOpen);
		if (bIsAiNPC) {
			return;
		}

		if (pcvr.GetInstance() == null) {
			return;
		}

		if(isOpen) {
			pcvr.GetInstance().PlayerMoveIntoTuLu();
		} else {
			pcvr.GetInstance().PlayerMoveOutTuLu();
		}
	}
	#endregion
}

public class playerRank
{
	public int rankNum = 0;
	public string Name;
	public bool IsPlayer;
	public bikeAiNetUnity AiNetScript;
	public bikeNetUnity NetScript;
	public Transform mBikeAimMark;
	public int mBikePathCount = 0;
	public int mBikePathKey;
	private GameObject _player;
	public GameObject player
	{
		get
		{
			return _player;
		}
		set
		{
			if(value == null)
			{
				return;
			}
			
			_player = value;
			Name = _player.name;
			
			if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
			{
				bike bikeScript = _player.GetComponent<bike>();
				if(bikeScript != null)
				{
					IsPlayer = !bikeScript.GetIsAiNPC();
				}
			}
			else
			{
				AiNetScript = _player.GetComponent<bikeAiNetUnity>();
				NetScript = _player.GetComponent<bikeNetUnity>();
			}
		}
	}
}