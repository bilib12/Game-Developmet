using System;
using UnityEngine;

namespace KnightsQuest
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] SpriteRenderer flagRenderer;
        [SerializeField] Color inactiveColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        [SerializeField] Color activeColor = new Color(0.2f, 0.9f, 0.3f, 1f);
        [SerializeField] AudioClip activateSfx;

        public bool IsActive { get; private set; }
        public static event Action<Checkpoint> OnCheckpointActivated;

        void Awake()
        {
            if (flagRenderer != null)
            {
                flagRenderer.color = inactiveColor;
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (IsActive) return;
            if (other.CompareTag("Player"))
            {
                Activate();
            }
        }

        public void Activate()
        {
            IsActive = true;
            if (flagRenderer != null)
            {
                flagRenderer.color = activeColor;
            }

            if (activateSfx != null)
            {
                AudioSource.PlayClipAtPoint(activateSfx, transform.position);
            }

            OnCheckpointActivated?.Invoke(this);
        }

        public void Deactivate()
        {
            IsActive = false;
            if (flagRenderer != null)
            {
                flagRenderer.color = inactiveColor;
            }
        }
    }
}

