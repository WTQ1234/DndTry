using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ET;
using System;
using System.Reflection;

public class ConfigController : SingleTon<ConfigController>
{
        public Dictionary<Type, object> TypeConfigCategarys { get; set; } = new Dictionary<Type, object>();

        public void Awake()
        {
            Instance = this;
            var assembly = Assembly.GetAssembly(typeof(TimerComponent));
            var referenceCollector = ReferenceCollector.Instance;
            if (referenceCollector == null)
            {
                return;
            }
            foreach (var item in referenceCollector.data)
            {
                var configTypeName = $"ET.{item.gameObject.name}";
                var configType = assembly.GetType(configTypeName);
                var typeName = $"ET.{item.gameObject.name}Category";
                var configCategoryType = assembly.GetType(typeName);
                var configCategory = Activator.CreateInstance(configCategoryType) as ACategory;
                configCategory.ConfigText = (item.gameObject as TextAsset).text;
                configCategory.BeginInit();
                TypeConfigCategarys.Add(configType, configCategory);
            }
        }

        public T Get<T>(int id) where T : class, IConfig
        {
            var category = TypeConfigCategarys[typeof(T)] as ACategory<T>;
            return category.Get(id);
        }

        public Dictionary<int, T> GetAll<T>() where T : class, IConfig
        {
            var category = TypeConfigCategarys[typeof(T)] as ACategory<T>;
            return category.GetAll();
        }
}
