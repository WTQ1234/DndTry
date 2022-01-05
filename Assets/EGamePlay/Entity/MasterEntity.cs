using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class MasterEntity : Entity
    {
        public static MasterEntity Instance { get; private set; }
        public static Dictionary<Type, List<Entity>> Entities { get; private set; } = new Dictionary<Type, List<Entity>>();
        public static Dictionary<long, Entity> EntitiesById { get; private set; } = new Dictionary<long, Entity>();

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

        public void SetEntity(Type entityType, Entity entity)
        {
            if (!Entities.ContainsKey(entityType))
            {
                Entities.Add(entityType, new List<Entity>());
            }
            Entities[entityType].Add(entity);

            EntitiesById.Add(entity.Id, entity);
        }

        public Entity GetEntity(long id)
        {
            if (EntitiesById.TryGetValue(id, out Entity entity))
            {
                return entity;
            }
            else
            {
                return null;
            }
        }

        public void RemoveEntity(Entity entity)
        {
            Type type = entity.GetType();
            if (Entities.ContainsKey(type))
            {
                Entities[type].Remove(entity);
            }
            long id = entity.Id;
            if (EntitiesById.ContainsKey(id))
            {
                EntitiesById.Remove(id);
            }
        }

        // todo 这里每帧都检测组件有点费
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