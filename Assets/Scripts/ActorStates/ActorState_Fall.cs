using UnityEngine;

class ActorState_Fall : ActorState
{
	public ActorState_Fall(ActorController parent) : base(parent) {
	}

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		UpdateLateralDelta (timeDelta, dir, parent.stopDeceralartion, parent.airAcceleration, parent.turnAirAcceleration);

		velocity.x = (parent.lateralSpeed * parent.moveSpeed * timeDelta);

		TryWallslide(dir);
	}

};