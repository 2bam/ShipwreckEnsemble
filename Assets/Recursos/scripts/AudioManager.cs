using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _AudioSFX;
    [SerializeField] private AudioSource _AmbientAudio;

    [Header("Sonidos Ambiente")]
    public AudioClip[] AmbientSound;

    [Header("Sonidos Start/End")]
    //public AudioClip SFXStart;
    public AudioClip SFXGameOver;

    [Header("Sonidos Player SFX")]
    public AudioClip[] SFXPlayerRotation;
    public AudioClip[] SFXPlayerEnsamble;
    public AudioClip[] SFXHit;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayStartGame();
    }

    public void PlaySound(AudioClip SFX)
    {
        _AudioSFX.PlayOneShot(SFX, 1); //reproduce el sonido que se le envía
    }

    public void PlayGameOver()
    {
        _AudioSFX.Stop(); //frena la musica ambiente
        PlaySound(SFXGameOver);
    }

    public void PlayStartGame()
    {
        //reproduce aleatoriamente un sonido de los que hay en el array AmbientSound
        int rnd = Random.Range(0, AmbientSound.Length);
        _AmbientAudio.clip = AmbientSound[rnd]; //agrega el tema al audiosource
        _AmbientAudio.time = 0; //resetea el tema
        _AmbientAudio.Play(); //reproduce el tema
    }
    public void PlayerRotation()
    {
        int rnd = Random.Range(0, SFXPlayerRotation.Length);
        PlaySound(SFXPlayerRotation[rnd]);
    }
    public void PlayerHit()
    {
        int rnd = Random.Range(0, SFXHit.Length);
        PlaySound(SFXHit[rnd]);
    }
}
