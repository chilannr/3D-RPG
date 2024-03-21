using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����һ����̬��ExtensionMethod
public static class ExtensionMethod
{
    // ����һ��˽�г���dotThreshold��ֵΪ0.5f
    private const float dotThreshold = 0.5f;

    // ����һ����չ����IsFacingTarget�������ж������Ƿ�����Ŀ��
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        // ����Ŀ��λ��������λ�õ�������
        var vectorToTarget = target.position - transform.position;
        // ���������й�һ������
        vectorToTarget.Normalize();

        // ��������ǰ��������Ŀ�������ĵ��
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        // ���������ڵ���dotThreshold������Ϊ��������Ŀ�꣬����true�����򷵻�false
        return dot >= dotThreshold;
    }
}
