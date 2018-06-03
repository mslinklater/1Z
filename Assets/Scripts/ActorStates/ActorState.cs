using UnityEngine;

public abstract class ActorState
{
	private ActorController m_parent;
	private Animator m_animator;
	private OneBody2D m_rigidBody;

	public ActorState(ActorController parent) {
		m_parent = parent;
		m_rigidBody = m_parent.rigidBody;
		m_animator = m_parent.animator;
	}

	protected ActorController parent {
		get { return m_parent; }
	}

	protected Animator animator {
		get { return m_animator; }
	}

	protected OneBody2D rigidBody {
		get { return m_rigidBody; }
	}

	protected float GetWalkDeltaFromVelocity() {
		return parent.rigidBody.velocity.x / parent.moveSpeed;
	}
	protected void UpdateLateralDelta(float timeDelta, int dir, float stopDeceralartion, float walkAcceleration, float turnAcceleration) {
		if (dir == 0) {
			// No direction held
			parent.lateralSpeed = Mathf.MoveTowards (parent.lateralSpeed, 0.0f, stopDeceralartion * timeDelta);
		} else {
			bool sameDir = (Mathf.Sign (parent.lateralSpeed) == Mathf.Sign (dir)) || (dir == 0) || (parent.lateralSpeed == 0);

			parent.lateralSpeed = Mathf.MoveTowards (parent.lateralSpeed, dir < 0 ? -1.0f : 1.0f, sameDir ? walkAcceleration * timeDelta : turnAcceleration * timeDelta);
		}
	}

	protected void UpdateFacingDirection() {
		if ((parent.lateralSpeed > 0 && !parent.isFacingRight) || (parent.lateralSpeed < 0 && parent.isFacingRight)) {
			parent.Flip();
		}
	}

	protected bool TryWallslide(int dir) {
		if (rigidBody.hitWall && Physics2D.OverlapCircle(parent.grabLocations(1).position, 0.11f, parent.m_solidMask) && rigidBody.velocity.y <= 0)
		{
			if (dir == -Mathf.Sign (rigidBody.wallDirection)) {
				parent.SetState(ActorController.State.WallSlide);
				return true;
			}						
		}
		return false;
	}

	public virtual void Init() {} 
	public virtual void Update() {}
	public virtual void UpdateVelocity(float timeDelta, int dir, ref Vector2 velocity) {}
	public virtual void FixedUpdate() {}
	public virtual void Jump() {}
	public virtual void Exit() {}
};