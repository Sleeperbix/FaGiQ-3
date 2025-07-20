using System.Collections.Generic;
public class C_QuestionMultipleChoice
{
	public string CategoryTitle; 
	public string QuestionText;
	public string AnswerA;
	public string AnswerB;
	public string AnswerC;
	public string AnswerD;
	public string CorrectAnswer;
	public string AnswerFact;
	public List<string> Tags = new List<string>();
	public Godot.Color ButtonTextColour;
	public Godot.Color ButtonBGColour;
	public string QuestionImage;
	public string BGImage;
	public string SelectionSound;
	public string BGMusic;
}
