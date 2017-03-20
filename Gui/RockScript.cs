using UnityEngine;
using System.Collections;

public class RockScript : MonoBehaviour {
	public float LifeTime=3.0f;
	public float Force=10;
	public float probability;

	private GameObject rockParticle = null;
	void Start()
	{
		//Debug.Log("transform.childCount " + transform.childCount);
		rockParticle = transform.GetChild(0).gameObject;
		if(rockParticle == null)
		{
			Debug.Log("rockParticle is null! name " + transform.name);
		}
	}

	public void AddForce()
	{
		//Debug.Log("2222222222222222222222222");
		if(transform)
		{
			if(rockParticle == null)
			{
				Start();
			}

			if(rockParticle != null)
			{
				rockParticle.SetActive(true);
			}


			transform.collider.enabled=true;
		    transform.GetComponent<Rigidbody>().isKinematic=false;
	
            // Debug.Log("+++++++++++++++"+(new Vector3(599f,46f,768f)-transform.position).x+(new Vector3(599f,46f,768f)-transform.position).z);
		    transform.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right*Force,ForceMode.Impulse);
		}
	}
	
	public void BenginDestoryColne()
	{
		StartCoroutine("DestoryColne");
	}

	public IEnumerator DestoryColne()
	{
		yield return new WaitForSeconds(LifeTime);
		Destroy(gameObject);
	}

	void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.red;
		Vector3 direction = transform.TransformDirection(Vector3.right) * 5;
		Gizmos.DrawRay(transform.position, direction);
	}
}
