using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Ship;
using UnityEngine;

public class ShipAudio : MonoBehaviour
{
	public float MinVolume = 0.01f;
	public float MaxVolume = 0.4f;
	public float MinPitch = 0.3f;
	public float MaxPitch = 0.8f;

	private ShipMovement movement_;
	private AudioSource audio_;

    // Start is called before the first frame update
    void Start()
    {
	    audio_ = GetComponent<AudioSource>();
	    movement_ = GetComponent<ShipMovement>();
    }

    // Update is called once per frame
    void Update()
    {
	    if (audio_ == null)
	    {
		    audio_ = GetComponent<AudioSource>();
			if (audio_ == null)
				return;
	    }

	    float speed = movement_.GetSpeedPercentage();

	    audio_.volume = Mathf.Lerp(MinVolume, MaxVolume, speed);
	    audio_.pitch = Mathf.Lerp(MinPitch, MaxPitch, speed);
    }
}
