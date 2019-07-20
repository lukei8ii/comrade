﻿using UnityEngine;
using System.Collections;

public class SfxManager : MonoBehaviour
{
    public enum Clip {
        Burp,
        Drink,
        GlassBreak,
        Hiccup,
        PassOut,
        Woosh,
        PotatoHit,
        Slap,
        GlassDown
    };

    public AudioClip[] burps;
    public AudioClip[] drinks;
    public AudioClip[] glassBreaks;
    public AudioClip[] hiccups;
    public AudioClip passOut;
    public AudioClip[] wooshes;
    public AudioClip[] potatoHits;
    public AudioClip[] slaps;
    public AudioClip putGlassDown;

	private AudioSource m_AudioSource;

	// Use this for initialization
	void Start()
    {
		m_AudioSource = GetComponent<AudioSource>();

        Messenger.AddListener<PlayerController>(Events.OnSlap, Whoosh);
        Messenger.AddListener<PlayerController>(Events.OnSlapHit, Slapped);
        Messenger.AddListener<PlayerController>(Events.OnSlapSlapped, PotatoHit);
        Messenger.AddListener<GameObject>(Events.OnPotatoCollide, PotatoCollide);
        Messenger.AddListener<PlayerController>(Events.OnDrinkVodka, Drink);
        Messenger.AddListener<PlayerController>(Events.OnVodkaDeflected, GlassBreak);
        Messenger.AddListener<PlayerController>(Events.OnThrowPotato, Whoosh);
        Messenger.AddListener<PlayerController>(Events.OnPotatoHit, PotatoHit);
        Messenger.AddListener<PlayerController>(Events.OnGlassDown, GlassDown);
    }

    private void GlassDown(PlayerController controller)
    {
        Play(Clip.GlassDown);
    }

    private void GlassBreak(PlayerController controller)
    {
        Play(Clip.GlassBreak);
    }

    void Slapped(PlayerController controller)
    {
        Play(Clip.Slap);
    }

    private void Drink(PlayerController controller)
    {
        Play(Clip.Drink);
    }

    private void Whoosh(PlayerController controller)
    {
        Play(Clip.Woosh);
    }

    private void PotatoHit(PlayerController controller)
    {
        Play(Clip.PotatoHit);
    }

    private void PotatoCollide(GameObject potato)
    {
        Play(Clip.PotatoHit);
    }

    public void Play(Clip clip)
    {
        switch (clip)
        {
            case Clip.Burp:
                PlayRandom(burps);
                break;
            case Clip.Drink:
                PlayRandom(drinks);
                break;
            case Clip.GlassBreak:
                PlayRandom(glassBreaks);
                break;
            case Clip.Hiccup:
                PlayRandom(hiccups);
                break;
            case Clip.PassOut:
                m_AudioSource.PlayOneShot(passOut);
                break;
            case Clip.Woosh:
                PlayRandom(wooshes);
                break;
            case Clip.PotatoHit:
                PlayRandom(potatoHits);
                break;
            case Clip.Slap:
                PlayRandom(slaps);
                break;
            case Clip.GlassDown:
                m_AudioSource.PlayOneShot(putGlassDown);
                break;
            default:
                break;
        }
    }

    void PlayRandom(AudioClip[] clips)
    {
        m_AudioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    IEnumerator PlayRandomDelayed(AudioClip[] clips, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        m_AudioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<PlayerController>(Events.OnSlap, Whoosh);
        Messenger.RemoveListener<PlayerController>(Events.OnSlapHit, Slapped);
        Messenger.RemoveListener<PlayerController>(Events.OnSlapSlapped, PotatoHit);
        Messenger.RemoveListener<GameObject>(Events.OnPotatoCollide, PotatoCollide);
        Messenger.RemoveListener<PlayerController>(Events.OnDrinkVodka, Drink);
        Messenger.RemoveListener<PlayerController>(Events.OnVodkaDeflected, GlassBreak);
        Messenger.RemoveListener<PlayerController>(Events.OnThrowPotato, Whoosh);
        Messenger.RemoveListener<PlayerController>(Events.OnPotatoHit, PotatoHit);
        Messenger.RemoveListener<PlayerController>(Events.OnGlassDown, GlassDown);
    }
}
