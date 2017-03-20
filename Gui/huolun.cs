using UnityEngine;
using System.Collections;

public class huolun : MonoBehaviour {
  
	
    void Start () 
	{
	
	}
	void Update () 
	{
	    
		transform.Translate(Vector3.right*Time.deltaTime*10);
		if(transform.position.x>=370)
		{
			transform.position=new Vector3(294,61,809);
			
		}
		
		
	}
	
}
