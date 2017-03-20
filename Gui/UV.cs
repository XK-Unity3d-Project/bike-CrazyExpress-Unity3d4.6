using UnityEngine;
using System.Collections;

public class UV : MonoBehaviour {
	public float speed=0.5f;
	public int Array=1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		bool s=true;

	float offset = -Time.time *speed;
//		Debug.Log (offset);
  renderer.materials[Array].SetTextureOffset("_MainTex",new Vector2 (0,offset));

	
	}
}
