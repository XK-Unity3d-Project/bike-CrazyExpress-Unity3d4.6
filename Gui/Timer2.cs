﻿using UnityEngine;
using System.Collections;

public class Timer2: MonoBehaviour {
	[HideInInspector]
	public UISprite fenshiUI;
	[HideInInspector]
	public UISprite fengeUI;
	[HideInInspector]
	public UISprite miaoshiUI;
	[HideInInspector]
	public UISprite miaogeUI;
	public GameObject fenshi;
	public GameObject fenge;
	public GameObject miaoshi;
	public GameObject miaoge;

	public void intToTimerImage(int time)
	{
		int miao= time%60;
		int fen=time/60;
		//Debug.Log("fen="+fen+"miao="+miao+"dsds"+GlobalScript.GetInstance().playerLife);
		intToImage(fen,miao);
	}
	public void intToImage(int fen,int miao)
	{
		fenshiUI.spriteName="d"+fen/10;
		fengeUI.spriteName="d"+fen%10;
		miaoshiUI.spriteName="d"+miao/10;
		miaogeUI.spriteName="d"+miao%10;
		//		xu_shiwei.spriteName=spriteList[(num/10)].name;
		//		xu_gewei.spriteName=spriteList[num%10].name;
	}
}
