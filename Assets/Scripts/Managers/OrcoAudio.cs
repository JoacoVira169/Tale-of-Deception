using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OrcoAudio : MonoBehaviour
{
    public AudioClip clipCaminarOrco;
    public AudioClip clipCorrerOrco;
    public AudioClip clipGolpeHachaOrco;
    public AudioClip clipImpactoPlayer;
    public AudioClip clipDañoOrco;

    private AudioSource altavoz;
    private AudioSource altavozEfectos;

     void Awake()
    {
        altavoz = GetComponent<AudioSource>();
        altavoz.loop = false;
        altavoz.playOnAwake = false;
        altavoz.pitch = 1f;
        altavoz.dopplerLevel = 0f;

        altavozEfectos = gameObject.AddComponent<AudioSource>();
        altavozEfectos.playOnAwake = false;
        altavozEfectos.loop = false;
        altavozEfectos.pitch = 1f;
        altavozEfectos.dopplerLevel = 0f;
        altavozEfectos.outputAudioMixerGroup = altavoz.outputAudioMixerGroup;
        altavozEfectos.volume = altavoz.volume;
        altavozEfectos.spatialBlend = altavoz.spatialBlend;
        altavozEfectos.minDistance = altavoz.minDistance;
        altavozEfectos.maxDistance = altavoz.maxDistance;
        altavozEfectos.rolloffMode = altavoz.rolloffMode;
    }
    public void ReproducirPasoCaminando()
    {
        ReproducirPaso(clipCaminarOrco);
    }

    public void ReproducirPasoCorriendoOrco()
    {
        ReproducirPaso(clipCorrerOrco);
    }

    public void ReproducirGolpeHacha()
    {
        ReproducirEfecto(clipGolpeHachaOrco);
    }

    public void ReproducirImpactoPlayer()
    {
        ReproducirEfecto(clipImpactoPlayer);
    }

    public void ReproducirDañoOrco()
    {
        ReproducirEfecto(clipDañoOrco);
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

        altavoz.pitch = 1f;
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

