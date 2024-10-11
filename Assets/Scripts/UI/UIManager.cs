using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonobehaviour<UIManager>
{
    private bool _pauseMenuOn = false;
    public bool PauseMenuOn { get => _pauseMenuOn; set => _pauseMenuOn = value; }

    [SerializeField] private UIInventoryBar uIInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();
    }

    private void PauseMenu()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (!PauseMenuOn) EnablePauseMenu();
        else DisablePauseMenu();
    }

    private void EnablePauseMenu()
    {
        // Destroy any currently dragged items
        uIInventoryBar.DestroyCurrentlyDraggedItems();

        // Clear currently selected items
        uIInventoryBar.ClearCurrentlySelectedItems();

        PauseMenuOn = true;
        PlayerController.Instance.PlayerMovement.PlayerInputIsDisabled = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        System.GC.Collect();

        HighlightButtonForSelectedTab();
    }

    public void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItems();

        PauseMenuOn = false;
        PlayerController.Instance.PlayerMovement.PlayerInputIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    private void HighlightButtonForSelectedTab()
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabSelected)
    {
        for (int i = 0; i < menuTabs.Length; i++)
        {
            if (i != tabSelected) menuTabs[i].SetActive(false);
            else menuTabs[i].SetActive(true);
        }
        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!!!");
        Application.Quit();
    }
}
