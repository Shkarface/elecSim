using UnityEngine;

public class LoadData : ScriptableObject
{
    [Header("Impedance (R + jX)")]
    [Tooltip("The resistance of the load, in Ohms")]
    public float Real = 100;
    [Tooltip("The inductance (or capacitance if negative) of the load, in Ohms")]
    public float Imaginary;
    [Tooltip("the required power for the load to work, in Watts")]
    public float RequiredPower = 100;
}
