using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliasButton : MonoBehaviour
{
    private AudioController ac;
    [SerializeField] private EliasSetLevel setLevel;

    private void Start()
    {
        ac = AudioController.instance;
    }

    public void SetLevel()
    {
        ac.SetEliasLevel(setLevel);
    }

    public void StartElias()
    {
        ac.StartElias();
    }

}
