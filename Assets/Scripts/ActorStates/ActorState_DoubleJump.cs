using UnityEngine;

class ActorState_DoubleJump : ActorState
{
	public ActorState_DoubleJump(ActorController parent) : base(parent) {
	}

	public override void Init() {
		// Double jump
		parent.lateralSpeed = 0;//  (float)m_direction;
		rigidBody.velocity = new Vector2(parent.lateralSpeed, parent.doubleJumpForce); // 1?
//		animator.SetTrigger ("double_jump");
		animator.SetTrigger("jump");
		rigidBody.velocity = new Vector2(0, parent.jumpForce);
		parent.transform.SetParent (null);
	}
};