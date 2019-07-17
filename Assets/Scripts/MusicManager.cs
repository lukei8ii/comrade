using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip music;

    private AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();

        if (music)
        {
            m_AudioSource.clip = music;
            m_AudioSource.loop = true;
            m_AudioSource.Play();
        }
    }

}
