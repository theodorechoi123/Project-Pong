using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;
    public float mzMaxTimer = 1f;
    public float timer = 0f;
    public bool isFlashing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > mzMaxTimer)
        {
            muzzleFlash.SetActive(false);
            isFlashing = false;
        }
    }

    public void MuzzleFlashing()
    {
        isFlashing = true;
        muzzleFlash.SetActive(true);
        timer = 0f;
    }
}
