using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

//玩家手电控制脚本
public class FlashLightController : MonoBehaviour {

	private Light mylight;

	void Start () {
		//获取玩家的聚光灯对象
		mylight = GetComponent<Light> ();
	}	

	void Update () {
		//根据玩家的输入，开启或关闭手电（调整聚光灯对象的亮度）
		if (CrossPlatformInputManager.GetButtonDown ("Fire3")) {
			if (mylight.intensity < 0.1)
				mylight.intensity = 5f;
			else
				mylight.intensity = 0.0f;
		}
			
	}
}
