using UnityEngine;

[RequireComponent(typeof(ReflectionProbe))]
public class ProbeArea : MonoBehaviour
{
    protected ReflectionProbe ReflectionProbe;
    public ProbeArea[] ConnectedAreas;

    private float _LastUpdateTime;

    protected void Awake() => ReflectionProbe = GetComponent<ReflectionProbe>();
    public void RenderProbe()
    {
        if (_LastUpdateTime + 0.01f > Time.time)
            return;
        _LastUpdateTime = Time.time;

#if UNITY_EDITOR
        if (ReflectionProbe == null)
            ReflectionProbe = GetComponent<ReflectionProbe>();
#endif

        ReflectionProbe.RenderProbe();
        foreach (var area in ConnectedAreas)
            area.RenderProbe();
    }
    
}
