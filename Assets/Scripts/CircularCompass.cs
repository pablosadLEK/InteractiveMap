using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CircularCompass : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float rotationSpeed = 5f;
    public Button regionAnimationButton;
    public Button exitAnimationButton;
    public GameObject[] regions;

    private RectTransform compassHandle;
    private bool rotationStopped = false;
    private bool isAnimating = false; // Flag to track if an animation is currently playing

    void Start()
    {
        compassHandle = GetComponent<RectTransform>();

        if (regionAnimationButton != null)
        {
            regionAnimationButton.onClick.AddListener(OnRegionAnimationButtonClick);
        }

        if (exitAnimationButton != null)
        {
            exitAnimationButton.onClick.AddListener(OnExitAnimationButtonClick);
            exitAnimationButton.gameObject.SetActive(false); // Hide the exit animation button initially
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        RotateCompass(eventData);
        HighlightSelectedRegion();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rotationStopped = true;
    }

    private void RotateCompass(PointerEventData eventData)
    {
        Vector2 center = (compassHandle.parent as RectTransform).rect.center;
        Vector2 fromCenter = eventData.position - center;
        float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;

        compassHandle.localRotation = Quaternion.Euler(0f, 0f, angle * rotationSpeed);
    }

    private void HighlightSelectedRegion()
    {
        float rotationPercentage = (compassHandle.localRotation.eulerAngles.z + 360) % 360 / 360f;
        int regionIndex = Mathf.FloorToInt(rotationPercentage * regions.Length);

        for (int i = 0; i < regions.Length; i++)
        {
            if (i == regionIndex)
            {
                HighlightRegion(regions[i]);
            }
            else
            {
                UnhighlightRegion(regions[i]);
            }
        }
    }

    private void HighlightRegion(GameObject region)
    {
        Renderer regionRenderer = region.GetComponent<Renderer>();

        if (regionRenderer != null)
        {
            regionRenderer.material.color = Color.red;
            Debug.Log("Highlighted region: " + region.name);
        }
        else
        {
            Debug.LogWarning("RegionRenderer is null for " + region.name);
        }
    }

    private void UnhighlightRegion(GameObject region)
    {
        Renderer regionRenderer = region.GetComponent<Renderer>();
        if (regionRenderer != null)
        {
            regionRenderer.material.color = Color.white;
        }
    }

    public bool IsRotationStopped()
    {
        return rotationStopped;
    }

    public void OnRegionAnimationButtonClick()
    {
        if (!isAnimating)
        {
            float rotationPercentage = (compassHandle.localRotation.eulerAngles.z + 360) % 360 / 360f;
            int regionIndex = Mathf.FloorToInt(rotationPercentage * regions.Length);
            GameObject highlightedRegion = regions[regionIndex];

            Debug.Log("Highlighted region: " + highlightedRegion.name);

            Animator regionAnimator = highlightedRegion.GetComponent<Animator>();

            if (regionAnimator != null)
            {
                string triggerName = highlightedRegion.name + "Trigger";
                Debug.Log("Triggering animation: " + triggerName);
                regionAnimator.SetTrigger(triggerName);
                isAnimating = true; // Set flag to indicate animation is playing
            }
            else
            {
                Debug.LogWarning("No region animator found.");
            }

            // Hide the region animation button and show the exit animation button
            if (regionAnimationButton != null && exitAnimationButton != null)
            {
                regionAnimationButton.gameObject.SetActive(false);
                exitAnimationButton.gameObject.SetActive(true);
            }
        }
    }

    public void OnExitAnimationButtonClick()
    {
        if (isAnimating)
        {
            // Get the highlighted region
            float rotationPercentage = (compassHandle.localRotation.eulerAngles.z + 360) % 360 / 360f;
            int regionIndex = Mathf.FloorToInt(rotationPercentage * regions.Length);
            GameObject highlightedRegion = regions[regionIndex];
            Animator regionAnimator = highlightedRegion.GetComponent<Animator>();

            if (regionAnimator != null)
            {
                Debug.Log("Triggering animation: Exit Animation");
                regionAnimator.SetTrigger("ExitTrigger");
                isAnimating = false; // Reset animation flag
            }
        }

        // Hide the exit animation button and show the region animation button
        if (regionAnimationButton != null && exitAnimationButton != null)
        {
            regionAnimationButton.gameObject.SetActive(true);
            exitAnimationButton.gameObject.SetActive(false);
        }
    }
}
