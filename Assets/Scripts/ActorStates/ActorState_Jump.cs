using UnityEngine;

class ActorState_Jump : ActorState
{
	bool m_allowWallJump;

	public ActorState_Jump(ActorController parent) : base(parent) {
	}

	public override void Init() {
		rigidBody.velocity = new Vector2(0, parent.jumpForce);
		rigidBody.gravityScale = 0.2f;
		parent.transform.SetParent (null);
		animator.SetTrigger("jump");
		m_allowWallJump = false;
	}

	public override void FixedUpdate() {
		if(!allowJumpForce()) {
			rigidBody.gravityScale = 1.0f;
		}
	}
		
	public override void Jump() {
		if (m_allowWallJump) {
			parent.Flip ();
			parent.SetState (ActorController.State.WallJump);
		}
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		UpdateLateralDelta (timeDelta, dir, parent.airDeceralartion, parent.airAcceleration, parent.turnAirAcceleration);
		UpdateFacingDirection();

		if (rigidBody.hitWall) {
			m_allowWallJump = true;
		}

		if (Mathf.Abs (parent.lateralSpeed) > float.Epsilon) {
			velocity.x += parent.lateralSpeed * parent.moveSpeed * timeDelta;
		} else {
			velocity.x = Mathf.MoveTowards (velocity.x, 0, 0.3f * timeDelta);
		}
		velocity.x = Mathf.Clamp (velocity.x, -parent.moveSpeed, parent.moveSpeed);

		// Attempt wallslide
		TryWallslide(dir);
	}

	public override void Exit() {
		rigidBody.gravityScale = 1.0f;
	}

	private bool allowJumpForce() {
		return parent.jumpHeldTime > 0 && parent.jumpHeldTime < parent.maxJumpFrames;
	}

};