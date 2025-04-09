using System.Collections;
using UnityEngine;

public enum EnemyType
{
    None, FlyingMoster
}

public class EnemyManager : MonoBehaviour
{
    private Color orginalColor;
    private Renderer objectRenderer;
    public float colorChangeDuration = 0.5f;
    public float enemyHp = 10.0f;
    public float speed = 2.0f;
    public float maxDistance = 3.0f;
    private Vector3 startPos;
    //private int direction = 1;
    public EnemyType enemyType = EnemyType.None;


    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        orginalColor = objectRenderer.material.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            Vector3 spawnPosition;
            if (collision.name == "AttackObjRight")
            {
                spawnPosition = collision.transform.position + transform.right * 0.5f;
                ParticleManager.Instance.ParticlePlay(ParticleType.PlayerAttack, spawnPosition, new Vector3(-4, 4, 4));
            }
            else
            {
                spawnPosition = collision.transform.position - transform.right * 0.5f;
                ParticleManager.Instance.ParticlePlay(ParticleType.PlayerAttack, spawnPosition, new Vector3(4, 4, 4));
            }

            enemyHp -= 5.0f;
            if (enemyHp > 0) StartCoroutine(ChangeColorTemporatily());
            else Destroy(gameObject);
        }
    }



    private IEnumerator ChangeColorTemporatily()
    {
        SoundManager.Instance.PlaySFX(SFXType.EnemyDamagedSFX);
        objectRenderer.material.color = Color.red;
        yield return new WaitForSeconds(colorChangeDuration);
        objectRenderer.material.color = orginalColor;
    }

}
