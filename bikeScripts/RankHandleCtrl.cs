using UnityEngine;
using System.Collections;

using System.Runtime.InteropServices;
using System.Collections.Generic;

public class RankHandleCtrl : MonoBehaviour {
	
	static public int mRankCount = 0;
	static public  playerRank [] mRankPlayer = null;
	public static bool IsStopCheckRank = false;
	public  playerRank [] mRankPlayerTest = null;

	static RankHandleCtrl _Instance;
	public static RankHandleCtrl GetInstance()
	{
		if(_Instance == null)
		{
			GameObject obj =  new GameObject("_RankHandleCtrl");
			_Instance = obj.AddComponent<RankHandleCtrl>();
		}
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		InitRankPlayer();
		InvokeRepeating("checkNetPlayerRank", 3, 0.125f);
	}

	void InitRankPlayer()
	{
		mRankPlayer = null;
		int maxRankPlayer = 8;
		if(mRankPlayer == null)
		{
			mRankPlayer = new playerRank[maxRankPlayer];
			for(int i = 0; i < maxRankPlayer; i++)
			{
				mRankPlayer[i] = new playerRank();
			}
		}

		mRankCount = 0;
		IsStopCheckRank = false;
	}

	public void SetRankPlayerArray(GameObject rankObj, bool isPlayer)
	{
		if(mRankCount > 7)
		{
			return;
		}

		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			return;
		}

		if(FreeModeCtrl.IsServer)
		{
			return;
		}
		mRankPlayer[mRankCount].player = rankObj;
		mRankPlayer[mRankCount].IsPlayer = isPlayer;
		mRankCount++;
		//Debug.Log("**********name " + rankObj.name + ", mRankCount " + mRankCount);
		if(mRankCount >= 7)
		{
			if(GlobalScript.GetInstance().player != null)
			{
				GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
			}

			GlobalScript.GetInstance().ChangeNPC();
		}
	}

	public void SetBikePathCount(string objName)
	{
		if(objName == "")
		{
			return;
		}
		
		int max = mRankPlayer.Length;
		for(int i = 0; i < max; i++)
		{
			if(mRankPlayer[i].Name == objName)
			{
				mRankPlayer[i].mBikePathCount++;
				break;
			}
		}
	}

	public void SetBikeAimMark(string aimPathName, int id, string objName)
	{
		if(aimPathName == "" || objName == "")
		{
			return;
		}

		GameObject pathObj = GameObject.Find(aimPathName);
		if(pathObj == null)
		{
			return;
		}

		AiPathCtrl pathScript = pathObj.GetComponent<AiPathCtrl>();
		if(pathScript == null)
		{
			return;
		}

		Transform path = pathObj.transform;
		if(path == null)
		{
			return;
		}

		if(id < 0 || id >= path.childCount)
		{
			return;
		}
		
		Transform tranAim = path.GetChild( id );
		int max = mRankPlayer.Length;
		for(int i = 0; i < max; i++)
		{
			if(mRankPlayer[i].Name == objName)
			{
				mRankPlayer[i].mBikeAimMark = tranAim;
				mRankPlayer[i].mBikePathKey = pathScript.KeyState;
				if(mRankPlayer[i].AiNetScript != null)
				{
					mRankPlayer[i].AiNetScript.SetAimMark(tranAim);
				}
				break;
			}
		}
	}

	public GameObject parentPlayer = null;// = mRankPlayer[mRankNum - 1].player;
	public GameObject childPlayer = null;// = mRankPlayer[mRankNum - 1].player;
	public Transform aimMarkPar;
	public Transform aimMarkCh;
	void checkNetPlayerRank()
	{
		if(IsStopCheckRank)
		{
			CancelInvoke("checkNetPlayerRank");
			return;
		}

		if(mRankPlayer == null)
		{
			return;
		}
		
		int j = 0;
		bool isContinue = false;
		for(int i = 0; i < 8; i++)
		{
			if(mRankPlayer[i].NetScript != null)
			{
				mRankPlayer[i].IsPlayer = mRankPlayer[i].NetScript.CheckObjIsPlayer();
				isContinue = mRankPlayer[i].NetScript.GetIsGameOver();
			}
			else if(mRankPlayer[i].AiNetScript != null)
			{
				mRankPlayer[i].IsPlayer = false;
				isContinue = mRankPlayer[i].AiNetScript.GetIsStopMoveAi();
			}
			else
			{
				isContinue = true;
				mRankPlayer[i].IsPlayer = false;
			}

			if(isContinue)
			{
				isContinue = false;
				continue;
			}

			j = i + 1;
			if(j > 7)
			{
				break;
			}

			parentPlayer = mRankPlayer[i].player;
			childPlayer = mRankPlayer[j].player;
			aimMarkPar = mRankPlayer[i].mBikeAimMark;
			aimMarkCh = mRankPlayer[j].mBikeAimMark;
			if(parentPlayer == null || childPlayer == null || aimMarkPar == null || aimMarkCh == null)
			{
				continue;
			}

			int pathKeyP = mRankPlayer[i].mBikePathKey;
			int pathKeyC = mRankPlayer[j].mBikePathKey;
			if(pathKeyP < pathKeyC)
			{
//				if(mRankPlayer[i].IsPlayer || mRankPlayer[j].IsPlayer)
//				{
//					Debug.Log("***************************1111, IsPlayer_1 " + mRankPlayer[i].IsPlayer
//					          + ", IsPlayer_2 " + mRankPlayer[j].IsPlayer);
//				}
				updateNetPlayerRank( j );
				break;
			}
			else if(pathKeyP > pathKeyC)
			{
				continue;
			}

			int pathIdPar = aimMarkPar.parent.GetInstanceID();
			int pathIdCh = aimMarkCh.parent.GetInstanceID();
			if(pathIdPar == pathIdCh)
			{
				AiMark markScriptP = aimMarkPar.GetComponent<AiMark>();
				AiMark markScriptC = aimMarkCh.GetComponent<AiMark>();
				int markCountP = markScriptP.getMarkCount();
				int markCountC = markScriptC.getMarkCount();
				if(markCountP < markCountC)
				{
//					if(mRankPlayer[i].IsPlayer || mRankPlayer[j].IsPlayer)
//					{
//						Debug.Log("***************************2222, IsPlayer_1 " + mRankPlayer[i].IsPlayer
//						          + ", IsPlayer_2 " + mRankPlayer[j].IsPlayer);
//					}
					updateNetPlayerRank( j );
					break;
				}
				else if(markCountP > markCountC)
				{
					continue;
				}
			}

			Vector3 vecA = parentPlayer.transform.position - childPlayer.transform.position;
			Vector3 vecB = aimMarkPar.position - parentPlayer.transform.position;
			Vector3 vecC = childPlayer.transform.forward;
			Vector3 vecD = parentPlayer.transform.forward;
			vecA.y = 0f;
			vecB.y = 0f;
			vecC.y = 0f;
			vecD.y = 0f;
			vecA = vecA.normalized;
			vecB = vecB.normalized;
			vecC = vecC.normalized;
			vecD = vecD.normalized;
			
			float cosDB = Vector3.Dot(vecD, vecB);
			float cosDC = Vector3.Dot(vecD, vecC);
			float cosAC = Vector3.Dot(vecA, vecC);
			if( (cosDB > 0f && cosDC > 0f && cosAC < 0f)
			   || (cosDB <= 0f && cosDC > 0f && cosAC > 0f)
			   || (cosDB <= 0f && cosDC <= 0f && cosAC < 0f) )
			{
//				if(mRankPlayer[i].IsPlayer || mRankPlayer[j].IsPlayer)
//				{
//					Debug.Log("*********33333, IsPlayer_1 " + mRankPlayer[i].IsPlayer
//					          + ", IsPlayer_2 " + mRankPlayer[j].IsPlayer
//					          + ", pathIdPar " + pathIdPar + ", pathIdCh " + pathIdCh);
//				}
				updateNetPlayerRank( j );
				break;
			}
			//Debug.Log("i = " + i + ", j = " + j);
		}
		return;
	}

	void updateNetPlayerRank(int rankNumCh)
	{
		GameObject objParent = mRankPlayer[rankNumCh - 1].player;
		GameObject objChild = mRankPlayer[rankNumCh].player;
		if(objParent == null || objChild == null)
		{
			return;
		}

		bikeNetUnity netScript = mRankPlayer[rankNumCh].NetScript;
		if(netScript != null)
		{
			netScript.PlaySurpassAction( objParent.transform );
		}

		//update info
		mRankPlayer[rankNumCh - 1].rankNum = rankNumCh - 1;
		mRankPlayer[rankNumCh].rankNum = rankNumCh;
				
		bool isPlayerTmp = mRankPlayer[rankNumCh - 1].IsPlayer;
		mRankPlayer[rankNumCh - 1].IsPlayer = mRankPlayer[rankNumCh].IsPlayer;
		mRankPlayer[rankNumCh].IsPlayer = isPlayerTmp;
		
		bikeAiNetUnity AiNetScriptTmp = mRankPlayer[rankNumCh - 1].AiNetScript;
		mRankPlayer[rankNumCh - 1].AiNetScript = mRankPlayer[rankNumCh].AiNetScript;
		mRankPlayer[rankNumCh].AiNetScript = AiNetScriptTmp;
		
		bikeNetUnity NetScriptTmp = mRankPlayer[rankNumCh - 1].NetScript;
		mRankPlayer[rankNumCh - 1].NetScript = mRankPlayer[rankNumCh].NetScript;
		mRankPlayer[rankNumCh].NetScript = NetScriptTmp;
		
//		if(mRankPlayer[rankNumCh - 1].IsPlayer || mRankPlayer[rankNumCh].IsPlayer)
//		{
//			Debug.Log("1111***parAimId " + mRankPlayer[rankNumCh - 1].mBikeAimMark.GetInstanceID()
//			          + ", chAimId " + mRankPlayer[rankNumCh].mBikeAimMark.GetInstanceID());
//		}

		Transform mBikeAimMarkTmp = mRankPlayer[rankNumCh - 1].mBikeAimMark;
		mRankPlayer[rankNumCh - 1].mBikeAimMark = mRankPlayer[rankNumCh].mBikeAimMark;
		mRankPlayer[rankNumCh].mBikeAimMark = mBikeAimMarkTmp;
		
//		if(mRankPlayer[rankNumCh - 1].IsPlayer || mRankPlayer[rankNumCh].IsPlayer)
//		{
//			Debug.Log("2222***parAimId " + mRankPlayer[rankNumCh - 1].mBikeAimMark.GetInstanceID()
//			          + ", chAimId " + mRankPlayer[rankNumCh].mBikeAimMark.GetInstanceID());
//		}

		int mBikePathCountTmp = mRankPlayer[rankNumCh - 1].mBikePathCount;
		mRankPlayer[rankNumCh - 1].mBikePathCount = mRankPlayer[rankNumCh].mBikePathCount;
		mRankPlayer[rankNumCh].mBikePathCount = mBikePathCountTmp;
		
		int pathCountP = mRankPlayer[rankNumCh - 1].mBikePathCount;
		int pathCountC = mRankPlayer[rankNumCh].mBikePathCount;
		if(pathCountP < pathCountC)
		{
			mRankPlayer[rankNumCh - 1].mBikePathCount = pathCountC;
		}

		int pathKeyTmp = mRankPlayer[rankNumCh - 1].mBikePathKey;
		mRankPlayer[rankNumCh - 1].mBikePathKey = mRankPlayer[rankNumCh].mBikePathKey;
		mRankPlayer[rankNumCh].mBikePathKey = pathKeyTmp;

		mRankPlayer[rankNumCh - 1].player = objChild;
		mRankPlayer[rankNumCh].player = objParent;

		//ScreenLog.Log("**********************updateNetPlayerRank");
		if(GlobalScript.GetInstance().player != null)
		{
			GlobalScript.GetInstance().player.RankList = new List<playerRank>(mRankPlayer);
		}

		if(mRankCount >= 7)
		{
			GlobalScript.GetInstance().ChangeNPC();
		}
		return;
	}
}
