using System;
using UnityEngine;

namespace KnightsQuest
{
    public class CollectibleCoin : MonoBehaviour
    {
        [SerializeField] int value = 1;
        [SerializeField] float rotateSpeed = 120f;
        [SerializeField] float bobSpeed = 3f;
        [SerializeField] float bobHeight = 0.15f;
        [SerializeField] AudioClip collectSfx;

        public static event Action<int> OnCoinCollected;

        Vector3 m_StartPos;
        bool m_Collected;

        void Start()
        {
            m_StartPos = transform.position;
        }

        void Update()
        {
            if (m_Collected) return;

            // Simple visual bobbing & spin effect
            float newY = m_StartPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(m_StartPos.x, newY, m_StartPos.z);
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (m_Collected) return;

            if (other.CompareTag("Player"))
            {
                m_Collected = true;
                if (collectSfx != null)
                {
                    AudioSource.PlayClipAtPoint(collectSfx, transform.position, 0.8f);
                }

                OnCoinCollected?.Invoke(value);
                Destroy(gameObject);
            }
        }
    }
}

