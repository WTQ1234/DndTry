namespace ET
{
	[Config]
	public partial class AttrConfigCategory : ACategory<AttrConfig>
	{
		public static AttrConfigCategory Instance;
		public AttrConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class AttrConfig: IConfig
	{
		public int Id { get; set; }
		public AttrType AttrType;
		public string EnumName;
		public float DefalutValue;
		public string AttrFormula;
		public int FormulaLevel;
		public string Description;
	}
}
