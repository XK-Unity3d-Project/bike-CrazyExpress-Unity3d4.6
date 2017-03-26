using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class Jixuyouxi : MonoBehaviour {
	public  GameObject go;
	public GameObject start;
	public GameObject qingtoubi;

	public AudioClip ContinueTimeAudio = null;
	public AudioClip StartBtAudio = null;

	private UISprite ui;
	private bool startTimer;
	// Use this for initialization
	void Awake()
	{
		ui=go.GetComponent<UISprite>();
	}

	void Start()
	{
		InputEventCtrl.GetInstance().ClickStartBtEvent += ClickStartBtEvent;
		gameObject.SetActive(false);
	}

	public void StartTimer()
	{
		pcvr.CloseGameDongGan();
		if(ui.spriteName!="dfj9")
		{
			ui.spriteName="dfj9";
		}

		AudioManager.Instance.PlaySFX( ContinueTimeAudio );
		StartCoroutine("Timer");
	}

	public IEnumerator Timer()
	{
		for(int j=8;j>=0;j--)
		{
			yield return new WaitForSeconds(1);

			AudioManager.Instance.PlaySFX( ContinueTimeAudio );
			ui.spriteName="dfj"+j;
		}
		gameObject.SetActive(false);
		GlobalScript.GetInstance().player.IsGameOver=true;
	}

	void ClickStartBtEvent(ButtonState val)
	{
		//ScreenLog.Log("Jixuyouxi::ClickStartBtEvent -> val " + val);
		if(val == ButtonState.DOWN)
		{
			return;
		}

		if(GlobalScript.GetInstance().player.IsGameOver || GlobalScript.GetInstance().player.Life > 0)
		{
			return;
		}

		if(GlobalData.GetInstance().Icon >= GlobalData.GetInstance().XUTOUBI 
		   || GlobalData.GetInstance().IsFreeMode)
		{
			AudioManager.Instance.PlaySFX( StartBtAudio );
			GlobalData.GetInstance().Icon -= GlobalData.GetInstance().XUTOUBI;
			pcvr.GetInstance().SubPlayerCoin(GlobalData.GetInstance().XUTOUBI);
			GlobalScript.GetInstance().player.AddLife(GlobalData.GetInstance().GAMETIME);
			start.SetActive(false);
			qingtoubi.SetActive(false);
			pcvr.StartLightStateP1 = LedState.Mie;
			pcvr.OpenGameDongGan();
		}
	}
	
	void Update () 
	{
		if(GlobalData.GetInstance().Icon >= GlobalData.GetInstance().XUTOUBI)
		{
			if(!start.activeSelf)
			{
			 	start.SetActive(true);
				qingtoubi.SetActive(false);
				pcvr.StartLightStateP1 = LedState.Shan;
			}
		}
		else
		{
			if(!GlobalData.GetInstance().IsFreeMode)
			{
				start.SetActive(false);
				qingtoubi.SetActive(true);
				pcvr.StartLightStateP1 = LedState.Mie;
			}
			else
			{
				start.SetActive(true);
				qingtoubi.SetActive(false);
				pcvr.StartLightStateP1 = LedState.Shan;
			}
		}
	}
}
