using System;
using UnityEngine;

[CreateAssetMenu]
public class SwitchData : ScriptableObject
{
    [Tooltip("Price of equipment in $USD")]
    public double Price = 5;
    [Tooltip("The maximum current that can flow through the circuit breaker")]
    public double MaxCurrent = 25;
    public bool OverloadProtection;
}
