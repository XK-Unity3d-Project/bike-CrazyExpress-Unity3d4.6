using UnityEngine;
using System.Collections;

public class AiPathCtrl : MonoBehaviour {

	public int KeyState;
	public Transform mNextPath1 = null;
	public Transform mNextPath2 = null;
	
	// Use this for initialization
	void Start () {
		int count = transform.childCount;
		for(int i = 0; i < count; i++)
		{
			Transform mark = transform.GetChild(i);
			AiMark markScript = mark.GetComponent<AiMark>();
			markScript.setMarkCount( i );
			if(i < (count - 1))
			{
				markScript.mNextMark = transform.GetChild(i + 1);
			}
			else
			{
				if(mNextPath1 != null && mNextPath1.childCount > 0)
				{
					markScript.mNextMark = mNextPath1.GetChild(0);
				}
				else if(mNextPath2 != null && mNextPath2.childCount > 0)
				{
					markScript.mNextMark = mNextPath2.GetChild(0);
				}
			}
		}

		AiPathCtrl aiPath = gameObject.GetComponent<AiPathCtrl>();
		aiPath.enabled = false;
	}
}
