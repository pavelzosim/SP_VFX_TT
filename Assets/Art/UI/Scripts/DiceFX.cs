using UnityEngine;

public class DiceFX : MonoBehaviour
{
    public ParticleSystem VFX_Confetti; // ParticleSystem for the confetti effect

    private void Start()
    {
        if (VFX_Confetti != null)
        {
            VFX_Confetti.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop and clear particles at the start
        }

        // Disable automatic playback at the start
        var mainModule = VFX_Confetti.main;
        mainModule.playOnAwake = false; // Disable automatic playback on awake
        mainModule.loop = false; // Disable looping
    }

    // This method will be called to play the confetti particles
    public void PlayConfetti()
    {
        if (VFX_Confetti != null)
        {
            VFX_Confetti.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); // Stop old particles
            VFX_Confetti.Play(); // Play the confetti effect
        }
    }
}
