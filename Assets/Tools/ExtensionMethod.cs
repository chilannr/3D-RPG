using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 定义一个静态类ExtensionMethod
public static class ExtensionMethod
{
    // 定义一个私有常量dotThreshold，值为0.5f
    private const float dotThreshold = 0.5f;

    // 定义一个扩展方法IsFacingTarget，用于判断物体是否面向目标
    public static bool IsFacingTarget(this Transform transform, Transform target)
    {
        // 计算目标位置与物体位置的向量差
        var vectorToTarget = target.position - transform.position;
        // 对向量进行归一化处理
        vectorToTarget.Normalize();

        // 计算物体前方向量与目标向量的点积
        float dot = Vector3.Dot(transform.forward, vectorToTarget);

        // 如果点积大于等于dotThreshold，则认为物体面向目标，返回true；否则返回false
        return dot >= dotThreshold;
    }
}
