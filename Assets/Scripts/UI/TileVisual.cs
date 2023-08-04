using UnityEngine;

public class TileVisual : MonoBehaviour
{
    [SerializeField] private Transform highlight;
    [SerializeField] private Transform fog;

    public void TurnHighlightOff()
    {
        highlight.gameObject.SetActive(false);
    }

    public void TurnHighlightOn()
    {
        highlight.gameObject.SetActive(true);
    }

    public void FogOff()
    {
        fog.gameObject.SetActive(false);
    }

    public void FogOn()
    {
        fog.gameObject.SetActive(true);
    }
}
