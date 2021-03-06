using System;
using UnityEngine;

namespace EGamePlay
{
    public class Component : MonoBehaviour, IDisposable
    {
        public CardEntity OwnerEntity { get => GetComponent<CardEntity>(); }
        public Entity Entity { get; set; }
        public bool IsDisposed { get; set; }
        public virtual bool Enable { get; set; } = true;
        public bool Disable => Enable == false;


        public T GetEntity<T>() where T : Entity
        {
            return Entity as T;
        }

        public virtual void Setup()
        {

        }

        public virtual void Setup(object initData = null)
        {

        }

        protected virtual void Awake()
        {

        }

        public virtual void Update()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        public void Dispose()
        {
            if (Entity.EnableLog) Log.Debug($"{GetType().Name}->Dispose");
            IsDisposed = true;
        }

        public virtual T Publish<T>(T TEvent) where T : class
        {
            Entity.Publish(TEvent);
            return TEvent;
        }

        public virtual void Subscribe<T>(Action<T> action) where T : class
        {
            Entity.Subscribe(action);
        }

        public virtual void UnSubscribe<T>(Action<T> action) where T : class
        {
            Entity.UnSubscribe(action);
        }
    }
}