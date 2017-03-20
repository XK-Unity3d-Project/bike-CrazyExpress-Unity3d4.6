using UnityEngine;
using System.Collections;

public class AddTime : MonoBehaviour {
	public GameObject Shiwei;
	public GameObject Gewei;
	public float Duration;
	public float KeepTime;
	private UISprite shiUI;
	private UISprite geUI;
	public Vector3 posFrom;
	public Vector3 posTo;
	private TweenPosition tween;

	public int []CityJiaShiDian;
	public int []OutdoorJiaShiDian;

	public int []CityJiaShiDianLow;
	public int []OutdoorJiaShiDianLow;

	public int []CityJiaShiDianHigh;
	public int []OutdoorJiaShiDianHigh;
	private int jiaShiCount = 0;

	void Awake()
	{
		shiUI=Shiwei.GetComponent<UISprite>();
		geUI=Gewei.GetComponent<UISprite>();
//		GlobalScript.GetInstance ().AddTimeEvent += AddTimeEvent;
		GlobalScript.GetInstance().player.AddBufferEvent += AddBufferEvent;
		tween = GetComponent<TweenPosition> ();
	}
//	void Start()
//	{
//		AddTimel();
//	}
	void AddTimeEvent()
	{
		//AddTimel ((int)BufferKind.Jiashidian);
	}

	void AddBufferEvent(BufferKind kind)
	{
		//Debug.Log("test ************* kind " + kind);
		if(kind == BufferKind.Shoubiao || kind == BufferKind.Jiashidian)
		{
			if(kind == BufferKind.Jiashidian)
			{
				int timeAdd = 40;

				string gameDiff = GlobalData.GetInstance().GameDiff;
				string leve = Application.loadedLevelName;
				if(leve == "CityBike" || leve == "CityBikeNetUnity")
				{
					switch(gameDiff)
					{
					case "0":
						if(jiaShiCount < CityJiaShiDianLow.Length)
						{
							timeAdd = CityJiaShiDianLow[jiaShiCount];
						}
						break;

					case "1":
						if(jiaShiCount < CityJiaShiDian.Length)
						{
							timeAdd = CityJiaShiDian[jiaShiCount];
						}
						break;

					case "2":
						if(jiaShiCount < CityJiaShiDianHigh.Length)
						{
							timeAdd = CityJiaShiDianHigh[jiaShiCount];
						}
						break;

					default:
						if(jiaShiCount < CityJiaShiDian.Length)
						{
							timeAdd = CityJiaShiDian[jiaShiCount];
						}
						break;
					}
				}
				else
				{
					switch(gameDiff)
					{
					case "0":
						if(jiaShiCount < OutdoorJiaShiDianLow.Length)
						{
							timeAdd = OutdoorJiaShiDianLow[jiaShiCount];
						}
						break;

					case "1":
						if(jiaShiCount < OutdoorJiaShiDian.Length)
						{
							timeAdd = OutdoorJiaShiDian[jiaShiCount];
						}
						break;

					case "2":
						if(jiaShiCount < OutdoorJiaShiDianHigh.Length)
						{
							timeAdd = OutdoorJiaShiDianHigh[jiaShiCount];
						}
						break;

					default:
						if(jiaShiCount < OutdoorJiaShiDian.Length)
						{
							timeAdd = OutdoorJiaShiDian[jiaShiCount];
						}
						break;
					}
				}

				//Debug.Log("timeAdd " + timeAdd + ", gameDiff " + gameDiff);
				GlobalScript.GetInstance().player.AddLife( timeAdd );
				AddTimel( timeAdd );
				jiaShiCount++;
			}
			else
			{
				AddTimel((int)kind);
			}
		}
	}
	public void AddTimel(int time)
	{
		//Debug.Log("addddddddddddddddddddddddddd"+time);
		intToImg(time);
		AddAlphaTween(true);
		AddPosTween ();
		StopCoroutine("Keep");
		StartCoroutine("Keep");
	}
	void AddPosTween()
	{
		tween.from = posFrom;
		tween.to = posTo;
		tween.ResetToBeginning();
		tween.PlayForward ();
	}
	private void intToImg(int time)
	{
      if(shiUI!=null)
		{
			shiUI.spriteName="jdjs"+time/10;
			//Debug.Log("addddddddddddddddddddddddddd"+shiUI.spriteName);
		}
		if(geUI!=null)
		{
			geUI.spriteName="jdjs"+time%10;
			//Debug.Log("addddddddddddddddddddddddddd"+geUI.spriteName);
		}
	}
	private IEnumerator Keep()
	{
		yield return new WaitForSeconds(KeepTime+Duration);
		AddAlphaTween(false);

	}
	private void AddAlphaTween(bool isadd)
	{
		for(int i=0;i<transform.childCount;i++)
		{
			TweenAlpha 	alpha=transform.GetChild(i).GetComponent<TweenAlpha>();
			//TweenPosition positon=transform;
			if(isadd)
			{
		   
				alpha.duration=Duration;
				alpha.from=0;
				alpha.to=1;
				alpha.ResetToBeginning();
				alpha.PlayForward();
			}
		   else
			{
			
				alpha.duration=Duration;
				alpha.from=1;
				alpha.to=0;
				alpha.ResetToBeginning();
				alpha.PlayForward();
			}
		}
	}
	// Update is called once per frame
//	void Update () {
//	
//	}
}
