
using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public enum BikeCamEvent : int
{
	Null = 0,
	JingZhi = 1,
	HangPai,
	JingZhiMiaoZhun,
	MiaoZhunGenSui,
	TeDingDongZuo,
	GameOver
}

public class BikeCameraNet : MonoBehaviour
{
	public AudioReverbZone AudioZone = null;
	public GameObject RainObj = null;
	private bool isPlayRain = false;
	
	public GameObject MusicUiObj = null;
	
	public BlurEffect BlurEffectScript = null;
	public int MinBlurIter = 0;
	public int MaxBlurIter = 3;
	public float ChangeBlurTime = 0.5f;
	
	//private float blurTime = 0f;
	//private float blurIter = 0f;
	//private float speed = 0f;
	
	//private float smoothValTmp = 0f;
	static private float smoothPer = 1f;
	//private float minSmoothPer = 0.01f;
	
	//static private bool isClone = false;
	
	public AudioClip [] bakeAudio;
	public AudioClip BigFeiBanAudio;
	
	public Transform mDaoJuCam = null;
	GameObject mBike = null;
	
	public MotionBlur mMotionBlur = null;
	
	public Vector3 mOffsetPos = new Vector3(0f, 4f, -8f);
	
	static public GameObject mBikePlayer = null;
	static public bool bIsAimPlayer = false;
	
	private float mSmooth = 15f;
	private float smoothVal = 0f;
	private float followSpeed = 0.01f;
	
	//private Transform mCamPoint_backTmp = null;
	private Transform mAimPoint = null;
	private Transform mCamPoint_back = null; //back
	private Transform mCamPoint_forward = null; //forward
	private Transform mCamPoint_left = null; //left
	//private Transform mCamPoint_right = null; //right
	//private Transform mCamPoint_piaoYi = null; //right

	private Transform []HangPaiTrans;

	//private Transform []JingZhiMiaoZhun;

	int mzgsIndex = 0;
	private Transform []MiaoZhunGenSui;
	
	private Transform backPointParent;
	Vector3 backLocalPos;

	private Transform mCamPoint_free = null;
	
	static private Transform mCamTran = null;
	
	bikeNetUnity bikeScript = null;
	//float TestBikeSpeed = 0f;
	private float mFollowFallBikeTime = 0f;
	public AudioSource AudioSourceObj = null;
	
	//	private Vector3 mVecBikeToCam = Vector3.zero;
	
	//static public bool isChangeCamPos = false;
	static public bool IsIntoFeiBan = false;
	enum CamDir
	{
		FORWARD,
		BACK,
		LEFT,
		RIGHT,
		FREE
	}
	private CamDir camDir = CamDir.BACK;
	//private CamDir camDirOld = CamDir.FREE;
	
	private GameObject _mClonePlayerGroup = null;
	public GameObject mClonePlayerGroup
	{
		get
		{
			return _mClonePlayerGroup;
		}
		set
		{
			_mClonePlayerGroup = value;
		}
	}
	
	//private bool bIsFollow = true;
	//private bool bIsToCamP0 = false;
	
	public Transform getBikePlayerTran()
	{
		if(mBikePlayer != null)
		{
			return mBikePlayer.transform;
		}
		else
		{
			return null;
		}
	}

	BikeCamEvent camPosEvent;

	public void setBikeCamPosEvent(BikeCamEvent camEvent, CamPosEvent eventScript)
	{
		switch(camEvent)
		{
		case BikeCamEvent.JingZhi:
			break;

		case BikeCamEvent.HangPai:
			if((int)camPosEvent != (int)BikeCamEvent.HangPai)
			{
				//ScreenLog.Log("setBikeCamPosEvent -> set hangPai, camPosEvent " + camPosEvent
				             // + ", BikeCamEvent.HangPai " + BikeCamEvent.HangPai);

				camPosEvent = BikeCamEvent.HangPai;
				mzgsIndex = Random.Range(0, HangPaiTrans.Length - 1);
				//ScreenLog.Log("setBikeCamPosEvent -> set hangPai index = " + mzgsIndex);

				transform.position = HangPaiTrans[mzgsIndex].position;
				transform.LookAt(mAimPoint);
			}
			break;

		case BikeCamEvent.JingZhiMiaoZhun:
//			if(camPosEvent != BikeCamEvent.JingZhiMiaoZhun)
//			{
//				mzgsIndex = Random.Range(0, JingZhiMiaoZhun.Length - 1);
//				transform.position = JingZhiMiaoZhun[mzgsIndex].position;
//				transform.LookAt(mAimPoint);
//			}
			break;

		case BikeCamEvent.MiaoZhunGenSui:
			if(camPosEvent != BikeCamEvent.MiaoZhunGenSui)
			{
				//mzgsIndex = Random.Range(0, MiaoZhunGenSui.Length - 1);
				mzgsIndex = (int)eventScript.MiaoZhunGenSuiNum;
				if(mzgsIndex == (int)MiaoZhunGenSuiCtrl.THIRD_PERSON)
				{
					transform.position = mCamPoint_back.position;
				}
				else
				{
					transform.position = MiaoZhunGenSui[mzgsIndex].position;
				}
				transform.LookAt(mAimPoint);
			}
			break;

		case BikeCamEvent.TeDingDongZuo:
			break;

		case BikeCamEvent.GameOver:
			if(camPosEvent != BikeCamEvent.MiaoZhunGenSui)
			{
				mzgsIndex = Random.Range(0, MiaoZhunGenSui.Length - 1);
				transform.position = MiaoZhunGenSui[mzgsIndex].position;
				transform.LookAt(mAimPoint);
			}
			break;

		default:
			break;
		}
		camPosEvent = camEvent;
	}

	[RPC]
	public void setBikePlayer(GameObject bikePlayer)
	{
		if(bikePlayer != null)// && mBikePlayer == null)
		{
			if(mBikePlayer != null && mBikePlayer.name == bikePlayer.name )
			{
				return;
			}
			//ScreenLog.Log("set camera aim to " + bikePlayer.name);
			
			Map.Player = bikePlayer;
			mBikePlayer = bikePlayer;
			
			bikeScript = mBikePlayer.GetComponent<bikeNetUnity>();
			if(bikeScript == null)
			{
				Debug.LogWarning("BikeCamera::Start -> bikeScript is null!");
			}
			else
			{
				//ScreenLog.Log("setBikePlayer -> init");
				smoothPer = 1f;
				//isClone = true;
				mAimPoint = bikeScript.mAimPoint;
				mCamPoint_back = bikeScript.mCamPoint_back;
				//mCamPoint_forward = bikeScript.mCamPoint_forward;
				mCamPoint_left = bikeScript.mCamPoint_left;
				//mCamPoint_right = bikeScript.mCamPoint_right;
				//mCamPoint_piaoYi = bikeScript.mCamPoint_piaoYi;
				backPointParent = mCamPoint_back.parent;
				backLocalPos = mCamPoint_back.localPosition;

				HangPaiTrans = bikeScript.HangPaiTrans;
				//JingZhiMiaoZhun = bikeScript.JingZhiMiaoZhun;
				MiaoZhunGenSui = bikeScript.MiaoZhunGenSui;

				camPosEvent = BikeCamEvent.Null;
			}
		}
	}

	void playBakeAudio()
	{
		if(NetworkServerScript.GetIsServer())
		{
			return;
		}

		if(bakeAudio.Length <= 0)
		{
			return;
		}
		
		int indexAudio = Random.Range(0, bakeAudio.Length);
		AudioSourceObj.clip = bakeAudio[indexAudio];
		AudioSourceObj.volume = 0.5f;
		AudioSourceObj.Play();
	}
	
	IEnumerator checkBakeAudio()
	{
		if(NetworkServerScript.GetIsServer())
		{
			yield break;
		}

		if(bakeAudio.Length <= 0)
		{
			yield break;
		}

		if(!Go.IsStartGame)
		{
			yield return new WaitForSeconds(1.0f);
			yield return checkBakeAudio();
		}

		if(!AudioSourceObj.isPlaying && !MusicUiObj.activeSelf)
		{
			playBakeAudio();
			MusicUiObj.SetActive(true);
			yield return new WaitForSeconds(2f);

			MusicUiObj.SetActive(false);
		}

		if(GlobalScript.GetInstance().player.IsPass)
		{
			//Debug.Log("stop background audio");
			AudioSourceObj.Stop();
			StopCoroutine( "checkBakeAudio" );
			yield break;
		}

		yield return new WaitForSeconds( 1 );

		yield return StartCoroutine( "checkBakeAudio" );
	}
	
	void callBackgroundAudio()
	{
		//ScreenLog.Log("callBackgroundAudio -> start");
		StartCoroutine( "checkBakeAudio" );
	}
	
	void Update()
	{
		if(GlobalScript.GetInstance().player.Life <= 0 || CameraShake.GetInstance().bIsOpenCamEffect)
		{
			if(!BlurEffectScript.enabled)
			{
				BlurEffectScript.enabled = true;
				if(CameraShake.GetInstance().bIsOpenCamEffect)
				{
					BlurEffectScript.iterations = (int)(0.3f * MaxBlurIter);
				}
				else
				{
					BlurEffectScript.iterations = MaxBlurIter;
				}
			}
			return;
		}
		else if(BlurEffectScript.enabled)
		{
			BlurEffectScript.enabled = false;
			BlurEffectScript.iterations = 0;
		}
	}

	NetworkServerNet NetworkServerScript;

	public static BikeCameraNet _Instance;
	public static BikeCameraNet GetInstance()
	{
		return _Instance;
	}

	public void PlayFeiBanAudio()
	{
		AudioManager.Instance.PlaySFX(BigFeiBanAudio);
	}

	void Start()
	{
		_Instance = this;
		camPosEvent = BikeCamEvent.Null;
		NetworkServerScript = NetworkServerNet.GetInstance();

		if(mMotionBlur == null)
		{
			Debug.LogWarning("mMotionBlur is null");
		}
		BlurEffectScript.enabled = false;
		
		float[] distances = new float[32];
		for(int i = 0; i < 32; i++)
		{
			distances[i] = 400f;
		}
		distances[9] = 260;
		distances[15] = 100;
		distances[18] = 600;
		distances[19] = 50;
		distances[13] = 250;
		distances[20] = 50;
		distances[21] = 250;
		camera.layerCullDistances = distances;
		
		Random.seed = (int)(Time.realtimeSinceStartup * 100000f);
		
		RainObj.SetActive(false);
		
		if(NetworkServerScript != null && !NetworkServerScript.GetIsServer())
		{
			Invoke("callBackgroundAudio", 3.5f);
		}
		
		mCamTran = transform;
		
		Screen.showCursor = false;
		smoothVal =  mSmooth * 0.015f;
		//smoothValTmp = smoothVal;
		
		if(mBike != null)
		{
			mBikePlayer = mBike;
		}
		
		mMotionBlur = gameObject.GetComponent<MotionBlur>();
		//		if(mMotionBlur == null)
		//		{
		//			ScreenLog.Log("BikeCamera::Start -> mMotionBlur is null!");
		//		}
	}
	
	IEnumerator rainContrl()
	{
		if(bikeScript == null)
		{
			yield break;
		}
		
		//ScreenLog.Log("rainContrl -> init");
		if(GlobalScript.GetInstance().player.IsPass)
		{
			RainObj.SetActive(false);
			StopCoroutine( "rainContrl" );
			yield break;
		}
		
		float time = 0f;
		bool isIntoSuiDao = bikeScript.GetIsIntoSuiDao();
		if(isPlayRain && !isIntoSuiDao)
		{
			isPlayRain = false;
			RainObj.SetActive(true);
			
			time = Random.Range(30f, 60f);
		}
		else
		{
			isPlayRain = true;
			RainObj.SetActive(false);
			time = Random.Range(20f, 70f);
		}
		yield return new WaitForSeconds(time);
		
		yield return StartCoroutine( "rainContrl" );
	}
	
	//FixedUpdate
	//LateUpdate 
	void FixedUpdate()
	{
		if(GlobalScript.GetInstance().player.IsPass)
		{
			return;
		}
		
		if(mBikePlayer == null)
		{
			//ScreenLog.Log("BikeCameraNet -> mBikePlayer is null");
			return;
		}
		
		if(bikeScript.GetIsIntoSuiDao())
		{
			isPlayRain = true;
			RainObj.SetActive(false);
		}
		setCameraPos();
	}
	
	public void setCameraPos()
	{
		//ScreenLog.Log("setCameraPos");
		if(mBikePlayer == null)
		{
			if(mAimPoint)
			{
				mCamTran.LookAt(mAimPoint);
			}
			return;
		}
		
		if(bikeScript != null)
		{
			if( bikeScript.checkBikeState( 0 ) )
			{
				//bIsFollow = false;
				//fixCameraPos();
				return;
			}
		}
		else
		{
			Debug.LogWarning("BikeCamera::Update -> bikeScript is null!");
			return;
		}
		
		if(mAimPoint == null)
		{
			return;
		}
		
		if(Time.timeScale != 0f)
		{
			if(bikeScript.getIsPlayCool())
			{
				//DepthBigFeiBan.enabled = true;
				if(!IsIntoFeiBan)
				{
					AudioManager.Instance.PlaySFX( BigFeiBanAudio );
				}
				Time.timeScale = 0.5f;

				IsIntoFeiBan = true;
				camDir = CamDir.LEFT;
			}
			else if(!bikeScript.GetIsSlowWorld())
			{
				Time.timeScale = 1f;
			}

			if(!bikeScript.getIsDoFly())
			{
				//DepthBigFeiBan.enabled = false;
				IsIntoFeiBan = false;
				camDir = CamDir.BACK;
			}
		}
		
		bool isServer = bikeScript.GetIsServer();

		float bikeSpeed = bikeScript.GetBikeSpeed();
		if( !isServer && GlobalScript.GetInstance().player != null )
		{
			bikeSpeed = GlobalScript.GetInstance().player.Speed;
		}
		//TestBikeSpeed = bikeSpeed;

		//ScreenLog.Log("isServer " + isServer);
		if(isServer)
		{
			float dis = 0.0f;
			Vector3 miaoZhunGSPos = Vector3.zero;

			switch(camPosEvent)
			{
			case BikeCamEvent.JingZhi:
				break;

			case BikeCamEvent.HangPai:
				transform.position = HangPaiTrans[mzgsIndex].position;
				transform.LookAt(mAimPoint);
				break;

			case BikeCamEvent.JingZhiMiaoZhun:
				transform.LookAt(mAimPoint);
				break;

			case BikeCamEvent.MiaoZhunGenSui:
				
				if(mzgsIndex == (int)MiaoZhunGenSuiCtrl.THIRD_PERSON)
				{
					miaoZhunGSPos = mCamPoint_back.position;
				}
				else
				{
					miaoZhunGSPos = MiaoZhunGenSui[mzgsIndex].position;
				}

				dis = Vector3.Distance(mCamTran.position, miaoZhunGSPos);
				if(dis > 10.0f && bikeSpeed < 10.0f)
				{
					mCamTran.LookAt(mAimPoint);
					mCamTran.position = Vector3.Lerp(mCamTran.position, miaoZhunGSPos, smoothVal);
				}
				
				if(bikeSpeed < 10.0f)
				{
					return;
				}
				
				mCamTran.LookAt(mAimPoint);
				mCamTran.position = Vector3.Lerp(mCamTran.position, miaoZhunGSPos, smoothVal);
				break;

			case BikeCamEvent.TeDingDongZuo:
				mCamTran.LookAt(mAimPoint);
				mCamTran.position = mCamPoint_back.position;
				break;

			case BikeCamEvent.GameOver:
				dis = Vector3.Distance(mCamTran.position, MiaoZhunGenSui[mzgsIndex].position);
				if(dis > 10.0f && bikeSpeed < 10.0f)
				{
					mCamTran.LookAt(mAimPoint);
					mCamTran.position = Vector3.Lerp(mCamTran.position, MiaoZhunGenSui[mzgsIndex].position, smoothVal);
				}
				
				if(bikeSpeed < 10.0f)
				{
					return;
				}
				
				mCamTran.LookAt(mAimPoint);
				mCamTran.position = Vector3.Lerp(mCamTran.position, MiaoZhunGenSui[mzgsIndex].position, smoothVal);
				break;

			default:
				break;
			}

			return;
		}
		
		//float steer = bikeScript.GetBikeSteer();
		//bikeSpeed = 30; //test
		//ScreenLog.Log("bikeSpeed " + bikeSpeed);

		bool xiaoFeiBan = bikeScript.GetIsIntoXiaoFeiBan();
		if (xiaoFeiBan)
		{
			float minVel = 50f;
			if (bikeSpeed > minVel) {
				mMotionBlur.enabled = true;
				
				float minBlurAmount = 0.6f;
				float k = (0.8f - minBlurAmount) / (80f - minVel);
				mMotionBlur.blurAmount = k * (bikeSpeed - minVel) + minBlurAmount;
				//mMotionBlur.blurAmount = 0.92f;
			} else if (mMotionBlur.enabled) {
				mMotionBlur.enabled = false;
			}
		}
		else
		{
			mMotionBlur.enabled = false;
		}

		Vector3 camPos = mCamPoint_back.position;
		switch(camDir)
		{
		case CamDir.FORWARD:
			mCamTran.position = mCamPoint_forward.position;
			mCamTran.eulerAngles = mCamPoint_forward.eulerAngles;
			break;
			
		case CamDir.BACK:
			
			bIsAimPlayer = true;
			bool isAimBike = true;
			if(followSpeed < smoothVal)
			{
				followSpeed += 0.001f;
				if(followSpeed > smoothVal)
				{
					followSpeed = smoothVal;
				}
				
				camPos = Vector3.Lerp(mCamTran.position,
				                      mCamPoint_back.position, followSpeed);
			}
			else
			{
				float dis = Vector3.Distance(mCamTran.position,
				                             mCamPoint_back.position);
				if(bikeSpeed > 0f)
				{
					camPos = Vector3.Lerp(mCamTran.position,
					                      mCamPoint_back.position, smoothPer * smoothVal);
					
					if(bikeSpeed < 5 && Time.timeScale == 1f)
					{
						isAimBike = false;
						camPos = transform.position;
						mCamPoint_back.parent = null;
					}
					else if(mCamPoint_back.parent == null)
					{
						mCamPoint_back.parent = backPointParent;
						mCamPoint_back.localPosition = backLocalPos;
					}
				}
				else
				{
					if(dis > 0.3f)
					{
						camPos = Vector3.Lerp(mCamTran.position,
						                      mCamPoint_back.position, smoothVal);

						dis = Vector3.Distance(camPos,
						                       mCamPoint_back.position);
						if(dis <= 0.29f)
						{
							camPos = mCamPoint_back.position;
						}
					}
					else
					{
						isAimBike = false;
						camPos = transform.position;
					}
				}
			}
			
			mCamTran.position = camPos;
			if(isAimBike)
			{
				mCamTran.LookAt(mAimPoint);
			}
			break;
			
		case CamDir.LEFT:
			mCamTran.position = mCamPoint_left.position;
			mCamTran.eulerAngles = mCamPoint_left.eulerAngles;
			break;

		case CamDir.FREE:
			mCamTran.position = Vector3.Lerp(mCamTran.position, mCamPoint_free.position, 0.2f);
			mCamTran.eulerAngles = mCamPoint_free.eulerAngles;
			break;
		}
	}

	void fixCameraPos()
	{
		if(mAimPoint == null)
		{
			return;
		}
		
		if(mFollowFallBikeTime <= 2.5f)
		{
			float distanceUp = 2.5f;
			float distanceAway = 5f;
			mCamTran.position = mAimPoint.position + Vector3.up * (distanceUp + 1.65f)
				- mAimPoint.forward * distanceAway;
			
			// make sure the camera is looking the right way!
			mCamTran.LookAt(mAimPoint);
		}
		mFollowFallBikeTime += Time.deltaTime;
	}


//	void OnGUI()
//	{
//		int speed = Mathf.FloorToInt(TestBikeSpeed);
//		GUI.Box(new Rect(0, 100, 100, 20), "speed " + speed.ToString());
//	}
}
