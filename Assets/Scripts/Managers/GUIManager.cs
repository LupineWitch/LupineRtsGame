using Assets.Scripts.Commandables;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Classes.Events;

public class GUIManager : MonoBehaviour
{
    [SerializeField]
    private Image selectedEntityPortrait;
    [SerializeField]
    private BasicCommandControler commandControler;
    [SerializeField]
    private GameObject actionButtonsContainer;
    [SerializeField]
    private Sprite defaultButtonIcon;
    [SerializeField]
    private Sprite emptyButtonIcon;

    private Button[] menuButtons;

    private void Awake()
    {
        commandControler.SelectionChanged += SelectionWasUpdated;
        commandControler.CommandContextChanged += CommandContextHasChanged;
        selectedEntityPortrait.enabled = false;
    }

    private void Start()
    {
        menuButtons = actionButtonsContainer.GetComponentsInChildren<Button>();
    }

    private void OnDisable()
    {
        selectedEntityPortrait.enabled = false;
    }

    private void SelectionWasUpdated(object sender, SelectionChangedEventArgs e)
    {
        if(e.SelectedEntities.Count <= 0)
            selectedEntityPortrait.enabled = false;
        else
        {
            var entity = e.SelectedEntities.First();
            selectedEntityPortrait.sprite = entity.Preview;
            selectedEntityPortrait.enabled = true;
        }

    }

    private void CommandContextHasChanged(object sender, CommandContextChangedArgs args)
    {
        IReadOnlyCollection<CommandDirective> menuCommands = args.MenuCommands;
        if(menuCommands == default)
            return;

        int actionsCount = menuCommands.Count();
        int index;
        for(index = 0; index < menuButtons.Length; index++)
        {
            if (index >= actionsCount)
                break;

            CommandDirective directiveAtIndex = menuCommands.ElementAt(index);
            Transform child = menuButtons[index].transform.GetChild(0);
            var buttonIcon = child.gameObject.GetComponent<Image>();
            
            if(directiveAtIndex == null)
            {
                menuButtons[index].interactable = false;
                buttonIcon.sprite = emptyButtonIcon;
            }
            else
            {
                menuButtons[index].interactable = true;
                buttonIcon.sprite = directiveAtIndex.ButtonIcon ?? defaultButtonIcon;
            }
        }

        if (index > menuCommands.Count)
        {
            Debug.LogWarningFormat("Passed {0} menu actions, where the menu has the capacity of {1}", index, menuButtons.Length);
            return;
        }

        for(;index < menuButtons.Length; index++)
        {
            menuButtons[index].interactable = false;
            Transform child = menuButtons[index].transform.GetChild(0);
            var buttonIcon = child.gameObject.GetComponent<Image>();
            buttonIcon.sprite = emptyButtonIcon;
        }
    }
}
