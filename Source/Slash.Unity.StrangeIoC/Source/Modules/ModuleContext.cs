﻿namespace Slash.Unity.StrangeIoC.Modules
{
    using System;
    using System.Collections.Generic;

    using strange.extensions.command.api;
    using strange.extensions.command.impl;
    using strange.extensions.context.api;
    using strange.extensions.context.impl;
    using strange.extensions.dispatcher.api;
    using strange.extensions.dispatcher.eventdispatcher.api;
    using strange.extensions.dispatcher.eventdispatcher.impl;
    using strange.extensions.implicitBind.api;
    using strange.extensions.implicitBind.impl;
    using strange.extensions.injector.api;
    using strange.extensions.mediation.api;
    using strange.extensions.mediation.impl;
    using strange.extensions.sequencer.api;
    using strange.extensions.sequencer.impl;
    using strange.framework.api;
    using strange.framework.impl;

    using Slash.Unity.StrangeIoC.Configs;
    using Slash.Unity.StrangeIoC.Modules.Signals;

    using UnityEngine;

    public class ModuleContext : CrossContext
    {
        /// A list of Views Awake before the Context is fully set up.
        protected static ISemiBinding ViewCache = new SemiBinding();

        /// <summary>
        ///   Registered bridges.
        /// </summary>
        private readonly List<Type> bridgeTypes;

        /// <summary>
        ///   Registered modules.
        /// </summary>
        private readonly List<Module> modules;

        /// <inheritdoc />
        public ModuleContext()
        {
            this.modules = new List<Module>();
            this.bridgeTypes = new List<Type>();
        }

        /// A Binder that maps Events to Commands
        public ICommandBinder CommandBinder { get; set; }

        public StrangeConfig Config { get; set; }

        /// A Binder that serves as the Event bus for the Context
        public IEventDispatcher Dispatcher { get; set; }

        //Interprets implicit bindings
        public IImplicitBinder ImplicitBinder { get; set; }

        /// <summary>
        ///   Indicates if module is ready to be launched.
        /// </summary>
        public bool IsReadyToLaunch
        {
            get
            {
                // Check if context view is set.
                if (this.contextView == null)
                {
                    return false;
                }

                foreach (var module in this.modules)
                {
                    if (!module.Context.IsReadyToLaunch)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// A Binder that maps Views to Mediators
        public IMediationBinder MediationBinder { get; set; }

        /// A Binder that maps Events to Sequences
        public ISequencer Sequencer { get; set; }

        public void AddBridge(Type bridgeType)
        {
            this.bridgeTypes.Add(bridgeType);
        }

        public void AddSubModule(StrangeConfig config)
        {
            this.modules.Add(new Module { Config = config });
        }

        /// <inheritdoc />
        public override void AddView(object viewObject)
        {
            var view = viewObject as IView;
            if (view == null)
            {
                Debug.LogError("View object " + viewObject + " doesn't implement IView interface");
                return;
            }

            if (this.MediationBinder != null)
            {
                this.MediationBinder.Trigger(MediationEvent.AWAKE, view);
            }
            else
            {
                this.CacheView(view as MonoBehaviour);
            }
        }

        /// <inheritdoc />
        public override object GetComponent<T>()
        {
            return this.GetComponent<T>(null);
        }

        /// <inheritdoc />
        public override object GetComponent<T>(object name)
        {
            var binding = this.injectionBinder.GetBinding<T>(name);
            if (binding != null)
            {
                return this.injectionBinder.GetInstance<T>(name);
            }
            return null;
        }

        public void Init()
        {
            //If firstContext was unloaded, the contextView will be null. Assign the new context as firstContext.
            if (firstContext == null || firstContext.GetContextView() == null)
            {
                firstContext = this;
            }
            else
            {
                firstContext.AddContext(this);
            }
            this.addCoreComponents();
            this.autoStartup = false;
        }

        /// <inheritdoc />
        public override void Launch()
        {
            base.Launch();

            // Launch sub-modules.
            foreach (var module in this.modules)
            {
                module.Context.Launch();
            }

            // Fire up bridges.
            foreach (var bridgeType in this.bridgeTypes)
            {
                this.injectionBinder.GetInstance(bridgeType);
            }

            //It's possible for views to fire their Awake before bindings. This catches any early risers and attaches their Mediators.
            this.MediateViewCache();
            //Ensure that all Views underneath the ContextView are triggered
            var contextViewBehaviour = this.contextView as ContextView;
            if (contextViewBehaviour != null)
            {
                this.MediationBinder.Trigger(MediationEvent.AWAKE, contextViewBehaviour);
            }
            else
            {
                Debug.LogError("Context view " + this.contextView + " has no attached ContextView behaviour");
            }

            var launchedSignal = this.injectionBinder.GetInstance<ModuleLaunchedSignal>();
            launchedSignal.Dispatch();

            this.Dispatcher.Dispatch(ContextEvent.START);
        }

        /// <inheritdoc />
        public override void OnRemove()
        {
            base.OnRemove();
            this.CommandBinder.OnRemove();
        }

        /// <inheritdoc />
        public override void RemoveView(object view)
        {
            this.MediationBinder.Trigger(MediationEvent.DESTROYED, view as IView);
        }

        /// <inheritdoc />
        public override IContext SetContextView(object view)
        {
            this.contextView = view as ContextView;
            if (this.contextView == null)
            {
                throw new ContextException(
                    "MVCSContext requires a ContextView of type MonoBehaviour",
                    ContextExceptionType.NO_CONTEXT_VIEW);
            }

            this.injectionBinder.Bind<GameObject>()
                .ToValue(((ContextView)this.contextView).gameObject)
                .ToName(ContextKeys.CONTEXT_VIEW);

            return this;
        }

        /// <inheritdoc />
        public override IContext Start()
        {
            // Start sub modules.
            foreach (var module in this.modules)
            {
                // Create context for module.
                module.Context = new ModuleContext { Config = module.Config };
                module.Context.Init();
                this.AddContext(module.Context);

                module.Context.Start();

                // Setup views for modules.
                module.Config.SetupView(module.Context);
            }

            return base.Start();
        }

        /// <inheritdoc />
        protected override void addCoreComponents()
        {
            base.addCoreComponents();

            this.injectionBinder.Bind<IInstanceProvider>().Bind<IInjectionBinder>().ToValue(this.injectionBinder);
            this.injectionBinder.Bind<IContext>().ToValue(this).ToName(ContextKeys.CONTEXT);
            this.injectionBinder.Bind<ICommandBinder>().To<EventCommandBinder>().ToSingleton();
            //This binding is for local dispatchers
            this.injectionBinder.Bind<IEventDispatcher>().To<EventDispatcher>();
            //This binding is for the common system bus
            this.injectionBinder.Bind<IEventDispatcher>()
                .To<EventDispatcher>()
                .ToSingleton()
                .ToName(ContextKeys.CONTEXT_DISPATCHER);
            this.injectionBinder.Bind<IMediationBinder>().To<MediationBinder>().ToSingleton();
            this.injectionBinder.Bind<ISequencer>().To<EventSequencer>().ToSingleton();
            this.injectionBinder.Bind<IImplicitBinder>().To<ImplicitBinder>().ToSingleton();

            // Enable signals.
            this.injectionBinder.Unbind<ICommandBinder>();
            this.injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }

        /// <summary>
        ///   Caches early-riser Views.
        ///   If a View is on stage at startup, it's possible for that
        ///   View to be Awake before this Context has finished initing.
        ///   `cacheView()` maintains a list of such 'early-risers'
        ///   until the Context is ready to mediate them.
        /// </summary>
        /// <param name="view"></param>
        protected virtual void CacheView(MonoBehaviour view)
        {
            if (ViewCache.constraint.Equals(BindingConstraintType.ONE))
            {
                ViewCache.constraint = BindingConstraintType.MANY;
            }
            ViewCache.Add(view);
        }

        /// <inheritdoc />
        protected override void instantiateCoreComponents()
        {
            base.instantiateCoreComponents();

            this.CommandBinder = this.injectionBinder.GetInstance<ICommandBinder>();

            this.Dispatcher = this.injectionBinder.GetInstance<IEventDispatcher>(ContextKeys.CONTEXT_DISPATCHER);
            this.MediationBinder = this.injectionBinder.GetInstance<IMediationBinder>();
            this.Sequencer = this.injectionBinder.GetInstance<ISequencer>();
            this.ImplicitBinder = this.injectionBinder.GetInstance<IImplicitBinder>();

            ((ITriggerProvider)this.Dispatcher).AddTriggerable(this.CommandBinder as ITriggerable);
            ((ITriggerProvider)this.Dispatcher).AddTriggerable(this.Sequencer as ITriggerable);
        }

        /// <inheritdoc />
        protected override void mapBindings()
        {
            base.mapBindings();

            // Module lifecycle.
            this.injectionBinder.Bind<ModuleLaunchedSignal>().ToSingleton();

            if (this.Config != null)
            {
                // Map bindings for module.
                this.Config.MapBindings(this.injectionBinder);
                this.Config.MapBindings(this.CommandBinder);
                this.Config.MapBindings(this.MediationBinder);
            }

            // Inject bridges.
            foreach (var bridgeType in this.bridgeTypes)
            {
                this.injectionBinder.Bind(bridgeType).ToSingleton();
            }
        }

        protected virtual void MediateViewCache()
        {
            if (this.MediationBinder == null)
            {
                throw new ContextException(
                    "MVCSContext cannot mediate views without a mediationBinder",
                    ContextExceptionType.NO_MEDIATION_BINDER);
            }

            var values = ViewCache.value as object[];
            if (values == null)
            {
                return;
            }
            var aa = values.Length;
            for (var a = 0; a < aa; a++)
            {
                var viewObject = values[a];
                var view = viewObject as IView;
                if (view == null)
                {
                    Debug.LogError("View object " + viewObject + " doesn't implement IView interface");
                    continue;
                }
                this.MediationBinder.Trigger(MediationEvent.AWAKE, view);
            }
            ViewCache = new SemiBinding();
        }

        private class Module
        {
            public StrangeConfig Config { get; set; }

            public ModuleContext Context { get; set; }
        }
    }
}