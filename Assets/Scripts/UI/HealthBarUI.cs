using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab; // 血条UI的预制体
    public Transform barPoint; // 血条位置的Transform
    public bool alwaysVisible; // 是否始终可见

    public float visibleTime; // 可见时间
    private float timeLeft;

    Image healthSlider; // 血条UI的Image组件
    Transform UIbar; // 血条UI的Transform
    Transform cam; // 主摄像机的Transform

    CharacterStats currentStats; // 当前角色的Stats组件

    private void Awake()
    {
        currentStats=GetComponent<CharacterStats>(); // 获取CharacterStats组件
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar; // 订阅UpdateHealthBarOnAttack事件，用于更新血条
    }

    private void OnEnable()
    {
        cam = Camera.main.transform; // 获取主摄像机的Transform组件

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace) // 找到渲染模式为WorldSpace的Canvas
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform; // 在该Canvas下实例化血条UI
                healthSlider = UIbar.GetChild(0).GetComponent<Image>(); // 获取血条UI的Image组件
                UIbar.gameObject.SetActive(alwaysVisible); // 设置血条UI的初始可见性
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int MaxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject); // 如果当前生命值小于等于0，销毁血条UI

        UIbar.gameObject.SetActive(true); // 设置血条UI可见

        timeLeft = visibleTime;
        float sliderPercent = (float)currentHealth / MaxHealth; // 计算血条百分比
        healthSlider.fillAmount = sliderPercent; // 更新血条UI的填充量
    }
    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;

            UIbar.forward = -cam.forward;

            if (timeLeft <= 0&& !alwaysVisible )
                UIbar.gameObject.SetActive(false);
            else
               timeLeft -= Time.deltaTime; 
            
        }
    }
}