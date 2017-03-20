using UnityEngine;
using System.Collections;

public class LurenZhui : MonoBehaviour {
	public GameObject Trigger;
	public float Speed=20.0f;
	public GameObject target;
	private bool pao;
	private CharacterController cc;
	// Use this for initialization
	void Start () {
		Trigger.GetComponent<TriggerScript>().TriggerEnter+=TriggerEnter;
		cc=GetComponent<CharacterController>();
	}

	public void TriggerEnter()
	{
		pao=true;
		GetComponent<Animator>().SetBool("pao",true);
	}
	// Update is called once per frame
	void Update () {
	if(pao)
		{
			Quaternion q=Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(target.transform.position-transform.position),Time.deltaTime);
			transform.rotation=q;
			cc.SimpleMove(transform.TransformDirection( Vector3.forward)*Speed);
		}
	
	}
}
