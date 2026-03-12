using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlsPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform rebindingContainer;

    private ControlsRebindingUI rebindingUI;

    void OnEnable()
    {
        if (rebindingUI == null)
            InitializeControlsUI();
    }

    private void InitializeControlsUI()
    {
        if (rebindingContainer == null)
        {
            GameObject containerGO = new GameObject("RebindingContainer");
            containerGO.transform.SetParent(transform, false);
            rebindingContainer = containerGO.transform;

            VerticalLayoutGroup vlg = containerGO.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 10;
            vlg.padding = new RectOffset(20, 20, 20, 20);
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            RectTransform rt = containerGO.GetComponent<RectTransform>();
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        rebindingUI = GetComponent<ControlsRebindingUI>();
        if (rebindingUI == null)
            rebindingUI = gameObject.AddComponent<ControlsRebindingUI>();
    }

    public void ResetBindings()
    {
        if (KeyBindingManager.Instance != null)
        {
            KeyBindingManager.Instance.ResetAllBindings();

            foreach (Transform child in rebindingContainer)
                Destroy(child.gameObject);

            rebindingUI = null;
            InitializeControlsUI();
        }
    }

    public void GoBack()
    {
        gameObject.SetActive(false);
    }
}
