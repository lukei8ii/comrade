using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int playerNumber;

    Slider m_Slider;

    // Start is called before the first frame update
    void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.value = m_Slider.maxValue;
        Messenger.AddListener<PlayerController>(Events.HealthAdjusted, UpdateHealth);
    }

    private void UpdateHealth(PlayerController controller)
    {
        if (controller.playerNumber == playerNumber)
        {
            m_Slider.value = controller.HealthPercentage();
        }
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.HealthAdjusted, UpdateHealth);
    }
}
