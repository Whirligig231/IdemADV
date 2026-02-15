using System;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour, Taskable
{
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.enabled = false;
    }

    private void Update()
    {
        if (videoPlayer.enabled && videoPlayer.time > 0.5f && !videoPlayer.isPlaying)
            videoPlayer.enabled = false;
    }

    public void LoadVideo(string fname)
    {
        string url = "file://" + Application.streamingAssetsPath + "/Video/" + fname + ".mp4";
        videoPlayer.enabled = true;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.Play();
    }

    public bool HasFinished()
    {
        return !videoPlayer.enabled;
    }
}
