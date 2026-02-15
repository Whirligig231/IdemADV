using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private Dictionary<string, AudioClip> loadedSongs;
    private string nextMusicTitle = "";
    private float musicT = 1.0f;
    private int musicDir = 0;

    private void Awake()
    {
        loadedSongs = new Dictionary<string, AudioClip>();
    }

    private void Update()
    {
        AudioSource audio = GetComponent<AudioSource>();

        musicT += musicDir * Time.deltaTime * 0.5f;
        musicT = Mathf.Clamp01(musicT);

        if (musicT > 0.9999f && musicDir > 0)
        {
            musicT = 0.0f;
            musicDir = -1;
            audio.Stop();
            audio.clip = loadedSongs[nextMusicTitle];
            audio.Play();
        }

        audio.volume = Mathf.Pow(1.0f - musicT, 0.7f);
    }

    private void PreloadMusic(string musicTitle)
    {
        if (loadedSongs.ContainsKey(musicTitle))
            return;

        AudioClip song = SALoader.LoadOggAudio("Music/" + musicTitle + ".ogg");
        loadedSongs[musicTitle] = song;
    }

    public void ChangeMusic(string musicTitle)
    {
        PreloadMusic(musicTitle);
        nextMusicTitle = musicTitle;
        musicDir = 1;
    }
}
