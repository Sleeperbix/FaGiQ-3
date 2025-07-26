using Godot;
using System;

public partial class ManagerGame : Node
{
    public static ManagerGame Instance;

    public static C_QuestionMultipleChoice SelectedQuestion;
    public SceneFader SceneFader { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        SceneFader = GetParent().GetNode<SceneFader>("SceneFader");
        if (SceneFader == null)
        {
            GD.PrintErr("SceneFader not assigned to ManagerGame!");
        }
    }
    public void SetSelectedQuestion(C_QuestionMultipleChoice question)
    {
        SelectedQuestion = question;
    }

    public async void TransitionToScene(string scenePath)
    {
        if (SceneFader != null)
        {
            await SceneFader.FadeToScene(scenePath);
        }
    }
}
