using Godot;
using System;
using System.Collections.Generic;

public partial class ManagerMultipleChoice : Node
{
	[Export] public string MasterListFilePath = "res://Assets/Questions.txt";
	private List<C_QuestionMultipleChoice> allQuestions = new List<C_QuestionMultipleChoice>();
	private List<C_QuestionMultipleChoice> allQuestionsPool;

	[Export] public Node QuestionButtonBarNode;
	List<Button> buttons = new List<Button>();


	public override void _Ready()
	{
		HBoxContainer container = QuestionButtonBarNode.GetNode<HBoxContainer>("HBoxContainer");
		foreach (var child in container.GetChildren())
		{
			if (child is Button btn) buttons.Add(btn);
			GD.Print("Found Child!");
		}
		ReadMasterFile();
		if (allQuestions.Count < buttons.Count)
		{
			GD.PrintErr("Not enough questions to fill all buttons.");
			// TODO Reshuffle all questions back into allQuestionsPool.
			return;
		}
		CreateQuestionPool();
		SetQuestionsToButtons();		
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
				GD.Print($"Found line in file: {line}");
				string jsonPath = $"res://Assets/{line}";

				// Using the path generated from the current line from the file, check if that file exists.
				if (!FileAccess.FileExists(jsonPath))
				{
					GD.PrintErr($"JSON file not found: {jsonPath}");
					continue;
				}

				// Open the file found at the filepath. Save the file as jsonFile. Check the file is okay, then parse it as json.
				var jsonFile = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
				string jsonText = jsonFile.GetAsText();

				Json json = new Json();
				Error err = json.Parse(jsonText);

				if (err != Error.Ok)
				{
					GD.PrintErr($"Failed to pase JSON at {jsonPath}: {json.GetErrorMessage()}");
					continue;
				}

				var parsed = json.Data.As<Godot.Collections.Dictionary>();

				if (parsed == null || !parsed.ContainsKey("Global") || !parsed.ContainsKey("Questions"))
				{
					GD.PrintErr($"Invalid structure in {jsonPath}");
					continue;
				}

				var global = parsed["Global"].As<Godot.Collections.Dictionary>();
				var questions = parsed["Questions"].As<Godot.Collections.Array>();

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
						ButtonBGColour = new Color(global["ButtonBGColour"].ToString()),
						ButtonTextColour = new Color(global["ButtonTextColour"].ToString())
					};
					GD.Print($"Loaded  {q}");
					allQuestions.Add(q);
				}
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

		AddQuestionsToPool(global, questions);
	}
	private void CreateQuestionPool()
	{
		// Shuffle allQuestions into a new random order called allQuestionsPool. This list will be where the questions are picked from for the buttons.
		var rng = new Random();
		allQuestionsPool = new List<C_QuestionMultipleChoice>(allQuestions);
		allQuestionsPool.Sort((a, b) => rng.Next(-1, 2)); // Random shuffle ?		
	}
	private void SetQuestionsToButtons()
	{
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
		}
	}
}
