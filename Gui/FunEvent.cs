using UnityEngine;
using System.Collections;

using Frederick.ProjectAircraft;

public class FunEvent : MonoBehaviour {

	public AudioClip JiaShiDianAudio = null;

	// Use this for initialization
	//public GameObject imag;
	public float BaoGuoduation;
	public float backduation;
	//public float duation;
	public Vector3 from;
	public Vector3 to;
	private bool playing;
	//private UISprite ui;
	private UITexture UiTextureCom;
	/**
	 * TextureArray[0] -> "1_01" -- suDu.
	 * TextureArray[1] -> "1_06" -- Baoguo_1.
	 * TextureArray[2] -> "1_04" -- Baoguo_2.
	 * TextureArray[3] -> "1_05" -- Daojishi.
	 * TextureArray[4] -> "2_01" -- Diedao.
	 * TextureArray[5] -> "1_02" -- Jiashidian.
	 * TextureArray[6] -> "2_01" -- Luduan.
	 */
	public Texture[] TextureArray;
	public Texture[] TextureArrayCh;
	public Texture[] TextureArrayEn;
	private float localDuation=-1;
	private Transform trans;
	private TweenPosition tween;
	private TishiInfo tishi;
	//private float speed;
	private float oldduation;
	private float diedaoShijian;

	void Start()
	{
		GameTextType gameTextVal = GlobalData.GetGameTextMode();
		GlobalScript.GetInstance ().ShowTishiEvent += ShowTishiEvent;
		//ui = GetComponent<UISprite> ();
		UiTextureCom = GetComponent<UITexture> ();
		if (gameTextVal == GameTextType.Chinese) {
			TextureArray = TextureArrayCh;
		}
		else {
			TextureArray = TextureArrayEn;
			UiTextureCom.mainTexture = TextureArray[0];
		}

		tween = GetComponent<TweenPosition> ();
		trans = gameObject.transform;
		oldduation = tween.duration;
		//speed = Mathf.Abs (to.x - from.x) /tween.duration;
	}

/*
	string suDuSpriteName = "1_01";
	string BaoguoSpriteName = "1_06";
	string BaoguoSpriteName_1 = "1_04";
	string DaojishiSpriteName = "1_05";
	string DiedaoSpriteName = "2_01";
	string JiashidianSpriteName = "1_02";
	string LuduanSpriteName = "2_01";
*/
	void ShowTishiEvent(TishiInfo tishi)
	{
		if(playing && tishi != TishiInfo.Jiashidian)
		{
			//ScreenLog.Log("ShowTishiEvent -> tishi " + tishi + ", this.tishi " + this.tishi);
			return;
		}

		//ScreenLog.Log ("ShowTishiEvent -> tiShi " + tishi.ToString());
		if (tishi == TishiInfo.Diedao) 
		{
			diedaoShijian = Time.timeSinceLevelLoad;
			//ScreenLog.Log("diedaoshijian"+diedaoShijian);
		}

		if (playing) 
		{
			//ScreenLog.Log("is playing"+playing+this.tishi+tishi);
			if(this.tishi == TishiInfo.Diedao && tishi == TishiInfo.Sudu)
			{
				float detTime = Time.timeSinceLevelLoad - diedaoShijian;
				if(detTime < 0.5f)
				{
					return;
				}
			}

			StopAllCoroutines();
			tween.enabled = false;
			tween.from = trans.localPosition;
			tween.to = from;

			tween.ResetToBeginning(); 
			tween.PlayForward();

			localDuation = -1;
			tween.onFinished.Clear();

			playing = false;
			this.tishi = tishi;
			StartPlay();
			//EventDelegate.Add(tween.onFinished, StartPlay);
		}
		else
		{
			this.tishi = tishi;
			StartPlay();
		}
	}

	void StartPlay()
	{
		//low speed ctrl
		//if(playing && ui.spriteName == suDuSpriteName)
		if(playing && UiTextureCom.mainTexture.name == TextureArray[0].name)
		{
			return;
		}

		tween.onFinished.Clear();
		localDuation = -1;
		switch (tishi)
		{
		case TishiInfo.Baoguo:
			IsBackInfo = false;
			//ui.spriteName = BaoguoSpriteName;
			UiTextureCom.mainTexture = TextureArray[1];
			localDuation=BaoGuoduation;
			//GlobalScript.GetInstance().player.AddBuffer(BufferKind.BaoGuo);

			//Time.timeScale = 0;
			break;

		case TishiInfo.Daojishi:
			//ui.spriteName = DaojishiSpriteName;
			UiTextureCom.mainTexture = TextureArray[3];
			break;

		case TishiInfo.Diedao:
			//ui.spriteName = DiedaoSpriteName;
			UiTextureCom.mainTexture = TextureArray[4];
			break;

		case TishiInfo.Jiashidian:
			//ScreenLog.Log("show addPoint info");
			//ui.spriteName = JiashidianSpriteName;
			UiTextureCom.mainTexture = TextureArray[5];
			if(JiaShiDianAudio != null)
			{
				AudioManager.Instance.PlaySFX(JiaShiDianAudio);
			}
			GlobalScript.GetInstance().AddTime();
			GlobalScript.GetInstance().player.AddBuffer(BufferKind.Jiashidian);
			break;

		case TishiInfo.Luduan:
			//ui.spriteName = LuduanSpriteName;
			UiTextureCom.mainTexture = TextureArray[6];
			break;

		case TishiInfo.Sudu:
			//ui.spriteName = suDuSpriteName;
			UiTextureCom.mainTexture = TextureArray[0];
			break;
		}

		playing = true;
		tween.from = from;
		tween.to = to;
		tween.duration = oldduation;
		tween.ResetToBeginning();
		tween.PlayForward ();

		if(Time.timeScale > 0f )
		{
			StartCoroutine ("Back");
		}
	}

	public bool IsBackInfo = false;
	public void BackInfo()
	{
		if(IsBackInfo)
		{
			return;
		}
		IsBackInfo = true;

		tween.ResetToBeginning();
		tween.from = to;
		tween.to = from;
		tween.duration = oldduation;
		//ScreenLog.Log ("BackInfo -> " + localDuation + tishi.ToString() + playing);
		tween.PlayForward ();
		if (localDuation != -1) 
		{
			EventDelegate.Add (tween.onFinished, Onfinished);
		}
		else
		{
			StopAllCoroutines();
		}
	}

	IEnumerator Back()
	{
		yield return new WaitForSeconds(backduation);
		tween.ResetToBeginning();
		tween.from = to;
		tween.to = from;
		tween.duration = oldduation;

		//ScreenLog.Log ("Back -> " + localDuation + tishi.ToString() + playing);
		tween.PlayForward ();
		if (localDuation != -1) 
		{
			EventDelegate.Add (tween.onFinished, Onfinished);	
		}
		else
		{
			playing = false;
			//ScreenLog.Log("Back -> test **********");
			StopAllCoroutines();		
		}
	}

	void Onfinished()
	{
		playing = false;
		//ScreenLog.Log ("Onfinished -> " + localDuation+tishi.ToString() + playing);
		if (localDuation != -1) 
		{
			tween.onFinished.Clear ();
			StartCoroutine ("Baoguo");
		} 
	}

	IEnumerator Baoguo()
	{
		playing = true;
		yield return new WaitForSeconds (localDuation);
		//ui.spriteName = BaoguoSpriteName_1;
		UiTextureCom.mainTexture = TextureArray[2];
		tween.from = from;
		tween.to = to;
		tween.duration = oldduation;
		tween.ResetToBeginning();
		tween.PlayForward ();
		localDuation = -1;

		//ScreenLog.Log("Baoguo...");
		StartCoroutine("Back");
	}

//  void OnGUI()
//	{
//		if (GUI.Button (new Rect (0, 0, 80, 40), "fun")) {
//				
//			GlobalScript.GetInstance().ShowTishi(TishiInfo.Jiashidian);
//		}
//	}
}
