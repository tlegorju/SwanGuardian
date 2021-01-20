using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBarController : MonoBehaviour
{
    Image lifeBarImage;
    [SerializeField] float boundMin = .2f, boundMax = .8f;
    [SerializeField] Color hurtColor = Color.red, healthyColor = Color.green;


    private void Awake()
    {
        lifeBarImage = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        BabySwanController controller = GetComponentInParent<BabySwanController>();
        if (controller != null)
        {
            controller.OnLoseLife += UpdateLifeBar;
            UpdateLifeBar(controller.Life);
        }
        else
            Debug.Log("notfound");
    }

    // Update is called once per frame
    void UpdateLifeBar(float newLife)
    {
        if (newLife>=1 || newLife <= 0)
        {
            lifeBarImage.enabled = false;
            return;
        }

        lifeBarImage.enabled = true;

        lifeBarImage.fillAmount = newLife;
        lifeBarImage.color = Color.Lerp(hurtColor, healthyColor, Mathf.InverseLerp(boundMin, boundMax, newLife));
    }
}
