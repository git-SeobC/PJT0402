using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ParticleType
{
    PlayerAttack,
    PlayerDamage,
    PlayerDie,
    ItemGet,
}

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }

    private Dictionary<ParticleType, GameObject> particlePrefabDic = new Dictionary<ParticleType, GameObject>();
    private Dictionary<ParticleType, Queue<GameObject>> particlePools = new Dictionary<ParticleType, Queue<GameObject>>();

    public GameObject playerAttackEffectPrefab;
    public GameObject playerDamageEffectPrefab;
    public GameObject playerDieEffectPrefab;
    public GameObject ItemGetEffectPrefab;

    public int poolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        particlePrefabDic.Add(ParticleType.PlayerAttack, playerAttackEffectPrefab);
        particlePrefabDic.Add(ParticleType.PlayerDamage, playerDamageEffectPrefab);

        foreach (var type in particlePrefabDic.Keys)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(particlePrefabDic[type]);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            particlePools.Add(type, pool);
        }
    }

    public void ParticlePlay(ParticleType type, Vector3 position, Vector3 scale)
    {
        if (particlePools.ContainsKey(type))
        {
            GameObject particleObj = particlePools[type].Dequeue();

            if (particleObj != null)
            {
                particleObj.transform.position = position;
                particleObj.transform.localScale = scale;
                particleObj.SetActive(true);

                Animator animator = particleObj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play(0);
                    StartCoroutine(AnimationEndCoroutine(type, particleObj, animator));
                }
            }
        }
        else
        {
            if (type == ParticleType.PlayerDie)
            {
                GameObject particleObj = Instantiate(playerDieEffectPrefab);
                particleObj.transform.position = position;
                particleObj.transform.localScale = scale;
                particleObj.SetActive(true);

                Animator animator = particleObj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play(0);
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    Destroy(particleObj, stateInfo.length);
                }
            }
            else if (type == ParticleType.ItemGet)
            {
                GameObject particleObj = Instantiate(ItemGetEffectPrefab);
                particleObj.transform.position = position;
                particleObj.transform.localScale = scale;
                particleObj.SetActive(true);

                Animator animator = particleObj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play(0);
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    Destroy(particleObj, stateInfo.length);
                }
            }
        }
    }

    IEnumerator AnimationEndCoroutine(ParticleType type, GameObject obj, Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        obj.SetActive(false);
        particlePools[type].Enqueue(obj);
    }
}
