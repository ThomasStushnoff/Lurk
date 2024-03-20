using System;
using Objects;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom editor for the <see cref="AudioData"/> scriptable object.
/// Waveform reference:
/// <a href="https://github.com/jackyyang09/Simple-Unity-Audio-Manager/blob/master/Editor/AudioPlaybackToolEditor.cs"></a>
/// </summary>
[CustomEditor(typeof(AudioData))]
public class AudioDataEditor : Editor
{
    private const int SampleResolution = 512;
    private AudioSource _previewSource;
    private Texture2D _waveformTexture;
    private Texture2D _nonRepeatIcon;
    private Texture2D _pauseIcon;
    private Texture2D _playIcon;
    private Texture2D _repeatIcon;
    private Texture2D _restartIcon;
    private AudioData _currentData;
    private float _playbackPosition;
    private bool _isDraggingPlaybackLine;
    private bool _isRepeating;

    private void Awake()
    {
        _restartIcon = Resources.Load<Texture2D>("Sprites/UI/backward-step-solid");
        _playIcon = Resources.Load<Texture2D>("Sprites/UI/play-solid");
        _pauseIcon = Resources.Load<Texture2D>("Sprites/UI/pause-solid");
        _repeatIcon = Resources.Load<Texture2D>("Sprites/UI/repeat-solid");
        _nonRepeatIcon = Resources.Load<Texture2D>("Sprites/UI/non-repeat-solid");
    }

    private void OnEnable()
    {
        // Create a new audio source for previewing the audio
        _previewSource = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", 
            HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        
        // Subscribe to the update event to keep the playback line moving
        EditorApplication.update += OnAudioUpdate;
    }

    private void OnDisable()
    {
        // Clean up the preview source and waveform texture
        if (_previewSource) DestroyImmediate(_previewSource.gameObject);
        if (_waveformTexture) DestroyImmediate(_waveformTexture);
        
        // Unsubscribe from the update event
        EditorApplication.update -= OnAudioUpdate;
    }

    private void OnAudioUpdate()
    {
        if (!_previewSource.isPlaying || _isDraggingPlaybackLine) return;
        
        _playbackPosition = _previewSource.time;
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        _currentData = (AudioData)target;
        
        DrawDefaultInspector();
        
        // Draw the audio preview section if the clip exists.
        if (!_currentData.clip) return;
        
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Audio Preview", EditorStyles.boldLabel);
        
        if (!_waveformTexture || Math.Abs(_waveformTexture.width - EditorGUIUtility.currentViewWidth) > float.Epsilon)
            GenerateWaveform(_currentData.clip, (int)EditorGUIUtility.currentViewWidth, 300);

        DrawWaveformAndControls(_currentData.clip, EditorGUIUtility.currentViewWidth, 300);
    }
    
    private void DrawWaveformAndControls(AudioClip clip, float width, float height)
    {
        var rect = GUILayoutUtility.GetRect(width, height);
        if (_waveformTexture)
            GUI.DrawTexture(rect, _waveformTexture);

        // Playback line.
        var normalizedPosition = _playbackPosition / clip.length;
        var lineRect = new Rect(rect.x + normalizedPosition * rect.width, rect.y, 2, rect.height);
        EditorGUI.DrawRect(lineRect, Color.red);

        // Time label.
        var labelStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.UpperRight, fontSize = 11 };
        EditorGUI.LabelField(new Rect(EditorGUIUtility.currentViewWidth - 110, rect.y, 100, 20),
            $"{_playbackPosition:F2}s / {clip.length:F2}s", labelStyle);

        // Process events for the waveform and playback line interaction.
        ProcessEvents(rect, clip);

        #region Playback Controls
        
        GUILayout.BeginHorizontal();
        
        // Restart button.
        if (GUILayout.Button(_restartIcon, GUILayout.Width(25), GUILayout.Height(25)))
        {
            // Restart audio playback logic
            _playbackPosition = 0f;
            _previewSource.time = _playbackPosition;
            _previewSource.Play();
        }

        // Play/Pause button.
        var playStopIcon = _previewSource.isPlaying ? _pauseIcon : _playIcon;
        if (GUILayout.Button(playStopIcon, GUILayout.Width(25), GUILayout.Height(25)))
        {
            if (_previewSource.isPlaying)
            {
                _previewSource.Stop();
            }
            else
            {
                _previewSource.clip = clip;
                _previewSource.time = _playbackPosition;
                _previewSource.Play();
            }
        }

        // Repeat button.
        if (GUILayout.Button(_isRepeating ? _nonRepeatIcon : _repeatIcon, GUILayout.Width(25), GUILayout.Height(25)))
            _isRepeating = _previewSource.loop = !_isRepeating;

        GUILayout.EndHorizontal();

        #endregion

    }
    
    private void GenerateWaveform(AudioClip clip, int width, int height)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        _waveformTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        var waveformColor = new Color(1.0f, 0.7f, 0.2f);

        // Fill texture with background color.
        for (var x = 0; x < _waveformTexture.width; x++)
        {
            for (var y = 0; y < _waveformTexture.height; y++)
            {
                _waveformTexture.SetPixel(x, y, backgroundColor);
            }
        }

        // Draw waveform.
        var step = Mathf.CeilToInt((float)samples.Length / SampleResolution);
        for (var x = 0; x < width; x++)
        {
            float average = 0;
            for (var j = 0; j < step; j++)
            {
                var index = samples.Length / width * x + j;
                if (index < samples.Length)
                    average += Mathf.Abs(samples[index]);
            }
            
            // Get the average value of the samples.
            average /= step;
            
            // Calculate the height of the waveform.
            var heightOfWave = Mathf.Clamp((int)(average * height * 0.75f), 1, height);
            
            // Draw the waveform.
            for (var y = (height - heightOfWave) / 2; y < (height + heightOfWave) / 2; y++)
                _waveformTexture.SetPixel(x, y, waveformColor);
        }
        
        // Apply the changes to the texture.
        _waveformTexture.Apply();
    }
    
    /// <summary>
    /// Processes the events for the waveform and playback line interaction.
    /// </summary>
    /// <param name="rect">The rect of the waveform.</param>
    /// <param name="clip">The audio clip.</param>
    private void ProcessEvents(Rect rect, AudioClip clip)
    {
        var e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown when rect.Contains(e.mousePosition):
                _isDraggingPlaybackLine = true;
                UpdatePlaybackPosition(e.mousePosition, rect, clip);
                break;
            case EventType.MouseDrag when _isDraggingPlaybackLine:
                UpdatePlaybackPosition(e.mousePosition, rect, clip);
                break;
            case EventType.MouseUp:
                _isDraggingPlaybackLine = false;
                break;
        }
    }
    
    /// <summary>
    /// Updates the playback position based on the mouse position.
    /// </summary>
    /// <param name="mousePosition">The current mouse position.</param>
    /// <param name="rect">The rect of the waveform.</param>
    /// <param name="clip">The audio clip.</param>
    private void UpdatePlaybackPosition(Vector2 mousePosition, Rect rect, AudioClip clip)
    {
        _playbackPosition = (mousePosition.x - rect.x) / rect.width * clip.length;
        _playbackPosition = Mathf.Clamp(_playbackPosition, 0, clip.length);
        _previewSource.time = _playbackPosition;
        Repaint();
    }
}