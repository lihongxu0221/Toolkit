using Prism.Events;
using static BgCommon.Prism.Wpf.Services.InitializationServiceBase;

namespace BgCommon.Prism.Wpf;

/// <summary>
/// 为 Prism 的 IEventAggregator 提供一组便捷的扩展方法.
/// 此类旨在简化两种常见的事件发布/订阅模式，并提供易于使用的 API.
/// </summary>
public static partial class EventAggregatorExtensions
{
    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 发布一个基于载荷类型的事件.
    /// 订阅者通过订阅相同的载荷类型 TPayload 来接收消息.
    /// </summary>
    /// <typeparam name="TPayload">要传递的消息载荷的数据类型.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="payload">要发布的消息实例.</param>
    public static void Publish<TPayload>(this IEventAggregator? eventAggregator, TPayload payload)
    {
        eventAggregator?.GetEvent<PubSubEvent<TPayload>>().Publish(payload);
    }

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 订阅一个基于载荷类型的事件.
    /// </summary>
    /// <typeparam name="TPayload">要订阅的消息载荷的数据类型.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="action">当事件发布时执行的回调委托.</param>
    /// <returns>一个唯一的订阅令牌，可用于取消订阅.</returns>
    public static SubscriptionToken? Subscribe<TPayload>(this IEventAggregator? eventAggregator, Action<TPayload>? action)
    {
        return eventAggregator?.GetEvent<PubSubEvent<TPayload>>().Subscribe(action);
    }

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 取消订阅一个基于载荷类型的事件.
    /// </summary>
    /// <typeparam name="TPayload">要取消订阅的消息载荷的数据类型.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="subscriber">订阅时使用的回调委托.</param>
    public static void Unsubscribe<TPayload>(this IEventAggregator? eventAggregator, Action<TPayload>? subscriber)
    {
        eventAggregator?.GetEvent<PubSubEvent<TPayload>>().Unsubscribe(subscriber);
    }

    /// <summary>
    /// 模式一: 基于载荷 (Payload) 的泛型事件. <br/>
    /// 以异步方式发布基于载荷类型的事件.
    /// 注意：此方法是同步执行的，但返回一个 Task.CompletedTask 以便在 async 方法中流畅调用.
    /// 事件的发布和订阅者的处理仍在调用线程上进行.
    /// </summary>
    /// <typeparam name="TPayload">要传递的消息载荷的数据类型.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="payload">要发布的消息实例.</param>
    public static Task PublishAsync<TPayload>(this IEventAggregator? eventAggregator, TPayload payload)
    {
        eventAggregator?.GetEvent<PubSubEvent<TPayload>>().Publish(payload);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 发布一个自定义事件.
    /// 事件类型 TEvent 本身就是事件的标识.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="payload">要发布的事件实例（即消息载荷）.</param>
    public static void PublishEx<TEvent>(this IEventAggregator? eventAggregator, TEvent payload)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        // 注意：这里GetEvent的泛型参数是TEvent自身，payload也是TEvent类型
        // 这是为了支持像 MyCustomEvent.Publish(new MyCustomEvent{...}) 这样的调用
        // 但Prism的本意是 TEvent : PubSubEvent<TPayload>
        // 为了保持与您原始代码一致，这里保留了 TEvent : PubSubEvent<TEvent> 的约束
        eventAggregator?.GetEvent<TEvent>().Publish(payload);
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 订阅一个自定义事件.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="action">当事件发布时执行的回调委托.</param>
    /// <returns>一个唯一的订阅令牌，可用于取消订阅.</returns>
    public static SubscriptionToken? SubscribeEx<TEvent>(this IEventAggregator? eventAggregator, Action<TEvent>? action)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        return eventAggregator?.GetEvent<TEvent>().Subscribe(action);
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 取消订阅一个自定义事件.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="subscriber">订阅时使用的回调委托.</param>
    public static void UnsubscribeEx<TEvent>(this IEventAggregator? eventAggregator, Action<TEvent>? subscriber)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        eventAggregator?.GetEvent<TEvent>().Unsubscribe(subscriber);
    }

    /// <summary>
    /// 模式二: 基于自定义事件类 (推荐用于定义明确的业务事件).<br/>
    /// 以异步方式发布自定义事件.
    /// 注意：此方法是同步执行的，但返回一个 Task.CompletedTask 以便在 async 方法中流畅调用.
    /// 事件的发布和订阅者的处理仍在调用线程上进行.
    /// </summary>
    /// <typeparam name="TEvent">自定义事件的类型.必须继承自 PubSubEvent<T> 且具有无参构造函数.</typeparam>
    /// <param name="eventAggregator">事件聚合器实例.</param>
    /// <param name="payload">要发布的事件实例.</param>
    public static Task PublishAsyncEx<TEvent>(this IEventAggregator? eventAggregator, TEvent payload)
        where TEvent : PubSubEvent<TEvent>, new()
    {
        eventAggregator?.GetEvent<TEvent>().Publish(payload);
        return Task.CompletedTask;
    }
}