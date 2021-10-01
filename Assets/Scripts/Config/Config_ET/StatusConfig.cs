namespace ET
{
	[Config]
	public partial class StatusConfigCategory : ACategory<StatusConfig>
	{
		public static StatusConfigCategory Instance;
		public StatusConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class StatusConfig: IConfig
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
}
