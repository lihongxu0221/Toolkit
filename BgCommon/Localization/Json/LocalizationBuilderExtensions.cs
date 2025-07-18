using BgCommon.Localization.IO;
using BgCommon.Localization.Json.Converters;
using BgCommon.Localization.Json.Models;
using BgCommon.Localization.Json.Models.v1;

namespace BgCommon.Localization;

/// <summary>
/// 为<see cref=“Localization Builder”/>类提供扩展方法。<br/>
/// 使用Json 不支持多语言包资源重复的键值对。
/// </summary>
public static partial class LocalizationBuilderExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions =
        new()
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            Converters = { new TranslationsContainerConverter() }
        };

    public static LocalizationBuilder FromJsonString(
        this LocalizationBuilder builder,
        string jsonString,
        CultureInfo culture
    )
    {
        return builder.FromJsonString(jsonString, default, culture);
    }

    public static LocalizationBuilder FromJsonString(
        this LocalizationBuilder builder,
        string jsonString,
        string? baseName,
        CultureInfo culture
    )
    {
        builder.AddLocalization(
            new LocalizationSet(null, baseName, culture, ComputeLocalizationPairs(jsonString), true)
        );
        return builder;
    }

    public static LocalizationBuilder FromJson(this LocalizationBuilder builder, string path, CultureInfo culture)
    {
        return builder.FromJson(Assembly.GetCallingAssembly(), path, culture);
    }

    public static LocalizationBuilder FromJson(this LocalizationBuilder builder, Assembly assembly, string path, CultureInfo culture)
    {
        if (!path.EndsWith(".json"))
        {
            throw new ArgumentException(
                $"Parameter {nameof(path)} in {nameof(FromJson)} must be path to the JSON file."
            );
        }

        string? contents = EmbeddedResourceReader.ReadToEnd(path, assembly);

        builder.AddLocalization(
            new LocalizationSet(
                null,
                Path.GetFileNameWithoutExtension(path).Trim().ToLowerInvariant(),
                culture,
                ComputeLocalizationPairs(contents),
                true
            )
        );

        return builder;
    }

    private static IEnumerable<KeyValuePair<string, string?>> ComputeLocalizationPairs(
        string? contents
    )
    {
        if (contents is null)
        {
            throw new ArgumentNullException(nameof(contents));
        }

        Version schemaVersion =
            new(
                JsonSerializer
                    .Deserialize<ITranslationsContainer>(contents, DefaultJsonSerializerOptions)
                    ?.Version ?? "1.0.0"
            );

        if (!schemaVersion.Major.Equals(1))
        {
            throw new LocalizationBuilderException(
                $"Localization file with schema version \"{schemaVersion.ToString() ?? "unknown"}\" is not supported."
            );
        }

        TranslationFile? translationFile = JsonSerializer.Deserialize<TranslationFile>(
            contents,
            DefaultJsonSerializerOptions
        );

        if (translationFile is null)
        {
            throw new LocalizationBuilderException("Unable to extract data from json file.");
        }

        Dictionary<string, string> localizedStrings = new();

        foreach (TranslationEntity localizedString in translationFile.Strings)
        {
            if (localizedStrings.ContainsKey(localizedString.Name))
            {
                throw new LocalizationBuilderException(
                    $"The contents of the JSON file contains duplicate \"{localizedString.Name}\" keys."
                );
            }

            localizedStrings.Add(localizedString.Name, localizedString.Value);
        }

        return localizedStrings!;
    }
}