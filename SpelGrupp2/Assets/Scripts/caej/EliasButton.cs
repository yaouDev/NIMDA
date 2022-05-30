using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliasButton : MonoBehaviour
{
    public EliasPlayer eliasPlayer;
    public EliasSetLevel setLevel;

    public void ChangeToSetLevel()
    {
        eliasPlayer.QueueEvent(setLevel.CreateSetLevelEvent(eliasPlayer.Elias));
    }

    public void StartElias()
    {
        eliasPlayer.StartElias();
    }
}
