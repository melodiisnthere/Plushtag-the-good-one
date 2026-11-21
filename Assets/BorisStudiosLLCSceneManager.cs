using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace BorisStudiosLLC
{
    public class BorisStudiosLLCSceneManager : MonoBehaviour
    {
        [Header("BorisStudiosLLC Scene Manager")]

        [SerializeField]
        private string GameSceneName = "YourSceneName";

        [Header("TextMeshPro")]

        [SerializeField]
        private TMP_Text LoadingText;

        [SerializeField]
        private TMP_Text ProgressText;

        [SerializeField]
        private TMP_Text StatusText;

        [Header("Loading Settings")]

        [SerializeField]
        private float MinimumLoadingTime = 2.5f;

        [SerializeField]
        private float StatusChangeTime = 1.25f;

        private AsyncOperation SceneLoadOperation;

        private readonly string[] LoadingStages =
        {
            "Loading BorisStudiosLLC",
            "Loading PlayFab",
            "Loading Player Data",
            "Loading Textures",
            "Loading Materials",
            "Loading Audio",
            "Loading Game Systems",
            "Loading Plush Tag"
        };

        private void Start()
        {
            StartCoroutine(LoadGame());
        }

        private IEnumerator LoadGame()
        {
            float StartTime = Time.realtimeSinceStartup;

            SetStatus("Loading BorisStudiosLLC");

            SceneLoadOperation =
                SceneManager.LoadSceneAsync(GameSceneName);

            if (SceneLoadOperation == null)
            {
                SetStatus("Failed To Load Game");
                yield break;
            }

            SceneLoadOperation.allowSceneActivation = false;

            int CurrentStage = 0;
            float StageTimer = 0f;

            while (!SceneLoadOperation.isDone)
            {
                StageTimer += Time.unscaledDeltaTime;

                if (StageTimer >= StatusChangeTime)
                {
                    StageTimer = 0f;

                    CurrentStage++;

                    if (CurrentStage >= LoadingStages.Length)
                        CurrentStage = LoadingStages.Length - 1;

                    SetStatus(LoadingStages[CurrentStage]);
                }

                UpdateLoadingDots();
                UpdateProgress();

                float Progress = Mathf.Clamp01(
                    SceneLoadOperation.progress / 0.9f
                );

                if (Progress >= 1f)
                {
                    float Elapsed =
                        Time.realtimeSinceStartup - StartTime;

                    if (Elapsed >= MinimumLoadingTime)
                    {
                        SetStatus("Starting Primal Paranoia");

                        yield return new WaitForSecondsRealtime(0.25f);

                        SceneLoadOperation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }

        private void UpdateLoadingDots()
        {
            if (LoadingText == null)
                return;

            int Dots =
                Mathf.FloorToInt(Time.unscaledTime * 2f) % 3 + 1;

            LoadingText.text =
                "Loading" + new string('.', Dots);
        }

        private void UpdateProgress()
        {
            if (ProgressText == null)
                return;

            if (SceneLoadOperation == null)
            {
                ProgressText.text = "0%";
                return;
            }

            float Progress = Mathf.Clamp01(
                SceneLoadOperation.progress / 0.9f
            );

            int Percentage =
                Mathf.RoundToInt(Progress * 100f);

            ProgressText.text = Percentage + "%";
        }

        private void SetStatus(string Status)
        {
            if (StatusText == null)
                return;

            StatusText.text = (Status);
        }
    }
}