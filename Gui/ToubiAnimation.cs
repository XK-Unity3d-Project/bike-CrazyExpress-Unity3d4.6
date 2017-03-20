using UnityEngine;
using System.Collections;

public class ToubiAnimation : MonoBehaviour {
	public bool CanStart;
	public float initInterval=0.5f;
	private float interval;
	private UISprite ui;
	// Use this for initialization
	void Start () {
		ui=GetComponent<UISprite>() as UISprite;
		interval=initInterval;
	}
	
	// Update is called once per frame
	void Update () {
		if(CanStart&&ui)
		{
			if(ui.spriteName=="qingTouBi")
			{
				ui.spriteName="StartBtDown";
			}
			else if(ui.spriteName=="StartBtDown"&&interval<=0)
			{
				ui.spriteName="StartBtUp";
				interval=initInterval;
			}
			else if(ui.spriteName=="StartBtUp"&&interval<=0)
			{
				ui.spriteName="StartBtDown";
				interval=initInterval;
			}
			interval-=Time.deltaTime;
		}
	}
}
