using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeaponSwitcher : MonoBehaviour {

	public Transform[] weaponList;	//玩家武器列表

	private IKController ikController;	//玩家IK控制器
	private int currentIdx = 0;	//当前使用的枪序号
	private int weaponNum = 0;	//武器列表中的枪总数

	void Start () {
		//获取玩家IK控制器
		ikController = transform.GetComponent<IKController> ();	
		//获取玩家当前枪的数量
		weaponNum = weaponList.Length;
		//设置当前枪序号为0；
		currentIdx = 0;
		//让玩家使用下一把抢
		changeNextWeapon ();
	}

	void Update () {
		//玩家按下Fire2，执行换枪逻辑
		if (CrossPlatformInputManager.GetButtonDown("Fire2")) {
			changeNextWeapon ();
		}
	}

	public void changeNextWeapon()
	{
		//只有武器列表中的枪支数量大于1，才执行换枪动作
		if (weaponNum <= 1) 
			return;

		//取出下一把枪的序号
		int newIdx = (currentIdx + 1) % weaponNum;

		//使用下一把枪的IK标记物，设置玩家角色的IK，使玩家正确持枪
		Transform newWeapon = weaponList [newIdx];
		Transform rightHand = newWeapon.Find ("RightHandObj");
		Transform leftHand  = newWeapon.Find ("LeftHandObj");
		Transform gunBarrelEnd = newWeapon.Find ("GunBarrelEnd");
		ikController.leftHandObj = leftHand;
		ikController.rightHandObj = rightHand;
		ikController.lookObj = gunBarrelEnd;

		//激活新枪，禁用旧枪
		newWeapon.gameObject.SetActive (true);
		weaponList [currentIdx].gameObject.SetActive (false);

		//更新当前使用的武器序号
		currentIdx = newIdx;
	}
}
