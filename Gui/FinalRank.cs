using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class FinalRank : MonoBehaviour {
	public ChengJiu ChengJiuScript = null;
	
	public AudioClip RankAudio = null;

	private bool isLinkMode = false;
//	private UISprite UISpriteScript = null;
	private UITexture UITextureCom = null;
	public Texture[] TextureArray;
	public Texture[] TextureArrayCh;
	public Texture[] TextureArrayEn;
	public Texture[] TextureArrayRank;
	private TweenPosition TweenPosScript = null;

	private Transform RankTran = null;

	// Use this for initialization
	void Start()
	{
		GlobalScript.GetInstance ().ShowFinalRankEvent += ShowFinalRankEvent;
		//UISpriteScript = GetComponent<UISprite>();
		UITextureCom = GetComponent<UITexture>();
		GameTextType gameTextVal = GlobalData.GetGameTextMode();
		if (gameTextVal == GameTextType.Chinese) {
			TextureArray = TextureArrayCh;
		}
		else {
			TextureArray = TextureArrayEn;
		}
		TweenPosScript = GetComponent<TweenPosition>();
		RankTran = transform;
	}

	void ShowFinalRankEvent()
	{
		//Debug.Log("ShowFinalRankEvent...");
		if(isLinkMode)
		{
			AudioManager.Instance.PlaySFX( RankAudio );
			RankTran.localScale = new Vector3(1f, 1f, 1f);
			//UISpriteScript.spriteName = GlobalScript.GetInstance().player.FinalRank+"th";
			int rankNum = GlobalScript.GetInstance().player.FinalRank;
			UITextureCom.mainTexture = TextureArrayRank[rankNum - 1];
			TweenPosScript.PlayForward();
			EventDelegate.Add(TweenPosScript.onFinished, onFinishedTweenPos);
		}
		else
		{
			int count = ChengJiuScript.GetChengJiuCount();
			//count = 1; //test
			if(count <= 0)
			{
				onFinishedTweenPos();
				return;
			}

			AudioManager.Instance.PlaySFX( RankAudio );
			RankTran.localScale = new Vector3(2f, 1f, 1f);
			//UISpriteScript.spriteName = count.ToString();
			UITextureCom.mainTexture = TextureArray[count - 1];
			TweenPosScript.PlayForward();

			EventDelegate.Add(TweenPosScript.onFinished, onFinishedTweenPos);
		}
	}

	public caipiao caipiaoScript = null;
	void onFinishedTweenPos()
	{
		//Debug.Log("onFinishedTweenPos -> FinalRank");
		EventDelegate.Remove(TweenPosScript.onFinished, onFinishedTweenPos);
		caipiaoScript.BanckToStartSence();
	}
}
