using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class ChengJiu : MonoBehaviour {
	public AudioClip ChengJiuAudio = null;

	private TweenPosition TweenPosScript = null;
	private TweenScale TweenScaleScript = null;

	private bool isStartMove = false;
	private float moveTime = 0f;
	
	private int chengJiuCount = 0;
	private ChengJiu chengJiuScript = null;
	//private UISprite uiSpriteScript = null;
	private UITexture UITextureCom = null;
	public Texture[] TextureArray;
	public Texture[] TextureArrayCh;
	public Texture[] TextureArrayEn;
	public Score ScoreScript = null;

	// Use this for initialization
	void Awake () {
		chengJiuScript = GetComponent<ChengJiu>();
		chengJiuScript.enabled = false;

//		uiSpriteScript = GetComponent<UISprite>();
//		uiSpriteScript.enabled = false;
		
		UITextureCom = GetComponent<UITexture>();
		UITextureCom.enabled = false;
		GameTextType gameTextVal = GlobalData.GetGameTextMode();
		if (gameTextVal == GameTextType.Chinese) {
			TextureArray = TextureArrayCh;
		}
		else {
			TextureArray = TextureArrayEn;
		}
		TweenPosScript = GetComponent<TweenPosition>();
		TweenScaleScript = GetComponent<TweenScale>();
	}
	
	// Update is called once per frame
	void Update () {
		checkMoveChengJiu();
	}

	public int GetChengJiuCount()
	{
		return chengJiuCount;
	}

	public void callTweenScale()
	{
		if(chengJiuCount >= 5)
		{
			Debug.Log("BaoGuo have send over!");
			return;
		}
		
		//Debug.Log("play chengJiu audio");
		AudioManager.Instance.PlaySFX( ChengJiuAudio );

		chengJiuScript.enabled = true;
		//uiSpriteScript.enabled = true;
		UITextureCom.enabled = true;

		chengJiuCount++;
//		switch(chengJiuCount)
//		{
//		case 1:
//			uiSpriteScript.spriteName = "1";
//			break;
//		case 2:
//			uiSpriteScript.spriteName = "2";
//			break;
//		case 3:
//			uiSpriteScript.spriteName = "3";
//			break;
//		case 4:
//			uiSpriteScript.spriteName = "4";
//			break;
//		case 5:
//			uiSpriteScript.spriteName = "5";
//			break;
//		}
		UITextureCom.mainTexture = TextureArray[chengJiuCount - 1];

		TweenPosScript.ResetToBeginning();
		TweenScaleScript.ResetToBeginning();

		TweenScaleScript.PlayForward();

		EventDelegate.Add(TweenScaleScript.onFinished, onFinishedScale);
	}

	void onFinishedScale()
	{
		//Debug.Log("onFinishedScale -> test");
		EventDelegate.Remove(TweenScaleScript.onFinished, onFinishedScale);

		startMoveChengJiu();
	}

	void startMoveChengJiu()
	{
		moveTime = Time.realtimeSinceStartup;
		isStartMove = true;
	}

	void checkMoveChengJiu()
	{
		if(isStartMove)
		{
			if(Time.realtimeSinceStartup - moveTime > 1.5f)
			{
				isStartMove = false;
				callTweenPostion();
			}
		}
	}

	void callTweenPostion()
	{
		TweenPosScript.ResetToBeginning();
		TweenPosScript.PlayForward();
		EventDelegate.Add(TweenPosScript.onFinished, OnFinisedPos);
	}

	void OnFinisedPos()
	{
		//Debug.Log("OnFinisedPos -> ChengJiu play over");
		Time.timeScale = 1f;
		chengJiuScript.enabled = false;
		UITextureCom.enabled = false;
		ScoreScript.resetAddScoreBaoGuo();

		EventDelegate.Remove(TweenPosScript.onFinished, OnFinisedPos);
	}
}
