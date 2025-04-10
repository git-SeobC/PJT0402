using UnityEngine;

public class PlayerDie : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    public float dieParticleAddPosY = 0.5f;

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    public void Die(Collider2D collision = null)
    {
        gameObject.SetActive(false);
        Vector3 particlePos = new Vector3(transform.position.x, transform.position.y + dieParticleAddPosY, transform.position.z);
        ParticleManager.Instance.ParticlePlay(ParticleType.PlayerDie, particlePos, new Vector3(7, 7, 7));
        if (collision.CompareTag("Trap"))
        {
            SoundManager.Instance.PlaySFX(SFXType.BladeDieSFX);
        }
        else if (collision.CompareTag("DeathZone"))
        {

        }
        else
        {
            SoundManager.Instance.PlaySFX(SFXType.DefaultDieSFX);
        }
    }
}
