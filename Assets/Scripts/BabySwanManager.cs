using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabySwanManager : MonoBehaviour
{
    private static BabySwanManager instance;
    public static BabySwanManager Instance { get { return instance; } }


    public BabySwanController[] babySwans;
    public List<BabySwanController> babySwanSaved = new List<BabySwanController>();
    public List<BabySwanController> babySwanDead = new List<BabySwanController>();

    private void Awake()
    {
        if (instance)
            Destroy(this);

        instance = this;
    }

    public void OnBabyDies(BabySwanController baby)
    {
        babySwanDead.Add(baby);
        CheckIfGameOver();
        CheckIfWon();
    }

    public void OnBabySaved(BabySwanController baby)
    {
        babySwanSaved.Add(baby);
        CheckIfWon();
    }

    public int NumberOfBabyLeftToSave()
    {
        return babySwans.Length - (NumberOfDeadBaby() + babySwanSaved.Count);
    }

    public int NumberOfDeadBaby()
    {
        return babySwanDead.Count;
    }

    public void CheckIfWon()
    {
        if (NumberOfDeadBaby() != babySwans.Length && NumberOfBabyLeftToSave() == 0)
            Debug.Log("YOU WIN");
    }

    public void CheckIfGameOver()
    {
        if(NumberOfDeadBaby() == babySwans.Length)
            Debug.Log("All baby dead, you lose");
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
