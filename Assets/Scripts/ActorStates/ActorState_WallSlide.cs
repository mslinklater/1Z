using UnityEngine;

class ActorState_WallSlide : ActorState
{
	private float m_wallSlideSpeed;

	public ActorState_WallSlide(ActorController parent) : base(parent) {
	}

	public override void Init() {
		m_wallSlideSpeed = 0;
		animator.SetBool ("wall_slide", true);
	}

	public override void Exit() {
		animator.SetBool ("wall_slide", false);
	}

	public override void Jump() {
		parent.Flip ();
		parent.SetState(ActorController.State.WallJump);
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		UpdateLateralDelta (timeDelta, dir, parent.stopDeceralartion, parent.walkAcceleration, parent.turnAcceleration);

		m_wallSlideSpeed = Mathf.MoveTowards (m_wallSlideSpeed, parent.maxWallSlideSpeed, timeDelta * parent.wallFriction);

		velocity.x = (parent.lateralSpeed * parent.moveSpeed * timeDelta);
		velocity.y = -m_wallSlideSpeed;

		if (dir != -Mathf.Sign (rigidBody.wallDirection)) {
			// Pull off
			parent.SetState (ActorController.State.Fall);
		} else if (!rigidBody.hitWall) {
			// Not colliding
			parent.SetState (ActorController.State.Fall);
		} else if (!Physics2D.OverlapCircle (parent.grabLocations(1).position, 0.11f, parent.m_solidMask)) {
			// No grab
			parent.SetState (ActorController.State.Fall);
		}			
	}

};