using UnityEngine;
using System.Collections;

public class EnemyZombieAI : MonoBehaviour
{
	public float patrolSpeed = 2f;							// The nav mesh agent's speed when patrolling.
	public float chaseSpeed = 5f;							// The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;						// The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 1f;						// The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;						// An array of transforms for the patrol route.
	
	
	private EnemyZombieSight enemySight;						// Reference to the EnemySight script.
	private UnityEngine.AI.NavMeshAgent nav;								// Reference to the nav mesh agent.
	private Transform player;								// Reference to the player's transform.
	private DonePlayerHealth playerHealth;					// Reference to the PlayerHealth script.
	private DoneLastPlayerSighting lastPlayerSighting;		// Reference to the last global sighting of the player.
	private float chaseTimer;								// A timer for the chaseWaitTime.
	private float patrolTimer;								// A timer for the patrolWaitTime.
	private int wayPointIndex;								// A counter for the way point array.
	
	public float distance;
	
	private EnemyZombieAttack enemyZombieAttack;
	
	
	void Awake ()
	{
		// Setting up the references.
		enemySight = GetComponent<EnemyZombieSight>();
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag(DoneTags.player).transform;
		playerHealth = player.GetComponent<DonePlayerHealth>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneLastPlayerSighting>();
		
		enemyZombieAttack = GetComponentInChildren<EnemyZombieAttack>();
	}
	
	
	void Update ()
	{
		// DEBUG LINE.
		Debug.Log(enemyZombieAttack.isBiting);
		// Debug.Log(transform.position);
		if (!enemyZombieAttack.isBiting)
		{
		var distance = Vector3.Distance(player.transform.position, transform.position);

		// If the player is in sight and is alive...
		if(enemySight.playerInSight && playerHealth.health > 0f && distance >= nav.stoppingDistance)
		{
			// ... shoot.
			// Shooting();
			Chasing(); // not “stop and shoot”.
			Debug.Log("EnemyZombieAI() Chasing()1");
		}
		
		else if(distance <=  nav.stoppingDistance && playerHealth.health > 0f)
		{
			// ... stop navigation.
			nav.isStopped = true;
			// nav.Stop();
			Debug.Log("EnemyZombieAI() nav.Stop" + " because 'distance' is less than NAV.StoppingDistance : " + nav.remainingDistance);
			/*
			if (nav.remainingDistance >= nav.stoppingDistance)
			{
				nav.Resume();
				// nav.isStopped = false;
			}
			*/
			
			LookAtPlayer();
			
		}
		
		
		// If the player has been sighted and isn't dead...
		else if(enemySight.personalLastSighting != lastPlayerSighting.resetPosition && playerHealth.health > 0f)
		{
			// ... chase.
			Chasing();
			Debug.Log("EnemyZombieAI() Chasing()2");
			// Debug.Log(distance + " : distance");
		}
		
		
		// Otherwise...
		else// if(playerHealth.health > 0f)
		{
			// ... patrol.
			Patrolling();
			Debug.Log("EnemyZombieAI() Patrolling()");
		}
	
		}
		else{
		nav.isStopped = true;
		}
	}
	
	/*
	void Shooting ()
	{
		// Stop the enemy where it is.
		nav.Stop();
	}
	*/
	/*
	void TryToBite()
	{
		
		if (distance <= 1)
		{
			Debug.Log("TryToBite() " + distance + " <= 1");
			nav.Stop();
		}
	}
	*/
	
	void Chasing ()
	{
		// Create a vector from the enemy to the last sighting of the player.
		Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
		Debug.Log("ONE");
		
		// If the the last personal sighting of the player is not close...
		if(sightingDeltaPos.sqrMagnitude > 4f)
		{
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			nav.destination = enemySight.personalLastSighting;
			Debug.Log("TWO");
		}
		
		// Set the appropriate speed for the NavMeshAgent.
		nav.speed = chaseSpeed;
		Debug.Log("THREE");
		// Debug.Log(nav.remainingDistance);
		
		Debug.Log(chaseTimer + " " + chaseWaitTime);
			
		
		// If near the last personal sighting...
		if(nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			chaseTimer += Time.deltaTime;
			Debug.Log("FOUR");
			// Debug.Log(nav.remainingDistance);
			// Debug.Log(chaseTimer + " " + chaseWaitTime);
			
			// If the timer exceeds the wait time...
			if(chaseTimer >= chaseWaitTime)
			{
				Debug.Log("FIVE");
				// ... reset last global sighting, the last personal sighting and the timer.
				lastPlayerSighting.position = lastPlayerSighting.resetPosition;
				enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
				chaseTimer = 0f;
			}
		}
		else
			// If not near the last sighting personal sighting of the player, reset the timer.
			chaseTimer = 0f;
	}

	
	void Patrolling ()
	{
		// Set an appropriate speed for the NavMeshAgent.
		nav.speed = patrolSpeed;
		
		// If near the next waypoint or there is no destination...
		if(nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			patrolTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			if(patrolTimer >= patrolWaitTime)
			{
				// ... increment the wayPointIndex.
				if(wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;
				
				// Reset the timer.
				patrolTimer = 0;
			}
		}
		else
			// If not near a destination, reset the timer.
			patrolTimer = 0;
		
		// Set the destination to the patrolWayPoint.
		nav.destination = patrolWayPoints[wayPointIndex].position;
	}
	
	void LookAtPlayer()
	{
		// ROTATION.
		// Get direction to playerZ (fof rotation).
		Vector3 pos = (enemySight.personalLastSighting - transform.position).normalized;

		//create the rotation we need to be in to look at the target
		var _lookRotation = Quaternion.LookRotation(pos);// * Quaternion.Euler(0, 180, 0);;

		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 5f);
	}
}
