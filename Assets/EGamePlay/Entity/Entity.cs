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
        public static T Create<T>(object initData = null, GameObject owner = null, Entity parent = null) where T : Entity
        {
            bool needObj = owner == null;
            Type type = typeof(T);
            if (needObj) owner = new GameObject();
            var entity = owner.AddComponent(type) as T;

            entity.Setup(initData, needObj);
            Master.AddEntity(type, entity);
            (parent == null ? Master : parent).AddChild(entity);

            long id = IdFactory.NewInstanceId();
            entity.InstanceId = entity.Id = id;
            entity.Name = typeof(T).ToString();

            if (EnableLog) Log.Debug($"Entity->Create, {typeof(T).Name}={entity.InstanceId}");

            return entity;
        }

        public static Entity Create(Type type ,object initData = null, GameObject owner = null, Entity parent = null)
        {
            bool needObj = owner == null;
            if (needObj) owner = new GameObject();
            var entity = owner.AddComponent(type) as Entity;

            entity.Setup(initData, needObj);
            Master.AddEntity(type, entity);
            (parent == null ? Master : parent).AddChild(entity);

            long id = IdFactory.NewInstanceId();
            entity.InstanceId = entity.Id = id;
            entity.Name = type.ToString();

            if (EnableLog) Log.Debug($"Entity->Create, {type.Name}={entity.InstanceId}");

            return entity;
        }

        public virtual void Setup(object initData = null, bool asGameObject = false)
        {
            if (asGameObject)
            {
                if (this is MasterEntity) { }
                else AddComponent<GameObjectComponent>();
            }
        }

        public static void Destroy(Entity entity)
        {
            entity.OnDestroy();
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
            var childrenComponent = GetComponent<ChildrenComponent>();
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
                component.OnDestroy();
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
            Master.AllComponents.Add(component);
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
            component.OnDestroy();
            component.Dispose();
            this.Components.Remove(typeof(T));
            OnRemoveComponentAction?.Invoke((component));
        }

        public virtual T GetComponent<T>() where T : Component
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

        public void AddChild(Entity child)
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
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
            var childrenComponent = GetComponent<ChildrenComponent>();
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

        public T CreateChild<T>() where T : Entity
        {
            return Create<T>(null, null, this) as T;
        }

        public T CreateChild<T>(object initData) where T : Entity
        {
            return Create<T>(initData, null, this) as T;
        }

        public Entity[] GetChildren()
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                return new Entity[0];
            }
            return childrenComponent.GetChildren();
        }

        public Entity[] GetTypeChildren<T>() where T : Entity
        {
            var childrenComponent = GetComponent<ChildrenComponent>();
            if (childrenComponent == null)
            {
                return new Entity[0];
            }
            return childrenComponent.GetTypeChildren<T>();
        }

        public T Publish<T>(T TEvent) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                return TEvent;
            }
            eventComponent.Publish(TEvent);
            return TEvent;
        }

        public TEvent Publish<TEvent, TParam>(TEvent evnt, TParam param) where TEvent : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                return evnt;
            }
            eventComponent.Publish(evnt);
            return evnt;
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            eventComponent.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.UnSubscribe(action);
            }
        }

        public void Fire(string signal)
        {

        }
    }
}