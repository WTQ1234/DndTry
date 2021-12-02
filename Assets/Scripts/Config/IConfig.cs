using System;
namespace ET
{
	/// <summary>
	/// 每个Config的基类
	/// </summary>
	public interface IConfig
	{
		int Id { get; set; }	// id
		// string Key {get; set;}	// Key 若配置Key，则可使用Key进行获取
	}

	// 配置表属性，暂时没有考虑怎么弄
	// [AttributeUsage(AttributeTargets.Class)]
	// public class ConfigAttribute: BaseAttribute
	// {
	// }
}