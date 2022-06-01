using System.Collections;
using UnityEngine;

public class EliasDemoEventTrigger : MonoBehaviour
{
    public bool useEliasFromAudioController;

	public EliasPlayer eliasPlayer;
    public bool useSetLevel;
    public EliasSetLevel setLevel;

    public bool useSetLevelOnTrack;
    public EliasSetLevelOnTrack setLevelOnTrack;

    public bool usePlayStinger;
	public EliasPlayStinger playStinger;

	public bool useActionPreset;
	public string actionPresetName;
	public bool allowRequiredThemeMissmatch;

	public bool useDoubleEffectParam;
	public EliasSetEffectParameterDouble doubleEffectParam;

	public bool useSetSendVolume;
	public EliasSetSendVolume setSendVolume;

    private void Start()
    {
        if (useEliasFromAudioController) eliasPlayer = AudioController.instance.eliasPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(eliasPlayer.Elias);

        if (useSetLevel)
        {
            eliasPlayer.QueueEvent(setLevel.CreateSetLevelEvent(eliasPlayer.Elias));
        }
        if (useSetLevelOnTrack)
        {
            eliasPlayer.QueueEvent(setLevelOnTrack.CreateSetLevelOnTrackEvent(eliasPlayer.Elias));
        }
        if (usePlayStinger)
		{
			eliasPlayer.QueueEvent (playStinger.CreatePlayerStingerEvent (eliasPlayer.Elias));
		} 
		if (useActionPreset)
		{
			eliasPlayer.RunActionPreset (actionPresetName, allowRequiredThemeMissmatch);
		} 
		if (useDoubleEffectParam)
		{
			eliasPlayer.QueueEvent (doubleEffectParam.CreateSetEffectParameterEvent(eliasPlayer.Elias));
        }
        if (useSetSendVolume)
        {
            eliasPlayer.QueueEvent(setSendVolume.CreateSetSendVolumeEvent(eliasPlayer.Elias));
        }
    }
}