using UnityEngine;
using System.Collections;

public class Instructions : MonoBehaviour
{
    int m_JoystickCount = 0;

    public GameObject keyboardInstructions;
    public GameObject joystickInstructions;

    void Update()
    {
        var joystickCount = Input.GetJoystickNames().Length;
        if (joystickCount != m_JoystickCount)
        {
            m_JoystickCount = joystickCount;
            Debug.Log($"{joystickCount} Joysticks currently connected");
        }

        if (IsJoystickConnected())
        {
            keyboardInstructions.SetActive(false);
            joystickInstructions.SetActive(true);
        } else
        {
            keyboardInstructions.SetActive(true);
            joystickInstructions.SetActive(false);
        }
    }

    public bool IsJoystickConnected()
    {
        return m_JoystickCount > 0;
    }
}
