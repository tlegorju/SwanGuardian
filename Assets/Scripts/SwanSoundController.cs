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

    public void PlayKwak()
    {
        FMODUnity.RuntimeManager.PlayOneShot(kwakEvent);
    }

    public void PlayFootstep()
    {
        FMODUnity.RuntimeManager.PlayOneShot(walkEvent);
    }

    public void GetIntoWater()
    {
        FMODUnity.RuntimeManager.PlayOneShot(getIntoWaterEvent);
        swimmingInstance = FMODUnity.RuntimeManager.CreateInstance(swimEvent);
        swimmingInstance.start();
    }

    public void ComeOutOfWater()
    {
        swimmingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
