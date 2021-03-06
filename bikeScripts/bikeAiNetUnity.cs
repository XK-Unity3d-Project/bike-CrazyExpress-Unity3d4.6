
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using SLAB_HID_DEVICE;

public class bikeAiNetUnity : MonoBehaviour {

	public string AiNetName;
	#region #bike net data start
	public NetworkPlayer ownerPlayer;
	private int playerRotate;
	private Vector3 playerTransform;

	public static bool isNetworkCheck = false;
	#endregion #bike net data end
	
	#region #bike data start
	public GameObject TouYingObj;
	//private Projector TouYingPro;
	public Material TouYingMatZhong;
	public Material TouYingMatZuo;
	public Material TouYingMatYou;
	
	public GameObject FengXiaoObj;
	Transform kaCheTran;
	
	static public bool isIntoSuiDao = false;
	bool isIntoSuiDaoNPC = false;
	
	private GameObject playerSkinnedObj = null;
	
	public GameObject ParticleObj = null;
	public GameObject WaterParticleObj = null;
	
	private Vector3 mPosOld = Vector3.zero;
	private bool bIsGameOver = false;
	
	static private Transform mLastAimNPC = null;
	
	static public bool bIsHitFall = false;
	
	public Transform wangQiuTran = null;
	
	bool bIsIntoFeiBan = false;
	private bool IsIntoXiaoFeiBan = false;
	
	static public GameObject mBikeGameCtrl = null;
	private bool bIsRenderMat = true;
	
	private int mWuDiTimeCount = 0;
	private bool bIsWuDiState = false;

	private bool bIsDoHurtAction = false;
	private bool bIsInitInfo = false;
	
	public static bool bIsGameStart = false;
	public static bool bIsSelectPlayer = false;
	
	public static float MaxMouseDownCount = 9500f;
	public static float MinMouseDownCount = 0f;
	
	//float mMouseDownCount = 0;
	
	//private static int mGameTime = 1000;
	public Transform mBikeAimMark = null;
	//private int mBikePathCount = 0;

	static public int mRankCount = 0;
	static public  playerRank [] mRankPlayer = null;
	public Transform mBikePlayer = null;
	
	static public float mMaxVelocity = 60f; //km/h

//	public Vector3 bikePosOffset = Vector3.zero;
//	public Vector3 bikeAngOffset = Vector3.zero;
	
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
	
	//private float throttle = 0f;
	private float mStateSpeed = 1.0f;
	private float currentEnginePower = 0f;
	public bool bIsFall = false;
	
	float mSpeed = 0f;
	int _npcBikeSpeed = 0;
	public int npcBikeSpeed
	{
		get
		{
			return _npcBikeSpeed;
		}

		set
		{
			if(value != _npcBikeSpeed)
			{
				_npcBikeSpeed = value;
				//set npc action speed
				SetNpcActoinSpeed();
			}
		}
	}

	private bool isCloneObject = false;
	private int mRunState = -1;
	private int ROOT = 0;
	private const int STATE_RUN1 = 1; //run1 action
	private const int STATE_RUN2 = 2; //run2 action
	private const int STATE_RUN3 = 3; //run action
	private const int STATE_RUN4 = 4; //run3 action
	
	private bool bIsMixAction = false;
	private bool bIsDoFly = false;
	private bool bIsMoveUp = false;
	
	private float mThrottleForce = 0f;

	static public int mAiCtrlNum = 0;
	
	//private bool isPlayCool2 = false;
	//private bool isPlayCool3 = false;
	#endregion #end bike data
	
	#region #start AI
	
	private Transform mAiPathCtrl = null;
	
	static public Transform mPlayerObject = null;
	public bool bIsStopAi = false;
	
	private Vector3 [] mAiPathMarkPos = null;
	private float [] aiMarkSpeed = null;
	Transform NpcTran;
	#endregion #end AI
	
	#region #bike contrl
	
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
				
				int markMaxSpeed = 75; //ai zuiDa suDu = markSpeed * 1.5f
				int markMinSpeed = 40; //ai suiJi zuiDi sudu
				int randSpeed = Random.Range(markMinSpeed, markMaxSpeed);
				aiMarkSpeed[index] = randSpeed;
			}
		}
	}
	
	
	public GameObject getRankPlayer(int index)
	{
		return mRankPlayer[index].player;
	}

	// Use this for initialization
	void Start()
	{
		BikeAniScript = GetComponent<BikeAnimatorCtrl>();
		bIsStopAi = false;
//		if(AiNetName == "NPC_06NetUnity")
//		{
//			bikeAngOffset = new Vector3(0, 90, 0);
//		}
		StartInitInfo();
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
	}

	void StartInitInfo()
	{
		CancelInvoke("CheckAimPlayer");
		InvokeRepeating("CheckAimPlayer", 5.0f, 3.0f);

		gameObject.name = AiNetName;
		if(FreeModeCtrl.IsServer)
		{
			rigidbody.useGravity = true;
			rigidbody.isKinematic = false;
		}
		else
		{
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}

		//TouYingPro = TouYingObj.GetComponent<Projector>();
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

		NpcTran = transform;
		mSpeed = 0f;
		
		mPlayer = gameObject;
		setPlayerMixAction( gameObject );
		setPlayerChildRigibody( transform, false );

		SetCenterOfMass();
		startCheckAiPosition();
		
		int maxRankPlayer = 7;
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
			
			if(!FreeModeCtrl.IsServer)
			{
				RankHandleCtrl.GetInstance().SetRankPlayerArray(gameObject, false);
				mRankCount++;
			}
			
			if(mBakeTimePointPos == Vector3.zero && mAiPathCtrl != null)
			{
				if (mAiPathCtrl.childCount > 0)
				{
					Transform mark = mAiPathCtrl.GetChild(0);
					mBakeTimePointPos = mark.position;
					mBakeTimePointRot = mark.right;
					mBikeAimMark = mark;
					if(!FreeModeCtrl.IsServer && mBikeAimMark != null)
					{
						RankHandleCtrl.GetInstance().SetBikeAimMark(mBikeAimMark.parent.name,
						                                            GetAimMarkId(mBikeAimMark), gameObject.name);
					}
				}
			}
			initAiPlayerPathInfo();
		}
	}

	public bool GetIsStopMoveAi()
	{
		return bIsStopAi;
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
	
	void GetInput()
	{
		if(mSpeed > 1f)
		{
			if(mSpeed > 50f)
			{
				if( mRunState != STATE_RUN4 )
				{
					mRunState = STATE_RUN4;
					PlayAnimation(PlayerAniEnum.run3);
				}
			}
			else
			{
				if( mRunState != STATE_RUN2)
				{
					mRunState = STATE_RUN2;
					PlayAnimation(PlayerAniEnum.run2);
				}
			}
		}
		else
		{
			if(mRunState != ROOT)
			{
				mRunState = ROOT;
				PlayAnimation(PlayerAniEnum.root);
			}
		}
		return;
	}
	
	static float minAnimationSpeed = 0.25f;
	static float maxAnimationSpeed = 2.3f;
	static float keyAnimation = (maxAnimationSpeed - minAnimationSpeed) / 60f;
	void SetNpcActoinSpeed()
	{
		if(FreeModeCtrl.IsServer)
		{
			if(mRunState == STATE_RUN2 || mRunState == STATE_RUN4)
			{
				mStateSpeed = keyAnimation * npcBikeSpeed + minAnimationSpeed;
				
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
				case STATE_RUN2:
//					AnimationState AniRun2 = mPlayer.animation["run2"];
//					AniRun2.speed = mStateSpeed;
					break;
				case STATE_RUN4:
//					AnimationState AniRun4 = mPlayer.animation["run3"];
//					AniRun4.speed = mStateSpeed;
					break;
				}
				networkView.RPC("setServerBikeActionSpeed", RPCMode.OthersBuffered, mStateSpeed, mRunState);
			}
		}
	}
	
	[RPC]
	void setServerBikeActionSpeed( float actionSpeed, int ActionState )
	{
		if(FreeModeCtrl.IsServer)
		{
			return;
		}

		switch(ActionState)
		{
		case STATE_RUN2:
//			AnimationState AniRun2 = animation["run2"];
//			AniRun2.speed = actionSpeed;
			break;
		case STATE_RUN4:
//			AnimationState AniRun4 = animation["run3"];
//			AniRun4.speed = actionSpeed;
			break;
		}
	}

	[RPC]
	void SendAimMarkToClient()
	{

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
	
	[RPC]
	void SendAimMarkInfoToClient(string pathName, int idMark)
	{
		RankHandleCtrl.GetInstance().SetBikeAimMark(pathName, idMark, AiNetName);
	}

	[RPC]
	void SendPathCountToClient()
	{
		RankHandleCtrl.GetInstance().SetBikePathCount( AiNetName );
	}

	public int mPathNode = 0;
	//float mToEndPointAiTime = 0;
	bool bIsTurnAiBike = false;

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
		
		if(dis <= minDis)
		{
			bool isNextPath = false;
			int markCount = mAiPathCtrl.childCount;
			int endCount = markCount - 1;

			Transform aimMark = mAiPathCtrl.GetChild(mPathNode);
			AiMark markScript = aimMark.GetComponent<AiMark>();
			mBikeAimMark = markScript.mNextMark;
			if(FreeModeCtrl.IsServer && mBikeAimMark != null)
			{
				networkView.RPC ("SendAimMarkInfoToClient", RPCMode.OthersBuffered, mBikeAimMark.parent.name, GetAimMarkId(mBikeAimMark));
			}

			mBakeTimePointPos = aimMark.position;
			mBakeTimePointRot = aimMark.right;
			
			if(mPathNode >= endCount)
			{
				networkView.RPC("SendPathCountToClient", RPCMode.OthersBuffered);
			}

			if(mPathNode >= endCount && !bIsStopAi)
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

						initAiPlayerPathInfo();
						isNextPath = true;
					}
				}
			}
			
			if(mPathNode < endCount && !isNextPath)
			{
				mPathNode++;
			}
			return false;
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

	public bool getIsDoFly()
	{
		return bIsDoFly;
	}
	
	public bool getIsPlayCool()
	{
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
	void ResetTouYing()
	{
		TouYingObj.SetActive(true);
	}
	
	void StopPlayFallAction()
	{
		networkView.RPC("ResetPlayFallAction", RPCMode.OthersBuffered);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(bIsStopAi)
		{
			return;
		}
		
		GameObject obj = other.gameObject;
		string tagObj = obj.tag;
		if(!FreeModeCtrl.IsServer)
		{
			if(tagObj == "zhongDian" && isIntoSuiDaoNPC)
			{
				bIsStopAi = true;
			}
			else if(tagObj == "IntoSuidao")
			{
				isIntoSuiDaoNPC = true;
			}
			return;
		}

		if(bIsFall)
		{
			return;
		}

		string lay = LayerMask.LayerToName( obj.layer );
		if(lay == "qiche")
		{
			TouYingObj.SetActive(false);
			Invoke("ResetTouYing", 2.0f);
			networkView.RPC("makeBikeFall", RPCMode.AllBuffered);
			return;
		}

		if(tagObj == "rock")
		{
			TouYingObj.SetActive(false);
			Invoke("ResetTouYing", 2.0f);
			networkView.RPC("makeBikeFall", RPCMode.AllBuffered);
			return;
		}

		if (lay == "Player")
		{	
			float rand = Random.Range(0f, 100f);
			if(rand < 70)
			{
				TouYingObj.SetActive(false);
				Invoke("ResetTouYing", 2.0f);
				networkView.RPC("makeBikeFall", RPCMode.AllBuffered);
			}
			return;
		}

		StartCoroutine(OnCollisionObject( obj.transform, other, 0));
		return;
	}
	
	void OnTriggerExit(Collider other)
	{
		if(bIsStopAi)
		{
			return;
		}

		if(!FreeModeCtrl.IsServer)
		{
			return;
		}
		GameObject obj = other.gameObject;
		StartCoroutine(OnCollisionObject( obj.transform, other, 1));
	}
	
	public LayerMask TerrainLayer;
	public LayerMask DiMainSmokeLayer;
	
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

	static public void setIsGameStart(bool isGameStart)
	{
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

		if(GlobalScript.GetInstance().player.IsPass)
		{
			return;
		}

//		if(!GlobalScript.GetInstance().player.bIsGameStart)
//		{
//			return;
//		}
//		else
//		{
//			if(!isSetStartGame)
//			{
//				isSetStartGame = true;
//				networkView.RPC("SendToSetIsGameStart", RPCMode.OthersBuffered);
//			}
//		}
		
//		if(mGameTime <= 0 || bIsFall)
//		{
//			return;
//		}
		
//		if( checkBikeState( 0 ) )
//		{
//			return;
//		}

		if(bikeNetUnity.mGameTime <= 0)
		{
			return;
		}

		if(!GlobalScript.GetInstance().player.bIsGameStart)
		{
			return;
		}
		
		if(isStopMoveNPC)
		{
			return;
		}

		if(!FreeModeCtrl.IsServer)
		{
			if(mPlayer == null)
			{
				mPlayer = gameObject;
			}
			checkObjTransform();
			return;
		}

		GetInput();

		CalculateEnginePower();
		if(IsIntoXiaoFeiBan && mSpeed < 75)
		{
			rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 2400f, ForceMode.Acceleration);
		}

		if(mSpeed < 10f)
		{
			rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 1000f, ForceMode.Acceleration);
		}
		ApplyThrottle();
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

	public void RemovePlayer()
	{
		if(!FreeModeCtrl.IsServer)
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
		if(PlayerCreatNet.IsDisconnected)
		{
			RemovePlayer();
			return;
		}

		if(GlobalScript.GetInstance().player.IsPass)
		{
			return;
		}

		if(isStopMoveNPC)
		{
			return;
		}

		float dTime = Time.realtimeSinceStartup - lastTimeRender;
		if(dTime < 0.03f)
		{
			return;
		}
		lastTimeRender = Time.realtimeSinceStartup;
		
//		if(GlobalScript.GetInstance().player != null)
//		{
//			mGameTime = GlobalScript.GetInstance().player.Life;
//			mGameTime = 100;
//		}
		//mGameTime = 100; //test

		if(FreeModeCtrl.IsServer)
		{
			if(!bIsStopAi)
			{
				checkAiForwardHit();
			}

			checkBikeSpeed();
			checkIsMoveNPC();
			networkView.RPC("sendTranInfoToClient", RPCMode.OthersBuffered, NpcTran.position, NpcTran.rotation);
		}
		moveBikeWheels();
	}
	
	void resetIsDoHurtAction()
	{
		bIsDoHurtAction = false;
	}
//	
//	[RPC]
//	void SendMakeBikeFall()
//	{
//		if(Network.isServer)
//		{
//			return;
//		}
//		
//		if(bIsFall)
//		{
//			return;
//		}
//		makeBikeFall();
//	}
	
	public bool checkBikeState( int key )
	{
		if(!FreeModeCtrl.IsServer)
		{
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
		
		if (isFall || bIsHitFall)
		{
			bIsFall = true;
			if(!isCloneObject)
			{
				isCloneObject = true;
				Invoke("cloneObject", 0.5f);
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
		rigidbody.useGravity = true;
		rigidbody.isKinematic = false;
		rigidbody.freezeRotation = false;

		rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * 10000f, ForceMode.Acceleration);
//		animation.enabled = false;

		mBikePlayer.gameObject.SetActive(true);
		int key = Random.Range(0, 100) % 4 - 1;
		mBikePlayer.rigidbody.AddForce(key * mBike.transform.right * Time.deltaTime * 30000f, ForceMode.Acceleration);

		Invoke("ResetBikePlayFallAction", 1.0f);
	}

	void ResetBikePlayFallAction()
	{
		if(!FreeModeCtrl.IsServer)
		{
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}
		rigidbody.freezeRotation = true;
		
		mBikePlayer.gameObject.SetActive(false);
//		animation.enabled = true;
	}

	[RPC]
	void ResetPlayFallAction()
	{
		if(FreeModeCtrl.IsServer)
		{
			return;
		}
//		animation.enabled = true;
		mBikePlayer.gameObject.SetActive(false);
		bIsFall = false;
	}
	
	[RPC]
	void makeBikeFall()
	{
		BikePlayFallAction();
		checkBikeState( (int)CheckBikeKey.HIT_CAR );
	}
	
	void OnBikeCollisionCliff()
	{
		checkBikeState( (int)CheckBikeKey.HIT_CLIFF );
	}
	
	void setPlayerChildRigibody(Transform tran, bool isRagdoll)
	{
		mBikePlayer.gameObject.SetActive(isRagdoll);
//		if(!isRagdoll)
//		{
//			return;
//		}
//		
//		Rigidbody [] rigids = tran.GetComponentsInChildren<Rigidbody>();
//		int count = rigids.Length;
//		for(int index = 1; index < count; index++)
//		{
//			rigids[index].transform.collider.enabled = true;
//			rigids[index].mass = 70f;
//			rigids[index].useGravity = true;
//			rigids[index].isKinematic = false;
//			rigids[index].detectCollisions = true;
//		}
	}
	
	void ApplyThrottle()
	{
		rigidbody.AddForce(mBike.transform.forward * Time.deltaTime * mThrottleForce);
	}

	void MakeNpcDoAction()
	{
		if(mSpeed > 0)
		{
			if(mSpeed > 50f)
			{
				if( mRunState != STATE_RUN4  || !GetNpcAniBool(PlayerAniEnum.run3) )
				{
					mRunState = STATE_RUN4;
					PlayAnimation(PlayerAniEnum.run3);
				}
			}
			else
			{
				if( mRunState != STATE_RUN2 || !GetNpcAniBool(PlayerAniEnum.run2) )
				{
					mRunState = STATE_RUN2;
					PlayAnimation(PlayerAniEnum.run2);
				}
			}
		}
	}

	[RPC]
	void SendSpeedToServer(float val)
	{
		//ScreenLog.Log("val " + val);
		mSpeed = val;
		MakeNpcDoAction();
	}
	
	void checkBikeSpeed()
	{
		mSpeed = HidXKUnity_DLL.GetBikeSpeed( rigidbody.velocity.magnitude );
		npcBikeSpeed = (int)mSpeed;
		MakeNpcDoAction();
		networkView.RPC("SendSpeedToServer", RPCMode.OthersBuffered, mSpeed);
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

	bool isAimPlayer_04;
	bool isAimPlayer_07;
	bikeNetUnity playerScript_04;
	bikeNetUnity playerScript_07;
	float mAimPlayerTime = 0;

	bool CheckIsAimPlayer()
	{
		if(mAimPlayer == null)
		{
			return false;
		}

		mAimPlayerTime += Time.deltaTime;
		if(mAimPlayerTime >= 8f)
		{
			return false;
		}

		Vector3 vecA = gameObject.transform.position;
		Vector3 vecB = mAimPlayer.transform.position;
		vecA.y = 0;
		vecB.y = 0;
		float dis = Vector3.Distance(vecA, vecB);
		if(dis <= 3)
		{
			return false;
		}
		return true;
	}
	
	void resetAddAiSpeedState()
	{
		mAimPlayer = null;
		addAiSpeedState = 0;
	}

	//-------------------------------------------------------------------------
	void CalculateEnginePower()
	{
		if(!FreeModeCtrl.IsServer)
		{
			return;
		}
		
		float timeAdd = 200f;
		//float timeSub = 10f * timeAdd;
		float dTime = Time.deltaTime;
		float markSpeed = 20f; //ai zhiNeng zuiDi suDu
		switch(addAiSpeedState)
		{
		case 1:
			if(mAimPlayer != null && !CheckIsAimPlayer())
			{
				mAimPlayer = null;
				Invoke("resetAddAiSpeedState", 5f);
			}
			markSpeed = mAddAiSpeed;
			break;
			
		case -1:
			if(mAimPlayer != null && !CheckIsAimPlayer())
			{
				mAimPlayer = null;
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
			break;
		}
		//markSpeed = 30; //test
		
		if( mSpeed > markSpeed || mSpeed >= 100 )
		{
			currentEnginePower -= dTime * timeAdd;
		}
		else
		{
			currentEnginePower += dTime * timeAdd;
		}

		float maxPower = 1200f;
		if(currentEnginePower > maxPower)
		{
			currentEnginePower = maxPower;
		}

		float power = HidXKUnity_DLL.GetBikePower(currentEnginePower, 0f, 0f);
		mThrottleForce = HidXKUnity_DLL.GetBikeThrottleForce(power, rigidbody.mass);
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
		
		networkView.RPC("makeBikeFall", RPCMode.AllBuffered);
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
	
	
	GameObject eventCamObj;
	BikeCamEvent camEvent;
	public static bool IsHitJianSuDai = false;
	public static bool IsHitLuYan = false;
	int JianSuDaiNum = 0;
	
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
		if(!bIsMoveUp)
		{
			bIsMoveUp = true;
			//ScreenLog.Log("HitJianSuDai ... move bike head up");
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.UP, 60.0f, 10.0f);
			}
		}
		else
		{
			bIsMoveUp = false;
			//ScreenLog.Log("HitJianSuDai ... move bike head down");
			if (pcvr.GetInstance() != null) {
				pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.DOWN, 60.0f, 10.0f);
			}
		}
	}

	bool isStopMoveNPC = false;
	void StopMoveNPC()
	{
		if(isStopMoveNPC)
		{
			return;
		}
		isStopMoveNPC = true;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		mRunState = ROOT;
		PlayAnimation(PlayerAniEnum.root);

		networkView.RPC("SendStopMoveNPCToClient", RPCMode.OthersBuffered);
	}

	[RPC]
	void SendStopMoveNPCToClient()
	{
		if(isStopMoveNPC)
		{
			return;
		}
		isStopMoveNPC = true;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		mRunState = ROOT;
		PlayAnimation(PlayerAniEnum.root);
	}

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
		switch (colTag)
		{	
		case "cliff":
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

		case "zhongDian":
			if(!FreeModeCtrl.IsServer)
			{
				yield break;
			}

			if(bIsStopAi)
			{
				yield break;
			}

			if(!isIntoSuiDaoNPC)
			{
				yield break;
			}
			bIsStopAi = true;
			float timeVal = Random.Range(0.5f, 3f);
			Invoke("StopMoveNPC", timeVal);
			break;

		case "tennisBallAmmo":
			if(!FreeModeCtrl.IsServer)
			{
				yield break;
			}
			
			wangQiuAmmoNet wangQiuAmmoNetScript = colObj.GetComponent<wangQiuAmmoNet>();
			if(colObj.activeSelf && wangQiuAmmoNetScript.firePlayerName != gameObject.name)
			{
				OtherClientMakeBikeFall();
			}
			break;

		case "IntoSuidao":
//			if(!FreeModeCtrl.IsServer)
//			{
//				yield break;
//			}

			isIntoSuiDaoNPC = true;
			break;

		case "xiaoFeiBan":
			if(!FreeModeCtrl.IsServer)
			{
				yield break;
			}

			if(!IsIntoXiaoFeiBan && (type == 0 || type == 2))
			{
				IsIntoXiaoFeiBan = true;
				Invoke("resetIsIntoXiaoFeiBan", 0.6f);
			}
			break;
		}
	}

	public bool GetIsIntoXiaoFeiBan()
	{
		return IsIntoXiaoFeiBan;
	}
	
	void resetIsIntoXiaoFeiBan()
	{
		IsIntoXiaoFeiBan = false;
		if (pcvr.GetInstance() != null) {
			pcvr.GetInstance().HandleBikeHeadQiFu(BikeHeadMoveState.PLANE, mSpeed, 0.0f);
		}
	}
	
	void makeObjFly(Transform col, BufferKind kind)
	{
//		Transform parentCol = col.parent;
//		if(mScriptCam != null)
//		{
//			parentCol.parent = mScriptCam.transform;
//		}
//		
//		DaoJuNetCtrl DaoJuNetCtrlScript = parentCol.GetComponent<DaoJuNetCtrl>();
//		if(DaoJuNetCtrlScript != null)
//		{
//			DaoJuNetCtrlScript.closeMeshRender();
//		}
//		
//		if(mJiTuiScript == null)
//		{
//			mScriptCam = Camera.main.gameObject.GetComponent<BikeCameraNet>();
//			if(mScriptCam != null)
//			{
//				if(mScriptCam.mDaoJuCam != null)
//				{
//					mJiTuiScript = mScriptCam.mDaoJuCam.GetComponent<JiTui>();
//				}
//			}
//		}
//		
//		if(mJiTuiScript != null)
//		{
//			//ScreenLog.Log("test *********** mJiTuiScript.StartTransform");
//			mJiTuiScript.StartTransform(parentCol.gameObject, kind);
//		}
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
		if(!FreeModeCtrl.IsServer) {
			return;
		}
		
		bIsFall = false;
		isCloneObject = false;
		currentEnginePower = 0f;
		
		transform.position = mBakeTimePointPos;
		transform.forward = mBakeTimePointRot;
		Vector3 vecAng = transform.eulerAngles;
		vecAng.z = 0;
		transform.eulerAngles = vecAng;

		networkView.RPC("sendTranInfoToOther", RPCMode.OthersBuffered, transform.position, transform.rotation);
		
		mSpeed = 0f;
		bIsIntoFeiBan = false;
		transform.collider.enabled = true;

		StartInitInfo();
	}
	
	//--------------------------------------------------------------------------------------
	//check npc position
	void startCheckAiPosition()
	{
		if(!FreeModeCtrl.IsServer)
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
			ParticleObj.SetActive(false);
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
		}
		else
		{
			ParticleObj.SetActive(false);
		}
	}

	float mAddAiSpeed;
	short mAiAddSpeed = 1;
	short mAiSubSpeed = -1;
	private short addAiSpeedState = 0;
	GameObject mAimPlayer;
	//bikeNetUnity aimPlayerScript;
	GameObject PlayerObj_04;
	GameObject PlayerObj_07;

	void CheckAimPlayer()
	{
		if(FreeModeCtrl.IsServer)
		{
			return;
		}

		if(GlobalScript.GetInstance().player.IsPass)
		{
			//Destroy(gameObject);
			return;
		}

		if(PlayerObj_04 == null)
		{
			PlayerObj_04 = GameObject.Find("NPC_04NetUnity");
			if(PlayerObj_04 != null)
			{
				playerScript_04 = PlayerObj_04.GetComponent<bikeNetUnity>();
			}
		}

		if(PlayerObj_07 == null)
		{
			PlayerObj_07 = GameObject.Find("NPC_07NetUnity");
			if(PlayerObj_07 != null)
			{
				playerScript_07 = PlayerObj_07.GetComponent<bikeNetUnity>();
			}
		}

		if(PlayerObj_04 == null && PlayerObj_07 == null)
		{
			return;
		}

		bool isAimPlayer_04 = false;
//		int rankNum = 0;
//		int rankNum_04 = 0;
//		int rankNum_07 = 0;

		int rv = Random.Range(0, 100);
		if(rv < 50 && PlayerObj_04 != null && playerScript_04 != null)
		{
			isAimPlayer_04 = true;
		}

//		int max = RankHandleCtrl.mRankPlayer.Length;
//		string npcName = gameObject.name;
//		for(int i = 0; i < max; i++)
//		{
//			switch(RankHandleCtrl.mRankPlayer[i].Name)
//			{
//			case "NPC_04NetUnity":
//				rankNum_04 = i;
//				break;
//			case "NPC_07NetUnity":
//				rankNum_07 = i;
//				break;
//			default:
//				if(RankHandleCtrl.mRankPlayer[i].Name == npcName)
//				{
//					rankNum = i;
//				}
//				break;
//			}
//		}

		if(isAimPlayer_04)
		{
			if(playerScript_04 != null)
			{
				playerScript_04.SendAimInfoToServer(AiNetName, true);
			}
			else if(playerScript_07 != null)
			{
				playerScript_07.SendAimInfoToServer(AiNetName, true);
			}
		}
		else
		{
			if(playerScript_07 != null)
			{
				playerScript_07.SendAimInfoToServer(AiNetName, true);
			}
			else if(playerScript_04 != null)
			{
				playerScript_04.SendAimInfoToServer(AiNetName, true);
			}
		}
	}

	[RPC]
	public void SendAimPlayerToServer(string objName, int stateAddSpeed)
	{
		if(!FreeModeCtrl.IsServer)
		{
			return;
		}

		if(mAimPlayer != null)
		{
			return;
		}

		GameObject aimObj = GameObject.Find( objName );
		changeAiBikeSpeed(aimObj, stateAddSpeed);
	}

	void changeAiBikeSpeed(GameObject aimPlayer, int stateAddSpeed)
	{
		if(mAimPlayer != null)
		{
			return;
		}

		mAimPlayerTime = 0f;
		mAimPlayer = aimPlayer;
		//aimPlayerScript = aimPlayer.GetComponent<bikeNetUnity>();

		if(stateAddSpeed == 1)
		{
			addAiSpeedState = mAiAddSpeed;
			mAddAiSpeed = Random.Range(60f, 75f);
		}
		else
		{
			addAiSpeedState = mAiSubSpeed;
			mAddAiSpeed = Random.Range(20f, 40f);
		}
//		Debug.Log("changeAiBikeSpeed -> name "+gameObject.name+", stateAddSpeed " + stateAddSpeed
//		          +", mAddAiSpeed "+mAddAiSpeed);
	}

	public Transform mAiRayStartTran = null;
	private bool bIsTurnLeftAiBike = false;
	private bool bIsTurnRightAiBike = false;
	BoxCollider boxColBike;
	void checkAiForwardHit()
	{
		RaycastHit hit;
		Vector3 startPos = transform.position;
		if(boxColBike == null)
		{
			boxColBike = GetComponent<BoxCollider>();
		}

		if(boxColBike)
		{
			startPos = boxColBike.center;
			startPos.z += (boxColBike.size.z - 0.1f);
			startPos.x += (boxColBike.size.x - 0.05f) * getRandPer(100) * Random.Range(-1, 2);
			startPos.y += (boxColBike.size.y - 0.05f) * getRandPer(100) * Random.Range(-1, 2);
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
//				case "feiBan":
//					isHandleTurn = true;
//					break;
					
				case "car":
					isHandleTurn = true;
					break;
					
				case "Player":
					isHandleTurn = true;
					break;
				}
			}
			
			if(!bIsTurnAiBike && isHandleTurn)
			{
				if(!isTurnRight)
				{
					bIsTurnLeftAiBike = true;
					bIsTurnRightAiBike = false;
				}
				else
				{
					bIsTurnLeftAiBike = false;
					bIsTurnRightAiBike = true;
				}
				
				bIsTurnAiBike = true;
				makeAiTurn();
			}
		}
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
		bIsTurnAiBike = false;
	}
	#endregion

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
	void sendTranInfoToClient(Vector3 pos, Quaternion rot)
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

	public void SetAimMark(Transform aimMarkTmp)
	{
		mBikeAimMark = aimMarkTmp;
	}

	public void checkObjTransform()
	{
		if(correctPlayerPos == Vector3.zero)
		{
			return;
		}
		
		float dis = Vector3.Distance(transform.position, correctPlayerPos);
		float angleDt = Vector3.Distance(transform.rotation.eulerAngles, correctPlayerRot.eulerAngles);
		if(dis < 0.3f && angleDt < 0.5f)
		{
			return;
		}
		
		if(dis > 100f)
		{
			transform.position = correctPlayerPos;
			transform.rotation = correctPlayerRot;
			return;
		}
		transform.position = Vector3.Lerp(transform.position, correctPlayerPos, 0.1f);
		transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, 1);
	}

	BikeAnimatorCtrl BikeAniScript;
	void PlayAnimation(PlayerAniEnum ani)
	{
		if (BikeAniScript == null) {
			return;
		}
		//Debug.Log(gameObject.name+"::PlayAnimation -> action "+ani);
		BikeAniScript.PlayAnimation(ani, 1f);
	}

	bool GetNpcAniBool(PlayerAniEnum ani)
	{
		if (BikeAniScript == null) {
			return false;
		}
		return BikeAniScript.GetAniBool(ani);
	}
}