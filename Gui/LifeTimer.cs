using UnityEngine;
using System.Collections;
using Frederick.ProjectAircraft;

public class LifeTimer : Timer {
	public AudioClip TimeAudio20Miao = null;
	private AudioSource audioTimeDaoJiShi = null;

	void Awake()
	{
		fenshiUI=fenshi.GetComponent<UISprite>();
		fengeUI=fenge.GetComponent<UISprite>();
		miaoshiUI=miaoshi.GetComponent<UISprite>();
		miaogeUI=miaoge.GetComponent<UISprite>();
	}
	// Use this for initialization
	void Start () {
		GlobalScript.GetInstance().player.AddBufferEvent+=AddBufferEvent;
		//GlobalScript.GetInstance().player.Start+=StartTimer;
		intToTimerImage(GlobalScript.GetInstance().player.Life);
		//intToTimerImage(GlobalScript.GetInstance().player.Life);
		//StartTimer();
		GlobalScript.GetInstance().player.ContinueGame+=ContinueGame;
	}
	private void AddBufferEvent(BufferKind kind)
	{
		if(kind==BufferKind.Shoubiao||kind==BufferKind.Jiashidian)
		{
			UpdateImage();
		}
	}

	public void UpdateImage()
	{
		TweenScale sc=	GetComponent<TweenScale>();
		sc.from=new Vector3(1,1,1);
		sc.to=new Vector3(1.2f,1.2f,1);
		sc.ResetToBeginning();
		sc.PlayForward();
		EventDelegate.Add(sc.onFinished,onfinished);
		
		
	}
	//delegate void dehandel();
	public void onfinished()
	{
		PR="jdf";
		intToTimerImage(GlobalScript.GetInstance().player.Life);
		StartCoroutine("EndUpdateImage");
	}
	public IEnumerator EndUpdateImage()
	{
		//Debug.Log("aaaaaaaaa");
		yield return new WaitForSeconds(3);
		//Debug.Log("ccccccccccccccccccc");
		PR="d";
		TweenScale sc=GetComponent<TweenScale>();
		sc.from=new Vector3(1.2f,1.2f,1);
		sc.to=new Vector3(1,1,1);
		sc.ResetToBeginning();
		sc.PlayForward();
		EventDelegate.Remove(sc.onFinished,onfinished);
	}
	public void ContinueGame()
	{
		StartTimer();
	}
	public void StopTimer()
	{
		StopAllCoroutines ();
	}
	public void StartTimer()
	{
		StartCoroutine("Timer");
	}

	public IEnumerator Timer()
	{
		intToTimerImage(GlobalScript.GetInstance().player.Life);
		while(GlobalScript.GetInstance().player.Life>0)
		{
			if(Time.timeScale != 1f)
			{
				yield return new  WaitForSeconds(1);
				continue;
			}

			yield return new  WaitForSeconds(1);
			GlobalScript.GetInstance().player.Life--;
			if(GlobalScript.GetInstance().player.Life == 10)
			{
				audioTimeDaoJiShi = AudioManager.Instance.audio;
				audioTimeDaoJiShi.clip = TimeAudio20Miao;
				audioTimeDaoJiShi.Play();

				GlobalScript.GetInstance().ShowTishi(TishiInfo.Daojishi);
			}
			else if(GlobalScript.GetInstance().player.Life == 0
			        || GlobalScript.GetInstance().player.Life > 10
			        || GlobalScript.GetInstance().player.IsPlayHuanHu)
			{
				if(audioTimeDaoJiShi != null)
				{
					audioTimeDaoJiShi.Stop();
				}
			}

			GlobalScript.GetInstance().player.LostTime++;
			intToTimerImage(GlobalScript.GetInstance().player.Life);
		}
	}

	// Update is called once per frame
//	void Update () {
//	
//	}
}
