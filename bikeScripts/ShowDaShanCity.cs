using UnityEngine;
using System.Collections;

public class ShowDaShanCity : MonoBehaviour {
	public GameObject DaShanObj;

	void Start()
	{
		DaShanObj.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		DaShanObj.SetActive(true);
	}
}
