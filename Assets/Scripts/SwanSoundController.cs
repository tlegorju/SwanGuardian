using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwanSoundController : MonoBehaviour
{
    [SerializeField] string kwakEvent;
    [SerializeField] string walkEvent;
    [SerializeField] string swimEvent;
    [SerializeField] string getIntoWaterEvent;

    FMOD.Studio.EventInstance swimmingInstance;

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

        if(playingState==FMOD.Studio.PLAYBACK_STATE.STOPPED)
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
}
