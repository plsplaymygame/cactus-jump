using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusBehaviour : MonoBehaviour {
	public float vspeed; // vertical speed
	public float acc; // acceleration
	public float jumpHeight;
	public bool onGround;
	public Vector3 prevPosition; // previous position
	
	public GameObject platform; // the game object this instance is a passenger of. Passengers move according to its platform.
	public int platformId;

	public int id;
	
	private const float delta = 0.01667f;

	// Use this for initialization
	private void Start ()
	{
		vspeed = 0;
		acc = 0.3f;
		jumpHeight = 6.5f;
		onGround = false;
		prevPosition = transform.position;
		platform = null;
		platformId = -1;
	}

	public void LateUpdateMe()
	{	
		ResolveCollisions();
		prevPosition = transform.position;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		Move();
	}
	
	private void LateUpdate()
	{
		//ResolveCollisions();
		//prevPosition = transform.position; // update the previous positions
	}
	
	private void Move()
	{
		var jump = 
			(id == 0 && Input.GetKeyDown(KeyCode.Q)) || 
			(id == 1 && Input.GetKeyDown(KeyCode.W)) || 
			(id == 2 && Input.GetKeyDown(KeyCode.E)) ||
			(id == 3 && Input.GetKeyDown(KeyCode.R)) ||
			(id == 4 && Input.GetKeyDown(KeyCode.T));
		
		if ((onGround || platform != null) && jump) // jumping
		{
			vspeed = jumpHeight;
			onGround = false;
			if (platform != null) // release the child from the parent
			{
				platform = null;
				platformId = -1;
				transform.parent = null;
			}
		}
		else if (!onGround)
		{
			if (platform == null)
			{
				if (vspeed > -20) // gravity
				{
					vspeed -= acc;
				}
			}
		}
		if (platform == null)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y + vspeed * delta, 0);
		}
	}
	
	void ResolveCollisions()
	{
		Vector2 center = new Vector2(transform.position.x, transform.position.y);
		BoxCollider2D myCollider = GetComponent<BoxCollider2D>();
		
		CollisionWithSolid(center, myCollider);
		
		CollisionWithPlayer(center, myCollider);
	}

	private void CollisionWithSolid(Vector2 center, BoxCollider2D myCollider)
	{
		int layerId = LayerMask.NameToLayer("Solid");
		int layerMask = 1 << layerId;
		Collider2D overlap = Physics2D.OverlapBox(center, GetComponent<BoxCollider2D>().size, 0, layerMask);
		if (overlap != null)
		{
			// when collision occurs while falling
			if (vspeed < 0)
			{
				BoxCollider2D otherCollider = (BoxCollider2D) overlap;
				float colliderBottom = myCollider.transform.position.y - myCollider.bounds.extents.y;
				float otherColliderTop = otherCollider.transform.position.y + otherCollider.bounds.extents.y;
				
				transform.Translate(0, otherColliderTop - colliderBottom, 0); // resolve the collision by moving away from the other collider
				onGround = true;
				vspeed = 0;
			}
		}
	}

	private void CollisionWithPlayer(Vector2 center, BoxCollider2D myCollider)
	{
		if (platform != null) // do not need to check for collision when a passenger of a "platform"
			return;
		
		// need to sort the colliders
		Collider2D[] colliders = Physics2D.OverlapBoxAll(center, GetComponent<BoxCollider2D>().size, 0, 1 << LayerMask.NameToLayer("Cactus"));
		foreach (Collider2D collider in colliders)
		{
			if (collider.GetInstanceID() == myCollider.GetInstanceID()) // do not want to resolve collision with our own collider
				continue;
			
			CactusBehaviour cactusBehaviour = collider.gameObject.GetComponent<CactusBehaviour>();
			Vector3 otherPrevPosition = cactusBehaviour.prevPosition;
			BoxCollider2D otherCollider = (BoxCollider2D) collider;
			float colliderBottom = myCollider.transform.position.y - myCollider.bounds.extents.y;
			float otherColliderTop = otherCollider.transform.position.y + otherCollider.bounds.extents.y;
			
			// when a collsion has occurred from the bottom
			if (prevPosition.y > otherPrevPosition.y)
			{
				// when this instance does not have a "parent" platform
				if (platform == null)
				{
					platform = otherCollider.gameObject;
					platformId = cactusBehaviour.id;
					transform.SetParent(otherCollider.gameObject.transform);
				}
				transform.Translate(0, otherColliderTop - colliderBottom, 0); // resolve the collision by moving away from the other collider
				vspeed = 0;
			}
		}
	}
}
