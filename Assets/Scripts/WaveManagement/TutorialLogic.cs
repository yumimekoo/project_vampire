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
            Debug.Log("Player has moved.");
        }
        if (!shot && Input.GetButton("Fire1"))  
        {
            shot = true;
            Debug.Log("Player has shot.");
        }
        if (!dashed && Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashed = true;
            Debug.Log("Player has dashed.");
        }
        if (moved && shot && dashed)
        {
            tutorialCompleted = true;
            Debug.Log("Tutorial completed! in dem register logic");
            GameState.isTutroial = false;
            StartCoroutine(overlay.PlayWaveAnimation());
        }
    }
}

