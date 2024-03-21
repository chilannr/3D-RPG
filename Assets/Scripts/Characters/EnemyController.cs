using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,PATROL,CHASE,DEAD
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{

    private EnemyStates enemyStates;
    
    private NavMeshAgent agent;

    private Animator anim;
    private Collider coll;

    protected CharacterStats characterStats;

    [Header("Basic Setting")]
    public float sightRadius;
    private float speed;//Ĭ��
    protected GameObject attackTarget;

    public float lookAtTime;
    private float remainLookAtTime;
    public float lastAttackTime;
    public bool isGuard;
    private Quaternion guardRoation;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;


    //bool��϶���
    bool isWalk;
    bool isChase;
    bool isFollow;
    public bool isDead;
    bool playerDead;
    bool isHit;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        coll = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRoation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            GetNewWayPoint();
            enemyStates = EnemyStates.PATROL;
        }
        isDead = false;
  
       
    }
    private void OnEnable()
    {
         GameManager.Instance.AddObserver(this);
    }
    void OnDisable()
    {
        if (!GameManager.IsInstialized) return;
         GameManager.Instance.RemoveObserver(this);
    }
    private void Update()
    {
        if (characterStats.CurrentHealth <= 0)
            isDead = true;
        if (!playerDead) 
        { 
        SwitchStates();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
          }
        if (playerDead)
        {
            GameManager.Instance.RemoveObserver(this);
        }
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk",isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);

    }
    void SwitchStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //������ַ���player
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                        isWalk = false;
                    transform.rotation = Quaternion.Lerp(transform.rotation, guardRoation,0.01f);
                }

                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //�ж��Ƿ������Ѳ�ߵ㣻
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                
                //��϶���
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {//���ѻ���һ״̬

                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                        agent.destination = transform.position;
                    }
                    else if (isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                     
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                //�ڹ�����Χ���򹥻�
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.collDown;
                        //�����ж�
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //ִ�й���
                        Attack();

                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject,2f);
                break;

        }
    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else
            return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else
            return false;
    }
    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
   
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }


    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach(var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }    
        }
        attackTarget = null;
        return false;
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //����������
            anim.SetTrigger("Attack");
        }else if (TargetInSkillRange())
        {
            //���ܹ�������
            anim.SetTrigger("Skill");
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }
    //Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform) && !GetComponent<CharacterStats>().isHit && (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)) { 

        var targtStats = attackTarget.GetComponent<CharacterStats>();
        targtStats.TakeDamage(characterStats, targtStats); 
        }
    }

    public void EndNotify()
    {
        //��ʤ
        //ֹͣ�����ƶ�
        //ֹͣAgent
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
        anim.SetBool("Win", true);
    }
}
