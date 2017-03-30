using UnityEngine;
using System.Collections;
using System.IO;

using Frederick.ProjectAircraft;

public class Screenshot : MonoBehaviour {
	public GameObject Texcure;
	public GameObject cam1;
	public float duation=1.0f;
	
	public AudioClip PaiZhaoAudio = null;
	private bool isPlayPaiZhao = false;

	private Camera[] cameras;
	private Material material;

	private ColorCorrectionCurves colorCtrl;

	public static bool IsShootPlayer = false;

	// Use this for initialization
	void Start () {
		IsShootPlayer = false;
		if(cam1 != null)
		{
			colorCtrl = cam1.GetComponent<ColorCorrectionCurves>();
		}

		cameras=Camera.allCameras;
		material = Texcure.GetComponent<UITexture> ().material;
		GlobalScript.GetInstance ().player.IsPassChange += IsPassChange;
	}

	void IsPassChange()
	{
//		if (pcvr.bIsHardWare) {
//			MyCOMDevice.GetInstance().ForceRestartComPort();
//		}

		pcvr.CloseGameDongGan();
		if(!isPlayPaiZhao)
		{
			//Debug.Log("IsPassChange -> play audio");
			isPlayPaiZhao = true;
			AudioManager.Instance.PlaySFX(PaiZhaoAudio);
			caipiao.OnPlayerGameOver();
		}
		GetCapture();
	}

	private void DisableOthersCam()
	{
		foreach (Camera c in cameras)
		{
			c.enabled=false;
	    }
		gameObject.camera.enabled=true;
	}
	// Update is called once per frame

//    void OnGUI()
//	{
//		if(GUI.Button(new Rect(20,80,80,40),"Screenshot"))
//		{
//			//Application.CaptureScreenshot("背景图.png");
////			StartCoroutine("GetCapture");
//	GetCapture();
//		}
//	}
	void GetCapture()
	{
		if(colorCtrl != null)
		{
			colorCtrl.saturation = 0f;
		}

		int width = Screen.width;
		
		int height = Screen.height;
		//Debug.Log("w " + width + "h " + height);
		// 创建一个RenderTexture对象  
		RenderTexture rt = new RenderTexture(width, height, 24);  
		// 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
		cam1.camera.targetTexture = rt;  
		cam1.camera.Render();  
		//ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。  
		//ps: camera2.targetTexture = rt;  
		//ps: camera2.Render();  
		//ps: -------------------------------------------------------------------  
		
		// 激活这个rt, 并从中中读取像素。  
		RenderTexture.active = rt;  
		Texture2D tex = new Texture2D(width,height,TextureFormat.RGB24,false);
		//yield return new WaitForEndOfFrame();
		tex.ReadPixels(new Rect(0,0,width,height),0,0,true);
		tex.Apply ();
		cam1.camera.targetTexture = null;  
		//ps: camera2.targetTexture = null;  
		RenderTexture.active = null; // JC: added to avoid errors  
		GameObject.Destroy(rt);  

		//byte[] imagebytes = tex.EncodeToPNG();//转化为png图
		
		tex.Compress(false);//对屏幕缓存进行压缩

		material.mainTexture=tex;

		Texcure.SetActive(true);

		DisableOthersCam();
		StartCoroutine ("Bianse");
		//image.mainTexture = tex;//对屏幕缓存进行显示（缩略图）
		
		//File.WriteAllBytes(Application.dataPath + "/screencapture.png",imagebytes);//存储png图
		//Application.CaptureScreenshot(Application.dataPath+"/screencapture.png");
	}
	IEnumerator Bianse()
	{
		float s = 1.0f;
		while (s>=0)
		{
			s-=Time.deltaTime/duation;
			//material.SetFloat("_Saturation",s);
			//Texcure.GetComponent<UITexture>().MarkAsChangedLite();
			//Texcure.GetComponent<UITexture>().enabled=false;
			//Texcure.GetComponent<UITexture>().enabled=true;
//			Debug.Log("mmmmmmmmmm");
			yield return 0;
		}

		gameObject.GetComponent<EndPage> ().isStart = true;
		GlobalScript.GetInstance ().ShowEndPage();
		GlobalScript.GetInstance ().ShowLostTime ();
		GlobalScript.GetInstance ().ShowFinalScore ();

		IsShootPlayer = true;
	}

}
