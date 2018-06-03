using UnityEngine;

class ActorState_LedgeGrab : ActorState
{
	public ActorState_LedgeGrab(ActorController parent) : base(parent) {
	}

	public override void Init() {
		rigidBody.gravityScale = 0.0f;
		animator.SetBool ("wall_hang", true);
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		velocity.x = (parent.isFacingRight ? 1.0f : -1.0f) * timeDelta;
		velocity.y = 0;
	}

	public override void Jump() {
		parent.SetState(ActorController.State.HangJump);
	}

	public override void Exit() {
		rigidBody.gravityScale = 1.0f;
		animator.SetBool ("wall_hang", false);
	}
};

