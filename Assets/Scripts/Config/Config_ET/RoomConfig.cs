// 自动导出 F:/1desktop/DndTry/DndTry/Assets/Other/Excel F:/1desktop/DndTry/DndTry/Assets/Other/Excel\RoomConfig.xlsx
using ET;

public partial class RoomDataCategory : ACategory<RoomData>
{
	public static RoomDataCategory Instance;
	public RoomDataCategory()
	{
		Instance = this;
	}
}

public partial class RoomData: IConfig
{
	public int Id { get; set; }
	public string Name;
	public int[] MonsterIds;
}
