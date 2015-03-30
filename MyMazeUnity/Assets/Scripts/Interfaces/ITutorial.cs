
public interface ITutorial {

    TutorialStep GetStep(TutorialPhase phase);

    TutorialStep GetCurrentStep();

    void CompleteStep(TutorialPhase phase);

    void StartStep(TutorialPhase phase);
    
    void StartNextStep();

    void NextStep();
}
