using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public AudioSource audioSource;
	
	
	// AudioClip list.
	public AudioClip soldierGetToPos10;
	
	
	// Bool switches.
	private bool soldierGetToPos1;
	
	
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (soldierGetToPos1)
		{
			audioSource.PlayOneShot(soldierGetToPos10, 0.70f);
			soldierGetToPos1 = false;
		}
    }
	
	
}
