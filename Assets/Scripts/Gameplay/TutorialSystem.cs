using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace MarketHustle.Gameplay
{
    /// <summary>
    /// Manages the tutorial system that weaves into the narrative.
    /// Instead of pop-ups, uses the player's cousin as a guide character.
    /// </summary>
    public class TutorialSystem : MonoBehaviour
    {
        public static TutorialSystem Instance { get; private set; }

        [System.Serializable]
        public class TutorialStep
        {
            public string stepId;
            public string cousinDialogue;
            public string instruction;
            public GameObject highlightObject; // Object to highlight
            public UnityEvent OnStepCompleted;
            public bool completed = false;
            public bool active = false;
        }

        [Header("Tutorial Steps")]
        public TutorialStep[] tutorialSteps;

        [Header("UI Elements")]
        public GameObject tutorialPanel;
        public TMP_Text cousinNameText;
        public TMP_Text dialogueText;
        public TMP_Text instructionText;
        public Image cousinPortrait;
        public Button continueButton;

        [Header("Highlighting")]
        public Image highlightOverlay;
        public Color highlightColor = new Color(1f, 1f, 0f, 0.3f);

        private int currentStepIndex = 0;
        private bool tutorialActive = true;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            if (tutorialSteps.Length > 0)
            {
                StartTutorial();
            }
        }

        public void StartTutorial()
        {
            tutorialActive = true;
            currentStepIndex = 0;
            ShowCurrentStep();
        }

        void ShowCurrentStep()
        {
            if (currentStepIndex >= tutorialSteps.Length)
            {
                EndTutorial();
                return;
            }

            TutorialStep currentStep = tutorialSteps[currentStepIndex];
            currentStep.active = true;

            // Update UI
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
                cousinNameText.text = "Cousin Marcus";
                dialogueText.text = currentStep.cousinDialogue;
                instructionText.text = currentStep.instruction;
            }

            // Highlight object
            if (currentStep.highlightObject != null && highlightOverlay != null)
            {
                HighlightObject(currentStep.highlightObject);
            }

            // Set up continue button
            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() => CompleteCurrentStep());
            }
        }

        void HighlightObject(GameObject obj)
        {
            // Position highlight overlay over the object
            RectTransform objRect = obj.GetComponent<RectTransform>();
            if (objRect != null && highlightOverlay != null)
            {
                RectTransform highlightRect = highlightOverlay.GetComponent<RectTransform>();
                highlightRect.position = objRect.position;
                highlightRect.sizeDelta = objRect.sizeDelta * 1.2f; // Slightly larger than object
                highlightOverlay.color = highlightColor;
                highlightOverlay.gameObject.SetActive(true);
            }
        }

        public void CompleteCurrentStep()
        {
            if (currentStepIndex < tutorialSteps.Length)
            {
                TutorialStep currentStep = tutorialSteps[currentStepIndex];
                currentStep.completed = true;
                currentStep.active = false;
                currentStep.OnStepCompleted?.Invoke();

                // Hide highlight
                if (highlightOverlay != null)
                {
                    highlightOverlay.gameObject.SetActive(false);
                }

                currentStepIndex++;
                ShowCurrentStep();
            }
        }

        void EndTutorial()
        {
            tutorialActive = false;
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
            Debug.Log("Tutorial completed!");
        }

        // Tutorial trigger methods (called by game systems)
        public void TriggerMovementTutorial()
        {
            if (!tutorialActive) return;
            // This would be called when the player first needs to move
            ShowTutorialStep("basic_movement");
        }

        public void TriggerStoreTutorial()
        {
            if (!tutorialActive) return;
            ShowTutorialStep("enter_store");
        }

        public void TriggerMoneyTutorial()
        {
            if (!tutorialActive) return;
            ShowTutorialStep("first_sale");
        }

        public void TriggerNeedsTutorial()
        {
            if (!tutorialActive) return;
            ShowTutorialStep("player_needs");
        }

        public void TriggerSkillsTutorial()
        {
            if (!tutorialActive) return;
            ShowTutorialStep("skill_system");
        }

        void ShowTutorialStep(string stepId)
        {
            for (int i = 0; i < tutorialSteps.Length; i++)
            {
                if (tutorialSteps[i].stepId == stepId && !tutorialSteps[i].completed)
                {
                    currentStepIndex = i;
                    ShowCurrentStep();
                    break;
                }
            }
        }

        // Skip tutorial (for experienced players)
        public void SkipTutorial()
        {
            foreach (TutorialStep step in tutorialSteps)
            {
                step.completed = true;
                step.OnStepCompleted?.Invoke();
            }
            EndTutorial();
        }

        // Check if tutorial is active
        public bool IsTutorialActive()
        {
            return tutorialActive;
        }

        // Get current tutorial step
        public TutorialStep GetCurrentStep()
        {
            if (currentStepIndex < tutorialSteps.Length)
            {
                return tutorialSteps[currentStepIndex];
            }
            return null;
        }
    }
}