using UnityEngine;
using System.Collections;

public class EnemyZombieSight : MonoBehaviour
{
	public float fieldOfViewAngle = 110f;				// Number of degrees, centred on forward, for the enemy see.
    public bool playerInSight;							// Whether or not the player is currently sighted.
	public Vector3 personalLastSighting;				// Last place this enemy spotted the player.
	
	
	private UnityEngine.AI.NavMeshAgent nav;							// Reference to the NavMeshAgent component.
	private SphereCollider col;							// Reference to the sphere collider trigger component.
	private Animator anim;								// Reference to the Animator.
	private DoneLastPlayerSighting lastPlayerSighting;	// Reference to last global sighting of the player.
    private GameObject player;							// Reference to the player.
	private Animator playerAnim;						// Reference to the player's animator component.
	private DonePlayerHealth playerHealth;				// Reference to the player's health script.
	private DoneHashIDs hash;							// Reference to the HashIDs.
	private Vector3 previousSighting;					// Where the player was sighted last frame.
	
	// Zolbie
	public bool canBite = true;
	public int timesBit = 0;
	public float timer = 0f;
	
	void Awake ()
	{
		// Setting up the references.
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneLastPlayerSighting>();
		player = GameObject.FindGameObjectWithTag(DoneTags.player);
		playerAnim = player.GetComponent<Animator>();
		playerHealth = player.GetComponent<DonePlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneHashIDs>();
		
		// Set the personal sighting and the previous sighting to the reset position.
		personalLastSighting = lastPlayerSighting.resetPosition;
		previousSighting = lastPlayerSighting.resetPosition;
	}
	
	/*
	void Update ()
	{
		// If the last global sighting of the player has changed...
		if(lastPlayerSighting.position != previousSighting)
			// ... then update the personal sighting to be the same as the global sighting.
			personalLastSighting = lastPlayerSighting.position;
		
		// Set the previous sighting to the be the sighting from this frame.
		previousSighting = lastPlayerSighting.position;
		
		// If the player is alive...
		if(playerHealth.health > 0f)
			// ... set the animator parameter to whether the player is in sight or not.
			anim.SetBool(hash.playerInSightBool, playerInSight);
		else
			// ... set the animator parameter to false.
			anim.SetBool(hash.playerInSightBool, false);
	}
	*/
	/*
	void OnTriggerStay (Collider other)
    {
		// If the player has entered the trigger sphere...
        if(other.gameObject == player)
        {
			// By default the player is not in sight.
			playerInSight = false;
			
			// Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			
			// If the angle between forward and where the player is, is less than half the angle of view...
			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;
				
				// ... and if a raycast towards the player hits something...
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
					// ... and if the raycast hits the player...
					if(hit.collider.gameObject == player)
					{
						// ... the player is in sight.
						playerInSight = true;
						
						// Set the last global sighting is the players current position.
						lastPlayerSighting.position = player.transform.position;
					}
				}
			}
			
			// Store the name hashes of the current states.
			int playerLayerZeroStateHash = playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;
			int playerLayerOneStateHash = playerAnim.GetCurrentAnimatorStateInfo(1).nameHash;
			
			// If the player is running or is attracting attention...
			if(playerLayerZeroStateHash == hash.locomotionState || playerLayerOneStateHash == hash.shoutState)
			{
				// ... and if the player is within hearing range...
				if(CalculatePathLength(player.transform.position) <= col.radius)
					// ... set the last personal sighting of the player to the player's current position.
					personalLastSighting = player.transform.position;
			}
        }
    }
	*/
	/*
	void OnTriggerExit (Collider other)
	{
		// If the player leaves the trigger zone...
		if(other.gameObject == player)
			// ... the player is not in sight.
			playerInSight = false;
	}
	*/
	/*
	float CalculatePathLength (Vector3 targetPosition)
	{
		// Create a path and set it based on a target position.
		UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
		if(nav.enabled)
			nav.CalculatePath(targetPosition, path);
		
		// Create an array of points which is the length of the number of corners in the path + 2.
		Vector3 [] allWayPoints = new Vector3[path.corners.Length + 2];
		
		// The first point is the enemy's position.
		allWayPoints[0] = transform.position;
		
		// The last point is the target position.
		allWayPoints[allWayPoints.Length - 1] = targetPosition;
		
		// The points inbetween are the corners of the path.
		for(int i = 0; i < path.corners.Length; i++)
		{
			allWayPoints[i + 1] = path.corners[i];
		}
		
		// Create a float to store the path length that is by default 0.
		float pathLength = 0;
		
		// Increment the path length by an amount equal to the distance between each waypoint and the next.
		for(int i = 0; i < allWayPoints.Length - 1; i++)
		{
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}
		
		return pathLength;
	}
	*/
	
	void OnTriggerEnter (Collider other)
	{
		Debug.Log("Enter Trigger Collider");
		canBite = true;
	}
	
	void OnTriggerStay (Collider other)
    {
		Debug.Log("Stay Trigger Collider");
		// If the player has entered the trigger sphere...
        if(other.gameObject == player)
        {
			
			Debug.Log("Player is in Trigger Collider");
			
			timer += Time.deltaTime;

			// canBite = true;
			//
			if ( timer >= 3 && canBite && timesBit < 1)
			// if(canBite)
			{
				
				Debug.Log("Times up!");
				
				
				Vector3 pos = (player.transform.position - transform.position).normalized;
				
				
				transform.parent = player.transform;
				
				Vector3 desiredPos = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z - 0.5f);
				
				// transform.position = Vector3.Lerp(transform.position, pos, 1f * Time.deltaTime);
				transform.position = desiredPos;
				Vector3 wPos = transform.TransformPoint(desiredPos);

				transform.parent = null;
				// transform.position = wPos;
				

				// transform.position = position.lookAt(player.transform.position);
				//find the vector pointing from our position to the target
				

				//create the rotation we need to be in to look at the target
				var _lookRotation = Quaternion.LookRotation(pos);

				//rotate us over time according to speed until we are in the required rotation
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 5f);
				
				// anim.SetBool(hash.neckBite, true);
				anim.SetTrigger("Tneckbite");
				
				canBite = false;
				timer = 0;
				timesBit += 1;
			}
			
			else
			{
				// anim.SetBool(hash.neckBite, false);
				
			}
			
			//
			
		}
    }
	
	void OnTriggerExit (Collider other)
	{
		timer = 0f;
		Debug.Log("Exited Trigger Collider");
		// If the player leaves the trigger zone...
		if(other.gameObject == player)
			canBite = false;
			timer = 0f;
			// ... the player is not in sight.
			playerInSight = false;
	}
}
