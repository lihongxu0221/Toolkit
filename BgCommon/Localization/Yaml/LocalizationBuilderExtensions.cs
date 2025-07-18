using BgCommon.Localization.IO;
using System.Reflection;

namespace BgCommon.Localization;

/// <summary>
/// 为<see cref=“Localization Builder”/>类提供扩展方法。
/// </summary>
public static partial class LocalizationBuilderExtensions
{
    public static LocalizationBuilder FromYaml(this LocalizationBuilder builder, string path, CultureInfo culture, bool isPublic)
    {
        return builder.FromYaml(Assembly.GetCallingAssembly(), path, culture, isPublic);
    }

    public static LocalizationBuilder FromYaml(this LocalizationBuilder builder, Assembly assembly, string path, CultureInfo culture, bool isPublic)
    {
        if (!(path.EndsWith(".yml") || path.EndsWith(".yaml")))
        {
            throw new ArgumentException(
                $"Parameter {nameof(path)} in {nameof(FromYaml)} must be path to the YAML file."
            );
        }

        string? contents = EmbeddedResourceReader.ReadToEnd(path, assembly);

        if (contents is null)
        {
            throw new LocalizationBuilderException(
                $"Resource {path} not found in assembly {assembly.FullName}."
            );
        }

        IDictionary<string, IDictionary<string, string>> deserializedYaml =
            YamlDictionariesDeserializer.FromString(contents);

        foreach (KeyValuePair<string, IDictionary<string, string>> localizedStrings in deserializedYaml)
        {
            string? name = (localizedStrings.Key is null || localizedStrings.Key == "default")
                    ? default
                    : localizedStrings.Key;

            Dictionary<string, string?> dicCopy = new Dictionary<string, string?>();
            foreach (KeyValuePair<string, string> keyValue in localizedStrings.Value)
            {
                if (isPublic)
                {
                    dicCopy.Add(keyValue.Key, keyValue.Value);
                }
                else
                {
                    dicCopy.Add(
                        $"{assembly.GetName().Name}_{keyValue.Key}",
                        keyValue.Value
                    );
                }
            }

            builder.AddLocalization(
                new LocalizationSet(assembly, name?.ToLowerInvariant(), culture, dicCopy, isPublic)
            );
        }

        return builder;
    }
}