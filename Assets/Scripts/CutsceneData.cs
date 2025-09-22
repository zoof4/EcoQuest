using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Cutscene/Cutscene Data")]
public class CutsceneData : ScriptableObject
{
    public Sprite[] cutsceneImages;
    public string[] cutsceneSubtitles;
    public AudioClip[] cutsceneAudioClips;
}