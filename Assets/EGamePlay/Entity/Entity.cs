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
        public static T Create<T>(
            object initData = null, GameObject owner = null, 
            GameObject prefab = null, Entity parent = null, Transform ownerParent = null, string Name = null) where T : Entity
        {
            Type type = typeof(T);
            return _Create(type, initData, owner, prefab, parent, ownerParent, Name) as T;
        }

        public static Entity Create(
            Type type = null, object initData = null, GameObject owner = null, 
            GameObject prefab = null, Entity parent = null, Transform ownerParent = null, string Name = null)
        {
            return _Create(type, initData, owner, prefab, parent, ownerParent, Name);
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
        private static Entity _Create(
            Type type = null, object initData = null, GameObject owner = null,
            GameObject prefab = null, Entity parent = null, Transform ownerParent = null, string Name = null)
        {
            bool needObj = owner == null;
            if (needObj) owner = prefab ? Instantiate(prefab) : new GameObject();
            var component = owner.GetComponent(type);
            var entity = (component != null ? component : owner.AddComponent(type)) as Entity;

            (parent == null ? Master : parent).AddChild(entity);

            if (ownerParent != null) entity.SetTransformParent(ownerParent);

            long id = IdFactory.NewInstanceId();
            entity.InstanceId = entity.Id = id;
            entity.Name = Name != null ? Name : type.ToString();
            Master.SetEntity(type, entity);

            entity.Setup(initData, needObj);

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
            this.Dispose();
            UnityEngine.Object.Destroy(gameObject, t);
        }
        #endregion

        #region 生命周期
        // todo 啥时候给改一下改成protected
        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        // 需要考虑 UpdateComponent ，即是否可以Update，这个组件可能可以删掉
        public virtual void Update()
        {
        }

        public virtual void OnDestroy()
        {

        }
        #endregion

        #region 事件
        private Dictionary<long, List<(string, EventDelegate)>> subscribeOnList = new Dictionary<long, List<(string, EventDelegate)>>();
        // 监听
        public void Subscribe(string key, EventDelegate action)
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            eventComponent.Subscribe(key, action);
        }
        // 监听挂在其他物体上
        public void SubscribeOnObj(Entity obj, string key, EventDelegate action)
        {
            // 将此监听数据保存起来，在Entity被销毁时，尝试进行释放
            if (subscribeOnList.ContainsKey(obj.Id))
            {
                subscribeOnList[obj.Id].Add((key, action));
            }
            else
            {
                subscribeOnList.Add(obj.Id, new List<(string, EventDelegate)> () {(key, action)});
            }
            obj.Subscribe(key, action);
        }
        // 取消监听
        public void UnSubscribe(string key, EventDelegate action)
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.UnSubscribe(key, action);
            }
        }
        // 取消挂在其他物体上的监听
        private void UnSubscribeOnDispose()
        {
            if (subscribeOnList.Count > 0)
            {
                foreach(var kv in subscribeOnList)
                {
                    Entity entity = Master.GetEntity(kv.Key);
                    List<(string, EventDelegate)> list = kv.Value;
                    foreach(var actionValue in list)
                    {
                        entity.UnSubscribe(actionValue.Item1, actionValue.Item2);
                    }
                }
                subscribeOnList.Clear();
            }
        }
        // 发布
        public void Publish(string key, EventParams eventParams)
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent != null)
            {
                eventComponent.Publish(key, eventParams);
            }
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

            // 清除事件监听
            UnSubscribeOnDispose();

            Parent?.RemoveChild(this);
            foreach (Component component in this.Components.Values)
            {
                //component.OnDestroy();
                component.Dispose();
            }
            this.Components.Clear();
            Master.RemoveEntity(this);
            InstanceId = 0;
            Id = 0;
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
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->AddComponent, {typeof(T).Name} initData={initData.ToString()}");
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
            //eventComponent.Publish(TEvent);
            return TEvent;
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent == null)
            {
                eventComponent = AddComponent<EventComponent>();
            }
            //eventComponent.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            var eventComponent = GetEntityComponent<EventComponent>();
            if (eventComponent != null)
            {
                //eventComponent.UnSubscribe(action);
            }
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