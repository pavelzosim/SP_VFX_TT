using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class VFX_ParticleCollide : MonoBehaviour
{
    public ParticleSystem VFX_DiceHit;
    public BoxCollider2D targetCollider;
    public RectTransform uiElementToBounce;
    public float bounceStrength = 0.2f;
    public float bounceDuration = 0.3f;

void Start()
{
    // Remove the manual OnParticleTrigger call
    if (VFX_DiceHit != null)
    {
        // Ensure simulation space is world space for proper collision detection
        var mainModule = VFX_DiceHit.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // Reset previous particles and prepare system
        VFX_DiceHit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        mainModule.playOnAwake = false;
        mainModule.loop = false;

        // Configure trigger module
        var triggerModule = VFX_DiceHit.trigger;
        triggerModule.enabled = true;
        triggerModule.SetCollider(0, targetCollider);
        triggerModule.inside = ParticleSystemOverlapAction.Kill;
    }
}

void OnParticleTrigger()
{
    // Only process if we have a valid particle system
    if (VFX_DiceHit == null) return;

    List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
    
    // Get entering particles
    int numEntered = VFX_DiceHit.GetTriggerParticles(
        ParticleSystemTriggerEventType.Enter, 
        particles,
        out var collisionData
    );

    if (numEntered > 0)
    {
        BounceUIElement();
        // Update particle system with modified particles
        VFX_DiceHit.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }
}
void BounceUIElement()
{
    if (uiElementToBounce == null)
    {
        Debug.LogError("uiElementToBounce is not assigned!");
        return;
    }

    // Kill any running animations on the UI element
    uiElementToBounce.DOKill(true);

    // Save the original scale of the UI element
    Vector3 originalScale = uiElementToBounce.localScale;

    // Log message to confirm bounce animation is starting
    Debug.Log("Starting bounce animation!");

    // Play the bounce animation
    uiElementToBounce.DOPunchScale(
        new Vector3(bounceStrength, bounceStrength, 0f),
        bounceDuration,
        vibrato: 3,
        elasticity: 0.5f
    ).SetEase(Ease.OutQuad)
     .OnComplete(() =>
     {
         // Restore the original scale after animation completes
         uiElementToBounce.localScale = originalScale;
         Debug.Log("Bounce animation completed!");
     });
}

    public void PlayVFX()
    {
        if (VFX_DiceHit != null)
        {
            VFX_DiceHit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            VFX_DiceHit.Play(true);
        }
    }
}