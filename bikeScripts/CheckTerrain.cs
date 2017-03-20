using UnityEngine;
using System.Collections;

public class CheckTerrain : MonoBehaviour {

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("OnCollisionEnter -> collision " + collision.gameObject.name);
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			bike bikeScript = collision.gameObject.GetComponent<bike>();
			if(bikeScript != null)
			{
				bikeScript.setParticleState( true );
			}
		}
		else
		{
			bikeNetUnity bikeScript = collision.gameObject.GetComponent<bikeNetUnity>();
			if(bikeScript != null)
			{
				bikeScript.setParticleState( true );
			}
		}
	}
	
//	void OnCollisionStay (Collision collision)
//	{
//		//Debug.Log("OnCollisionStay -> collision " + collision.gameObject.name);
//		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
//		{
//			bike bikeScript = collision.gameObject.GetComponent<bike>();
//			if(bikeScript != null)
//			{
//				bikeScript.setParticleState( true );
//			}
//		}
//		else
//		{
//			bikeNetUnity bikeScript = collision.gameObject.GetComponent<bikeNetUnity>();
//			if(bikeScript != null)
//			{
//				bikeScript.setParticleState( true );
//			}
//		}
//	}
	
	void OnCollisionExit (Collision collision)
	{
		//Debug.Log("OnCollisionExit -> collision " + collision.gameObject.name);
		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			bike bikeScript = collision.gameObject.GetComponent<bike>();
			if(bikeScript != null)
			{
				bikeScript.setParticleState( false );
			}
		}
		else
		{
			bikeNetUnity bikeScript = collision.gameObject.GetComponent<bikeNetUnity>();
			if(bikeScript != null)
			{
				bikeScript.setParticleState( false );
			}
		}
	}
}
