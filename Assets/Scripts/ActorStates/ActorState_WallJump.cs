using UnityEngine;

class ActorState_WallJump : ActorState
{
	bool m_allowWallJump;

	public ActorState_WallJump(ActorController parent) : base(parent) {
	}

	public override void Init() {
		parent.lateralSpeed = Mathf.Sign (rigidBody.wallDirection);
		rigidBody.velocity = new Vector2(0, parent.wallJumpForce);
		animator.SetTrigger ("jump");
		m_allowWallJump = false;
	}

	public override void Jump() {
		if (m_allowWallJump) {
			parent.Flip ();
			Init ();
		}
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		UpdateLateralDelta (timeDelta, dir, parent.walllAirDecceleration, parent.walllAirAcceleration, parent.walllAirAcceleration);
		UpdateFacingDirection ();
		velocity.x = (parent.lateralSpeed * parent.moveSpeed * timeDelta);
		if (rigidBody.hitWall) {
			m_allowWallJump = true;
		}
	}
};