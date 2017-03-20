using UnityEngine;
using System.Collections;

public class BikeGameCtrl : MonoBehaviour {
	public float AniRunSpeedMin = 0.1f;
	public float AniRunSpeedMax = 1f;
	public Score ScoreScript = null;
	public FunEvent FunEventScript = null;

	public GameObject [] mClonePlayerGroup = null;
	public GameObject [] mSpawnPlayerGroup = null;
	public Transform mAiPathCtrl = null;
	public Transform PathMark;
	static BikeGameCtrl _Instance;
	public static BikeGameCtrl GetInstance()
	{
		return _Instance;
	}

	void SpawnPlayer()
	{
		GameObject playerObj;
		for(int i = 0; i < 8; i++)
		{
			if(PathMark == null)
			{
				playerObj = (GameObject)Instantiate(mClonePlayerGroup[i], mSpawnPlayerGroup[i].transform.position,
			           								mSpawnPlayerGroup[i].transform.rotation);
			}
			else
			{
				playerObj = (GameObject)Instantiate(mClonePlayerGroup[i], PathMark.position, PathMark.rotation);
			}
		
			mSpawnPlayerGroup[i].SetActive(false);
			Destroy(mSpawnPlayerGroup[i]);
			playerObj.name = mClonePlayerGroup[i].name;
		}
		mSpawnPlayerGroup = null;
	}

	void Awake()
	{
		//GlobalData.GetInstance().gameMode = GameMode.SoloMode; //test.
//		bool isActiveDanJi = GlobalData.GetInstance().gameMode == GameMode.SoloMode ? true : false;
//		if (DanJiGameCtrl != null) {
//			DanJiGameCtrl.SetActive(isActiveDanJi);
//		}
//		if (LianJiGameCtrl != null) {
//			LianJiGameCtrl.SetActive(!isActiveDanJi);
//		}

		_Instance = this;
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			SpawnPlayer();
			bike.spawnRandPlayer();
			bike.mBikeGameCtrl = gameObject;
		}
	}

	// Use this for initialization
	void Start () {
		ChangeLeve.IsCanActiveSetPanel = true;
		XkGameCtrl xkgameCtrl = gameObject.GetComponent<XkGameCtrl>();
		if (xkgameCtrl == null) {
			gameObject.AddComponent<XkGameCtrl>();
		}
	}

	public FunEvent getBaoGuoScript()
	{
		return FunEventScript;
	}

	public Score getScoreScript()
	{
		return ScoreScript;
	}
}
