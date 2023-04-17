using Assets.Scripts.Classes.Events;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float CurrentProgress => progress;
    public bool ShouldHideOnCompletion { get; set; } = true;

    [SerializeField]
    private float rightOffset = 0.04f;
    [SerializeField]
    private float leftOffset = 0.03f;
    [SerializeField]
    private RectTransform fillBar;
    [SerializeField]
    private float progress = 0f;

    void Awake()
    {
        RefreshFillBarPosition();
    }

    private void OnEnable()
    {
        RefreshFillBarPosition();
    }

    public void SetProgress(float value)
    {
        progress = value;
        RefreshFillBarPosition();
    }

    private void RefreshFillBarPosition()
    {
        float newX = rightOffset + progress - leftOffset;
        fillBar.anchorMax = new Vector2(newX, fillBar.anchorMax.y);
        if (progress >= 1.0f && ShouldHideOnCompletion)
            this.gameObject.SetActive(false);
    }

    public void IncrementProgress(float value)
    {
        progress += value;
        RefreshFillBarPosition();
    }

    public void RespondToUpdatedProgress(object sender, ProgressEventArgs args)
    {
        if (args != null)
            SetProgress(args.Progress);
    }
}
