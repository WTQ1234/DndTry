namespace ET
{
	public partial class MonsterConfigCategory : ACategory<MonsterConfig>
	{
		public static MonsterConfigCategory Instance;
		public MonsterConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class MonsterConfig: IConfig
	{
		public int Id { get; set; }
		// public RaceType1 RaceType1;
		// public RaceType2 RaceType2;
		// public Template Template;
		// public float DefalutValue;
		// public string AttrFormula;
		// public string FormulaLevel;
		// public int[] Description;
		// public int[] Description;
	}
}
