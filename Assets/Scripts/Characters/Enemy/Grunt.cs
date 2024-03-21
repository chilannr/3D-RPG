using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ����һ����ΪGrunt���࣬�̳���EnemyController
public class Grunt : EnemyController
{
    [Header("Skill")]
    // ����һ������������kickForce����ʾ�߻����ȣ�Ĭ��ֵΪ8
    public float kickForce = 8;

    // ����һ����������KickOff������ʵ���߻�����
    public void KickOff()
    {
        // �������Ŀ�겻Ϊ��
        if (attackTarget != null && !GetComponent<CharacterStats>().isHit)
        {
            // ʹ�������򹥻�Ŀ��
            transform.LookAt(attackTarget.transform);

            // ���㹥��Ŀ��λ��������λ�õ�������
            Vector3 direction = attackTarget.transform.position - transform.position;
            // ���������й�һ������
            direction.Normalize();
            // ֹͣ����Ŀ���NavMeshAgent���
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            // ���ù���Ŀ���NavMeshAgent������ٶ�Ϊ��������߻�����
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            // ��������Ŀ���Animator�����Dizzy������
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
