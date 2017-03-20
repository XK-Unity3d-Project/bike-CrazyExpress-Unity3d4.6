using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Frederick.ProjectAircraft;
public class Toubi : MonoBehaviour {
	
	public GameObject RootObj;
	public GameObject StartPageObj;

	//public int xutoubi=2;
	//private int yitoubi=0;
	public GameObject G_yi_shiwei;
	public GameObject G_yi_gewei;
	public GameObject G_xu_shiwei;
	public GameObject G_xu_gewei;

	public AudioClip audioTouBi;
	private UISprite yi_shiwei;
	private UISprite yi_gewei;
	private UISprite xu_shiwei;
	private UISprite xu_gewei;
	private UIAtlas atlas;
	static Toubi _Instance;
	public static Toubi GetInstance()
	{
		return _Instance;
	}
	//private List<UISpriteData> spriteList;
	// Use this for initialization
	void Start () {
		_Instance = this;
		InitSprite();
		//spriteList=yi_gewei.atlas.spriteList;
//		if(GlobalData.GetInstance().xutoubi==0)
//		{
//		GlobalData.GetInstance().xutoubi=xutoubi;
//		}
		GlobalData.GetInstance().IconCountChange+=IconCountChange;
		ConvertNumToImg("xu",GlobalData.GetInstance().XUTOUBI);
		ConvertNumToImg("yi",GlobalData.GetInstance().Icon);

		//Toubi.PlayerPushCoin( 10 ); //test
	}

	public void IconCountChange()
	{
		AudioManager.Instance.PlaySFX(audioTouBi);
		ConvertNumToImg("yi",GlobalData.GetInstance().Icon);
	}

	public void ConvertNumToImg(string mod,int num)
	{
		if(mod=="yi")
		{
			if(num>99)
			{
				yi_shiwei.name="9";
				yi_gewei.name="9";
			}
			else
			{
				int coinShiWei = (int)((float)num/10.0f);
				//ScreenLog.Log("********* coinShiWei " + coinShiWei);
				yi_shiwei.spriteName = coinShiWei.ToString();
				yi_gewei.spriteName = (num%10).ToString();
			}
		}
		else if(mod=="xu")
		{
			xu_shiwei.spriteName=(num/10).ToString();
			xu_gewei.spriteName=(num%10).ToString();
		}
	}

	public void InitSprite()
	{
		yi_shiwei=G_yi_shiwei.GetComponent<UISprite>() as UISprite;
		yi_gewei=G_yi_gewei.GetComponent<UISprite>() as UISprite;
		xu_gewei=G_xu_gewei.GetComponent<UISprite>() as UISprite;
		xu_shiwei=G_xu_shiwei.GetComponent<UISprite>() as UISprite;
	}

	public void subPlayerCoin()
	{
		if(GlobalData.GetInstance().Icon >= GlobalData.GetInstance().XUTOUBI)
		{
			GlobalData.GetInstance().Icon -= GlobalData.GetInstance().XUTOUBI;
			pcvr.GetInstance().SubPlayerCoin( GlobalData.GetInstance().XUTOUBI );
			ConvertNumToImg("yi", GlobalData.GetInstance().Icon);
		}
	}

	int CoinNum;
	// Update is called once per frame
	void Update () {

		if(Network.isServer)
		{
			return;
		}

		if(Time.frameCount % 10 == 0)
		{
			ConvertNumToImg("xu", GlobalData.GetInstance().XUTOUBI);
		}

//		if(!pcvr.bIsHardWare)
//		{
//			if( Input.GetKeyUp(KeyCode.T) )
//			{
//				GlobalData.GetInstance().Icon++;
//			}
//		}

		if(GlobalData.GetInstance().Icon >= GlobalData.GetInstance().XUTOUBI)
		{
			if(Application.loadedLevel == (int)GameLeve.Movie && StartPageObj != null && !StartPageObj.activeSelf)
			{
				if(RootObj != null)
				{
					MediaPlayer.stopPlayMove();
					RootObj.SetActive(false);
					StartPageObj.SetActive(true);
				}
				else
				{
					ScreenLog.LogWarning("Toubi -> RootObj is null");
				}
			}
		}
	}

	public static void PlayerPushCoin( int coin )
	{
		GlobalData.GetInstance().Icon = coin;
		//Debug.Log("coin "+coin);
	}
}
