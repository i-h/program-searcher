using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class LoadingIcon : MonoBehaviour
{
    public Image LoadingImage;
    private float _startAnimationSpeed = 0.4f;
    private float _progressAnimationSpeed = 0.2f;
    private float _disposeAnimationSpeed = 0.6f;
    private bool _animating = false;
    private bool _disposing = false;
    private RectTransform _rt;
    private float _baseProgress = 0.33f;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (LoadingImage == null)
        {
            Debug.LogWarning("Loading icon image is not set!");
            gameObject.SetActive(false);
        }
        LoadingImage.fillAmount = 0.0f;
        StartCoroutine(AnimateTo(_baseProgress, _startAnimationSpeed, true));
    }

    IEnumerator AnimateTo(float amount, float speed, bool waitForEnd, string callbackMethod = "")
    {
        if (waitForEnd) _animating = true;
        float t = 0;
        float startAmount = LoadingImage.fillAmount;

        while(LoadingImage.fillAmount != amount)
        {
            LoadingImage.fillAmount = Mathf.SmoothStep(startAmount, amount, t);
            t += Time.deltaTime/speed;
            yield return new WaitForEndOfFrame();
        }
        if (waitForEnd) _animating = false;
        if (callbackMethod != "") Invoke(callbackMethod, 0.0f);
    }

    /// <summary>
    /// Destroy this Loading bar
    /// </summary>
    public void Dispose()
    {
        LoadingImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        StartCoroutine(AnimateTo(0.0f, _disposeAnimationSpeed, true));
        _disposing = true;
    }
    void DestroyObject()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_animating)
        {
            // Not animating && disposing means that we have finished the end animation
            if (_disposing) DestroyObject();

            float progress = ProgramFetcher.Instance.GetProgress();
            if (progress == 1.0f)
            {
                StartCoroutine(AnimateTo(progress, _progressAnimationSpeed, true, "Dispose"));
            }
            else
            {
                StartCoroutine(AnimateTo(GetDisplayProgress(progress), _progressAnimationSpeed, true));
            }
        }
    }

    float GetDisplayProgress(float trueProgress)
    {
        return Mathf.Clamp(_baseProgress + trueProgress / (1.0f - _baseProgress), 0.0f, 1.0f);
    }
}
