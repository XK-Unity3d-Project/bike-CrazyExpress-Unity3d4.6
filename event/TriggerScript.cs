using UnityEngine;
using System.Collections;

public class TriggerScript : MonoBehaviour {
	public delegate void EventHandel();
	public event EventHandel TriggerEnter;
	public event EventHandel TriggerExit;
	void OnTriggerEnter(Collider other) {
		if(TriggerEnter!=null)
		{
			if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
			{
				GameObject player = other.gameObject;
				bikeNetUnity bikeScript = player.GetComponent<bikeNetUnity>();
				if(bikeScript == null || !bikeScript.CheckPlayerClient())
				{
					//ScreenLog.LogWarning("OnTriggerEnter -> player client is false");
					return;
				}
			}

			//ScreenLog.Log(this.name + " ********************** "+other.name);
			TriggerEnter();
		}
	}

	void OnTriggerExit(Collider other) {
		if(TriggerExit!=null)
		{
			TriggerExit();
		}
	}
}
