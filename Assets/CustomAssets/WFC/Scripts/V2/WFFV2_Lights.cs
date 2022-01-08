using UnityEngine;

[ExecuteInEditMode]
public class WFFV2_Lights : MonoBehaviour
{
    public bool toggleLights;
    // Update is called once per frame
    void Update()
    {
        if (toggleLights)
        {
            toggleLights = false;
            foreach (Light l in this.GetComponentsInChildren<Light>())
            {
                if (l.enabled)
                {
                    l.enabled = false;
                }
                else if (!l.enabled)
                {
                    l.enabled = true;
                }
            }
        }
    }
}
