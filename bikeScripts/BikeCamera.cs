
using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class BikeCamera : MonoBehaviour
{
	public AudioReverbZone AudioZone = null;
	public GameObject RainObj = null;
	private bool isPlayRain = false;

	public GameObject MusicUiObj = null;

	public DepthOfFieldScatter DepthBigFeiBan = null;

	public BlurEffect BlurEffectScript = null;
	public int MinBlurIter = 0;
	public int MaxBlurIter = 3;
	public float ChangeBlurTime = 0.5f;
	
	private float blurTime = 0f;
	private float blurIter = 0f;
	private float speed = 0f;
	
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
	private Transform backPointParent;
	Vector3 backLocalPos;
	
	private Transform mCamPoint_free = null;

	static private Transform mCamTran = null;
	
	static private bike bikeScript = null;
	private float mFollowFallBikeTime = 0f;
	public AudioSource AudioSourceObj = null;
	
	bool isCallBackAudio = false;
	
	public AudioClip XiaoFeiBanAudio = null;
	private bool isPlayXiaoFeiBanAudio = false;

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

	public void setBikePlayer(GameObject bikePlayer)
	{
		if(bikePlayer != null)// && mBikePlayer == null)
		{
			if(mBikePlayer != null && mBikePlayer.name == bikePlayer.name )
			{
				return;
			}
			//Debug.Log("set camera aim to " + bikePlayer.name);

			Map.Player = bikePlayer;
			mBikePlayer = bikePlayer;

			bikeScript = mBikePlayer.GetComponent<bike>();
			if(bikeScript == null)
			{
				Debug.LogWarning("BikeCamera::Start -> bikeScript is null!");
			}
			else
			{
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
				//Debug.Log("setBikePlayer -> init, backLocalPos " + backLocalPos);
			}
		}
	}

	void playBakeAudio()
	{
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
		if(bakeAudio.Length <= 0)
		{
			yield break;
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
		//Debug.Log("callBackgroundAudio -> start");
		StartCoroutine( "checkBakeAudio" );
		if(Application.loadedLevel == (int)GameLeve.Leve1)
		{
			StartCoroutine( "rainContrl" );
		}
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

		if(GlobalScript.GetInstance().player.IsPass)
		{
			return;
		}

		if(Time.timeScale == 0f)
		{
			if(BlurEffectScript.iterations >= MaxBlurIter)
			{
				return;
			}

			if(!BlurEffectScript.enabled)
			{
				blurIter = 0f;
				BlurEffectScript.iterations = 0;
				blurTime = Time.realtimeSinceStartup;
				speed = (float)(MaxBlurIter - MinBlurIter) / ChangeBlurTime;

				BlurEffectScript.enabled = true;
			}

			float dTime = Time.realtimeSinceStartup - blurTime;
			//blurTime = Time.realtimeSinceStartup;
			blurIter = speed * dTime;
			BlurEffectScript.iterations = (int)blurIter;
		}
		else if(BlurEffectScript.enabled)
		{
			BlurEffectScript.enabled = false;
			BlurEffectScript.iterations = 0;
		}
	}
	
	void Start()
	{
		if(mMotionBlur == null)
		{
			Debug.LogWarning("mMotionBlur is null");
		}
		BlurEffectScript.enabled = false;
		BlurEffectScript.iterations = 0;

		DepthBigFeiBan.enabled = false;

		AudioZone.enabled = false;

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
		Invoke("callBackgroundAudio", 3.5f);

		mCamTran = transform;

		Screen.showCursor = false;
		smoothVal =  mSmooth * 0.015f;
		if(mBike != null)
		{
			mBikePlayer = mBike;
		}

		mMotionBlur = gameObject.GetComponent<MotionBlur>();
	}

	IEnumerator rainContrl()
	{
		if(bikeScript == null)
		{
			yield break;
		}

		//Debug.Log("rainContrl -> init");
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
			
			time = Random.Range(60f, 90f);
		}
		else
		{
			isPlayRain = true;
			RainObj.SetActive(false);
			time = Random.Range(20f, 50f);
		}
		yield return new WaitForSeconds(time);

		if(RainObj.activeSelf)
		{
			RainObj.SetActive(false);
			StopCoroutine( "rainContrl" );
			yield break;
		}
		
		yield return StartCoroutine( "rainContrl" );
	}
	
	//FixedUpdate
	//LateUpdate 
	void FixedUpdate()
	{
//		if(!pcvr.bIsHardWare || pcvr.IsTestGetInput)
//		{
//			bike.setBikeMouseDown();
//		}

		if(GlobalScript.GetInstance().player.IsPass) {
			return;
		}

		if(mBikePlayer == null)
		{
			return;
		}

		if(bikeScript.GetIsIntoSuiDao())
		{
			isPlayRain = true;
			RainObj.SetActive(false);
		}

		
		if(isCallBackAudio && GlobalScript.GetInstance().player.Life > 9)
		{
			isCallBackAudio = false;
			StartCoroutine( "checkBakeAudio" );
		}
		setCameraPos();
	}

	void setCameraPos()
	{
		//Debug.Log("setCameraPos");
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
				DepthBigFeiBan.enabled = true;
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
				DepthBigFeiBan.enabled = false;
				IsIntoFeiBan = false;
				camDir = CamDir.BACK;
			}
		}

		float bikeSpeed = bikeScript.GetBikeSpeed();
		bool xiaoFeiBan = bikeScript.GetIsIntoXiaoFeiBan();
		if (xiaoFeiBan)
		{
			if(!isPlayXiaoFeiBanAudio)
			{
				isPlayXiaoFeiBanAudio = true;
				AudioManager.Instance.PlaySFX( XiaoFeiBanAudio );
			}

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
			isPlayXiaoFeiBanAudio = false;
			mMotionBlur.enabled = false;
		}

		if(bikeScript.GetIsAiNPC() || !bIsAimPlayer)
		{
			mCamTran.position = Vector3.Lerp(mCamTran.position, mCamPoint_back.position, followSpeed);
			mCamTran.LookAt(mAimPoint);
		}
		else
		{
			Vector3 camPos = transform.position;
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
						if(dis > 0.1f)
						{
							camPos = Vector3.Lerp(mCamTran.position,
							                      mCamPoint_back.position, smoothVal);
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
////		if(bikeScript != null)
////		{
////			GameObject player = null;
////			string str = "";
////			for(int i = 0; i < 8; i++)
////			{
////				player = bikeScript.getRankPlayer( i );
////				if(player != null)
////				{
////					str = (i + 1) + "th: " + player.name;
////					GUI.Box(new Rect(Screen.width - 250, (10 + i * 25), 200, 20), str);
////				}
////			}
////		}
//	}
}