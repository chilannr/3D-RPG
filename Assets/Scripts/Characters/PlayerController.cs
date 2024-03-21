using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private GameObject attackTarget;
    private float lastAttackTime;
    bool isDead;

    private float stopDistance;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }
    private void OnEnable()
    {
        
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnAttackableClicked += EventAttackTarget;
        MouseManager.Instance.OnEnemyClicked += EventContinuousAttackTarget; 
        GameManager.Instance.RigisterPlayer(characterStats);
    }
    private void Start()
    {
        
        SaveManager.Instance.LoadPlayerData();
    }


    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnAttackableClicked -= EventAttackTarget;
        MouseManager.Instance.OnEnemyClicked -= EventContinuousAttackTarget;
    }

    private void Update()
    {

        isDead = characterStats.CurrentHealth <= 0;
        if (isDead)
        {
            GameManager.Instance.NotifyObserver();
           
        }
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    private  void SwitchAnimation()
    {
        anim.SetFloat("speed",agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }  
    public void MoveToTarget(Vector3 target)
    {
        
       StopAllCoroutines();
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    private void EventAttackTarget(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    private void EventContinuousAttackTarget(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
     
            StartCoroutine(ContinuousAttack());
        }
    }

    IEnumerator ContinuousAttack()
    {
        while (attackTarget != null && !attackTarget.GetComponent<EnemyController>().isDead)
        {
            agent.isStopped = false;
            agent.stoppingDistance = characterStats.attackData.attackRange;

            transform.LookAt(attackTarget.transform);

            while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
            {
                agent.destination = attackTarget.transform.position;

                yield return null;
            }
            agent.isStopped = true;

            // �����Ƿ񱩻�
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;

            //Attack
            if (lastAttackTime < 0)
            {
                anim.SetBool("Critical", characterStats.isCritical);
                anim.SetTrigger("Attack");
                //������ȴʱ��
                lastAttackTime = characterStats.attackData.collDown;
            }

            yield return new WaitForSeconds(characterStats.attackData.collDown);
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);
      
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;

            yield return null;
        }
        agent.isStopped = true;
        //Attack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //������ȴʱ��
            lastAttackTime = characterStats.attackData.collDown;
        }
    }
    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable") && !GetComponent<CharacterStats>().isHit)
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStats == Rock.RockStats.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStats = Rock.RockStats.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = 2*Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.Impulse);
            }
        }
        else if(attackTarget !=null && !GetComponent<CharacterStats>().isHit && (Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange))
        {
            var targtStats = attackTarget.GetComponent<CharacterStats>();
            targtStats.TakeDamage(characterStats, targtStats);
        }
    }
}
