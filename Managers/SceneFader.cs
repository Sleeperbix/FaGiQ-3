using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class SceneFader : CanvasLayer
{
	[Export] public float fadeTime = 1.0f;

	private ColorRect fadeRect;
	private Tween tween;

	public override void _Ready()
	{
		fadeRect = GetNode<ColorRect>("FadeRect");
		fadeRect.Color = new Color(0, 0, 0, 1);
		FadeIn();
	}

	public async Task FadeToScene(string scenePath)
	{
		await FadeOut();
		GetTree().ChangeSceneToFile(scenePath);
		await ToSignal(GetTree(), "idle_frame");
	}

	public async void FadeIn()
	{
		fadeRect.Visible = true;
		tween = CreateTween();
		tween.TweenProperty(fadeRect, "modulate:a", 0f, fadeTime);
		await ToSignal(tween, "finished");
		fadeRect.Visible = false;
	}

	public async Task FadeOut()
	{
		fadeRect.Visible = true;
		tween = CreateTween();
		tween.TweenProperty(fadeRect, "modulate:a", 1f, fadeTime);
		await ToSignal(tween, "finished");
	}
}
