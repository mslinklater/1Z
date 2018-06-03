using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	[SerializeField]
	private ActorController m_playerActor;

	[SerializeField]
	private float m_lerpTime = 0.1f;

	[SerializeField]
	private Vector2 m_outerWindow;

	[SerializeField]
	private Vector2 m_innerWindow;

	float m_cameraHeight;

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (m_playerActor.transform.position.x, m_playerActor.transform.position.y, transform.position.z);
	}

	static Vector2 ClampToWindow(Vector2 input, Vector2 window) {
		Vector2 output = new Vector2();
		if (input.x > window.x)
			output.x = input.x - window.x;
		else if (input.x < -window.x)
			output.x = input.x + window.x;
		else 
			output.x = 0;
		
		if (input.y > window.y)
			output.y = input.y - window.y;
		else if (input.y < -window.y)
			output.y = input.y + window.y;
		else 
			output.y = 0;

		return output;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float deltaTime = Time.fixedDeltaTime;;

		Vector2 target = m_playerActor.transform.position;
		if (m_playerActor.isInGroundState || m_playerActor.isWallSlide || target.y > m_playerActor.transform.position.y) {
			m_cameraHeight = m_playerActor.transform.position.y;
		}

		Vector2 tposition = transform.position;
		Vector2 diff = ClampToWindow((target - tposition), m_outerWindow);

		Vector2 position = tposition + diff;

		target.y = m_cameraHeight;
		diff = ClampToWindow((target - tposition), m_innerWindow);
				
		position += (diff * m_lerpTime * deltaTime);

		position.x = Mathf.Clamp(position.x, -120.0f, 78.0f);
		position.y = Mathf.Clamp(position.y, 6.0f, 100.0f);

		transform.position = new Vector3(position.x, position.y, transform.position.z);
	}
	
}
