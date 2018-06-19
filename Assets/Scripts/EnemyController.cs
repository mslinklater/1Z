using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	[SerializeField]
	private Transform m_feelPointAhead;
	[SerializeField]
	private Transform m_feelPointAbove;

	[SerializeField]
	bool m_headRight = true;

	[SerializeField]
	LayerMask m_feelLayer;

	private ActorController m_actorController;
	private OneBody2D m_RigidBody;

	public int bounceCount = 0;
	public float waitTime = 0;
	
	// Use this for initialization
	void Start () 
	{
		waitTime = Random.Range(0, 3);
		m_actorController = GetComponent<ActorController> ();
		m_RigidBody = GetComponent<OneBody2D> ();
	}

	void Update () 
	{
		if (m_RigidBody.hitWall) 
		{
			m_headRight = !m_headRight;
			bounceCount++;
		} 
		else if(!m_RigidBody.onGround) 
		{
			bounceCount = 0;
			waitTime = 3.0f;
		}


		if (bounceCount > 0) 
		{
		}
		
		int dir = 0;

		if (m_RigidBody.onGround) 
		{
			dir = m_headRight ? 1 : -1;
		}
		m_actorController.direction = dir;
	}
}
