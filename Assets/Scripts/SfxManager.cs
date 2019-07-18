using UnityEngine;
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
        Slap
    };

    public AudioClip[] burps;
    public AudioClip[] drinks;
    public AudioClip[] glassBreaks;
    public AudioClip[] hiccups;
    public AudioClip passOut;
    public AudioClip[] wooshes;
    public AudioClip[] potatoHits;
    public AudioClip[] slaps;

	private AudioSource m_AudioSource;

	// Use this for initialization
	void Start()
    {
		m_AudioSource = GetComponent<AudioSource>();

        Messenger.AddListener<PlayerController>(Events.OnSlap, Slapped);
        Messenger.AddListener<PlayerController>(Events.OnDrinkVodka, Vodkaed);
        Messenger.AddListener<PlayerController>(Events.OnThrowPotato, Potatoed);
    }

    void Slapped(PlayerController controller)
    {
        Play(Clip.Slap);
    }

    private void Vodkaed(PlayerController controller)
    {
        Play(Clip.Drink);
    }

    private void Potatoed(PlayerController controller)
    {
        // TODO: play woosh, if hit, play potatoHit
        //Play(Clip.PotatoHit);
        Play(Clip.Woosh);
    }

    public void Play(Clip clip)
    {
        switch (clip)
        {
            case Clip.Burp:
                PlayRandom(burps);
                break;
            case Clip.Drink:
                StartCoroutine(PlayRandomDelayed(drinks, 0.25f));
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
                StartCoroutine(PlayRandomDelayed(wooshes, 1f));
                break;
            case Clip.PotatoHit:
                PlayRandom(potatoHits);
                break;
            case Clip.Slap:
                StartCoroutine(PlayRandomDelayed(slaps, 0.125f));
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
}
