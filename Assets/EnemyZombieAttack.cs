using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombieAttack : MonoBehaviour
{
	public Animator anim;								// Reference to the animator.
	private DoneHashIDs hash;							// Reference to the HashIDs script.
	private GameObject player;							// Reference to the player's transform.
	private DonePlayerHealth playerHealth;				// Reference to the player's health.
	
	// Zombie
	public bool canBite = true;
	public int timesBit = 0;
	public float timerToNextBite = 0f;
	public bool eatPlayer = false;
	public bool isBiting = false;
	
	// public AudioSource audioSource;
	public AudioClip zScreamClip;						// zombie pre attack player scream sound effect.
	
	public GameObject zombieMain;
	
	// TEST REMOVE UNLESS CONFIRMED.
	// Collider m_Collider;
	
    // Start is called before the first frame update
    void Awake()
    {
		//gameObject.GetComponentInParent(typeof(HingeJoint)) as HingeJoint;
        //Animator 
		anim = gameObject.GetComponentInParent<Animator>();
		// anim = zombieMain.GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag(DoneTags.player);
		playerHealth = player.GetComponent<DonePlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneHashIDs>();
		
		// TEST REMOVE UNLESS CONFIRMED.
		// m_Collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnTriggerEnter (Collider other)
	{
		// Debug.Log("EnemyZombieAttack() Enter Trigger Collider");
		canBite = true;
		// col.enabled = false;
	}
	
	void OnTriggerStay (Collider other)
    {
		// Debug.Log("EnemyZombieAttack() Stay Trigger Collider");
		// If the player has entered the trigger sphere...
        if(other.gameObject == player)
        {
			
			// Debug.Log("Player is in Trigger Collider");
			
			timerToNextBite += Time.deltaTime;
			
			// Play zombie scream audio.
			AudioSource.PlayClipAtPoint(zScreamClip, transform.position);
			// audioSource.Stop();		// Stop current audio (zombie mosning)

			// canBite = true;
			//
			if ( timerToNextBite >= 1 && canBite && timesBit < 1)
			// if(canBite)
			{
				
				Debug.Log("EnemyZombieAttack() BitePpayer now");
				
				isBiting = true;
				canBite = false;
				timerToNextBite = 0;
				timesBit += 1;
				
				// TEST REMOVE UNLESS CONFIRMED.
				// eatPlayer = true;
				// m_Collider.enabled = false; // !m_Collider.enabled
				
				
				
				// Parent enemy zombie to player.
				zombieMain.transform.parent = player.transform;
				
				
				// The player takes damage.
				float damage = 100f; // later multiply by scale of time, when adding break free mecahnic”
				playerHealth.TakeDamage(damage);
				// Set zombie bite bool in player script to activate zombie bite death process.
				playerHealth.playerDeadBit = true;

				// Get direction to playerZ (fof rotation).
				Vector3 pos = (player.transform.position - zombieMain.transform.position).normalized;
				
				//create the rotation we need to be in to look at the target
				var _lookRotation = Quaternion.LookRotation(pos);
				
				//rotate us over time according to speed until we are in the required rotation
				zombieMain.transform.rotation = Quaternion.Slerp(zombieMain.transform.rotation, _lookRotation, Time.deltaTime * 5f);
				// zombieMain.transform.rotation = Quaternion.LookRotation(pos, Vector3.up);
				
				// Move right behind player (with offset)
				Vector3 desiredPos = new Vector3(player.transform.localPosition.x + 0.5f, player.transform.localPosition.y, player.transform.localPosition.z + 0.1f);
				zombieMain.transform.position = desiredPos;
				
				// Set world position to keep postion once for unparent.
				Vector3 wPos = transform.TransformPoint(desiredPos);
				// Unparent enemy zombie from player.
				zombieMain.transform.parent = null;
				
				
				// anim.SetBool(hash.neckBite, true);
				anim.SetTrigger("Tneckbite");
				
				var randomNum =  Random.Range(0, 4);
				Debug.Log(randomNum);
				// RANDOM START EATING 66%, OR DRAG PLAYER 33% (INTO DARKNESS?)
				if (randomNum >= 3)
				{
				StartCoroutine("DragPlayer");
				}
				else// if (randomNumHalf == 2)
				{
				StartCoroutine("EatPlayer");
				}
			}
		}
    }
	
	void OnTriggerExit (Collider other)
	{
		// timerToNextBite = 0f;
		// Debug.Log("Exited Trigger Collider");
		// If the player leaves the trigger zone...
		if(other.gameObject == player)
			// canBite = false;
			timerToNextBite = 0f;
			eatPlayer = false;
		
	}
	
	IEnumerator EatPlayer()
	{
		Debug.Log("EatPlayer() CoRoutine");
		
		while(anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
			yield return null;

		Debug.Log("EatPlayer() commit");
		anim.SetBool(hash.zombieEat, true);
	}
	
	IEnumerator DragPlayer()
	{
		Debug.Log("DragPlayer() CoRoutine");
		
		while(anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
			yield return null;

		Debug.Log("DragPlayer() commit");
		anim.SetBool(hash.dragPlayer, true);
		
		playerHealth.isDragged = true;
	}
}
