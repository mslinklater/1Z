using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	private ActorController m_actorController;

	public float m_deadZone = 0.8f;

	// Use this for initialization
	void Start () {
		m_actorController = GetComponent<ActorController> ();

		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) {
		}
	}

	void FixedUpdate() {
		// Debug.Log ("FixedUpdate:" + Time.fixedDeltaTime);
	}

	void Update() {
		// Debug.Log ("Update:" + Time.deltaTime);
		m_actorController.Jump (Input.GetButton ("Jump"));

		if (Input.GetButton ("Fire1")) {
			// m_actorController.Attack ();
		}

		int direction = 0;
		if (Input.GetAxis("Horizontal") < -m_deadZone || Input.GetButton("Left")) {
			direction = -1; 
		} else if (Input.GetAxis("Horizontal") > m_deadZone || Input.GetButton("Right")) {
			direction = 1;
		}
		m_actorController.direction = direction;
	}
}
