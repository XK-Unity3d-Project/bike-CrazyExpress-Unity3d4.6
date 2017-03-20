using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
	public static GameObject Player;
	public GameObject map;
	private Transform pos;
	// Use this for initialization
	void Start () {
		if(Player!=null)
		{
		pos=Player.transform;
		}
		//transform.localPosition=new Vector3(10,10,0);
	}

	void LateUpdate() {
	
		if(Player == null)
		{
			return;
		}
		else
		{
			pos = Player.transform;
		}

//		if(Time.frameCount % 200 == 0)
//		{
//			Debug.Log("player name " + Player.name + ", pos " + pos);
//		}
		map.transform.localPosition=new Vector3(1023-pos.position.x,1023-pos.position.z);
		transform.localEulerAngles=new Vector3(0,0,pos.rotation.eulerAngles.y);
		//transform.RotateAround(Vector3.zero,Vector3.forward,0.10f*Time.deltaTime);
	}
}
