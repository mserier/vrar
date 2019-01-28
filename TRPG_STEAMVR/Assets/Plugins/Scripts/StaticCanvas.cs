using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticCanvas : MonoBehaviour
{
    public static bool IsTalking = false;

    public Toggle voiceToggle;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnVoiceToggleChanged()
    {
        IsTalking = voiceToggle.isOn;
    }
}