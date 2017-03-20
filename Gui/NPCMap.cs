using UnityEngine;
using System.Collections;

public class NPCMap : MonoBehaviour {
	public Texture[] MapTexture;
	public NPC_MarkerMap[] gos;
	private int playerCount;
	// Use this for initialization
	void Start () {
		int indexVal = 0;
		if (Application.loadedLevel == (int)GameLeve.Leve2
		    || Application.loadedLevel == (int)GameLeve.Leve4) {
			indexVal = 1;
		}

		UITexture uiTextureCom = GetComponent<UITexture>();
		uiTextureCom.mainTexture = MapTexture[indexVal];
		GlobalScript.GetInstance().ChangeNPCEvent += ChangeNPCEvent;
	}

	void ChangeNPCEvent()
	{
		if(GlobalScript.GetInstance().player.RankList == null)
		{
			return;
		}

		playerCount = GlobalScript.GetInstance().player.RankList.Count;
		//Debug.Log("playerCount " + playerCount);

		int j=0;
		for(int i=0;i<playerCount;i++)
		{
			if(!GlobalScript.GetInstance().player.RankList[i].IsPlayer && j < gos.Length)
			{
				if(GlobalScript.GetInstance().player.RankList[i].player == null)
				{
					//Debug.LogWarning("player is null, i " + i);
					continue;
				}

				gos[j].NPC = GlobalScript.GetInstance().player.RankList[i].player;
		        j++;
			}
		}
	}

	void UpdateMapNpcMark()
	{
		for(int i=0;i<gos.Length;i++)
		{
			if(gos[i].NPC!=null)
			{
				if(!gos[i].marker.activeSelf)
				{
					gos[i].marker.SetActive(true);
				}
				gos[i].marker.transform.localPosition = new Vector3(gos[i].NPC.transform.position.x-1023,
				                                                    gos[i].NPC.transform.position.z-1023);
			}
			else
			{
				gos[i].marker.SetActive(false);
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		UpdateMapNpcMark();
	}
}
[System.Serializable]
public class NPC_MarkerMap
{
   public	GameObject marker;
	//[HideInInspector]
   public	GameObject NPC;
}
