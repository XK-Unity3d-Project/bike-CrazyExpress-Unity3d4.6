using UnityEngine;
using System.Collections;

public class HidenTerrain : MonoBehaviour {
	public GameObject TerrainObj;
	
	void OnTriggerEnter(Collider other)
	{
		TerrainObj.SetActive(false);
		gameObject.SetActive(false);
	}
}
