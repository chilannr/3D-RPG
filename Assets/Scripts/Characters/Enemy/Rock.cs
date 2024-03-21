using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Rock : MonoBehaviour
{

    public enum RockStats { HitPlayer,HitEnemy,HitNothing}
    private Rigidbody rb;

    public RockStats rockStats;
    [Header("Basic Setting")]
    public float force;
    private float forceCrash;
    public int damage;
    public GameObject target;
    public GameObject breakEffect;
    private Vector3 direction;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockStats = RockStats.HitPlayer;
        FlyToTarget();
    }
    private void FixedUpdate()
    {
        forceCrash = Mathf.Sqrt(rb.velocity.sqrMagnitude);
        if (rb.velocity.sqrMagnitude < 1f)
            {
                rockStats = RockStats.HitNothing;
                StartCoroutine(DestroyAfterDelay(30f));
            }
        
        //Debug.Log(rb.velocity.sqrMagnitude);
    }
    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockStats)
        {
            case RockStats.HitPlayer:
                if(other.gameObject.CompareTag("Player")&& rb.velocity.sqrMagnitude>6f)
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * forceCrash;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Hit");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());
                    rockStats = RockStats.HitNothing;
                }
                break;
            case RockStats.HitEnemy:
                if(other.gameObject.CompareTag("Enemy") && rb.velocity.sqrMagnitude > 1f)
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    other.gameObject.GetComponent<Animator>().SetTrigger("Hit");
                    other.gameObject.GetComponent<EnemyController>().lastAttackTime = other.gameObject.GetComponent<CharacterStats>().attackData.collDown;
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            
        }
       
    }
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameObject != null)
            Destroy(gameObject);
    }

}
