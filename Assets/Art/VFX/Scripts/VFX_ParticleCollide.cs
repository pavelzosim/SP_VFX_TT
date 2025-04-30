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
    public Collider targetCollider;

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

        if (eventCount == 0) return;

        for (int i = 0; i < eventCount; i++)
        {
            var evt = collisionEvents[i];

            if (evt.colliderComponent == targetCollider)
            {
                TriggerBounceUI();
            }
        }
    }

    private void TriggerBounceUI()
    {
        if (uiTargetToBounce == null) return;

        uiTargetToBounce.DOKill();
        uiTargetToBounce.localScale = Vector3.one;

        uiTargetToBounce
            .DOScale(bounceScale, bounceDuration)
            .SetEase(bounceEase)
            .OnComplete(() =>
            {
                uiTargetToBounce.DOScale(1f, bounceDuration * 0.5f).SetEase(Ease.OutQuad);
            });
    }

    public void ToggleEmission()
    {
        if (VFX_DiceHit == null) return;

        if (VFX_DiceHit.isEmitting)
        {
            VFX_DiceHit.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            VFX_DiceHit.Play(true);
        }
    }

    public void EmitBurst(int count)
    {
        if (VFX_DiceHit != null)
        {
            VFX_DiceHit.Emit(count);
        }
    }
}
