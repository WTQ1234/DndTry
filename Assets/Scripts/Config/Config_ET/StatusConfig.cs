// 自动导出 F:/1desktop/DndTry/DndTry/Assets/Other/Excel F:/1desktop/DndTry/DndTry/Assets/Other/Excel\StatusConfig.xlsx
using ET;

public partial class StatusDataCategory : ACategory<StatusData>
{
	public static StatusDataCategory Instance;
	public StatusDataCategory()
	{
		Instance = this;
	}
}

public partial class StatusData: IConfig
{
	public int Id { get; set; }
	public string ID;
	public string CName;
	public string Name;
	public StatusType StatusType;
	public int Duration;
	public bool ShowInSlots;
	public int MaxStack;
	public int[] ChildrenStatuses;
	public ActionControlType ActionControlType;
	public string AttrModifyFormula;
	public int[] Effects;
	public string ParticleRes;
	public string AudioRes;
	public string Description;
}
