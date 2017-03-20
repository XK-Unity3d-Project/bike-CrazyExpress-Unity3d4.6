using UnityEngine;
using System.Collections;

public class RedLightScript1 : MonoBehaviour {
public GameObject EnterTrigger;
	public GameObject ExitTrigger;
	public Material DRed;
	public Material IRed;
	public Material DYellow;
	public Material IYellow;
	public Material DGreen;
	public Material IGreen;
	public float RedTime;
	public float GreenTime;
	public float YellowTime;
	public LightColor CurrentLigth;
	private int redIndex=-1;
	private int yellowIndex=-1;
	private int greenIndex=-1;
	private Material[] materials; 
	private bool CanTrigger=true;
	private int PlayCount=0;
	private TriggerScript ts;
	private LightColor firstLight;
	// Use this for initialization
	void Start () {
			firstLight=CurrentLigth;
			GetMaterials();
	 GameObject[] gos=	GameObject.FindGameObjectsWithTag("Player");
		PlayCount=gos.Length;
		 ts=(TriggerScript)EnterTrigger.GetComponent("TriggerScript");	
		
	     RegisterEvent();
	
	}
	public void RegisterEvent()
	{
		
	    ts.TriggerEnter+=BrightLights;
	}
		public void UnregisterEvent()
	{
		ts.TriggerEnter-=BrightLights;
	}
	public void GetMaterials()
	{
	   materials=renderer.materials;
	  redIndex=InitMateralsIndexByName(DRed.name);
	   yellowIndex= InitMateralsIndexByName(DYellow.name);
	  greenIndex= InitMateralsIndexByName(DGreen.name);
	}
	public int InitMateralsIndexByName(string name)
	{
		int ind=-1;
		
		for(int i=0;i<materials.Length;i++)
		{

			if(materials[i].name.Contains(name))
				{
				ind= i;
				break;
				}

		}
		return ind;
		
	}	
	public void CloseLight()
	{
		
	}
	public void BrightLights()
	{
		//Debug.Log("triggeredtriggeredtriggeredtriggeredtriggeredtriggeredtriggered");
		PlayCount--;
		if(CanTrigger)
		{
		switch(CurrentLigth)
			{
			case LightColor.Red:
				BrightRedLight();
				break;
				case LightColor.Green:
				BrightYellowLight();
				break;
				case LightColor.Yellow:
				BrightGreenLight();
				break;
			}
			StartCoroutine("BeginBrightLights");
		}
		else
		{
			Debug.Log("can not trigger");
		}
		
	}
	public IEnumerator BeginBrightLights()
	{
		bool b=true;
		while(b)
		{
			switch(CurrentLigth)
			{
			case LightColor.Red:
				yield return new WaitForSeconds(RedTime);
				CurrentLigth=LightColor.Green;
				BrightGreenLight();
				break;
				case LightColor.Green:
				CanTrigger=false;
			
				Invoke("StopRegister",GreenTime);
				
					b=false;
				//StopCoroutine("BeginBrightLights");
	//			yield return new WaitForSeconds(GreenTime);
//				
//				CurrentLigth=LightColor.Yellow;
//					BrightYellowLight();
				//StartCoroutine("CloseTrigger");
				break;
				case LightColor.Yellow:
				yield return new WaitForSeconds(YellowTime);
			
				CurrentLigth=LightColor.Red;
				BrightRedLight();
				break;
			}
		}
	}
	public void StopRegister()
	{
		
		if(PlayCount>0)
		{
		CanTrigger=true;
			CurrentLigth=firstLight;
		}
		else
		{
			UnregisterEvent();
		}
	}
	public void BrightRedLight()
	{
		
		materials[redIndex]=IRed;
		materials[yellowIndex]=DYellow;
		materials[greenIndex]=DGreen;
		renderer.materials=materials;
	}
		public void BrightYellowLight()
	{
		materials[redIndex]=DRed;
		materials[yellowIndex]=IYellow;
		materials[greenIndex]=DGreen;
		renderer.materials=materials;
	}
		public void BrightGreenLight()
	{
		materials[redIndex]=DRed;
		materials[yellowIndex]=DYellow;
		materials[greenIndex]=IGreen;
		renderer.materials=materials;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
public enum LightColor
{
	Red,
	Yellow,
	Green
}