using UnityEngine;

class ActorState_HangJump : ActorState
{
	public ActorState_HangJump(ActorController parent) : base(parent) {
	}

	public override void Init() {
		parent.lateralSpeed = parent.direction;
		rigidBody.velocity = new Vector2(0, parent.hangJumpForce);
		animator.SetTrigger ("jump");
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		UpdateLateralDelta (timeDelta, dir, parent.stopDeceralartion, parent.airAcceleration, parent.turnAirAcceleration);

		if (Mathf.Abs (parent.lateralSpeed) > float.Epsilon) {
			velocity.x += parent.lateralSpeed * parent.moveSpeed * timeDelta;
		} else {
			velocity.x = Mathf.MoveTowards (velocity.x, 0, 0.3f * timeDelta);
		}
		velocity.x = Mathf.Clamp (velocity.x, -parent.moveSpeed, parent.moveSpeed);
	}
};