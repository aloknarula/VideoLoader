/*
Created by: Alok Narula | alok@VertexSoup.com | 07 April 2020

MIT License

Copyright (c) 2020 Alok Narula

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoLoader : MonoBehaviour
{
    // PUBLIC //
    
    // PROTECTED // 
    [SerializeField]
    protected VideoPlayer m_video;
    [SerializeField]
    protected string m_nextScene = "Game";
    [SerializeField]
    protected bool m_destroyWhenLoaded = true;
    [SerializeField]
    protected float m_fade = 1.0f;
    [SerializeField]
    protected GameObject [] m_hideTheseWhenActivated;

    
    // PRIVATE // 
    private enum State
    {
        Inactive,
        FadeIn,
        WaitingSceneForLoad,
        FadingOut
    }
    State m_state;
    AsyncOperation m_asyncOperation;

    /*
    bool m_loaded = false;
    bool m_startFadeIn = false;
    */
    
    // ACCESS //

    private void Start()
    {
        m_state = State.Inactive;
        DontDestroyOnLoad(gameObject);

        if(m_video == null)
        {
            m_video = GetComponent<VideoPlayer>();
            if(m_video == null)
            {
                Debug.LogError(name + " Video player not assigned to the Video Loader");
                gameObject.SetActive(false);
            }
        }
    }

    public void LoadSceneWithVideo()
    {
        if(m_video == null)
        {
            Debug.LogError(name + " Video Loader not set. Loading " + m_nextScene + " without video.", this);
            SceneManager.LoadScene(m_nextScene);
            return;
        }
        m_video.targetCameraAlpha = 0.0f;
        m_video.isLooping = true;
        m_video.Play();
        m_state = State.FadeIn;

        for(int i=0; i<m_hideTheseWhenActivated.Length; i++)
        {
            m_hideTheseWhenActivated[i].SetActive(false);
        }
    }

    private void Update()
    {
        switch(m_state)
        {
            default:
            case State.Inactive:
                break;

            case State.FadeIn:
                m_video.targetCameraAlpha = Mathf.MoveTowards(m_video.targetCameraAlpha, 1.0f, Time.deltaTime * m_fade);
                if (m_video.targetCameraAlpha >= 1.0f)
                {
                    m_asyncOperation = SceneManager.LoadSceneAsync(m_nextScene);
                    m_state = State.WaitingSceneForLoad;
                }
                break;

            case State.WaitingSceneForLoad:
                if(m_asyncOperation.isDone)
                {
                    m_state = State.FadingOut;
                }
                break;

            case State.FadingOut:
                m_video.targetCameraAlpha = Mathf.MoveTowards(m_video.targetCameraAlpha, 0.0f, Time.deltaTime * m_fade);
                if(m_video.targetCameraAlpha <= Mathf.Epsilon)
                {
                    if(m_destroyWhenLoaded)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
                break;

        }
        /*
        if(m_startFadeIn)
        {
            m_video.targetCameraAlpha = Mathf.MoveTowards(m_video.targetCameraAlpha, 1.0f, Time.deltaTime * m_fade);
            if (m_video.targetCameraAlpha >= 1.0f)
            {
                m_startFadeIn = false;
                m_asyncOperation = SceneManager.LoadSceneAsync(m_nextScene);
            }
        }
        else if (m_loaded || (m_video.targetCameraAlpha >= 1.0f && m_asyncOperation.isDone))
        {
            m_loaded = true;
            m_video.targetCameraAlpha = Mathf.MoveTowards(m_video.targetCameraAlpha, 0.0f, Time.deltaTime * m_fade);
            if(m_video.targetCameraAlpha <= Mathf.Epsilon)
            {
                // DONE //
                Destroy(m_video.gameObject);
                Destroy(gameObject);
            }
        }
        */
    }

}
