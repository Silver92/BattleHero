using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieSensor : MonoBehaviour {

	public float SoundRange = 15.0f;	//僵尸听觉距离
	public float SightRange = 25.0f;	//僵尸视觉距离
	public float SightAngle = 60;		//僵尸的视觉夹角
	public float SensorInterval = 0.5f;	//僵尸的感知时间间隔

	private float senseTimer = 0.0f;	//统计僵尸的感知时间

	private Transform zombieTransform;	//僵尸的位置
	private Transform nearbyPlayer;		//僵尸周围玩家对象的缓存
	private Transform zombieEye;		//僵尸眼睛的位置

	void Start()
	{
		//获取僵尸的transform组件
		zombieTransform = transform;						
		//获取僵尸眼睛的transform组件
		zombieEye = transform.Find ("eye").transform;
	}

	void FixedUpdate()
	{
		//以一定的时间间隔，进行感知
		if (senseTimer >= SensorInterval) {
			senseTimer = 0;
			SenseNearbyPlayer ();
		}
		senseTimer += Time.deltaTime;

	}

	void SenseNearbyPlayer()
	{
		nearbyPlayer = null;
		//获得玩家对象
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null) {
			//获得玩家的生命值管理组件，判断玩家是否活着
			PlayerHealth ph = player.GetComponent<PlayerHealth> ();
			if (ph != null && ph.isAlive)
			{
				//计算玩家与僵尸之间的距离
				float dist = Vector3.Distance (player.transform.position, zombieTransform.position);

				//如果玩家与僵尸的距离小于僵尸的听觉距离
				if (dist < SoundRange) {
					//缓存这个玩家
					nearbyPlayer = player.transform;
				}

				//如果玩家与僵尸的距离小于僵尸的视觉距离
				if (dist < SightRange) {
					//计算玩家是否在僵尸的视角内
					Vector3 direction = player.transform.position - zombieTransform.position;
						float degree = Vector3.Angle (direction, zombieTransform.forward);

					if (degree < SightAngle / 2 && degree > -SightAngle / 2) {
						Ray ray = new Ray();	
						ray.origin = zombieEye.position;		
						ray.direction = direction;		
						RaycastHit hitInfo;		
						//判断玩家和僵尸之间是否存在遮挡物
						if (Physics.Raycast (ray, out hitInfo, SightRange)) {
							if (hitInfo.transform == player.transform) {
								//如果僵尸能够看到玩家就缓存这个玩家
								nearbyPlayer = player.transform;
							}
						}
					}
				}
			}
		}
	}

	//获得当前缓存的附近玩家对象，如果附近没有玩家则返回null
	public Transform getNearbyPlayer()
	{
		return nearbyPlayer;
	}
}
