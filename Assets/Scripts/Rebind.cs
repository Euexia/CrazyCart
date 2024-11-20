using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputRebinding : MonoBehaviour
{
    public TMP_Text bindingDisplayNameText; 
    public TMP_Text instructionText;       

    private InputAction currentAction;
    private int bindingIndex;

    public void StartRebinding(InputAction action, int index)
    {
        currentAction = action;
        bindingIndex = index;

        if (instructionText != null)
        {
            instructionText.text = "Appuyez sur une nouvelle touche...";
        }

        action.PerformInteractiveRebinding(index)
            .OnComplete(operation =>
            {
                UpdateBindingDisplay();

                if (instructionText != null)
                {
                    instructionText.text = "";
                }

                operation.Dispose();
            })
            .Start();
    }

    public void UpdateBindingDisplay()
    {
        if (currentAction != null)
        {
            bindingDisplayNameText.text = currentAction.GetBindingDisplayString(bindingIndex);
        }
    }
}
