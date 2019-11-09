using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private List<VideoClip> _tutorialVideos;

    private int _currentVideo;
    private VideoPlayer _videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        _videoPlayer = this.gameObject.GetComponentInChildren<VideoPlayer>();
        _videoPlayer.clip = _tutorialVideos[_currentVideo];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextVideo()
    {
        _currentVideo++;
        if (_currentVideo < _tutorialVideos.Count)
        {
            this.gameObject.SetActive(true);
            _videoPlayer.clip = _tutorialVideos[_currentVideo];
        }
    }

    public void DisableTV()
    {
        this.gameObject.SetActive(false);
    }
}
