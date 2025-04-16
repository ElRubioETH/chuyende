using UnityEngine;

public class TogglePanelSimple : MonoBehaviour
{
    private GameObject panel;
    private bool isPanelActive = false;

    void Start()
    {
        panel = GameObject.Find("MyPanel"); // Đảm bảo Panel có tên là "MyPanel"
    }

    public void Toggle()
    {
        isPanelActive = !isPanelActive;
        panel.SetActive(isPanelActive);
    }
}
