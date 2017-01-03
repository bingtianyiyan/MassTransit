// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport.Hosting
{
    using System;
    using Configuration;
    using ConsumeConfigurators;
    using GreenPipes;
    using MassTransit.Builders;
    using MassTransit.Hosting;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConfigurators;


    /// <summary>
    /// A hosted service can specify receive endpoints using the service configurator
    /// </summary>
    public class ServiceBusServiceConfigurator :
        IServiceConfigurator
    {
        readonly IServiceBusBusFactoryConfigurator _configurator;
        readonly int _defaultConsumerLimit;
        readonly IServiceBusHost _host;

        public ServiceBusServiceConfigurator(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            _configurator = configurator;
            _host = host;
            _defaultConsumerLimit = Environment.ProcessorCount * 4;
        }

        public void ReceiveEndpoint(string queueName, int consumerLimit, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            _configurator.ReceiveEndpoint(_host, queueName, x =>
            {
                x.PrefetchCount = (ushort)consumerLimit;

                configureEndpoint(x);
            });
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumeContext> specification)
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddPipeSpecification<T>(IPipeSpecification<ConsumeContext<T>> specification) where T : class
        {
            _configurator.AddPipeSpecification(specification);
        }

        public void AddBusFactorySpecification(IBusFactorySpecification specification)
        {
            _configurator.AddBusFactorySpecification(specification);
        }

        public void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint)
        {
            ReceiveEndpoint(queueName, _defaultConsumerLimit, configureEndpoint);
        }

        ConnectHandle IConsumerConfigurationObserverConnector.ConnectConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _configurator.ConnectConfigurationObserver(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configurator.ConfigureSend(callback);
        }

        public void ConfigurePublish(Action<IPublishPipeConfigurator> callback)
        {
            _configurator.ConfigurePublish(callback);
        }

        void IConsumerConfigurationObserver.ConfigureConsumer<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
        {
            _configurator.ConfigureConsumer(configurator);
        }

        void IConsumerConfigurationObserver.ConfigureConsumerMessage<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
        {
            _configurator.ConfigureConsumerMessage(configurator);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _configurator.ConnectSagaConfigurationObserver(observer);
        }

        public void SagaConfigured<TSaga>(ISagaConfigurator<TSaga> configurator) where TSaga : class, ISaga
        {
            _configurator.SagaConfigured(configurator);
        }

        public void SagaMessageConfigured<TSaga, TMessage>(ISagaMessageConfigurator<TSaga, TMessage> configurator) where TSaga : class, ISaga where TMessage : class
        {
            _configurator.SagaMessageConfigured(configurator);
        }
    }
}