using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public enum EInterpotionType {
		Linear,
		Cubic,
		Quadratic,
	};

	public float m_speed;
	public Transform[] m_locations;
	public EInterpotionType m_interpolation = EInterpotionType.Linear;

	private float m_time = 0;
	private uint index = 0;
	private Vector2 m_size;
	private Vector2 m_offset;
	private bool m_oneWay;
	private BoxCollider2D m_collider;

	// Use this for initialization
	void Start () 
	{
		m_collider = GetComponent<BoxCollider2D> ();

		m_size.x = m_collider.bounds.size.x;
		m_size.y = m_collider.bounds.size.y;
		m_offset = m_collider.bounds.center - transform.position;

		m_oneWay = false;
		CollisionCreator collisionCreator = GetComponent<CollisionCreator> ();

		if (collisionCreator != null) 
		{
			m_oneWay = collisionCreator.isOneway;
		}

		// Remove children
		foreach(Transform location in m_locations) 
		{
			if(location.parent == transform) 
			{
				location.SetParent(null);
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float deltaTime = Time.fixedDeltaTime;

		if (m_locations.Length <= 1) 
		{
			return;
		}

		m_time += deltaTime * m_speed / m_locations.Length;

		while(m_time >= 1.0f) 
		{
			m_time -= 1.0f;
			index++;
			if(index == m_locations.Length) 
			{
				index = 0;
			}
		}

		uint next = index + 1;

		if (next == m_locations.Length) 
		{
			next = 0;
		}

		float time;

		switch (m_interpolation) 
		{
		case EInterpotionType.Cubic:
			time = EaseInOutCubic (m_time);
			break;
		case EInterpotionType.Quadratic:
			time = EaseInOutQuad (m_time);
			break;
		default:
			time = m_time;
			break;
		}
		
		Vector2 position = transform.position;
		Vector2 newPosition = Vector2.Lerp (m_locations [index].position, m_locations [next].position, time);
		Vector2 collisionPos = position + m_offset;
		Vector2 localVelocity = newPosition - position;
		
		RaycastHit2D hit2D = Physics2D.BoxCast(collisionPos, m_size, 0, localVelocity.normalized, localVelocity.magnitude + 0.01f);

		if (hit2D && hit2D.transform.parent != transform && (!m_oneWay || localVelocity.y >= 0)) 
		{
			Vector3 newPos = localVelocity;// - (hit2D.normal * 0.01f);
			hit2D.transform.position += newPos;
			// Debug.Log(Time.frameCount + "push" + hit2D.transform.name);
		} 

		transform.position = newPosition;
		
		DebugDraw ();
	}

	static private float EaseInOutQuad(float t, float from = 0.0f, float to = 1.0f)
	{
		float c = to - from;
		
		t *= 2.0f;
		if (t < 1)
			return c/2.0f*t*t + from;
		
		t--;
		return -c/2.0f * (t*(t-2.0f) - 1.0f) + from;
	}

	static private float EaseInOutCubic(float t, float from = 0.0f, float to = 1.0f)
	{
		float c = to - from;
		t *= 2.0f;
		
		if (t < 1)
			return c/2.0f*t*t*t + from;
		
		t -= 2.0f;
		return c/2.0f*(t*t*t + 2.0f) + from;
	}

	void DebugDraw() 
	{
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
