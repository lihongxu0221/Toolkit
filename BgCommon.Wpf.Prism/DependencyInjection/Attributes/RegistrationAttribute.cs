namespace BgCommon.Wpf.Prism.DependencyInjection.Attributes;

/// <summary>
/// 注入服务特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class RegistrationAttribute : Attribute
{
    private readonly Type? viewModel;
    private readonly Type[] serviceTypes;
    private readonly Registration mode = Registration.Normal;

    /// <summary>
    /// Gets ViewModel type
    /// </summary>
    public Registration Mode => mode;

    /// <summary>
    /// Gets ViewModel type
    /// </summary>
    public Type? ViewModelType => viewModel;

    /// <summary>
    /// Gets the service Types
    /// </summary>
    public Type[] ServiceTypes => serviceTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegistrationAttribute"/> class.
    /// </summary>
    /// <param name="mode">registration mode.</param>
    /// <param name="viewModel">true: viewModel type false: view type.</param>
    /// <param name="serviceTypes">the service Types.</param>
    public RegistrationAttribute(Registration mode, Type? viewModel = null, params Type[] serviceTypes)
    {
        if (serviceTypes.Length > 0)
        {
            if (serviceTypes.Any(t => t == null))
            {
                throw new ArgumentNullException(nameof(serviceTypes), "ServiceTypes cannot contain null values.");
            }
        }

        this.mode = mode;
        this.viewModel = viewModel;
        this.serviceTypes = serviceTypes;
    }
}