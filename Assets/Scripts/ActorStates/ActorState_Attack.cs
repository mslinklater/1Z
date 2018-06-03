using UnityEngine;

public class ActorState_Attack : ActorState
{
	public ActorState_Attack(ActorController parent) : base(parent) {
	}

	public override void Init() {
		animator.SetTrigger ("attack");
	}

	public override void Jump() {
		parent.SetState(ActorController.State.Jump);
	} 

	public override void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {
		velocity.x = Mathf.MoveTowards (velocity.x, 0, 0.3f * timeDelta);
	}

	public override void FixedUpdate() {
		if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1.0f) {
			parent.SetState(ActorController.State.Stand);
		}
	}

	public override void Exit() {
		animator.SetTrigger ("finished");
	}
}


