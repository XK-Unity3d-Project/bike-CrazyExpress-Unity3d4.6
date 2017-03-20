using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class Go : MonoBehaviour {
	public float duration;
	private UISprite ui;
	private TweenScale scale;
	public AudioClip [] TimeAudio;
	private AudioSource SoureAudio = null;
	public static bool IsStartGame = false;
	int indexTime = 0;
	NetworkServerNet NetworkServerScript;
	NetCtrl netCtrlScript;
	PlayerCreatNet playerCreatScript = null;
	static Go _Instance;
	public static Go GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start()
	{
		_Instance = this;
		if (GlobalData.GetInstance().gameMode == GameMode.OnlineMode) {
			GameObject bikegamectrl = GameObject.Find(GlobalData.bikeGameCtrl);
			playerCreatScript = bikegamectrl.GetComponent<PlayerCreatNet>();
			NetworkServerScript = NetworkServerNet.GetInstance();
			netCtrlScript = NetCtrl.GetInstance();
			if (NetworkServerScript != null && NetworkServerScript.GetIsServer()) {
				return;
			}
			else {
				SoureAudio = AudioManager.Instance.audio;
			}
		}
		else {
			SoureAudio = AudioManager.Instance.audio;
			ui=GetComponent<UISprite>();
			scale=GetComponent<TweenScale>();
			StartCoroutine("Gogo");
		}
	}

	public void checkIsPlayGame()
	{
		IsStartGame = true;
		playerCreatScript.HiddenServerWait();
		
		ui=GetComponent<UISprite>();
		scale=GetComponent<TweenScale>();
		StartCoroutine("Gogo");
	}

	private void ChangTween(bool isshow)
	{
      	if(isshow)
		{
			scale.from.x=0.1f;
			scale.from.y=0.1f;
			scale.from.z=0.1f;
			scale.to.x=1f;
			scale.to.y=1f;
			scale.to.z=1f;
		}
		scale.duration=duration;
		scale.ResetToBeginning();
		scale.PlayForward();
	}

	private void PlayTween()
	{
		ChangTween(true);
		ChangTween(false);
	}

	void playAudioTime()
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode
		   && NetworkServerScript != null
		   && NetworkServerScript.GetIsServer())
		{
			return;
		}

		if(TimeAudio.Length == 4 && TimeAudio[indexTime] != null)
		{
			SoureAudio.clip = TimeAudio[indexTime];
			SoureAudio.Play();
		}
		indexTime++;

		if(indexTime >= 4)
		{
			indexTime = 0;
		}
	}

	public IEnumerator Gogo()
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode
		   && NetworkServerScript != null
		   && NetworkServerScript.GetIsServer())
		{
			yield break;
		}

		if (netCtrlScript != null) {
			ScreenLog.Log("Gogo -> PlayerIntoGameCount " + netCtrlScript.PlayerIntoGameCount
			              +", CountLinkPlayer "+NetCtrl.CountLinkPlayer);
			if (netCtrlScript.PlayerIntoGameCount < NetCtrl.CountLinkPlayer) {
				yield return new WaitForSeconds(1.0f);
				yield return Gogo();
			}
		}

		Invoke("playAudioTime", 0.5f);
		ui.spriteName="go3";
		PlayTween();
		yield return new WaitForSeconds(1);

		Invoke("playAudioTime", 0.5f);
		ui.spriteName="go2";
		PlayTween();
	    yield return new WaitForSeconds(1);
		
		Invoke("playAudioTime", 0.5f);
		ui.spriteName="go1";
		PlayTween();
		BikeCamera.bIsAimPlayer = true;
		yield return new WaitForSeconds(1);

		Invoke("playAudioTime", 0.5f);
		ui.spriteName="go";
		PlayTween();
		//这里添加代码自行车开始可以控制.
		GlobalScript.GetInstance().player.StartGame();
		yield return new WaitForSeconds(1);

		gameObject.SetActive(false);
		pcvr.OpenGameDongGan();
		GuidanceImg.OpenYouMenUI();
	}
}