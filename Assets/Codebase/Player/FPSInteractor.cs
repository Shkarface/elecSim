using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSInteractor : MonoBehaviour
{
    public Image Indicator;
    public TextMeshProUGUI HelpText;
    public Image HelpTextBackground;
    public AudioClip InteractionSound;
    public float SphereCastRadius = 0.54f;
    public LayerMask InteractionLayer;

    private AudioSource _AudioSource;
    private string _HelpText;
    private Interactable _LastInteractable;

    protected void Awake() => _AudioSource = transform.parent.GetComponent<AudioSource>();

    protected void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, 3f, InteractionLayer))
        {
            Indicator.CrossFadeAlpha(1f, 0.2f, true);
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (_HelpText == null || _LastInteractable != interactable || true)
                    _HelpText = interactable.GetHelpText();
                _LastInteractable = interactable;

                if (!string.IsNullOrEmpty(_HelpText))
                {
                    HelpText.CrossFadeAlpha(1f, 0.3f, true);
                    HelpTextBackground.CrossFadeAlpha(.9f, 0.2f, true);
                    HelpText.SetText(_HelpText);
                }
                else
                    HideHelpText();
                if (Input.GetMouseButtonDown(0))
                {
                    if (interactable.Interact())
                        _AudioSource.PlayOneShot(InteractionSound);

                    _HelpText = null;
                }
            }
            else
                HideHelpText();
        }
        else
        {
            HideHelpText();
            Indicator.CrossFadeAlpha(0.1f, 0.2f, true);
        }
    }

    private void HideHelpText()
    {
        _HelpText = null;
        HelpText.CrossFadeAlpha(0.001f, 0.1f, true);
        HelpTextBackground.CrossFadeAlpha(0.001f, 0.1f, true);
    }
}
