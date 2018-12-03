// <copyright file="SerilogContextualLoggerInjectionBehavior.cs" company="PA-Group USA">
// Copyright (c) PA-Group USA. All Rights Reserved.
// </copyright>

namespace MahloService.Logging
{
  using System;
  using Serilog;
  using SimpleInjector;
  using SimpleInjector.Advanced;

  internal class SerilogContextualLoggerInjectionBehavior : IDependencyInjectionBehavior
  {
    private readonly IDependencyInjectionBehavior original;
    private readonly Container container;

    public SerilogContextualLoggerInjectionBehavior(ContainerOptions options)
    {
      this.original = options.DependencyInjectionBehavior;
      this.container = options.Container;
    }

    public void Verify(InjectionConsumerInfo consumer) => this.original.Verify(consumer);

    public InstanceProducer GetInstanceProducer(InjectionConsumerInfo i, bool t) =>
        i.Target.TargetType == typeof(ILogger)
            ? this.GetLoggerInstanceProducer(i.ImplementationType)
            : this.original.GetInstanceProducer(i, t);

    private InstanceProducer<ILogger> GetLoggerInstanceProducer(Type type) =>
        Lifestyle.Singleton.CreateProducer(() => Log.ForContext(type), this.container);
  }
}
