using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionDemolition : MonoBehaviour
{
    [Header("Inscribed")]
    [Tooltip("How many projectiles the player is allowed to fire before Game Over (if Goal.goalMet is still false).")]
    public int shotsAllowed = 3;
    [Tooltip("Delay after the last shot to allow physics to settle before checking win/lose.")]
    public float settleDelay = 0.75f;
    [Tooltip("If true, win will advance to the next scene in Build Settings.")]
    public bool advanceSceneOnWin = true;

    [Header("Scene References")]
    public UIManager uiManager;
    public Slingshot slingshot;

    [Header("Dynamic (Read Only)")]
    public int shotsTaken = 0;
    public bool gameOver = false;
    public bool won = false;

    private float _checkAtTime = -1f;

    void Awake()
    {
        Goal.goalMet = false;
        shotsTaken = 0;
        gameOver = false;
        won = false;
        _checkAtTime = -1f;

        if (uiManager == null) uiManager = FindFirstObjectByType<UIManager>();
        if (slingshot == null) slingshot = FindFirstObjectByType<Slingshot>();
    }

    void Update()
    {
        if (gameOver || won) return;

        if (Goal.goalMet)
        {
            HandleWin();
            return;
        }

        if (_checkAtTime > 0f && Time.unscaledTime >= _checkAtTime)
        {
            _checkAtTime = -1f;
            EvaluateLose();
        }
    }

    public void ShotFired()
    {
        if (gameOver || won) return;

        shotsTaken++;

        // After the last allowed shot, schedule a check. We use unscaled time so it still triggers
        // even if you later pause time with the UI.
        if (shotsTaken >= shotsAllowed)
        {
            _checkAtTime = Time.unscaledTime + Mathf.Max(0.01f, settleDelay);
        }
    }

    private void EvaluateLose()
    {
        if (won || Goal.goalMet) return;

        if (shotsTaken >= shotsAllowed)
        {
            gameOver = true;
            if (slingshot != null) slingshot.enabled = false;
            if (uiManager != null) uiManager.ShowGameOver();
        }
    }

    private void HandleWin()
    {
        won = true;
        if (slingshot != null) slingshot.enabled = false;

        if (!advanceSceneOnWin)
        {
            // If you want a win UI later, add it here.
            return;
        }

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (sceneCount <= 1)
        {
            // If scenes aren't set up yet, just reload.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        int next = (current + 1) % sceneCount;
        SceneManager.LoadScene(next);
    }
}

