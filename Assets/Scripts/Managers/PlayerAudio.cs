using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Pistas de Audio")]
    public AudioClip clipCaminar;
    public AudioClip clipCorrer;
    public AudioClip clipLento;
    public AudioClip clipSalto;
    public AudioClip clipAterrizaje;
    public AudioClip clipGolpeEspada;
    public AudioClip clipImpactoOrco;
    public AudioClip clipDaño;

    private AudioSource altavoz;
    private AudioSource altavozEfectos;

    void Awake()
    {
        altavoz = GetComponent<AudioSource>();
        altavoz.loop = false;
        altavoz.playOnAwake = false;

        altavozEfectos = gameObject.AddComponent<AudioSource>();
        altavozEfectos.playOnAwake = false;
        altavozEfectos.loop = false;
        altavozEfectos.outputAudioMixerGroup = altavoz.outputAudioMixerGroup;
        altavozEfectos.volume = altavoz.volume;
        altavozEfectos.spatialBlend = altavoz.spatialBlend;
        altavozEfectos.minDistance = altavoz.minDistance;
        altavozEfectos.maxDistance = altavoz.maxDistance;
        altavozEfectos.rolloffMode = altavoz.rolloffMode;
    }

    public void ReproducirPasoCaminando()
    {
        ReproducirPaso(clipCaminar);
    }

    public void ReproducirPasoCorriendo()
    {
        ReproducirPaso(clipCorrer);
    }

    public void ReproducirPasoAgachado()
    {
        ReproducirPaso(clipLento);
    }

    public void ReproducirSalto()
    {
        ReproducirEfecto(clipSalto);
    }

    public void ReproducirAterrizaje()
    {
        ReproducirEfecto(clipAterrizaje);
    }

    public void ReproducirGolpeEspada()
    {
        ReproducirEfecto(clipGolpeEspada);
    }

    public void ReproducirImpactoOrco()
    {
        ReproducirEfecto(clipImpactoOrco);
    }

    public void ReproducirDaño()
    {
        ReproducirEfecto(clipDaño);
    }

    public void DetenerPasos()
    {
        if (altavoz.isPlaying)
        {
            altavoz.Stop();
        }
    }

    private void ReproducirPaso(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        altavoz.pitch = Random.Range(0.9f, 1.1f);
        altavoz.clip = clip;
        altavoz.Play();
    }

    private void ReproducirEfecto(AudioClip clip)
    {
        if (clip != null)
        {
            altavozEfectos.PlayOneShot(clip);
        }
    }
}
