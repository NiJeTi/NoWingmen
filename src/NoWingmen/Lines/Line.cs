using UnityEngine;
using UnityEngine.UI;

namespace NoWingmen.Lines;

internal sealed class Line
{
    private readonly GameObject _instance;
    private readonly Transform _transform;
    private readonly Image _image;

    public Line(GameObject instance)
    {
        _instance = instance;
        _transform = instance.transform;
        _image = instance.GetComponentInChildren<Image>(true);

        _image.raycastTarget = false;
    }

    public void Draw(Vector3 source, Vector3 target, Color color, float thickness)
    {
        if (_instance == null)
        {
            return;
        }

        _instance.SetActive(true);

        var delta = target - source;

        _transform.localPosition = source;
        _transform.localEulerAngles = new Vector3(0f, 0f, -Mathf.Atan2(delta.x, delta.y) * Mathf.Rad2Deg);
        _transform.localScale = new Vector3(thickness, delta.magnitude, thickness);

        _image.color = color;
    }

    public void Hide()
    {
        if (_instance == null)
        {
            return;
        }

        _instance.SetActive(false);
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(_instance);
    }
}