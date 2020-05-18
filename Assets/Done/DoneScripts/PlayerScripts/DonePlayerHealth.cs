using UnityEngine;
using System.Collections;

public class DonePlayerHealth : MonoBehaviour
{
    public float health = 100f;							// How much health the player has left.
	public float resetAfterDeathTime = 5f;				// How much time from the player dying to the level reseting.
	public AudioClip deathClip;							// The sound effect of the player dying.
	
	public AudioClip deathBiteClip;						// Player death by zombie bite sound effect.
	
	private Animator anim;								// Reference to the animator component.
	private DonePlayerMovement playerMovement;			// Reference to the player movement script.
	private DoneHashIDs hash;							// Reference to the HashIDs.
	private DoneSceneFadeInOut sceneFadeInOut;			// Reference to the SceneFadeInOut script.
	private DoneLastPlayerSighting lastPlayerSighting;	// Reference to the LastPlayerSighting script.
	private float timer;								// A timer for counting to the reset of the level once the player is dead.
	private bool playerDead;							// A bool to show if the player is dead or not.
	
	// Death bools: set by Enemy causing action.
	public bool playerDead1;							// A bool to show if the player is dead or not from normal enemy bullet shot.
	public bool playerDeadBit;							// A bool to show if the player is dead or not from Zombie bite.

	public bool isDragged = false;
	public EnemyZombieShooting zombie;
	public Transform tempBoxPos;

	
	
	void Awake ()
	{
		// Setting up the references.
		anim = GetComponent<Animator>();
		playerMovement = GetComponent<DonePlayerMovement>();
		hash = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneHashIDs>();
		sceneFadeInOut = GameObject.FindGameObjectWithTag(DoneTags.fader).GetComponent<DoneSceneFadeInOut>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneLastPlayerSighting>();
		
		zombie = GameObject.FindObjectOfType<EnemyZombieShooting>();
		}
	
	
    void Update ()
	{
		// If health is less than or equal to 0...
		if(health <= 0f)
		{
			// ... and if the player is not yet dead...
			if(!playerDead)
			{
				// ... call the PlayerDying function.
				PlayerDying();
			}
			else
			{
				// Otherwise, if the player is dead, call the PlayerDead and LevelReset functions.
				PlayerDead();
				LevelReset();
			}
		}
	}
	
	
	void PlayerDying ()
	{
		if(playerDead1) // playerDead1 = player killed by regular enemy gun. (EnemyShooting)
		{
			// Debug.Log("Player Dead TYPE shot by enemy gun.");
			// The player is now dead.
			playerDead = true;
			
			// Set the animator's dead parameter to true also.
			anim.SetBool(hash.deadBool, playerDead);
			
			// Play the dying sound effect at the player's location.
			AudioSource.PlayClipAtPoint(deathClip, transform.position);
		}
		else if (playerDeadBit) // playerDeadBit = Player killed by zombie bite. (EnemyZombieAttack)
		{
			// Debug.Log("Player Dead TYPE zombie Bite.");
			// The player is now dead.
			playerDead = true;
			
			// Set the animator's dead parameter to true also.
			anim.SetBool(hash.deadBiteBool, playerDead);
			
			// Play the dying sound effect at the player's location.
			AudioSource.PlayClipAtPoint(deathClip, transform.position);
		}
		
	}
	
	
	void PlayerDead ()
	{
		if(playerDead1) // playerDead1 = player already dead by regular enemy gun. (EnemyShooting)
		{
			// Debug.Log("PlayerDead() 1");
			// If the player is in the dying state then reset the dead parameter.
			if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.dyingState)
				anim.SetBool(hash.deadBool, false);
			
			// Disable the movement.
			anim.SetFloat(hash.speedFloat, 0f);
			playerMovement.enabled = false;
			
			// Reset the player sighting to turn off the alarms.
			lastPlayerSighting.position = lastPlayerSighting.resetPosition;
			
			// Stop the footsteps playing.
			GetComponent<AudioSource>().Stop();
		}
		else if (playerDeadBit) // playerDeadBit = Player killed by zombie bite. (EnemyZombieAttack)
		{
			// Debug.Log("PlayerDead() Zombie Bite");
			// If the player is in the dying state then reset the dead parameter.
			if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.dyingBiteState)
				anim.SetBool(hash.deadBiteBool, false);
			
			// Disable the movement.
			anim.SetFloat(hash.speedFloat, 0f);
			playerMovement.enabled = false;
			
			// Reset the player sighting to turn off the alarms.
			lastPlayerSighting.position = lastPlayerSighting.resetPosition;
			
			// Stop the footsteps playing.
			GetComponent<AudioSource>().Stop();
			
			
			if (isDragged)
			{
				Dragged();
			}
		}
	}
	
	void LevelReset ()
	{
		// Increment the timer.
		timer += Time.deltaTime;
		
		//If the timer is greater than or equal to the time before the level resets...
		if(timer >= resetAfterDeathTime)
		{
			// ... reset the level.
			sceneFadeInOut.EndScene();
		}
	}
	
	public void Dragged()
	{
		Debug.Log("Player<> being dragged");
		
		isDragged = false;
		transform.parent = zombie.transform;
		
		// Move right behind player (with offset)
		// Vector3 newPos = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z - 0.5f);
		Vector3 newPos = new Vector3(tempBoxPos.position.x, zombie.transform.position.y, tempBoxPos.position.z);
		
		// ROTATION.
		// Get direction to playerZ (fof rotation).
		Vector3 pos = (newPos - zombie.transform.position).normalized;

		//create the rotation we need to be in to look at the target
		var _lookRotation = Quaternion.LookRotation(pos) * Quaternion.Euler(0, 180, 0);;

		//rotate us over time according to speed until we are in the required rotation
		zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, _lookRotation, Time.deltaTime * 5f);
		
		// MOVEMENT.
		// Drag player using Lerp movement over time.
		zombie.transform.position = Vector3.Lerp(zombie.transform.position, newPos, 0.25f * Time.deltaTime);
		
		Debug.Log("Player() End drag");
		
	}
	
	public void TakeDamage (float amount)
    {
		// Decrement the player's health by amount.
        health -= amount;
    }
}
