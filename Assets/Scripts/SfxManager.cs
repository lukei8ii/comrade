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
        Slap,
        GlassDown,
        Vomit,
        HitTable,
        Ahh,
        Ow,
        Curse,
        Laugh,
        Win
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
    public AudioClip vomit;
    public AudioClip hitTable;

    public AudioClip[] ahh1;
    public AudioClip[] ahh2;

    public AudioClip[] ow1;
    public AudioClip[] ow2;

    public AudioClip[] curse1;
    public AudioClip[] curse2;

    public AudioClip[] laugh1;
    public AudioClip[] laugh2;

    public AudioClip win1;
    public AudioClip win2;

    private AudioSource m_AudioSource;

	// Use this for initialization
	void Start()
    {
		m_AudioSource = GetComponent<AudioSource>();

        Messenger.AddListener<PlayerController>(Events.OnSlap, Whoosh);
        Messenger.AddListener<PlayerController>(Events.OnSlapHit, Slapped);
        Messenger.AddListener<PlayerController>(Events.OnSlapSlapped, PotatoHit);
        Messenger.AddListener<PlayerController>(Events.OnPotatoDeflected, PotatoHit);
        Messenger.AddListener<GameObject>(Events.OnPotatoCollide, PotatoCollide);
        Messenger.AddListener<PlayerController>(Events.OnDrinkVodka, Drink);
        Messenger.AddListener<PlayerController>(Events.OnVodkaHit, FinishDrinking);
        Messenger.AddListener<PlayerController>(Events.OnVodkaDeflected, GlassBreak);
        Messenger.AddListener<PlayerController>(Events.OnThrowPotato, Whoosh);
        Messenger.AddListener<PlayerController>(Events.OnPotatoHit, PotatoHit);
        Messenger.AddListener<PlayerController>(Events.OnGlassDown, GlassDown);
        Messenger.AddListener<PlayerController>(Events.OnVomit, Vomit);
        Messenger.AddListener<PlayerController>(Events.OnHitTable, HitTable);
        Messenger.AddListener<PlayerController>(Events.OnSlapDeflected, Angry);
        Messenger.AddListener<PlayerController>(Events.OnGameOver, Win);
    }

    private void Win(PlayerController controller)
    {
        Play(Clip.Win, controller.playerNumber, 1.75f);
        Play(Clip.Laugh, controller.playerNumber, 3f);
        Play(Clip.Laugh, controller.playerNumber, 5.1f);
    }

    private void Angry(PlayerController controller)
    {
        Play(Clip.Curse, controller.playerNumber, 0.5f);
    }

    private void HitTable(PlayerController controller)
    {
        Play(Clip.HitTable);
    }

    private void Vomit(PlayerController controller)
    {
        Play(Clip.Vomit);
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
        Play(Clip.Ow, controller.playerNumber, 0.125f);
    }

    private void Drink(PlayerController controller)
    {
        Play(Clip.Drink);
    }

    private void FinishDrinking(PlayerController controller)
    {
        var coinToss = Random.Range(0, 2);

        if (coinToss > 0)
        {
            Play(Clip.Ahh, controller.playerNumber);
        } else
        {
            Play(Clip.Burp);
        }
    }

    private void Whoosh(PlayerController controller)
    {
        Play(Clip.Woosh);
    }

    private void PotatoHit(PlayerController controller)
    {
        Play(Clip.PotatoHit);
        Play(Clip.Ow, controller.playerNumber, 0.125f);
    }

    private void PotatoCollide(GameObject potato)
    {
        Play(Clip.PotatoHit);
    }

    public void Play(Clip clip, int playerNumber=1, float delay=0f)
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
            case Clip.Vomit:
                m_AudioSource.PlayOneShot(vomit);
                break;
            case Clip.HitTable:
                m_AudioSource.PlayOneShot(hitTable);
                break;
            case Clip.Ahh:
                PlayRandomPlayerEffects(playerNumber, ahh1, ahh2, delay);
                break;
            case Clip.Ow:
                PlayRandomPlayerEffects(playerNumber, ow1, ow2, delay);
                break;
            case Clip.Curse:
                PlayRandomPlayerEffects(playerNumber, curse1, curse2, delay);
                break;
            case Clip.Laugh:
                PlayRandomPlayerEffects(playerNumber, laugh1, laugh2, delay);
                break;
            case Clip.Win:
                if (playerNumber == 1)
                    m_AudioSource.PlayOneShot(win1);
                else
                    m_AudioSource.PlayOneShot(win2);
                break;
            default:
                break;
        }
    }

    void PlayRandomPlayerEffects(int playerNumber, AudioClip[] player1, AudioClip[] player2, float delay)
    {
        if (playerNumber == 1)
        {
            StartCoroutine(PlayRandomDelayed(player1, delay));
        } else
        {
            StartCoroutine(PlayRandomDelayed(player2, delay));
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
        Messenger.RemoveListener<PlayerController>(Events.OnPotatoDeflected, PotatoHit);
        Messenger.RemoveListener<GameObject>(Events.OnPotatoCollide, PotatoCollide);
        Messenger.RemoveListener<PlayerController>(Events.OnDrinkVodka, Drink);
        Messenger.RemoveListener<PlayerController>(Events.OnVodkaHit, FinishDrinking);
        Messenger.RemoveListener<PlayerController>(Events.OnVodkaDeflected, GlassBreak);
        Messenger.RemoveListener<PlayerController>(Events.OnThrowPotato, Whoosh);
        Messenger.RemoveListener<PlayerController>(Events.OnPotatoHit, PotatoHit);
        Messenger.RemoveListener<PlayerController>(Events.OnGlassDown, GlassDown);
        Messenger.RemoveListener<PlayerController>(Events.OnVomit, Vomit);
        Messenger.RemoveListener<PlayerController>(Events.OnHitTable, HitTable);
        Messenger.RemoveListener<PlayerController>(Events.OnSlapDeflected, Angry);
        Messenger.RemoveListener<PlayerController>(Events.OnGameOver, Win);
    }
}
