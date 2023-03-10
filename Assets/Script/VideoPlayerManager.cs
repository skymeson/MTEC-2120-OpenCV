using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    //Raw Image to Show Video Images [Assign from the Editor]
    public RawImage image;
    //Set from the Editor
    public List<VideoClip> videoClipList;

    private List<VideoPlayer> videoPlayerList;
    private int videoIndex = 0;


    void Start()
    {
        StartCoroutine(playVideo());
    }

    IEnumerator playVideo(bool firstRun = true)
    {
        if (videoClipList == null || videoClipList.Count <= 0)
        {
            Debug.LogError("Assign VideoClips from the Editor");
            yield break;
        }

        //Init videoPlayerList first time this function is called
        if (firstRun)
        {
            videoPlayerList = new List<VideoPlayer>();
            for (int i = 0; i < videoClipList.Count; i++)
            {
                //Create new Object to hold the Video and the sound then make it a child of this object
                GameObject vidHolder = new GameObject("VP" + i);
                vidHolder.transform.SetParent(transform);

                //Add VideoPlayer to the GameObject
                VideoPlayer videoPlayer = vidHolder.AddComponent<VideoPlayer>();
                videoPlayerList.Add(videoPlayer);

                //Add AudioSource to  the GameObject
                AudioSource audioSource = vidHolder.AddComponent<AudioSource>();

                //Disable Play on Awake for both Video and Audio
                videoPlayer.playOnAwake = false;
                audioSource.playOnAwake = false;

                //We want to play from video clip not from url
                videoPlayer.source = VideoSource.VideoClip;

                //Set Audio Output to AudioSource
                videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

                //Assign the Audio from Video to AudioSource to be played
                videoPlayer.EnableAudioTrack(0, true);
                videoPlayer.SetTargetAudioSource(0, audioSource);

                //Set video Clip To Play 
                videoPlayer.clip = videoClipList[i];
            }
        }

        //Make sure that the NEXT VideoPlayer index is valid
        if (videoIndex >= videoPlayerList.Count)
            yield break;

        //Prepare video
        videoPlayerList[videoIndex].Prepare();

        //Wait until this video is prepared
        while (!videoPlayerList[videoIndex].isPrepared)
        {
            Debug.Log("Preparing Index: " + videoIndex);
            yield return null;
        }
        Debug.LogWarning("Done Preparing current Video Index: " + videoIndex);

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = videoPlayerList[videoIndex].texture;

        //Play first video
        videoPlayerList[videoIndex].Play();

        //Wait while the current video is playing
        bool reachedHalfWay = false;
        int nextIndex = (videoIndex + 1);
        while (videoPlayerList[videoIndex].isPlaying)
        {
            Debug.Log("Playing time: " + videoPlayerList[videoIndex].time + " INDEX: " + videoIndex);

            //(Check if we have reached half way)
            if (!reachedHalfWay && videoPlayerList[videoIndex].time >= (videoPlayerList[videoIndex].clip.length / 2))
            {
                reachedHalfWay = true; //Set to true so that we don't evaluate this again

                //Make sure that the NEXT VideoPlayer index is valid. Othereise Exit since this is the end
                if (nextIndex >= videoPlayerList.Count)
                {
                    Debug.LogWarning("End of All Videos: " + videoIndex);
                    yield break;
                }

                //Prepare the NEXT video
                Debug.LogWarning("Ready to Prepare NEXT Video Index: " + nextIndex);
                videoPlayerList[nextIndex].Prepare();
            }
            yield return null;
        }
        Debug.Log("Done Playing current Video Index: " + videoIndex);

        //Wait until NEXT video is prepared
        while (!videoPlayerList[nextIndex].isPrepared)
        {
            Debug.Log("Preparing NEXT Video Index: " + nextIndex);
            yield return null;
        }

        Debug.LogWarning("Done Preparing NEXT Video Index: " + videoIndex);

        //Increment Video index
        videoIndex++;

        //Play next prepared video. Pass false to it so that some codes are not executed at-all
        StartCoroutine(playVideo(false));
    }
}
