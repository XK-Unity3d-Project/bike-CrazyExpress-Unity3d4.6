using UnityEngine;
using System.Collections;

public class Station : MonoBehaviour {

	public GameObject[] Cars;
	public GameObject[] CarsNet;

	public GameObject EnterTrigger;
	public float KeepTime;
	public GameObject CarPath;

	public float refreshIntervalMin=1.0f;
	public float refreshIntervalMax=2.0f;
	public float TriggerInterval;

	private bool canTrigger = true; 

	// Use this for initialization
	void Start () {
		
		TriggerScript ts=(TriggerScript)EnterTrigger.GetComponent("TriggerScript");
		ts.TriggerEnter+=Enter;
	}

	public void Enter()
	{
		//ScreenLog.Log("test ********************* car event");
		if(EnterTrigger.activeSelf)
		{
			EnterTrigger.SetActive(false);
		}

		if(GlobalData.GetInstance().gameMode == GameMode.SoloMode)
		{
			if(Cars.Length <= 0)
			{
				return;
			}
		}
		else
		{
			if(CarsNet.Length <= 0 || FreeModeCtrl.IsServer)
			{
				return;
			}
		}

		if(canTrigger)
		{
	      StartCoroutine("RefreshCar");
	      Invoke("StopRefreshCar", KeepTime);
		
		  Invoke("CanTrigger", TriggerInterval);
		}

		canTrigger = false;
	}

	public void CanTrigger()
	{
		canTrigger = true;
	}

	public IEnumerator RefreshCar()
	{
		while(true)
		{
			float i = Random.Range(refreshIntervalMin, refreshIntervalMax);
			//ScreenLog.Log("spawn car time is " + i);
		  	yield return new WaitForSeconds(i);

			CreatCar();
		}
	}

//	int TestNetCarNum = 0;
	public void CreatCar()
	{
		if(GlobalData.GetInstance().gameMode == GameMode.OnlineMode)
		{
			if(CarsNet.Length <= 0)
			{
				return;
			}

			int carid = Random.Range(0, CarsNet.Length);
			int playerID = int.Parse(Network.player.ToString());

			GameObject newCar = (GameObject)Network.Instantiate(CarsNet[carid], transform.position,
			                                                     transform.rotation, playerID);

			CarNetCtrl carsc = (CarNetCtrl)newCar.GetComponent("CarNetCtrl");
			carsc.initCarNetInfo( CarPath );
		}
		else
		{
			if(Cars.Length <= 0)
			{
				return;
			}

			int carid = Random.Range(0, Cars.Length);
			GameObject newCar = (GameObject)Instantiate(Cars[carid], transform.position, transform.rotation);
			CarScript carsc = (CarScript)newCar.GetComponent("CarScript");

			carsc.CarPath = CarPath;
			carsc.isStart = true;
		}
	}

	public void StopRefreshCar()
	{
		StopCoroutine("RefreshCar");
	}
}