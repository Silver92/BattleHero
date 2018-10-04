using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerAttack : MonoBehaviour {

	public int shootingDamage = 1;				//玩家射击伤害
	public float shootingRange = 50.0f;			//玩家射击距离
	public float shootingInterval = 1.0f;
	public AudioClip shootingAudio;				//射击音效
	public GameObject GunShootingEffect;
	public GameObject bulletEffect;
	public Transform shootingEffectTransform;	//播放粒子效果的Transfrom属性

	private LineRenderer gunLine;		//线渲染器：射击时的激光射线效果
	private bool isShooting;			//玩家是否正在射击
	private Camera myCamera;			//摄像机组件
	private Ray ray;					
	private RaycastHit hitInfo;
	private GameObject instantiation;

	private float nextShootingTime;
	private float timer = 0.0f;

	private static float LINE_RENDERER_START=0.02f;	//射线初始宽度
	private static float LINE_RENDERER_END=0.05f;	//射线末端宽度

	//初始化函数，获取组件
	void Start () {
		gunLine = GetComponent<LineRenderer> ();		//获取线渲染器组件
		if (gunLine != null) gunLine.enabled = false;	//在游戏开始时禁用线渲染器组件
		myCamera = GetComponentInParent<Camera> ();		//获取父对象的摄像机组件
	}

	//每帧执行一次，在Update函数后调用，实现玩家射击行为
	void LateUpdate () {	
		isShooting=CrossPlatformInputManager.GetButton("Fire1");	//获取玩家射击键的输入
		//若在游戏进行中（Playing）获取玩家射击输入，则调用射击函数
		if (isShooting && timer >= shootingInterval && (GameManager.gm==null || GameManager.gm.gameState == GameManager.GameState.Playing)) {
			Shoot ();
			timer = 0;
		} else if (gunLine != null)	//若射击条件未满足，表示未进行射击，禁用线渲染器
			gunLine.enabled = false;
		
		timer += Time.deltaTime;
	}

	//射击函数
	void Shoot()
	{
		AudioSource.PlayClipAtPoint (shootingAudio, transform.position);	//播放射击音效
		//枪口闪光特效
		if (GunShootingEffect != null && shootingEffectTransform != null) {								
			(Instantiate (GunShootingEffect, 
				shootingEffectTransform.position, 
				shootingEffectTransform.rotation) as GameObject).transform.parent = shootingEffectTransform;
		}
		ray.origin = myCamera.transform.position;		//设置射线发射的原点：摄像机所在的位置
		ray.direction = myCamera.transform.forward;		//设置射线发射的方向：摄像机的正方向
		if (gunLine != null) {
			gunLine.enabled = true;							//进行射击时，启用线渲染器（激光射线效果）
			gunLine.SetPosition (0, transform.position);	//设置线渲染器（激光射线效果）第一个端点的位置：玩家枪械的枪口位置
		}
		//发射射线，射线有效长度为shootingRange，若射线击中任何游戏对象，则返回true，否则返回false
		if (Physics.Raycast (ray, out hitInfo, shootingRange)) {
			if (hitInfo.transform.gameObject.tag.Equals ("Enemy")) {	//当被击中的游戏对象标签为Enemy，表明射线击中敌人
				//获取敌人生命值组件
				ZombieHealth enemyHealth = hitInfo.transform.gameObject.GetComponent<ZombieHealth> ();
				if (enemyHealth != null) {
					//调用EnemyHealth脚本的TakeDamage（）函数，对敌人造成shootingDamage的伤害,注意，这里需要传入攻击者所在的位置
					enemyHealth.TakeDamage (shootingDamage, myCamera.transform.position);
				}
			}
			if (gunLine != null) {
				gunLine.SetPosition (1, hitInfo.point);	//当射线击中游戏对象时，设置线渲染器（激光射线效果）第二个端点的位置：击中对象的位置
				gunLine.SetWidth (LINE_RENDERER_START, 	//射线在射程内击中对象时，需要根据击中对象的位置动态调整线渲染器（激光射线效果）的宽度
					Mathf.Clamp ((hitInfo.point - ray.origin).magnitude / shootingRange, 
						LINE_RENDERER_START, LINE_RENDERER_END));
			}
			if (bulletEffect != null) {
				Instantiate (bulletEffect, hitInfo.point, Quaternion.identity);
			}
		} else {
			if (bulletEffect != null) {
				Instantiate (bulletEffect, ray.origin + ray.direction * shootingRange, Quaternion.identity);
			}

		}

//		if (gunLine != null) {
//			//当射线未击中游戏对象时，设置线渲染器（激光射线效果）第二个端点的位置：射线射出后的极限位置
//			gunLine.SetPosition (1, ray.origin + ray.direction * shootingRange);
//			//射线在射程内未击中对象，直接设置射线的初始与末尾宽度
//			gunLine.SetWidth (LINE_RENDERER_START, LINE_RENDERER_END);
//		}
	}
}
