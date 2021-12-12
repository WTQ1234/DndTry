// 自动导出 F:/1desktop/DndTry/DndTry/Assets/Other/Excel F:/1desktop/DndTry/DndTry/Assets/Other/Excel\MonsterConfig.xlsx
using ET;

public partial class MonsterDataCategory : ACategory<MonsterData>
{
	public static MonsterDataCategory Instance;
	public MonsterDataCategory()
	{
		Instance = this;
	}
}

public partial class MonsterData: IConfig
{
	public int Id { get; set; }
	public CreatureRaceType CreatureRaceType;
	public string RaceName;
	public string Name;
	public int Level;
	public int[] RandomAttr;
	public string FormulaLevel;
	public int[] States;
	public string Description;
}
