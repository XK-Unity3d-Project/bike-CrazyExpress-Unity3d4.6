using UnityEngine;
using System.Collections;

public class StartAnimation : MonoBehaviour {
	public float interval=0.5f;
	private UISprite ui;
	// Use this for initialization
	void Start () {
		pcvr.IsOpenStartLight = true;

		ui=GetComponent<UISprite>() as UISprite;
		InvokeRepeating("FrameAnimation",interval,1);
	}

	public void FrameAnimation()
	{
		if(ui.spriteName=="StartBtDown")
		{
			ui.spriteName="StartBtUp";
		}
		else 
		{
			ui.spriteName="StartBtDown";
		}
	}
}
