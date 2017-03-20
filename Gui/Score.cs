using UnityEngine;
using System.Collections;

public class Score : Number {
	public bool isFinal;
	public float KeepTime=3.0f;
	private BufferKind kind;
	int BaoGuoScore = 400;
	float ScoreTime = 0;
	public ChengJiu ChengJiuScript = null;

	// Use this for initialization
	void Start()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			UISprite deFenSprite = transform.parent.GetComponent<UISprite>();
			if (deFenSprite != null) {
				deFenSprite.enabled = false;
			}
			gameObject.SetActive(false);
			return;
		}

		intToImage(0);
		if (isFinal) {
			GlobalScript.GetInstance ().ShowFinalScoreEvent += ShowFinalScoreEvent;
		}
		else {
			GlobalScript.GetInstance().player.ScoreChange+=ScoreChange;
			GlobalScript.GetInstance().player.AddBufferEvent+=AddBufferEvent;
		}
	}
	private void ShowFinalScoreEvent()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}
		intToImage(GlobalScript.GetInstance().player.Score);
	}
	private void AddBufferEvent(BufferKind kind)
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}

		if(kind==BufferKind.Hanbao||kind==BufferKind.Jitui||kind==BufferKind.BaoGuo)
		{
			this.kind=kind;
			UpdateImage();
		}
	}
	public void ScoreChange()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}
		intToImage(GlobalScript.GetInstance().player.Score);
	}

	public void UpdateImage()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}

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
		PR="dfj";
		StartCoroutine(EndUpdateImage());
	}

	public IEnumerator EndUpdateImage()
	{
		yield return StartCoroutine(AddScore());
		yield return new WaitForSeconds(KeepTime);
		PR="df";
		TweenScale sc=GetComponent<TweenScale>();
		sc.from=new Vector3(1.2f,1.2f,1);
		sc.to=new Vector3(1,1,1);
		sc.ResetToBeginning();
		sc.PlayForward();
		EventDelegate.Remove(sc.onFinished,onfinished);
		EventDelegate.Add(sc.onFinished,onfinished1);
	}

	public void onfinished1()
	{
		PR="df";
		intToImage(GlobalScript.GetInstance().player.Score);
	}

	public IEnumerator AddScore()
	{
		int s = (int)kind;
		while(s>0)
		{
			//Debug.Log("AddScore -> score " + s);
			if(s>10)
			{
				GlobalScript.GetInstance().player.Score+=10;
			}
			else
			{
				GlobalScript.GetInstance().player.Score+=s;
			}
			s-=10;
			yield return new WaitForSeconds(0.0001f);
		}
	}

	void Update()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}

		if(IsAddBaoGuoScore && BaoGuoScore > 0)
		{
			addScoreBaoGuo();
		}
	}

	private bool _IsAddBaoGuoScore = false;
	public bool IsAddBaoGuoScore
	{
		get
		{
			return _IsAddBaoGuoScore;
		}
		set
		{
			if(value)
			{
				//Debug.Log("start add BaoGuo score");
				ChengJiuScript.callTweenScale();
			}
			_IsAddBaoGuoScore = value;
		}
	}

	public void resetAddScoreBaoGuo()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}
		IsAddBaoGuoScore = false;
		BaoGuoScore = 400;
	}

	public void addScoreBaoGuo()
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			return;
		}

		if(Time.realtimeSinceStartup - ScoreTime >= 0.03f)
		{
			ScoreTime = Time.realtimeSinceStartup;
			if(BaoGuoScore > 10)
			{
				GlobalScript.GetInstance().player.Score += 10;
			}
			else
			{
				GlobalScript.GetInstance().player.Score += BaoGuoScore;
			}
			BaoGuoScore -= 10;
		}
	}
}
