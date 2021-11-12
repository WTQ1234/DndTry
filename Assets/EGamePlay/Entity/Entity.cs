using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameUtils;
using UnityEngine;

namespace EGamePlay
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class EnableUpdateAttribute : Attribute
    {
        public EnableUpdateAttribute()
        {
        }
    }

    public abstract class Entity : MonoBehaviour, IDisposable
    {
        public static MasterEntity Master => MasterEntity.Instance;
        public static bool EnableLog { get; set; } = false;

        public long Id { get; set; }
        private string name_trans;
        public string Name
        {
            get => name_trans;
            set
            {
                name_trans = value;
                OnNameChangedAction?.Invoke((name_trans));
            }
        }
        public long InstanceId { get; set; }
        private Entity parent;
        public Entity Parent { get { return parent; } private set { parent = value; OnSetParent(value); } }
        public bool IsDisposed { get { return InstanceId == 0; } }
        public Dictionary<Type, Component> Components { get; set; } = new Dictionary<Type, Component>();
        public Action<string> OnNameChangedAction { get; set; }
        public Action<Component> OnAddComponentAction { get; set; }
        public Action<Component> OnRemoveComponentAction { get; set; }
        public Action<Entity> OnAddChildAction { get; set; }
        public Action<Entity> OnRemoveChildAction { get; set; }

        #region 创建与销毁
        public static T Create<T>(object initData = null, GameObject owner = null, GameObject prefab = null, Entity parent = null, Transform ownerParent = null) where T : Entity
        {
            Type type = typeof(T);
            return _Create(type, initData, owner, prefab, parent, ownerParent) as T;
        }

        public static Entity Create(Type type = null, object initData = null, GameObject owner = null, GameObject prefab = null, Entity parent = null, Transform ownerParent = null)
        {
            return _Create(type, initData, owner, prefab, parent, ownerParent);
        }

        /// <summary>
        /// 具体创建逻辑
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="initData">初始化参数</param>
        /// <param name="owner">Entity所在的Obj，不传owner，则视为创建一个新obj，否则只是创建一个Component</param>
        /// <param name="prefab">新obj对应的prefab</param>
        /// <param name="parent">Entity指定Parent</param>
        /// <param name="ownerParent">Transform指定Parent</param>
        /// <returns></returns>
        private static Entity _Create( Type type = null, object initData = null, GameObject owner = null, GameObject prefab = null, Entity parent = null, Transform ownerParent = null)
        {
            bool needObj = owner == null;
            if (needObj) owner = prefab ? Instantiate(prefab) : new GameObject();
            var component = owner.GetComponent(type);
            var entity = (component != null ? component : owner.AddComponent(type)) as Entity;

            entity.Setup(initData, needObj);
            Master.AddEntity(type, entity);
            (parent == null ? Master : parent).AddChild(entity);

            if (ownerParent != null) entity.SetTransformParent(ownerParent);

            long id = IdFactory.NewInstanceId();
            entity.InstanceId = entity.Id = id;
            entity.Name = type.ToString();

            if (EnableLog) Log.Debug($"Entity->Create, {type.Name}={entity.InstanceId}");

            return entity;
        }

        // 无法指定owner，一般不用
        public T CreateChild<T>(object initData = null) where T : Entity
        {
            return Create<T>(initData: initData, parent: this) as T;
        }

        public virtual void Setup(object initData = null, bool asGameObject = false)
        {
            if (asGameObject)
            {
                if (this is MasterEntity) { }
                else AddComponent<GameObjectComponent>();
            }
        }

        public void DestroyEntity(float t = 0)
        {
            UnityEngine.Object.Destroy(gameObject, t);
            //entity.Dispose();
        }
        #endregion

        #region 生命周期
        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnDestroy()
        {

        }
        #endregion

        public void Dispose()
        {
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->Dispose");
            var childrenComponent = GetEntityComponent<ChildrenComponent>();
            if (childrenComponent != null)
            {
                var Children = childrenComponent.Children;
                var Type2Children = childrenComponent.Type2Children;
                if (Children.Count > 0)
                {
                    for (int i = Children.Count - 1; i >= 0; i--)
                    {
                        Entity.Destroy(Children[i]);
                    }
                    Children.Clear();
                    Type2Children.Clear();
                }
            }

            Parent?.RemoveChild(this);
            foreach (Component component in this.Components.Values)
            {
                //component.OnDestroy();
                component.Dispose();
            }
            this.Components.Clear();
            InstanceId = 0;
            if (Master.Entities.ContainsKey(GetType()))
            {
                Master.Entities[GetType()].Remove(this);
            }
        }

        public virtual void OnSetParent(Entity parent)
        {

        }

        public T GetParent<T>() where T : Entity
        {
            return parent as T;
        }

        public T AddComponent<T>() where T : Component
        {
            var component = gameObject.AddComponent<T>();
            component.Entity = this;
            component.IsDisposed = false;
            component.Enable = true;
            this.Components.Add(typeof(T), component);
            //Master.AllComponents.Add(component);
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name}");
            component.Setup();
            OnAddComponentAction?.Invoke((component));
            return component;
        }

        public T AddComponent<T>(object initData) where T : Component
        {
            var component = gameObject.AddComponent<T>();
            component.Entity = this;
            component.IsDisposed = false;
            component.Enable = true;
            this.Components.Add(typeof(T), component);
            Master.AllComponents.Add(component);
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name} initData={initData}");
            component.Setup(initData);
            OnAddComponentAction?.Invoke((component));
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            var component = this.Components[typeof(T)];
            //component.OnDestroy();
            component.Dispose();
            this.Components.Remove(typeof(T));
            OnRemoveComponentAction?.Invoke((component));
        }

        public virtual T GetEntityComponent<T>() where T : Component
        {
            if (this.Components.TryGetValue(typeof(T),  out var component))
            {
                return component as T;
            }
            return null;
        }

        public void SetParent(Entity parent)
        {
            Parent?.RemoveChild(this);
            parent?.AddChild(this);
        }

        public void SetTransformParent(Transform parent)
        {
            if (parent.Equals(transform.parent)) return;
            gameObject.transform.parent = parent;
        }

        public void AddChild(Entity child)
        {
            var childrenComponent = GetEntityComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                childrenComponent = AddComponent<ChildrenComponent>();
            }

            var Children = childrenComponent.Children;
            var Type2Children = childrenComponent.Type2Children;
            Children.Add(child);
            if (!Type2Children.ContainsKey(child.GetType()))
            {
                Type2Children.Add(child.GetType(), new List<Entity>());
            }
            Type2Children[child.GetType()].Add(child);
            child.Parent = this;
            OnAddChildAction?.Invoke(child);
        }

        public void RemoveChild(Entity child)
        {
            var childrenComponent = GetEntityComponent<ChildrenComponent>();
            var Children = childrenComponent.Children;
            var Type2Children = childrenComponent.Type2Children;
            Children.Remove(child);
            if (Type2Children.ContainsKey(child.GetType()))
            {
                Type2Children[child.GetType()].Remove(child);
            }
            child.Parent = Master;
            OnRemoveChildAction?.Invoke(child);
        }

        //public Entity CreateChild(Type entityType)
        //{
        //    return Create(entityType, this);
        //}

        public Entity[] GetChildren()
        {
            var childrenComponent = GetEntityComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                return new Entity[0];
            }
            return childrenComponent.GetChildren();
        }

        public Entity[] GetTypeChildren<T>() where T : Entity
        {
            var childrenComponent = GetEntityComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                return new Entity[0];
            }
            return childrenComponent.GetTypeChildren<T>();
        }

        public T Publish<T>(T TEvent) where T : class
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent == null)
            {
                return TEvent;
            }
            eventComponent.Publish(TEvent);
            return TEvent;
        }

        public TEvent Publish<TEvent, TParam>(TEvent evnt, TParam param) where TEvent : class
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent == null)
            {
                return evnt;
            }
            eventComponent.Publish(evnt);
            return evnt;
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            eventComponent.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.UnSubscribe(action);
            }
        }

        public void Fire(string signal)
        {

        }
    }

    public class NewEntityData
    {
        object initData = null;
        Type type = null;
        bool asComponent = true;

        GameObject owner = null;
        GameObject ownerParent = null;
        GameObject prefab = null; 
        Entity parent = null;
    }
}