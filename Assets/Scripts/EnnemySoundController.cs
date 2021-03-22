using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemySoundController : MonoBehaviour
{
    [SerializeField] string attackingEvent;
    [SerializeField] string walkEvent;
    [SerializeField] string swimEvent;
    [SerializeField] string getIntoWaterEvent;
    [SerializeField] string fleeingEvent;
    [SerializeField] string eatingEvent;

    FMOD.Studio.EventInstance swimmingInstance;
    FMOD.Studio.EventInstance eatingInstance;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PlayAttack()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(attackingEvent, gameObject);
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

    public void StartEating()
    {
        eatingInstance = FMODUnity.RuntimeManager.CreateInstance(eatingEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(eatingInstance, transform, rb);
        eatingInstance.start();
    }

    public void StopEating()
    {
        eatingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eatingInstance.release();
    }

    public void Flee()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(fleeingEvent, gameObject);
    }
}
