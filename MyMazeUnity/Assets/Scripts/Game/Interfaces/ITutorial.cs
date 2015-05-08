
public interface ITutorial {

    TutorialStep GetStep(TutorialPhase phase);
    
    void CompleteStep(TutorialPhase phase);

    void StartStep(TutorialPhase phase);    
}
