using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable")]
    public string Tooltip;
    public abstract bool Interact();
    public abstract string GetHelpText();
}
