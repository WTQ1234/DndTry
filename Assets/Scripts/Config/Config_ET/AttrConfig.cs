// 自动导出 F:/1desktop/DndTry/DndTry/Assets/Other/Excel F:/1desktop/DndTry/DndTry/Assets/Other/Excel\AttrConfig.xlsx
using ET;

public partial class AttrDataCategory : ACategory<AttrData>
{
	public static AttrDataCategory Instance;
	public AttrDataCategory()
	{
		Instance = this;
	}
}

public partial class AttrData: IConfig
{
	public int Id { get; set; }
	public AttrType AttrType;
	public string EnumName;
	public float DefalutValue;
	public string AttrFormula;
	public int FormulaLevel;
	public string Description;
}
