using Godot;
using System;

public partial class CurseSelectionCard : Panel
{
    private PackedScene _cursePackedScene;
    private Curse _curse;
    private Label _cardName;
    private Label _cardDescription;
    private MeshInstance3D _cardModel;

    public override void _Ready()
    {
        _curse = _cursePackedScene.Instantiate<Curse>();
    }

    public CurseSelectionCard(PackedScene cursePackScene)
    {
        _cursePackedScene = cursePackScene;
        Initialize();
    }

    private void Initialize()
    {
        _cardName.Text = _curse.CurseData.Name;
        _cardDescription.Text = _curse.CurseData.Description;
    }

    private void ApplyUpgrade()
    {
        var curseManager = GetTree().Root.GetNode<CurseManager>("CurseManager");
        var curse = _cursePackedScene.Instantiate<Curse>();
        curseManager.AddChild(curse);
    }
}