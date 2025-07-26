using Godot;
using System;
using System.Collections.Generic;

public partial class ManagerMultipleChoice : Node
{
	[Export] public string MasterListFilePath = "res://Assets/Questions.txt";
	private List<C_QuestionMultipleChoice> allQuestionsMaster = new List<C_QuestionMultipleChoice>();
	private List<C_QuestionMultipleChoice> allQuestionsPool;

	[Export] public Node QuestionButtonBarNode;
	List<Button> buttons = new List<Button>();

	private C_QuestionMultipleChoice selectedQuestion;


	public override void _Ready()
	{
		InitializeButtons();
		ReadMasterFile();
		CreateQuestionPool();
		SetQuestionsToButtons();
	}
	private void InitializeButtons()
	{
		if (QuestionButtonBarNode == null)
		{
			GD.PrintErr("QuestionButtonBarNode not assigned. Assign it from the inspector!");
			return;
		}
		HBoxContainer container = QuestionButtonBarNode.GetNode<HBoxContainer>("HBoxContainer");
		if (container == null)
		{
			GD.PrintErr("HBoxContainer not found under QuestionButtonBarNode.");
			return;
		}
		foreach (var child in container.GetChildren())
		{
			if (child is Button btn) buttons.Add(btn);
		}

	}

	private void ReadMasterFile()
	{
		// Check master file exists. It must exist for the questions to be loaded.
		if (!FileAccess.FileExists(MasterListFilePath))
		{
			GD.PrintErr($"File not found: {MasterListFilePath}");
			return;
		}

		// Master file found, read each line in the file. 
		var file = FileAccess.Open(MasterListFilePath, FileAccess.ModeFlags.Read);
		while (!file.EofReached())
		{
			string line = file.GetLine().StripEdges();
			if (!string.IsNullOrEmpty(line))
			{
				string jsonPath = $"res://Assets/{line}";
				LoadQuestionsFromJson(jsonPath);
			}
		}
	}
	private void LoadQuestionsFromJson(string jsonPath)
	{
		if (!FileAccess.FileExists(jsonPath))
		{
			GD.PrintErr($"JSON file not found: {jsonPath}");
			return;
		}

		var jsonFile = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
		string jsonText = jsonFile.GetAsText();

		Json json = new Json();
		Error err = json.Parse(jsonText);

		if (err != Error.Ok)
		{
			GD.PrintErr($"Failed to parse JSON at {jsonPath}: {json.GetErrorMessage()}");
			return;
		}

		var parsed = json.Data.As<Godot.Collections.Dictionary>();

		if (parsed == null || !parsed.ContainsKey("Global") || !parsed.ContainsKey("Questions"))
		{
			GD.PrintErr($"Invalid structure in {jsonPath}");
			return;
		}

		var global = parsed["Global"].As<Godot.Collections.Dictionary>();
		var questions = parsed["Questions"].As<Godot.Collections.Array>();

		AddQuestionsToMasterList(global, questions);
	}
	private void AddQuestionsToMasterList(Godot.Collections.Dictionary global, Godot.Collections.Array questions)
	{
		foreach (Godot.Collections.Dictionary questionDict in questions)
		{
			var q = new C_QuestionMultipleChoice
			{
				QuestionText = questionDict["QuestionText"].ToString(),
				AnswerA = questionDict["AnswerA"].ToString(),
				AnswerB = questionDict["AnswerB"].ToString(),
				AnswerC = questionDict["AnswerC"].ToString(),
				AnswerD = questionDict["AnswerD"].ToString(),
				CorrectAnswer = questionDict["CorrectAnswer"].ToString(),
				AnswerFact = questionDict["AnswerFact"].ToString(),
				QuestionImage = questionDict["QuestionImage"].ToString(),
				CategoryTitle = global["CategoryTitle"].ToString(),
				BGImage = global["BGImage"].ToString(),
				ButtonBGColour = new Color(global["ButtonBGColour"].ToString()),
				ButtonTextColour = new Color(global["ButtonTextColour"].ToString())
			};

			GD.Print($"Loaded question: {q.QuestionText}");
			allQuestionsMaster.Add(q);
		}
	}

	private void CreateQuestionPool()
	{
		// Shuffle allQuestionsMaster into a new random order called allQuestionsPool. This list will be where the questions are picked from for the buttons.
		var rng = new Random();
		allQuestionsPool = new List<C_QuestionMultipleChoice>(allQuestionsMaster);
		allQuestionsPool.Sort((a, b) => rng.Next(-1, 2)); // Random shuffle ?		
	}
	private void SetQuestionsToButtons()
	{
		if (allQuestionsMaster.Count < buttons.Count)
		{
			GD.PrintErr("Not enough questions to fill all buttons.");
			// TODO: Refill allQuestionsPool from allQuestionsMaster and reshuffle.
			// e.g. RefillQuestions();
			return;
		}
		for (int i = 0; i < buttons.Count; i++)
		{
			var question = allQuestionsPool[i];
			var button = buttons[i];
			GD.Print($"Current Button: {buttons[i]}");

			button.Text = question.CategoryTitle;
			button.AddThemeColorOverride("font_color", question.ButtonTextColour);
			var stylebox = new StyleBoxFlat();
			stylebox.BgColor = question.ButtonBGColour;
			button.AddThemeStyleboxOverride("normal", stylebox);

			C_QuestionMultipleChoice capturedQuestion = question;
			button.Pressed += () => OnQuestionButtonPressed(capturedQuestion);
		}
	}
	private void OnQuestionButtonPressed(C_QuestionMultipleChoice question)
	{
		GD.Print($"Button pressed for question: {question.QuestionText}");

		// Lock all buttons to prevent multiple clicks
		foreach (var btn in buttons)
			btn.Disabled = true;

		// Store the selected question
		selectedQuestion = question;

		// Remove from pool so it’s not reused
		allQuestionsPool.Remove(question);

		ManagerGame.Instance.SetSelectedQuestion(question);
		ManagerGame.Instance.TransitionToScene("res://Scenes/QuestionScene-MC.tscn");

		// For now just log
		GD.Print("Selected question stored. Ready to transition.");
	}

}
