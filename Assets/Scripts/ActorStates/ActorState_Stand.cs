using UnityEngine;

class ActorState_Stand : ActorState
{
	public ActorState_Stand(ActorController parent) : base(parent) {
	}

	public override void Init() {
		// parent.animator.SetFloat ("speed", Mathf.Abs (parent.walkDelta));
	}

	public override void Jump() {
		parent.SetState(ActorController.State.Jump);
	}

	public override void Update() {
		parent.animator.SetFloat ("speed", Mathf.Abs (parent.lateralSpeed));
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		UpdateLateralDelta (timeDelta, dir, parent.stopDeceralartion, parent.walkAcceleration, parent.turnAcceleration);
		UpdateFacingDirection();

		velocity.x = (parent.lateralSpeed * parent.moveSpeed * timeDelta);
	}
};
