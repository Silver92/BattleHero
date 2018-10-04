using UnityEngine;
using System.Collections;

public class ZombieGenerator : MonoBehaviour {



	public Transform[] zombieSpawnTransform;	//僵尸的生成地数组
	public int maximumInstanceCount = 9;		//场景中的最大僵尸数量
	public float minGenerateTimeInterval = 5.0f;	//生成僵尸的最小时间间隔
	public float maxGenerateTimeInterval = 20.0f;	//生成僵尸的最大时间间隔
	public GameObject zombiePrefab;					//僵尸预制件

	private float nextGenerationTime = 0.0f;		//下一次生成僵尸的时刻
	private float timer = 0.0f;						//计时器，用于计算生成僵尸的时间
	private GameObject[] instances;					//僵尸数组对象池
	public static Vector3 defaultPosition = new Vector3(33, -6, -8);	//僵尸的默认生成地点
 
	void Start () {
		//生成僵尸对象池
		instances = new GameObject[maximumInstanceCount];
		//初始化僵尸对象池
		for(int i = 0; i < maximumInstanceCount; i++) {
			//生成一个僵尸
			GameObject zombie = Instantiate (zombiePrefab, 
				defaultPosition, Quaternion.identity) as GameObject;
			//禁用僵尸
			zombie.SetActive (false);
			//把僵尸放入僵尸对象池
			instances [i] = zombie;
		}
	}

	//在僵尸对象池中，找一个处于禁用状态的僵尸对象
	private GameObject GetNextAvailiableInstance ()   {
		for(var i = 0; i < maximumInstanceCount; i++) {
			if(!instances[i].activeSelf)
			{
				return instances[i];
			}
		}
		return null;
	}
	//在Position参数指定的位置，生成一个僵尸
	private bool generate(Vector3 position)
	{
		//从僵尸对象池中获得一个禁用状态的僵尸对象
		GameObject zombie = GetNextAvailiableInstance ();
		if (zombie != null) {
			//启用僵尸
			zombie.SetActive (true);
			//在指定位置初始化僵尸
			zombie.GetComponent<ZombieAI> ().Born (position);
			return true;
		}
		return false;
	}

	void Update () {   
		if (GameManager.gm.gameState != GameManager.GameState.Playing)
			return;
		
		//判断是否到达下一次生成僵尸的时间
		if (timer > nextGenerationTime) {

			//选择一个出生地点
			int i = Random.Range(0, zombieSpawnTransform.Length);
			//在选择的出生地生成一只僵尸
			generate (zombieSpawnTransform [i].position);
			//计算下一次生成僵尸的时间
			nextGenerationTime = Random.Range (minGenerateTimeInterval, maxGenerateTimeInterval);
			//清零timer
			timer = 0;
		}
		timer += Time.deltaTime;

	}
}
