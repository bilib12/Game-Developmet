using UnityEngine;
using Blocks.Character;

namespace KnightsQuest
{
    public class SimpleHazard : MonoBehaviour
    {
        [SerializeField] float damage = 25f;
        [SerializeField] float knockbackSpeed = 8f;
        [SerializeField] float hitStunTime = 0.2f;
        [SerializeField] AudioClip hitSfx;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var character = other.GetComponent<BuildingBlocksCharacter>();
                if (character != null && character.IsDamageable)
                {
                    Vector2 hitPoint = other.transform.position;
                    Vector2 hitDir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
                    if (Mathf.Abs(hitDir.x) < 0.001f)
                    {
                        hitDir = new Vector2(-character.FacingDirection, 0.5f).normalized;
                    }

                    HitReaction reaction = HitReaction.PushBack(knockbackSpeed, hitStunTime);
                    var damageInfo = new DamageInfo(damage, null, hitPoint, hitDir, reaction);

                    character.TakeDamage(damageInfo);

                    if (hitSfx != null)
                    {
                        AudioSource.PlayClipAtPoint(hitSfx, transform.position, 0.7f);
                    }
                }
            }
        }
    }
}
