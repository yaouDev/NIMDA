using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public static AIData instance;

    [SerializeField] private GameObject bullet;
    //[SerializeField] private GameObject muzzleflash;

    private void Start()
    {
        instance ??= this;
    }

    public GameObject getBullet
    {
        get { return bullet; }
    }
   /* public GameObject getMuzzleflash
    {
        get { return muzzleflash; }
    }
*/
    //har en array av olika


}
