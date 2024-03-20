using Audio;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for the <see cref="AudioPitchController"/> component.
/// </summary>
[CustomEditor(typeof(AudioPitchController))]
public class AudioPitchControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var t = (AudioPitchController)target;

        t.pitchType = (PitchType)EditorGUILayout.EnumPopup("Pitch Type", t.pitchType);

        switch (t.pitchType)
        {
            case PitchType.Random:
                t.minPitch = EditorGUILayout.Slider("Min Pitch", t.minPitch, -3.0f, 3.0f);
                t.maxPitch = EditorGUILayout.Slider("Max Pitch", t.maxPitch, -3.0f, 3.0f);
                break;
            case PitchType.Increase:
            case PitchType.Decrease:
                t.initialPitch = EditorGUILayout.Slider("Initial Pitch", t.initialPitch, -3.0f, 3.0f);
                t.pitchStep = EditorGUILayout.Slider("Pitch Step", t.pitchStep, 0.1f, 6.0f);
                break;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(t);
    }
}