using Godot;
using System;

public partial class ImportTestScript : Node
{
	[Export] public NodePath CategoryLabel;
	[Export] public NodePath QuestionLabel;
	[Export] public NodePath AnswerALabel;
	[Export] public NodePath AnswerBLabel;
	[Export] public NodePath AnswerCLabel;
	[Export] public NodePath AnswerDLabel;
	[Export] public NodePath CorrectAnswerLabel;
	[Export] public NodePath FactLabel;
	[Export] public NodePath QuestionImage;
	[Export] public NodePath BGMusic;

	public override void _Ready()
	{
		Label category = GetNode<Label>(CategoryLabel);
		Label question = GetNode<Label>(QuestionLabel);
		Label answerA = GetNode<Label>(AnswerALabel);
		Label answerB = GetNode<Label>(AnswerBLabel);
		Label answerC = GetNode<Label>(AnswerCLabel);
		Label answerD = GetNode<Label>(AnswerDLabel);
		Label answerCorrect = GetNode<Label>(CorrectAnswerLabel);
		Label fact = GetNode<Label>(FactLabel);
		TextureRect questionImage = GetNode<TextureRect>(QuestionImage);
		AudioStreamPlayer bgMusic = GetNode<AudioStreamPlayer>(BGMusic);

		// Declare the location of the file to be read for the testing
		string filePath = "res://Assets/00Default/import_test.txt"; 
		//string filePath = "res://Assets/Dogs/dogs.txt";
		//string filePath = "res://Assets/Birds/birds.txt";

		if (FileAccess.FileExists(filePath))
		{
			var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			string content = file.GetAsText();
			GD.Print(content);
			string[] result = content.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
			category.Text = result[0];
			question.Text = result[1];
			answerA.Text = result[2];
			answerB.Text = result[3];
			answerC.Text = result[4];
			answerD.Text = result[5];
			answerCorrect.Text = result[6];
			fact.Text = result[7];

			string imagePath = "res://Assets/" + result[10];
			Texture2D imageTexture = GD.Load<Texture2D>(imagePath);
			questionImage.Texture = imageTexture;
			GD.Print(imagePath);

			string musicPath = "res://Assets/" + result[12];
			AudioStream musicStream = GD.Load<AudioStream>(musicPath);
			bgMusic.Stream = musicStream;
			bgMusic.Play();

		}
		else
		{
			GD.PrintErr("File not found: " + filePath);
		}
	}
}
