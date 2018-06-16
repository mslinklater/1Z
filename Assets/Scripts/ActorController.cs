using UnityEngine;
using System.Collections;

public class ActorController : MonoBehaviour {
	public enum State
	{
		None,
		Stand,
		Walk,
		Jump,
		HangJump,
		WallJump,
		DoubleJump,
		Fall,
		Attack,
		WallSlide,
		LedgeGrab,
		Count
	};

	public CharacterControlAsset characterControlAsset;

	// ----------------------------------
	[SerializeField]
	private Transform[] m_grabLocations;

	// ----------------------------------
	private OneBody2D m_rigidBody;
	private Animator m_animator;
	private Vector2 m_startingPosition;
	public LayerMask m_solidMask;

	private float m_lateralSpeed = 0;
	private bool m_isFacingRight = true;
	private bool m_jumpHeld = false;
	private int m_fallFrame = -1;

	private int m_lateralDirection = 0;
	private uint m_jumpHeldTime = 0;
	private uint m_maxJumpFrames = 0;
	private State m_currentState = State.None;
	private ActorState m_currentActorState = null;
	private ActorState[] m_ActorStateTable;

	// ----------------------------------
	// States
	public bool isInGroundState {
		get { return m_currentState == State.Stand || m_currentState == State.Attack || m_currentState == State.Walk; }
	}

	public bool isJumpingState {
		get { return m_currentState == State.Jump || m_currentState == State.HangJump; }
	}

	public bool isLedgeGrab {
		get { return m_currentState == State.LedgeGrab; }
	}
		
	public bool isWallSlide {
		get { return m_currentState == State.WallSlide; }
	}

	public bool isAttacking {
		get { return m_currentState == State.Attack; }
	}

	// Serialized
	public float jumpForce {
		get { return characterControlAsset.jumpForce; }
	}

	public float doubleJumpForce {
		get { return characterControlAsset.doubleJumpForce; }
	}

	public float hangJumpForce {
		get { return characterControlAsset.hangJumpForce; }
	}

	public float wallJumpForce {
		get { return characterControlAsset.wallJumpForce; }
	}
		
	public bool isFacingRight {
		get { return m_isFacingRight; }
	}
		
	public uint jumpHeldTime { 
		get { return m_jumpHeldTime; }
	}

	public uint maxJumpFrames {
		get { return m_maxJumpFrames; }
	}

	public float walkAcceleration {
		get { return characterControlAsset.groundMoveForce; }
	}

	public float turnAcceleration {
		get { return characterControlAsset.groundTurnForce; }
	}

	public float stopDeceralartion {
		get { return characterControlAsset.groundStopForce; }
	}

	public float airAcceleration {
		get { return characterControlAsset.airMoveForce; }
	}

	public float turnAirAcceleration {
		get { return characterControlAsset.airTurnForce; }
	}

	public float airDeceralartion {
		get { return characterControlAsset.airStopForce; }
	}

	public float walllAirAcceleration {
		get { return characterControlAsset.wallJumpMoveForce; }
	}

	public float walllAirDecceleration {
		get { return characterControlAsset.wallJumpStopForce; }
	}

	public float wallFriction {
		get { return characterControlAsset.wallFriction; }
	}

	public float maxWallSlideSpeed {
		get { return characterControlAsset.maxWallSlideSpeed; }
	}

	public float moveSpeed {
		get { return characterControlAsset.moveSpeed; }
	}
		
	// External access
	public int direction {
		get { return m_lateralDirection; }
		set { m_lateralDirection = value; }
	}

	public float lateralSpeed {
		get { return m_lateralSpeed; }
		set { m_lateralSpeed = value; }
	}

	public OneBody2D rigidBody {
		get { return m_rigidBody; }
	}

	public Animator animator {
		get { return m_animator; }
	}

	public Transform grabLocations(int index) {
		return m_grabLocations[index];
	}

	// Use this for initialization
	void Start () {
		m_rigidBody = GetComponent<OneBody2D> ();
		m_animator = GetComponent<Animator> ();

		// State table
		m_ActorStateTable = new ActorState[(int)State.Count];

		// Try the children (in the case of the player using a proxy)
		if(m_animator == null)
		{
			Component[] components = transform.GetComponentsInChildren<Animator>();
			if(components.Length > 0) {
				m_animator = components[0] as Animator;
			}
		}

		m_startingPosition = transform.position;
	
		// Create states
		m_ActorStateTable[(int)State.Stand] = new ActorState_Stand(this);
		m_ActorStateTable[(int)State.Walk] = new ActorState_Stand(this); //Repeat for now
		m_ActorStateTable[(int)State.Jump] = new ActorState_Jump(this);
		m_ActorStateTable[(int)State.HangJump] = new ActorState_HangJump(this);
		m_ActorStateTable[(int)State.WallJump] = new ActorState_WallJump(this);
		m_ActorStateTable[(int)State.DoubleJump] = new ActorState_DoubleJump(this);
		m_ActorStateTable[(int)State.Fall] = new ActorState_Fall(this);
		m_ActorStateTable[(int)State.Attack] = new ActorState_Attack(this);
		m_ActorStateTable[(int)State.WallSlide] = new ActorState_WallSlide(this);
		m_ActorStateTable[(int)State.LedgeGrab] = new ActorState_LedgeGrab(this);

		Reset ();
	}

	void Reset() {
		transform.SetParent (null);
		transform.position = m_rigidBody.position;

		m_lateralSpeed = 0;
		m_isFacingRight = true;
		m_jumpHeldTime = 0;
		m_jumpHeld = false;
		m_lateralDirection = 0;

		// Start default state
		m_currentState = State.Stand;
		m_currentActorState = m_ActorStateTable [(int)State.Stand];
		m_currentActorState.Init ();

		Vector3 localScale = transform.localScale;
		localScale.x = 1.0f;
		transform.localScale = localScale;

		m_rigidBody.position = m_startingPosition;
		transform.position = m_rigidBody.position;

		m_rigidBody.Reset();
	}
	
	public void Attack() {
		// Attack
		if (!isAttacking && m_rigidBody.onGround) {
			SetState(State.Attack);
		}
	}

	public void Jump(bool held) {
		m_jumpHeld = held;
		if (!m_jumpHeld) {
			// Not down
			m_jumpHeldTime = 0;
		} else if (m_jumpHeldTime == 0) {
			m_jumpHeldTime = 1;

			m_maxJumpFrames = (Mathf.Abs (lateralSpeed) >= 0.3f) ? characterControlAsset.maxRunJumpFrames : characterControlAsset.maxStandJumpFrames;
				
			// Jump tapped
			m_currentActorState.Jump ();
		}
	}


	public void Update() {
		if (m_currentActorState == null) {
			Start ();
		}
		
		// Set animator
		m_currentActorState.Update();

		m_animator.SetBool ("air", !m_rigidBody.onGround);

		m_animator.SetLayerWeight(1, (m_rigidBody.onGround && m_fallFrame == -1 ? 0.0f : 1.0f));
	}

	void FixedUpdate() {
		if (m_currentActorState == null) {
			Start ();
		}

		if (m_currentState == State.Jump)
		{
			if (m_rigidBody.onGround)
			{
				m_fallFrame = -1;
				SetState(State.Stand);
			}
		}
		else
		{
			if (m_rigidBody.onGround)
			{
				m_fallFrame = -1;
				if (!isAttacking) {
					SetState(State.Stand);
				}
			}
			else if (isInGroundState)
			{
				if (m_fallFrame == -1)
				{
					m_fallFrame = Time.frameCount;
				}
				else {
					// Debug.Log(Time.frameCount - m_fallFrame);
					if (Time.frameCount - m_fallFrame > 2)
					{
						SetState(State.Fall);
						m_animator.SetTrigger("jump"); // fall?
					}
				}
			}
		}

		if (m_jumpHeld && m_jumpHeldTime < m_maxJumpFrames) {
			m_jumpHeldTime ++;
		} else {
			if (isInGroundState && !m_jumpHeld) {
				m_jumpHeldTime = 0;
			}
		}

		UpdateMovement (m_lateralDirection);
	}

	void UpdateMovement(int dir) {
		float timeDelta = Time.fixedDeltaTime * 60.0f;
		if (timeDelta <= 0) { 
			return;
		}

		// Off bottom of screen hack
		if (m_rigidBody.position.y < -10.0f) {
			Reset();
			return;
		}
		/*
		if (dir == 0) {
			m_walkDelta = Mathf.MoveTowards (m_walkDelta, 0.0f, m_stopDelta * timeDelta);
		} else {
			bool sameDir = (Mathf.Sign (m_walkDelta) == Mathf.Sign (dir)) || (dir == 0) || (m_walkDelta == 0);
			if (isInGroundState) {
				m_walkDelta = Mathf.MoveTowards (m_walkDelta, dir < 0 ? -1.0f : 1.0f, sameDir ? m_speedUpDelta * timeDelta : m_turnDelta * timeDelta);
			} else {
				float scaler = isJumpingState ? timeDelta : timeDelta * 0.5f;
				m_walkDelta = Mathf.MoveTowards (m_walkDelta, dir < 0 ? -1.0f : 1.0f, sameDir ? m_speedUpDeltaAir * scaler : m_turnDeltaAir * scaler);
			}
		} 

		*/

		m_currentActorState.FixedUpdate ();

		Vector2 velocity = m_rigidBody.velocity * timeDelta;

		m_currentActorState.UpdateVelocity (timeDelta, dir, ref velocity);

		/*
		if (!isAttacking) {
			if (isInGroundState) {
				velocity.x = (m_walkDelta * m_moveSpeed * timeDelta);
			} else {
				if (Mathf.Abs (m_walkDelta) > float.Epsilon) {
					velocity.x += m_walkDelta * m_moveSpeed * timeDelta;
				} else {
					velocity.x = Mathf.MoveTowards (velocity.x, 0, 0.3f * timeDelta);
				}
				velocity.x = Mathf.Clamp (velocity.x, -m_moveSpeed, m_moveSpeed);
			}

			if ((m_walkDelta > 0 && !m_isFacingRight) || (m_walkDelta < 0 && m_isFacingRight)) {
				Flip();
			}
		}
*/
		if (!isInGroundState) {
			/*
			if (isWallSlide)
			{
				m_wallSlideSpeed = Mathf.MoveTowards(m_wallSlideSpeed, m_maxFallSpeed, m_maxFallSpeed * timeDelta * (isWallSlide ? 0 : 0.1f));//m_maxFallSpeed * timeDelta * 0.1f); 
				if (Mathf.Abs(velocity.x) > float.Epsilon && Mathf.Sign(velocity.x) == m_rigidBody.wallDirection)
				{
					SetState(State.Fall);
				}
				else if (isWallSlide && !m_rigidBody.hitWall && !isLedgeGrab)
				{
					SetState(State.Fall); // Todo - falling but allow wall jump
					// Not great this - if you're not pushing AND drifted beyond a wall then you'd still be sliding
					if (dir != 0)
					{
						// m_wallSlide = false;
					}
				}
			}

			if (!isLedgeGrab)
			{
				if (m_rigidBody.hitWall && Physics2D.OverlapCircle(m_grabLocations[1].position, 0.11f, m_solidMask) && m_rigidBody.velocity.y <= 0)
				{
					if (dir == -Mathf.Sign (m_rigidBody.wallDirection)) {
						SetState(State.WallSlide);
					}						
				}
			}
						*/
		}

		// Clamp delta
		if (m_rigidBody.hitWall) {
			m_lateralSpeed = Mathf.Clamp (m_lateralSpeed, -characterControlAsset.pushMoveForce, characterControlAsset.pushMoveForce);
		}
			
		// Clamp fall speed
		velocity.y = Mathf.Max (velocity.y, -characterControlAsset.maxFallSpeed * timeDelta);

		// Attempt wall grab
		if (velocity.y <= 0) {
			if (isWallSlide || isJumpingState) {
				bool overlap = Physics2D.OverlapCircle (m_grabLocations[0].position, 0.0f, m_solidMask);
				RaycastHit2D result = Physics2D.Raycast (m_grabLocations[0].position, Vector3.down, 0.05f + (m_rigidBody.velocity.y * -Time.deltaTime), m_solidMask);
				if (!overlap && result.distance < 0.5) {
					if (result) {
						velocity.y = -result.distance * 60;

						SetState(State.LedgeGrab);
					}
				}
			}
		}
	
		// Debug.DrawLine(m_grabLocations[0].position, m_grabLocations[0].position + new Vector3(0.2f, 0, 0));

		// Set rigidbody
		m_rigidBody.velocity = velocity / timeDelta;
	}

	public void Flip() {
		m_isFacingRight = !m_isFacingRight;
		
		// Flip
		Vector3 localScale = transform.localScale;
		localScale.x *= -1.0f;
		transform.localScale = localScale;
	}

	public void SetState(State newState)
	{
		if (m_currentState != newState)
		{
			// Exit
			m_currentActorState.Exit ();

			Debug.Log(Time.frameCount + " from " + m_currentState.ToString() + "->" + newState.ToString());

			// Switch
			m_currentState = newState;
			m_currentActorState = m_ActorStateTable [(int)m_currentState];

			// Init
			m_currentActorState.Init ();
		}
	}
}
