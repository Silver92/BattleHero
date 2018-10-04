using UnityEngine;
using System.Collections;

public class ZombieHealth :MonoBehaviour{

	public int currentHP = 10;		//僵尸当前生命值
	public int maxHP = 10;			//僵尸最大生命值
	public int killScore = 5;		//僵尸被击杀后，玩家的得分
	public AudioClip enemyHurtAudio;		//僵尸受伤音效

	[HideInInspector]
	public Vector3 damageDirection = Vector3.zero;	//保存僵尸收到攻击时，攻击者所在的方向
	[HideInInspector]
	public bool getDamaged = false;					//保存僵尸是否收到攻击

	public bool IsAlive {
		get {
			return currentHP > 0;
		}
	}

	public void TakeDamage(int damage, Vector3 shootPosition){
		if (!IsAlive)
			return;
		//更新僵尸生命值
		currentHP -= damage;
		if (currentHP <= 0 ) currentHP = 0;
		if (IsAlive) {		
			//记录僵尸的中枪状态
			getDamaged = true;
			//记录僵尸受到攻击时，玩家所在的方向
			damageDirection = shootPosition - transform.position;
			damageDirection.Normalize ();		
		} 
		else
		{		
			if (GameManager.gm != null) {	
				GameManager.gm.AddScore (killScore);//玩家获得击杀敌人后得分
			}
		}

		if (enemyHurtAudio != null)				//在敌人位置处播放敌人受伤音效
			AudioSource.PlayClipAtPoint (enemyHurtAudio, transform.position);
	}

}
