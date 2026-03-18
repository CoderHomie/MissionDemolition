using UnityEngine;

public class SlingshotRubberBand : MonoBehaviour
{
    [Header("Anchors (set in Inspector)")]
    public Transform leftAnchor;
    public Transform rightAnchor;

    [Header("Line Renderers (set in Inspector)")]
    public LineRenderer leftLine;
    public LineRenderer rightLine;

    [Header("Behavior")]
    public bool hideWhenIdle = true;

    private Transform _projectile;
    private bool _aiming;

    void Awake()
    {
        AutoFindIfMissing();
        SetupLine(leftLine);
        SetupLine(rightLine);
        SetVisible(false);
    }

    void LateUpdate()
    {
        if (!_aiming || _projectile == null) return;
        UpdateLines(_projectile.position);
    }

    public void BeginAiming(Transform projectile)
    {
        _projectile = projectile;
        _aiming = true;
        SetVisible(true);
        if (_projectile != null) UpdateLines(_projectile.position);
    }

    public void EndAiming()
    {
        _aiming = false;
        _projectile = null;

        if (hideWhenIdle) SetVisible(false);
        else ResetToAnchors();
    }

    private void UpdateLines(Vector3 targetPos)
    {
        if (leftLine != null && leftAnchor != null)
        {
            leftLine.SetPosition(0, leftAnchor.position);
            leftLine.SetPosition(1, targetPos);
        }

        if (rightLine != null && rightAnchor != null)
        {
            rightLine.SetPosition(0, rightAnchor.position);
            rightLine.SetPosition(1, targetPos);
        }
    }

    private void ResetToAnchors()
    {
        if (leftLine != null && leftAnchor != null)
        {
            leftLine.SetPosition(0, leftAnchor.position);
            leftLine.SetPosition(1, leftAnchor.position);
        }
        if (rightLine != null && rightAnchor != null)
        {
            rightLine.SetPosition(0, rightAnchor.position);
            rightLine.SetPosition(1, rightAnchor.position);
        }
    }

    private void SetVisible(bool visible)
    {
        if (leftLine != null) leftLine.enabled = visible;
        if (rightLine != null) rightLine.enabled = visible;
    }

    private void SetupLine(LineRenderer lr)
    {
        if (lr == null) return;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
    }

    private void AutoFindIfMissing()
    {
        if (leftAnchor == null)
        {
            var t = transform.Find("LeftAnchor");
            if (t != null) leftAnchor = t;
        }
        if (rightAnchor == null)
        {
            var t = transform.Find("RightAnchor");
            if (t != null) rightAnchor = t;
        }
    }
}

