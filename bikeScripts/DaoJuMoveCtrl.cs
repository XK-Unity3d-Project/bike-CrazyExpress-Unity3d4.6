using UnityEngine;
using System.Collections;

public class DaoJuMoveCtrl : MonoBehaviour {

	public BufferKind DaoJuType;
	[Range(0.0f, 10.0f)] public float MoveSpeed = 1.0f;
	[Range(0.0f, 10.0f)] public float AddMoveSpeed = 1.0f;
	public Vector3 EndPos;
	public GameObject BaoZhaObj;

	Vector3 StartPos = new Vector3(0.0f, 0.0f, 4.0f);
	Transform tranObj;
	Vector3 moveVec;
	float disMax = 0.0f;

	// Use this for initialization
	void Start () {
		BaoZhaObj.SetActive(true);

		tranObj = transform;
		tranObj.localPosition = StartPos;
		EndPos.z = 4.0f;

		moveVec = EndPos - tranObj.localPosition;
		moveVec = moveVec.normalized;
		disMax = Vector3.Distance(tranObj.localPosition - EndPos, Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () {
		MoveSpeed += AddMoveSpeed;
		tranObj.localPosition = moveVec * MoveSpeed + tranObj.localPosition;
		//ScreenLog.Log("pos " + tranObj.localPosition);

		float moveDis = Vector3.Distance(tranObj.localPosition - StartPos, Vector3.zero);
		if(disMax <= moveDis)
		{
			GlobalScript.GetInstance().player.AddBuffer(DaoJuType);
			Destroy(gameObject);
		}
	}
}
