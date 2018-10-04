using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public int startHealth = 10;	//玩家的初始生命值
	public int currentHealth;		//玩家当前生命值
	public int restLife = 3;		//玩家剩余生命值

	public float respawnTime  = 5.0f;	//玩家死亡后，等待复活的时间
	public Transform spawnTransform;	//玩家复活的位置
	public GameObject gun;				//玩家的枪对象

	public bool isAlive { get { return currentHealth > 0; } }

	private Animator anim;
	private Rigidbody rigid;
	private CapsuleCollider capsuleCollider;
	private PlayerWeaponSwitcher playerWeaponSwitcher;	//玩家的换枪控制器
	private IKController userIKController;

	//初始化函数，设置玩家当前血量
	void Start () {
		currentHealth = startHealth;
		anim = GetComponent<Animator> ();
		rigid = GetComponent<Rigidbody> ();
		capsuleCollider = GetComponent<CapsuleCollider> ();
		playerWeaponSwitcher = GetComponent<PlayerWeaponSwitcher> ();
		userIKController = GetComponent<IKController> ();
	}

	//玩家扣血函数，在GameManager脚本中调用
	public void TakeDamage(int damage){
		//扣除玩家生命值
		currentHealth -= damage;
		if (currentHealth < 0)
			currentHealth = 0;

		//如果玩家死亡，
		if (currentHealth == 0) {
			//如果玩家还有剩余的生命，那么控制玩家在一段时间后复活
			if (restLife > 0) {
				Invoke ("respawn", respawnTime);
			}

			if (anim != null) {
				//播放死亡动画
				anim.SetBool ("isDead", true);
				//允许动画控制器，控制玩家的运动
				anim.applyRootMotion = true;
			}
			//取消刚体，碰撞体等对玩家位置朝向的影响
			rigid.useGravity = false;
			capsuleCollider.enabled = false;

			//禁用IK
			if (userIKController != null) {
				userIKController.enabled = false;
			}

			//禁用玩家所有的枪械
			if (playerWeaponSwitcher != null) {
				foreach (Transform trans in playerWeaponSwitcher.weaponList) {
					trans.gameObject.SetActive (false);
				}
			} else if(gun!=null) {
				gun.SetActive (false);
			}
		}
	}

	//玩家加血函数，在GameManager脚本中调用
	public void AddHealth(int value){
		currentHealth += value;
		if (currentHealth > startHealth)	//加血后当前生命值不能超过初始生命值
			currentHealth = startHealth;
	}

	//复活玩家
	public void respawn()
	{
		//恢复满血状态
		currentHealth = startHealth;
		//减去一条命
		restLife--;
		//放置玩家于出生地
		transform.position = spawnTransform.position;
		transform.rotation = spawnTransform.rotation;

		//启用重力，碰撞体，IK，禁止动画控制器影响玩家的运动
		rigid.useGravity = true;
		capsuleCollider.enabled = true;
		if (anim != null) {
			anim.SetBool ("isDead", false);
			anim.applyRootMotion = false;
		}
		if (userIKController != null) {
			userIKController.enabled = true;
		}

		//启用枪械对象
		if (playerWeaponSwitcher != null) {
			playerWeaponSwitcher.changeNextWeapon ();
		} else if (gun != null) {
			gun.SetActive (true);
		}
	}
}
