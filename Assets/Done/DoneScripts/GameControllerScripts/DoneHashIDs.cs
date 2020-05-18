using UnityEngine;
using System.Collections;

public class DoneHashIDs : MonoBehaviour
{
	// Here we store the hash tags for various strings used in our animators.
	public int dyingState;
	public int locomotionState;
	public int shoutState;
	public int deadBool;
	public int speedFloat;
	public int sneakingBool;
	public int shoutingBool;
	public int playerInSightBool;
	public int shotFloat;
	public int aimWeightFloat;
    public int angularSpeedFloat;
	public int openBool;
	
	// Added zombie.
	public int neckBite;
	public int deadBiteBool;
	public int dyingBiteState;
	public int zombieEat;
	public int eatingState;
	public int bitingState;
	public int dragPlayer;
	
	void Awake ()
	{
		dyingState = Animator.StringToHash("Base Layer.Dying");
		locomotionState = Animator.StringToHash("Base Layer.Locomotion");
		shoutState = Animator.StringToHash("Shouting.Shout");
		deadBool = Animator.StringToHash("Dead");
		speedFloat = Animator.StringToHash("Speed");
		sneakingBool = Animator.StringToHash("Sneaking");
		shoutingBool = Animator.StringToHash("Shouting");
		playerInSightBool = Animator.StringToHash("PlayerInSight");
		shotFloat = Animator.StringToHash("Shot");
		aimWeightFloat = Animator.StringToHash("AimWeight");
        angularSpeedFloat = Animator.StringToHash("AngularSpeed");
		openBool = Animator.StringToHash("Open");
		
		// Added zombie.
		neckBite = Animator.StringToHash("neckbite"); // zombie animator
		zombieEat = Animator.StringToHash("eating"); // zombie animator
		dragPlayer = Animator.StringToHash("crawling"); // zombie animator
		bitingState = Animator.StringToHash("Base Layer.zombie_neck_bite"); // zombie animator
		eatingState = Animator.StringToHash("Base Layer.ZombieEat"); // zombie animator

		// Added player.
		deadBiteBool = Animator.StringToHash("DeadFromBite"); // deadBool
		dyingBiteState = Animator.StringToHash("Base Layer.DeathFromBite"); // dyingState

	}
}
