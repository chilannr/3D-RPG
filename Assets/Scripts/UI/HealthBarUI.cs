using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab; // Ѫ��UI��Ԥ����
    public Transform barPoint; // Ѫ��λ�õ�Transform
    public bool alwaysVisible; // �Ƿ�ʼ�տɼ�

    public float visibleTime; // �ɼ�ʱ��
    private float timeLeft;

    Image healthSlider; // Ѫ��UI��Image���
    Transform UIbar; // Ѫ��UI��Transform
    Transform cam; // ���������Transform

    CharacterStats currentStats; // ��ǰ��ɫ��Stats���

    private void Awake()
    {
        currentStats=GetComponent<CharacterStats>(); // ��ȡCharacterStats���
        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar; // ����UpdateHealthBarOnAttack�¼������ڸ���Ѫ��
    }

    private void OnEnable()
    {
        cam = Camera.main.transform; // ��ȡ���������Transform���

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace) // �ҵ���ȾģʽΪWorldSpace��Canvas
            {
                UIbar = Instantiate(healthUIPrefab, canvas.transform).transform; // �ڸ�Canvas��ʵ����Ѫ��UI
                healthSlider = UIbar.GetChild(0).GetComponent<Image>(); // ��ȡѪ��UI��Image���
                UIbar.gameObject.SetActive(alwaysVisible); // ����Ѫ��UI�ĳ�ʼ�ɼ���
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int MaxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject); // �����ǰ����ֵС�ڵ���0������Ѫ��UI

        UIbar.gameObject.SetActive(true); // ����Ѫ��UI�ɼ�

        timeLeft = visibleTime;
        float sliderPercent = (float)currentHealth / MaxHealth; // ����Ѫ���ٷֱ�
        healthSlider.fillAmount = sliderPercent; // ����Ѫ��UI�������
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