using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WellCome_Manager : MonoBehaviour
{
    public GameObject loadingUI;         // Panel chứa UI loading
    public Slider progressBar;           // Thanh slider
    public TMP_Text progressText;        // Text phần trăm
    public string sceneToLoad = "Selection";

    void Start()
    {
        Invoke(nameof(BeginLoadScene), 9f); // Chờ 9 giây rồi mới bắt đầu loading
    }

    void BeginLoadScene()
    {
        loadingUI.SetActive(true); // Hiện loading UI
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (fakeProgress < 1f || operation.progress < 0.9f)
        {
            fakeProgress += Time.deltaTime * 0.3f; // tốc độ tăng loading bar
            float progress = Mathf.Min(fakeProgress, operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        operation.allowSceneActivation = true;
    }
}
