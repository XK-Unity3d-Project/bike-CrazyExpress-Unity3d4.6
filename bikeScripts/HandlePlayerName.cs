using UnityEngine;
using System.Collections;

public class HandlePlayerName : MonoBehaviour {

	Cloth cloth;
	void Start()
	{
		cloth = (Cloth)GetComponent<InteractiveCloth>();
	}

	void OnGUI()
	{
		if(GUILayout.RepeatButton("up"))
		{
			cloth.externalAcceleration = new Vector3(0, 1, 0);
		}
		
		if(GUILayout.RepeatButton("down"))
		{
			cloth.externalAcceleration = new Vector3(0, -1, 0);
		}

		if(GUILayout.RepeatButton("left"))
		{
			cloth.externalAcceleration = new Vector3(1, 0, 0);
		}

		if(GUILayout.RepeatButton("right"))
		{
			cloth.externalAcceleration = new Vector3(-1, 0, 0);
		}
	}

	//public Transform TranNPC;
	//public GUITexture textureObj;
//	Camera mainCamObj;
//	//Transform MianCam;
//	Transform tran;
//
//	// Use this for initialization
//	void Start () {
//		mainCamObj = Camera.main;
//		//MianCam = Camera.main.transform;
//		tran = transform;
//	}
//
//	Vector3 pos;
//	// Update is called once per frame
//	void Update () {
////		if(Time.frameCount % 10 == 0)
////		{
////			tran.forward = MianCam.forward;
////		}
//		//textureObj.
//		//Vector3 pos = Vector3.zero;
//		pos = mainCamObj.WorldToScreenPoint((TranNPC.position + Vector3.up * 2.0f));
//		//TranNPC.position
//		pos.x = pos.x / Screen.width;
//		pos.y = pos.y / Screen.height;
//		pos.z = 0.0f;
//
//		tran.position = pos;
//	}
}
