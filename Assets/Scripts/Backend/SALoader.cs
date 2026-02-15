using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class SALoader
{
    public static bool FileExists(string path)
    {
        return System.IO.File.Exists(Path.Combine(Application.streamingAssetsPath, path));
    }

    public static string LoadText(string path)
    {
        StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));
        string text = reader.ReadToEnd();
        reader.Close();
        return text;
    }

    public static void SaveText(string text, string path)
    {
        File.WriteAllText(Path.Combine(Application.streamingAssetsPath, path), text);
    }

    public static Texture2D LoadTexture(string path)
    {
        byte[] bytes = File.ReadAllBytes((Path.Combine(Application.streamingAssetsPath, path)));
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);
        return tex;
    }

    public static void SaveTexture(Texture2D tex, string path)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.streamingAssetsPath, path), bytes);
    }

    public static Sprite LoadSprite(string path)
    {
        Texture2D tex = LoadTexture(path);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public static AudioClip LoadOggAudio(string path)
    {
        return OggVorbis.VorbisPlugin.Load(Path.Combine(Application.streamingAssetsPath, path));
    }
}
