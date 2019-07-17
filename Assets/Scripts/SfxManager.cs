using UnityEngine;
using System.Collections;

public class SfxManager : MonoBehaviour
{
	public AudioClip ambiance;

	private AudioSource m_AudioSource;

	// Use this for initialization
	void Start()
    {
		m_AudioSource = GetComponent<AudioSource>();

		if (ambiance)
		{
            m_AudioSource.clip = ambiance;
            m_AudioSource.loop = true;
            m_AudioSource.Play();
		}
	}

    // Update is called once per frame
    void Update()
    {

    }
}
