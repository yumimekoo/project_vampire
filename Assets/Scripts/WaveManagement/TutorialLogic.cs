using UnityEngine;

public class TutorialLogic : MonoBehaviour
{
    private bool moved;
    private bool shot;
    private bool dashed;
    private bool tutorialCompleted;

    [SerializeField] private OverlayUI overlay;

    private void Update()
    {
        if (tutorialCompleted)
            return;

        if (!moved && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            moved = true;
        }
        if (!shot && Input.GetButton("Fire1"))  
        {
            shot = true;
        }
        if (!dashed && Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashed = true;
        }
        if (moved && shot && dashed)
        {
            tutorialCompleted = true;
            GameState.isTutroial = false;
            StartCoroutine(overlay.PlayWaveAnimation());
        }
    }
}

