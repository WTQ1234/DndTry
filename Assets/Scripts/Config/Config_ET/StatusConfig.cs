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
		public string Name;
		public string StatusType;
		public int Duration;
		public string ShowInStatusSlots;
		public string CanStack;
		public int MaxStack;
		public string EnableChildrenStatuses;
		public int[] ChildrenStatuses;
		public bool EnabledStateModify;
		public ActionControlType ActionControlType;
	}
}
