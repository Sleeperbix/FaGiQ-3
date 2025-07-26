using Godot;
using System;

public partial class ManagerQuestionRoom : Control
{
	[Export] private Control QuestionGUI;
	[Export] private Control TitleBarGUI;

	// Text labels, question text, answers etc.
	private RichTextLabel guiCategoryText;
	private RichTextLabel guiQuestionText;
	private RichTextLabel guiAnswerA;
	private RichTextLabel guiAnswerB;
	private RichTextLabel guiAnswerC;
	private RichTextLabel guiAnswerD;
	private TextureRect guiQuestionBG;


	public override void _Ready()
	{
		// Retrieve the selected question from ManagerGame
		var question = ManagerGame.SelectedQuestion;

		// Locate fields
		guiCategoryText = TitleBarGUI.GetNode<RichTextLabel>("TopBarBG/TopBarText");
		guiQuestionText = QuestionGUI.GetNode<RichTextLabel>("ColorRect/QuestionContainer/QuestionText");
		guiAnswerA = QuestionGUI.GetNode<RichTextLabel>("ColorRect/QuestionContainer/AnswerA");
		guiAnswerB = QuestionGUI.GetNode<RichTextLabel>("ColorRect/QuestionContainer/AnswerB");
		guiAnswerC = QuestionGUI.GetNode<RichTextLabel>("ColorRect/QuestionContainer/AnswerC");
		guiAnswerD = QuestionGUI.GetNode<RichTextLabel>("ColorRect/QuestionContainer/AnswerD");
		//guiCorrectAnswer
		//guiAnswerFact
		//guiQuestionImage
		guiQuestionBG = QuestionGUI.GetNode<TextureRect>("QuestionBG");

		// Assign fields
		guiCategoryText.Text = question.CategoryTitle;
		guiQuestionText.Text = question.QuestionText;
		guiAnswerA.Text = question.AnswerA;
		guiAnswerB.Text = question.AnswerB;
		guiAnswerC.Text = question.AnswerC;
		guiAnswerD.Text = question.AnswerD;
		//guiCorrectAnswer
		//guiAnswerFact
		//guiQuestionImage
		string imagePath = "res://Assets/" + question.BGImage;
		//string imagePath = "res://Assets/Birds/Images/background-science.jpg";
		Texture2D imageTexture = GD.Load<Texture2D>(imagePath);
		GD.Print(imagePath);
		guiQuestionBG.Texture = imageTexture;
		if (imageTexture == null)
		{
			GD.Print("Texture failed to load!!");
		}
		else
		{
			GD.Print("Texture Loaded!");
		}




		if (question != null)
		{
			GD.Print("Loaded question category: " + question.CategoryTitle);
		}
		else
		{
			GD.Print("SelectedQuestion is null!");
		}

		if (QuestionGUI != null)
		{
			GD.Print("Found Question GUI");
		}
	}
}
