using System;
using UnityEngine;

public class TextboxSound : MonoBehaviour
{
    private bool isSoundLine = false;
    private float t = 0.0f;
    private int SAMPLES_PER_BUCKET = 5000;
    private float[] rmsValues;

    private void Update()
    {
        t += Time.deltaTime;
    }

    private string GetLineFilename(string name, string textLine)
    {
        string path = "Dialogue/" + name.Trim().Replace(" ", "_");
        path += "/";

        for (int i = 0; i < 10; i++)
        {
            if (textLine[i] >= 'A' && textLine[i] <= 'Z')
                path += textLine[i];
            else if (textLine[i] >= 'a' && textLine[i] <= 'z')
                path += (char)(textLine[i] + 'A' - 'a');
            else if (textLine[i] >= '0' && textLine[i] <= '9')
                path += textLine[i];
            else
                path += '_';
        }

        path += '_';
        int hashMod = (textLine.GetHashCode() & 0xffff);
        path += hashMod.ToString("x4").ToUpper();

        return path;
    }

    private void BuildRMSValues(AudioClip clip)
    {
        int numBuckets = clip.samples / SAMPLES_PER_BUCKET;
        rmsValues = new float[numBuckets];
        float maxRMS = 0.0f;
        float[] allSamples = new float[clip.samples * clip.channels];
        clip.GetData(allSamples, 0);

        for (int i = 0; i < numBuckets; i++)
        {
            float totalInBucket = 0.0f;

            for (int j = 0; j < SAMPLES_PER_BUCKET; j++)
            {
                int frameNumber = i * SAMPLES_PER_BUCKET + j;
                for (int k = 0; k < clip.channels; k++)
                {
                    float thisSample = allSamples[frameNumber * clip.channels + k];
                    totalInBucket += thisSample * thisSample;
                }
            }

            totalInBucket /= SAMPLES_PER_BUCKET * clip.channels;
            rmsValues[i] = Mathf.Sqrt(totalInBucket);
            if (rmsValues[i] > maxRMS)
                maxRMS = rmsValues[i];
        }

        for (int i = 0; i < numBuckets; i++)
            rmsValues[i] /= maxRMS;
    }

    public void LoadLine(string name, string textLine)
    {
        string fname = GetLineFilename(name, textLine);
        Debug.Log(fname);
        t = 0.0f;
        if (SALoader.FileExists(fname + ".ogg"))
        {
            AudioSource source = GetComponent<AudioSource>();
            AudioClip clip = SALoader.LoadOggAudio(fname + ".ogg");
            source.clip = clip;
            BuildRMSValues(clip);
            isSoundLine = true;
        }
        else
            isSoundLine = false;
    }

    public void ClearLine()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source.isPlaying)
            source.Stop();
        isSoundLine = false;
    }

    public bool IsSoundLine()
    {
        return isSoundLine;
    }

    public float GetSoundLevel()
    {
        if (!IsSoundLine())
            return Mathf.Pow(Mathf.Sin(t * 8.0f), 2.0f);

        float sampleIndex = GetComponent<AudioSource>().timeSamples;
        int n = rmsValues.Length;
        int halfBucket = SAMPLES_PER_BUCKET / 2;
        int allButHalfBucket = SAMPLES_PER_BUCKET * (n - 1) + SAMPLES_PER_BUCKET / 2;
        if (sampleIndex < halfBucket)
        {
            return Mathf.Lerp(0, rmsValues[0], sampleIndex / halfBucket);
        }
        else if (sampleIndex >= allButHalfBucket)
        {
            return Mathf.Lerp(rmsValues[n-1], 0, (sampleIndex - allButHalfBucket) / halfBucket);
        }
        else
        {
            float minusHalf = sampleIndex - halfBucket;
            float minusHalfInBuckets = minusHalf / SAMPLES_PER_BUCKET;
            int bucketA = Mathf.FloorToInt(minusHalfInBuckets);
            int bucketB = bucketA + 1;
            float bucketT = (minusHalfInBuckets - Mathf.Floor(minusHalfInBuckets));
            return Mathf.Lerp(rmsValues[bucketA], rmsValues[bucketB], bucketT);
        }
    }

    public void PlayLine()
    {
        if (!isSoundLine)
            return;
        GetComponent<AudioSource>().Play();
    }

    public float GetLineLength()
    {
        if (!isSoundLine)
            return 0.0f;
        return GetComponent<AudioSource>().clip.length;
    }
}
