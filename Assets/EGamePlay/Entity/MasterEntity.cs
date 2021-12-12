using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class MasterEntity : Entity
    {
        public Dictionary<Type, List<Entity>> Entities { get; private set; } = new Dictionary<Type, List<Entity>>();
        public static MasterEntity Instance { get; private set; }

        // todo 这里应该也可以注释掉
        public List<Component> AllComponents { get; private set; } = new List<Component>();

        public override void Awake()
        {
            Instance = this;
            Instance.AddComponent<GameObjectComponent>();
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public void AddEntity(Type entityType, Entity entity)
        {
            if (!Entities.ContainsKey(entityType))
            {
                Entities.Add(entityType, new List<Entity>());
            }
            Entities[entityType].Add(entity);
        }

        public override void Update()
        {
            if (AllComponents.Count == 0)
            {
                return;
            }
            for (int i = AllComponents.Count - 1; i >= 0; i--)
            {
                var item = AllComponents[i];
                if (item.IsDisposed)
                {
                    AllComponents.RemoveAt(i);
                    continue;
                }
                if (item.Disable)
                {
                    continue;
                }
                item.Update();
            }
        }
    }
}