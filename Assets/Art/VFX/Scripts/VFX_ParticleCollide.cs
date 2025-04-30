using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(ParticleSystem))]
public class VFX_ParticleCollide : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem VFX_DiceHit;

    [Header("Collision Target")]
    public Collider targetCollider; // Use 3D Collider

    [Header("UI Bounce Target")]
    public RectTransform uiTargetToBounce;

    [Header("Bounce Animation Settings")]
    public float bounceScale = 1.2f;
    public float bounceDuration = 0.3f;
    public Ease bounceEase = Ease.OutBounce;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    void Awake()
    {
        if (VFX_DiceHit == null)
            VFX_DiceHit = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        int eventCount = VFX_DiceHit.GetCollisionEvents(other, collisionEvents);

        if (eventCount == 0)
        {
            Debug.Log("[DEBUG] No particle collision events received.");
            return;
        }

        for (int i = 0; i < eventCount; i++)
        {
            var evt = collisionEvents[i];

            if (evt.colliderComponent != null)
                Debug.Log($"[DEBUG] Hit detected on collider: {evt.colliderComponent.name}", this);
            else
                Debug.Log("[DEBUG] Event has NULL colliderComponent", this);

            if (evt.colliderComponent == targetCollider)
            {
                Debug.Log($"[HIT] Particle hit TARGET: {targetCollider.name} at {evt.intersection}", this);
                TriggerBounceUI();
            }
        }
    }

    private void TriggerBounceUI()
    {
        if (uiTargetToBounce == null)
        {
            Debug.LogWarning("UI target for bounce is not assigned.");
            return;
        }

        // Kill any running tween on the UI object first
        uiTargetToBounce.DOKill();

        // Reset to original scale (just in case)
        uiTargetToBounce.localScale = Vector3.one;

        // Perform the bounce scale animation
        uiTargetToBounce
            .DOScale(bounceScale, bounceDuration)
            .SetEase(bounceEase)
            .OnComplete(() =>
            {
                uiTargetToBounce.DOScale(1f, bounceDuration * 0.5f).SetEase(Ease.OutQuad);
            });

        Debug.Log("[DEBUG] Triggered UI bounce.");
    }

    public void ToggleEmission()
    {
        if (VFX_DiceHit == null) return;

        if (VFX_DiceHit.isEmitting)
        {
            VFX_DiceHit.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Debug.Log("[DEBUG] Stopped particle emission", this);
        }
        else
        {
            VFX_DiceHit.Play(true);
            Debug.Log("[DEBUG] Started particle emission", this);
        }
    }

    public void EmitBurst(int count)
    {
        if (VFX_DiceHit != null)
        {
            VFX_DiceHit.Emit(count);
            Debug.Log($"[DEBUG] Emitted {count} particles", this);
        }
    }
}
