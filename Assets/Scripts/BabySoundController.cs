using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySoundController : MonoBehaviour
{
    [SerializeField] string kwakEvent;
    [SerializeField] string walkEvent;
    [SerializeField] string swimEvent;
    [SerializeField] string getIntoWaterEvent;
    [SerializeField] string screamingEvent;
    [SerializeField] string arriveAtNestEvent;

    FMOD.Studio.EventInstance swimmingInstance;
    FMOD.Studio.EventInstance screamingInstance;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PlayKwak()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(kwakEvent, gameObject);
    }

    public void PlayFootstep()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(walkEvent, gameObject);
    }

    public void PlaySwim()
    {
        FMOD.Studio.PLAYBACK_STATE playingState;
        swimmingInstance.getPlaybackState(out playingState);

        if (playingState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
        {
            swimmingInstance.release();
            swimmingInstance = FMODUnity.RuntimeManager.CreateInstance(swimEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(swimmingInstance, transform, rb);
            swimmingInstance.start();
        }

    }

    public void GetIntoWater()
    {
        FMODUnity.RuntimeManager.PlayOneShot(getIntoWaterEvent);
        swimmingInstance = FMODUnity.RuntimeManager.CreateInstance(swimEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(swimmingInstance, transform, rb);
        swimmingInstance.start();
    }

    public void ComeOutOfWater()
    {
        swimmingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        swimmingInstance.release();
    }

    public void StartScreaming()
    {
        screamingInstance = FMODUnity.RuntimeManager.CreateInstance(swimEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(screamingInstance, transform, rb);
        screamingInstance.start();
    }

    public void StopScreaming()
    {
        screamingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        screamingInstance.release();
    }

    public void ArriveAtNest()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(arriveAtNestEvent, gameObject);
    }
}
