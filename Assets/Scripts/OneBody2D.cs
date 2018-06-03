using UnityEngine;
using System.Collections;

public class OneBody2D : MonoBehaviour {
	[SerializeField]
	private bool m_showDebug = true;

	[SerializeField]
	private float m_gravityMultiplier = 15.0f;

	[SerializeField]
	private float m_WallNormal = 0.5f;

	private Vector2 m_position;
	private Vector2 m_velocity;
	private Collider2D m_collider;

	private Vector2 m_offset;
	private Vector2 m_size;

	private float m_wallDirection;

	private bool m_onGround;
	private bool m_hitWall;
	private bool m_hitCeiling;
	private float m_gravityScale = 1.0f;

	private static float s_gravity = 9.2f;


	public float gravityScale {
		get {
			return m_gravityScale;
		}
		set {
			m_gravityScale = value;
		}
	}

	public Vector2 velocity {
		get {
			return m_velocity;
		}
		set {
			m_velocity = value;
		}
	}

	public Vector2 position {
		get {
			return m_position;
		}
		set {
			m_position = value;
		}
	}

	public bool onGround {
		get {
			return m_onGround;
		}
	}

	public bool hitWall {
		get {
			return m_hitWall;
		}
	}
	public bool hitCeiling {
		get { 
			return m_hitCeiling;
		}
	}
	public float wallDirection { 
		get {
			return m_wallDirection;
		}
	}
	
	// Use this for initialization
	void Start () {
		position = transform.position;

		m_collider = GetComponent<BoxCollider2D> ();

		m_size.x = m_collider.bounds.size.x;
		m_size.y = m_collider.bounds.size.y;
		m_offset = m_collider.bounds.center - transform.position;
	}

	void DrawDebug () {
		if (m_showDebug) {
			
			Vector2 pos = new Vector2 ();

			pos = m_offset;
			pos.x += transform.position.x;
			pos.y += transform.position.y;

			float sx = m_size.x * 0.5f;
			float sy = m_size.y * 0.5f;
			Debug.DrawLine (pos + new Vector2 (-sx, -sy), pos + new Vector2 (sx, -sy));
			Debug.DrawLine (pos + new Vector2 (-sx, sy), pos + new Vector2 (sx, sy));
			Debug.DrawLine (pos + new Vector2 (-sx, -sy), pos + new Vector2 (-sx, sy));
			Debug.DrawLine (pos + new Vector2 (sx, -sy), pos + new Vector2 (sx, sy));
		}
	}


	public void Reset() {
		m_velocity = Vector2.zero;
		m_hitWall = false;
		m_hitCeiling = false;
		m_onGround = false;
	}

	void FixedUpdate () {
		DrawDebug ();

		float frameTime = Time.deltaTime;
		if (frameTime <= 0) {
			return;
		}

		m_position = transform.position;

		Vector2 collisionPos = m_position + m_offset;
		Vector2 localVelocity;

		m_onGround = false;
		if (m_gravityScale > 0) {
			m_velocity.y -= s_gravity * m_gravityMultiplier * m_gravityScale * frameTime;
			localVelocity = new Vector2(0, m_velocity.y * frameTime);
			
			// Trace floor
			if (localVelocity.y < 0) {
				RaycastHit2D hit2D = Physics2D.BoxCast(collisionPos, m_size, 0, localVelocity, -localVelocity.y);
				if(hit2D) {
					m_position += localVelocity * hit2D.fraction + (hit2D.normal * 0.01f);
					m_velocity.y = 0;
					m_onGround = true;
					
					collisionPos.y = m_position.y + m_offset.y;
					
					if(hit2D.transform.GetComponent<MovingPlatform>() != null) {
						transform.SetParent(hit2D.transform);
					} else {
						transform.SetParent(null);
					}
				}
			}
	
			if (!m_onGround) {
				transform.SetParent (null);
			}
		}

		// Trace walking
		m_hitWall = false;
		m_hitCeiling = false;
		localVelocity = m_velocity * frameTime;

		for(uint resolveCount = 0; resolveCount < 3; resolveCount++) {
			float magnitude = localVelocity.magnitude;
			if (magnitude > 0.0f) {
				RaycastHit2D hit2D = Physics2D.BoxCast(collisionPos, m_size, 0, localVelocity, magnitude);

				bool collide = false;
				if(hit2D) {
					CollisionCreator collision = hit2D.transform.GetComponent<CollisionCreator>();
					if(collision && collision.isOneway) {
						collide = false;
					} else {
						collide = !Physics2D.GetIgnoreCollision(hit2D.collider, m_collider);
					}
				}

				if(collide)
				{
					if(Mathf.Abs(hit2D.normal.x) > m_WallNormal) {
						m_hitWall = true;
					} else {
						// Debug.Log (hit2D.transform.name + ":" + hit2D.transform.position);

						m_hitCeiling = true;
					}
					m_wallDirection = Mathf.Sign(hit2D.normal.x);

					/*
					if(m_hitWall) {
						localVelocity.x = 0;
					}
					m_position += (localVelocity * hit2D.fraction);// + (hit2D.normal * 0.01f);

					collisionPos = m_position + m_offset;
					localVelocity =  -(1.0f * Vector2.Dot(hit2D.normal, localVelocity) * hit2D.normal) + localVelocity;
					// localVelocity *= 1.0f - hit2D.fraction;
					*/
					collisionPos = m_position + (localVelocity * hit2D.fraction);

					// Hack - would be nice to remove this
					if(m_hitWall) {
						m_position.x += (localVelocity.x * hit2D.fraction) + (hit2D.normal.x * 0.01f);
						collisionPos.y = m_position.y;
					} else {
						collisionPos.x = m_position.x;
					}

					collisionPos += m_offset;
					localVelocity =  -(1.0f * Vector2.Dot(hit2D.normal, localVelocity) * hit2D.normal) + localVelocity;
				}
				else {
					m_position += localVelocity;
					break;
				}
			}
		}
		m_velocity = localVelocity / frameTime; // Change this to distance travelled then we can modify the velocity internally
		transform.position = m_position;

		if (m_showDebug) {
			Debug.Log(Time.frameCount + 
			          " :" + transform.name + 
			          " :" + " OnGround:"+m_onGround + 
			          " HitWall:"+m_hitWall + 
			          " Ground="+((transform.parent == null) ? "null" : transform.parent.name));
		}
	}
}
