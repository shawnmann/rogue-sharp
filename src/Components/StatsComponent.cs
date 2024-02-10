using Godot;

[GlobalClass]
public partial class StatsComponent : Node
{
	[Export] public StatsResource StatsResource;
	
	// Moxie: Courage, Determination
	public int Moxie { get; set; }
	public string MoxieName = TranslationServer.Translate("STAT_MOXIE");
	
	// Psyche: Mind, Spirit
	public int Psyche { get; set; }
	public string PsycheName = TranslationServer.Translate("STAT_PSYCHE");
	
	// Smarts: Intellect, Logic
	public int Smarts { get; set; }
	public string SmartsName = TranslationServer.Translate("STAT_SMARTS");
	
	// Wits: Awareness, Perception
	public int Wits { get; set; }
	public string WitsName = TranslationServer.Translate("STAT_WITS");
	
	// Muscle: Strength
	public int Muscle { get; set; }
	public string MuscleName = TranslationServer.Translate("STAT_MUSCLE");
	
	// Cool: Charisma, Charm, Popularity
	public int Cool { get; set; }
	public string CoolName = TranslationServer.Translate("STAT_COOL");
	
	// Quick: Agility, Swiftness
	public int Quick { get; set; }
	public string QuickName = TranslationServer.Translate("STAT_QUICK");
	
	public override void _Ready()
	{
		base._Ready();
		
		Moxie = StatsResource.Moxie;
		Psyche = StatsResource.Psyche;
		Smarts = StatsResource.Smarts;
		Wits = StatsResource.Wits;
		Muscle = StatsResource.Muscle;
		Cool = StatsResource.Cool;
		Quick = StatsResource.Quick;
	}
}
