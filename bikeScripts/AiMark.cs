using UnityEngine;
using System.Collections;

public class AiMark : MonoBehaviour {
	
//	public float mSpeed = 1f; //m/s
	private Transform _mNextMark = null;
	public Transform mNextMark
	{
		get
		{
			return _mNextMark;
		}
		set
		{
			_mNextMark = value;
		}
	}

	private int mMarkCount = 0;

	public void setMarkCount( int count )
	{
		mMarkCount = count;
	}

	public int getMarkCount()
	{
		return mMarkCount;
	}

	// Use this for initialization
	void Start () {
//		MeshRenderer mesh = GetComponent<MeshRenderer>();
//		if(mesh)
//		{
//			//mesh.enabled = false;
//		}
		Vector3 scale = transform.localScale;
		scale.y = 2f;
		transform.localScale = scale;

		tag = "mark";

		AiMark aiMark = gameObject.GetComponent<AiMark>();
		aiMark.enabled = false;
	}
}
