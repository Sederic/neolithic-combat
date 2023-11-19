using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
	#region Torch Variables
    [SerializeField] float flickerTime;
    [SerializeField] float flickerMin;
    [SerializeField] float flickerMax;

	private SpriteRenderer torchSR;
    public Light2D torchR;
	private float intensity;
    private float time;
    #endregion

    #region Unity Functions
	private void Start() 
	{
		torchSR = GetComponent<SpriteRenderer>();
        torchR = GetComponent<Light2D>();
		intensity = torchR.intensity;
	}

	private void Update() 
	{
        if (GetMillisecs() > 1000f / flickerTime) 
	    {
            // float newAlpha = alpha + Random.Range(flickerMin, flickerMax);
            float newIntensity = intensity + Random.Range(flickerMin, flickerMax);
            // Color newColor = torchSR.color;
		    // newColor.a = newAlpha;
		    // torchSR.color = newColor;
            torchR.intensity = newIntensity;
            // print(intensity);
            ResetTime();
        }
    }
    #endregion

    #region Helper Functions
	private float GetMillisecs() 
	{
		return (Time.realtimeSinceStartup - time) * 1000;
	}

	public void ResetTime() 
	{
		time = Time.realtimeSinceStartup;
	}
    #endregion
}
