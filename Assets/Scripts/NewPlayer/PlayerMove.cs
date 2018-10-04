using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMove : MonoBehaviour {
	public float moveSpeed = 6.0f;
	public float rotateSpeed = 10.0f;
	public float jumpVelocity = 5.0f;

	float minMouseRotateX = -45.0f;
	float maxMouseRotateX = 45.0f;
	float mouseRotateX;
	bool isGrounded;

	Camera myCamera;
	Animator anim;
	Rigidbody rigid;
	CapsuleCollider capsuleCollider;
	PlayerHealth playerHealth;

	//获取玩家对象的刚体，动画控制器，碰撞体，生命值管理等组件
	void Start(){
		myCamera = Camera.main;
		mouseRotateX = myCamera.transform.localEulerAngles.x;
		anim = GetComponent<Animator> ();
		rigid = GetComponent<Rigidbody> ();
		capsuleCollider = GetComponent<CapsuleCollider> ();
		playerHealth = GetComponent<PlayerHealth> ();
	}
	//定期检查玩家是否位于地面
	void FixedUpdate(){
		if (!playerHealth.isAlive)
			return;
		checkGround();
		if (isGrounded == false && anim != null)
			anim.SetBool ("isJump", false);
	}
	//判断玩家是否位于地面，更新PlayerMove类的isGrounded字段
	void checkGround()
	{            
		RaycastHit hitInfo;
		float shellOffset = 0.01f;
		float groundCheckDistance = 0.01f;
		Vector3 currentPos = transform.position;
		currentPos.y += capsuleCollider.height / 2f;
		if (Physics.SphereCast (currentPos, capsuleCollider.radius * (1.0f - shellOffset), Vector3.down, out hitInfo,
			    ((capsuleCollider.height / 2f) - capsuleCollider.radius) + groundCheckDistance, ~0, QueryTriggerInteraction.Ignore)) {
			isGrounded = true;
		} else {
			isGrounded = false;
		}

	}
	//玩家跳跃函数
	void Jump(bool isGround){
		
		if (CrossPlatformInputManager.GetButtonDown ("Jump") && isGround) {
			rigid.AddForce (Vector3.up * jumpVelocity, ForceMode.VelocityChange);
			if (anim != null)
				anim.SetBool ("isJump", true);
		} else {
			if(anim != null)
				anim.SetBool ("isJump", false);
		}
	}
	//每帧检测玩家的输入，更新玩家的位置和朝向
	void Update()
	{
		if (!playerHealth.isAlive) 
			return;
		
		if (GameManager.gm.gameState != GameManager.GameState.Playing)
			return;
		
		float h = CrossPlatformInputManager.GetAxisRaw ("Horizontal");
		float v = CrossPlatformInputManager.GetAxisRaw ("Vertical");
		Move (h, v);

		float rv = CrossPlatformInputManager.GetAxisRaw ("Mouse X");
		float rh = CrossPlatformInputManager.GetAxisRaw ("Mouse Y");
		Rotate (rh, rv);

		Jump (isGrounded);
	}
	//玩家移动函数
	void Move(float h,float v){
		transform.Translate ((Vector3.forward * v + Vector3.right * h) * moveSpeed * Time.deltaTime);
		if (h != 0.0f || v != 0.0f) {
			if (anim != null)
				anim.SetBool ("isMove", true);
		} else {
			if (anim != null)
				anim.SetBool ("isMove", false);
		}
	}
	//玩家转身和抬头低头函数
	void Rotate(float rh,float rv){
		transform.Rotate (0, rv * rotateSpeed, 0);
		mouseRotateX -= rh * rotateSpeed;
		mouseRotateX = Mathf.Clamp (mouseRotateX, minMouseRotateX, maxMouseRotateX);
		myCamera.transform.localEulerAngles = new Vector3 (mouseRotateX, 0.0f, 0.0f);
	}

}






















