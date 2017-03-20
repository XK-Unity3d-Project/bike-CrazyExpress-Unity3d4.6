using UnityEngine;
using System.Collections;

public class diaoche : MonoBehaviour {
	
	public NetworkPlayer ownerPlayer;

	public GameObject trigger;
	private Animator animator;

	// Use this for initialization
	void Start () {
	
		trigger.GetComponent<TriggerScript>().TriggerEnter += TriggerEnter;
		animator = GetComponent<Animator>();
	}

	public void TriggerEnter()
	{
		animator.SetBool("diao",true);
		//Invoke("Rest",animator.GetCurrentAnimatorStateInfo(0).length);

		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			networkView.RPC("sendToServerPlayAction", RPCMode.OthersBuffered, "1");
		}
	}

	public void Rest()
	{
		animator.SetBool("diao",false);
	}

	// Update is called once per frame
	void Update()
	{
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("zhuan"))
		{
			animator.SetBool("diao", false);

			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				networkView.RPC("sendToServerPlayAction", RPCMode.OthersBuffered, "0");
			}
		}
	}

	[RPC]
	void sendToServerPlayAction(string key)
	{
		switch(key)
		{
		case "0":
			if(animator.GetCurrentAnimatorStateInfo(0).IsName("zhuan"))
			{
				animator.SetBool("diao",false);
			}
			break;

		case "1":
			animator.SetBool("diao",true);
			break;
		}
	}

//	void OnGUI()
//	{
//		if(GUI.Button(new Rect(20,20,80,40),"test"))
//		{
//			TriggerEnter();
//		}
//	}
}
