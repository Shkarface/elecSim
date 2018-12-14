using UnityEngine;
using System.Threading.Tasks;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ElectricLight : ElectricLoad
{
    public LightData LightData;

    [Header("Light Properties")]
    public Light[] Lights;
    public Renderer Renderer;
    public Material OnMaterial;
    public Material OffMaterial;
    public ProbeArea ProbeArea;

    protected override async void VoltageChanged()
    {
        Current = CalculateCurrent(new System.Numerics.Complex(LightData.Real, LightData.Imaginary), LightData.RequiredPower);
        if (Math.Abs(Voltage) > 0)
            TurnOn();
        else
        {
            await Task.Delay(100);
            if (Math.Abs(Voltage) > 0)
                return;
            TurnOff();
        }
    }

    protected async void UpdateProbes()
    {
        if (ProbeArea == null)
            return;
        for (int i = 1; i <= 3; i++)
        {
            await Task.Delay(i * 100);
            ProbeArea.RenderProbe();
        }
    }

    public void TurnOn()
    {
        Renderer.material = OnMaterial;
        for (int i = 0; i < Lights.Length; i++)
        {
            Lights[i].enabled = true;
        }

        Renderer.UpdateGIMaterials();
        UpdateProbes();
    }
    public void TurnOff()
    {
        Renderer.material = OffMaterial;
        foreach (Light l in Lights)
            l.enabled = false;

        Renderer.UpdateGIMaterials();
        UpdateProbes();
    }
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ElectricLight))]
public class ElectricLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (targets.Length == 1)
        {
            ElectricLight obj = target as ElectricLight;
            EditorGUILayout.HelpBox($"Voltage:\t{obj.Voltage} V{Environment.NewLine}Current:\t{obj.Current.Magnitude} A{Environment.NewLine}Phase:\t{obj.Current.Phase * (Math.PI / 180.0)}", MessageType.Info);
        }
        if (GUILayout.Button("Turn On"))
            foreach (var obj in targets)
                (obj as ElectricLight).TurnOn();

        else if (GUILayout.Button("Turn Off"))
            foreach (var obj in targets)
                (obj as ElectricLight).TurnOff();
    }
}
#endif
