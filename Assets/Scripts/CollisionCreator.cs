using UnityEngine;
using System.Collections;

[SelectionBase]
public class CollisionCreator : MonoBehaviour {
	[SerializeField]
	private bool m_isOneway = false;
	
	private BoxCollider2D m_solidCollision;

	public bool isOneway {
		get {
			return m_isOneway;
		}
	}

	// Use this for initialization
	void Awake () {
		Component[] colliders = transform.GetComponentsInChildren<BoxCollider2D>();

		Bounds tempBounds = new Bounds();;
		foreach (Component component in colliders) {
			BoxCollider2D collider = component as BoxCollider2D;
			
			if(tempBounds.size.sqrMagnitude == 0) {
				tempBounds = collider.bounds;
			} else {
				tempBounds.Encapsulate(collider.bounds);
			}
			
			Component.Destroy(collider);
		}

		// Add the solid collision
		m_solidCollision = gameObject.AddComponent<BoxCollider2D> () as BoxCollider2D;
		m_solidCollision.size = new Vector2 (tempBounds.extents.x / transform.localScale.x, tempBounds.extents.y / transform.localScale.y) * 2.0f;
		m_solidCollision.offset = new Vector2((tempBounds.center.x - transform.position.x) / transform.localScale.x, 
		                                 (tempBounds.center.y - transform.position.y) / transform.localScale.y);

	}
}
