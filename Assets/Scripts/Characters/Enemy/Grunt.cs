using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 定义一个名为Grunt的类，继承自EnemyController
public class Grunt : EnemyController
{
    [Header("Skill")]
    // 定义一个公共浮点数kickForce，表示踢击力度，默认值为8
    public float kickForce = 8;

    // 定义一个公共方法KickOff，用于实现踢击动作
    public void KickOff()
    {
        // 如果攻击目标不为空
        if (attackTarget != null && !GetComponent<CharacterStats>().isHit)
        {
            // 使物体面向攻击目标
            transform.LookAt(attackTarget.transform);

            // 计算攻击目标位置与物体位置的向量差
            Vector3 direction = attackTarget.transform.position - transform.position;
            // 对向量进行归一化处理
            direction.Normalize();
            // 停止攻击目标的NavMeshAgent组件
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            // 设置攻击目标的NavMeshAgent组件的速度为方向乘以踢击力度
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            // 触发攻击目标的Animator组件的Dizzy触发器
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
