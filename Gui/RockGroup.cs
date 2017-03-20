using UnityEngine;
using System.Collections;

public class RockGroup : MonoBehaviour {

	public GameObject startTrigger;//滚石的启动触发器
	public GameObject stopTrigger; //滚石的停止触发器
	public RockModer rockModer;
	public float interval;//每隔 interval尝试一次创建石块但受概率影响不一定会成功
	public int minCount=1;//每隔一段时间最少产生的石块；
	public int s;
	public float minCountTime=2.0f;
	private GameObject[] ChilderRocks;//存储一组石块的每一个石块
	private	float timer;//辅助调试用的，没实际意义
	private int triggerCount=0;
	private int copyCount=0;

	// Use this for initialization
	void Start () {
		if(startTrigger)
		{
		 	TriggerScript triggerscript=(TriggerScript)startTrigger.GetComponent("TriggerScript");
			triggerscript.TriggerEnter+=Enter;
		}

        if(stopTrigger)
		{
			TriggerScript stopTriggerscript=(TriggerScript)stopTrigger.GetComponent("TriggerScript");
			stopTriggerscript.TriggerEnter+=StopTrggerEnter;
		}
		InitChilderRock();
	}

	/// <summary>
	/// 获取所有的子物体
	/// </summary>
	public void InitChilderRock()
	{
	   	int	childerCount=gameObject.transform.childCount;
		//Debug.Log(childerCount);
		ChilderRocks=new GameObject[childerCount];

		for(int i=0;i<childerCount;i++)
		{
			ChilderRocks[i]=gameObject.transform.GetChild(i).gameObject;
		}
	}

	public void StopTrggerEnter()
	{
		triggerCount--;
		if(triggerCount <= 0)
		{
		   if(ChilderRocks != null && ChilderRocks.Length > 0)
		   {
				//Debug.Log("stop to spawn rock...");
	          	StopCoroutine("StartCopyAndForce")	;	
		   }
		}	
	}
		
	/// <summary>
	/// 当TriggerEnter事件被触发时响应
	/// </summary>
	public void Enter()
	{
		if(triggerCount >= 1)
		{
			return;
		}

		triggerCount++;
		if(ChilderRocks != null && ChilderRocks.Length > 0)
		{
//			CopyAndForceAll();
//			Debug.Log("cccccccccccccccc");
			if(rockModer==RockModer.Repeat)
			{
				 StartCoroutine("StartCopyAndForce");
				 StartCoroutine("MinSafeguard");
			}
			else if(rockModer==RockModer.OneTime)
			{
               	ForceAll();
			}
			else
			{
				ForcePossible();
			}
		}

		Invoke("StopTrggerEnter", 10.0f);
	}

	public IEnumerator MinSafeguard()
	{
		while(true)
		{
			yield return new WaitForSeconds(minCountTime);
			//Debug.Log("sssssssssssssssss"+minCountTime);
			if(copyCount<minCount)
			{
				int m=minCount-copyCount;
				CompelCopy(m);
			}
		}
	}

	public  void CompelCopy(int m)
	{
		copyCount = 0;
		for(int i = 0; i < m; i++)
		{
			int r = Random.Range(0, ChilderRocks.Length - 1);
			CopyAndForce(ChilderRocks[r], r);
		}
	}


	public GameObject []RockNetPrefab;

	public  void CopyAndForce(GameObject go, int netIndex)
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			if(netIndex >= RockNetPrefab.Length)
			{
				Debug.LogError("CopyAndForce -> netIndex is wrong!");
				return;
			}

			int rockId = int.Parse(Network.player.ToString());
			GameObject rockPrefab = RockNetPrefab[netIndex];

			GameObject go1 = Network.Instantiate(rockPrefab, go.transform.position, go.transform.rotation, rockId) as GameObject;
			RockNet r = (RockNet)go1.GetComponent("RockNet");
			r.AddForce();
			r.BenginDestoryColne();
		}
		else
		{
			GameObject go1 = Instantiate(go, go.transform.position, go.transform.rotation) as GameObject;
			RockScript r = (RockScript)go1.GetComponent("RockScript");
			r.AddForce();
			r.BenginDestoryColne();
		}
	}

	/// <summary>
	/// 对所有子物体有一定概率施加力.
	/// </summary>
	public  void ForcePossible()
	{
		float rand = Random.value;

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			for(int i = 0; i < ChilderRocks.Length; i++)
			{
				//Debug.Log("5555555555555555555555"+ChilderRocks.Length+"rand"+rand);
				RockNet r = (RockNet)ChilderRocks[i].GetComponent("RockNet");
				if(rand < r.probability)
				{
					ChilderRocks[i].SetActive(false);

					int rockId = int.Parse(Network.player.ToString());
					GameObject rockPrefab = RockNetPrefab[i];
					
					GameObject go1 = Network.Instantiate(rockPrefab, ChilderRocks[i].transform.position,
					                                     ChilderRocks[i].transform.rotation, rockId) as GameObject;

					r = (RockNet)go1.GetComponent("RockNet");

					r.AddForce();
					r.BenginDestoryColne();
				}
			}
		}
		else
		{
			for(int i = 0; i < ChilderRocks.Length; i++)
			{
				//Debug.Log("5555555555555555555555"+ChilderRocks.Length+"rand"+rand);
				RockScript r = (RockScript)ChilderRocks[i].GetComponent("RockScript");
				if(rand < r.probability)
				{
					r.AddForce();
					r.BenginDestoryColne();
				}
			}
		}
	}

	/// <summary>
	/// 对所有子物体施加力
	/// </summary>
	public void ForceAll()
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			for(int i = 0; i < ChilderRocks.Length; i++)
			{
				RockNet r = (RockNet)ChilderRocks[i].GetComponent("RockNet");
				
				ChilderRocks[i].SetActive(false);
				
				int rockId = int.Parse(Network.player.ToString());
				GameObject rockPrefab = RockNetPrefab[i];
				
				GameObject go1 = Network.Instantiate(rockPrefab, ChilderRocks[i].transform.position,
				                                     ChilderRocks[i].transform.rotation, rockId) as GameObject;
				
				r = (RockNet)go1.GetComponent("RockNet");

				r.AddForce();
				r.BenginDestoryColne();
			}
		}
		else
		{
			for(int i = 0; i < ChilderRocks.Length; i++)
			{
				RockScript r = (RockScript)ChilderRocks[i].GetComponent("RockScript");
				r.AddForce();
				r.BenginDestoryColne();
			}
		}
	}

	/// <summary>
	/// 每隔一段时间，每个子物体有概率复制一个自己并施加力.
	/// </summary>
	/// <returns>
	/// The copy and force.
	/// </returns>
	public IEnumerator StartCopyAndForce()
	{
		while(true)
		{
			//Debug.Log("tttttttttttttttttttttttt"+timer);
			yield return new WaitForSeconds (interval);
			CopyAndForceAll();
		}
	}

	/// <summary>
	/// 每个子物体根据概率决定是否复制自己并施加力
	/// </summary>
	public void CopyAndForceAll()
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			if(ChilderRocks == null)
			{
				ScreenLog.LogWarning("CopyAndForceAll -> ChilderRocks is null");
				return;
			}

			for(int i = 0; i < ChilderRocks.Length; i++)
			{
				float rand = Random.value;
				//Debug.Log("5555555555555555555555"+ChilderRocks.Length+"rand"+rand);
				//RockNet r = (RockNet)ChilderRocks[i].GetComponent("RockNet");
				RockScript r = (RockScript)ChilderRocks[i].GetComponent("RockScript");
				if(rand < r.probability)
				{
					copyCount++;
					CopyAndForce(ChilderRocks[i], i);
				}
			}
		}
		else
		{
			for(int i = 0; i < ChilderRocks.Length; i++)
			{
				float rand = Random.value;
				//Debug.Log("5555555555555555555555"+ChilderRocks.Length+"rand"+rand);
				RockScript r = (RockScript)ChilderRocks[i].GetComponent("RockScript");
				if(rand < r.probability)
				{
					copyCount++;
					CopyAndForce(ChilderRocks[i], i);
				}
			}
		}
	}

	// Update is called once per frame
//	void Update () {
//	//timer+=Time.deltaTime;
//	}
}

public enum RockModer:int
{
	OneTime,
	Repeat,
	Possible
}