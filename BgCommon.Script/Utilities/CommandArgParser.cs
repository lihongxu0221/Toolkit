namespace BgCommon.Script;

internal static class CommandArgParser
{
    public static string TrimMatchingQuotes(this string input, char quote)
    {
        if (input.Length >= 2)
        {
            // "-sconfig:My Script.cs.config"
            if (input.First() == quote && input.Last() == quote)
            {
                return input.Substring(1, input.Length - 2);
            }

            // -sconfig:"My Script.cs.config"
            else if (input.Last() == quote)
            {
                var firstQuote = input.IndexOf(quote);
                if (firstQuote != input.Length - 1) // not the last one
                {
                    return string.Concat(
                        input.AsSpan(0, firstQuote),
                        input.AsSpan(firstQuote + 1, input.Length - 2 - firstQuote));
                }
            }
        }

        return input;
    }

    public static IEnumerable<string> Split(this string str, Func<char, bool>? controller = null)
    {
        int nextPiece = 0;

        for (int c = 0; c < str.Length; c++)
        {
            if (controller?.Invoke(str[c]) ?? false)
            {
                yield return str.Substring(nextPiece, c - nextPiece);
                nextPiece = c + 1;
            }
        }

        yield return str.Substring(nextPiece);
    }

    public static string[] SplitCommandLine(this string commandLine)
    {
        bool inQuotes = false;
        bool isEscaping = false;

        return commandLine.Split(c =>
        {
            if (c == '\\' && !isEscaping)
            {
                isEscaping = true;
                return false;
            }

            if (c == '\"' && !isEscaping)
            {
                inQuotes = !inQuotes;
            }

            isEscaping = false;

            return !inQuotes && char.IsWhiteSpace(c)/*c == ' '*/;
        })
        .Select(arg => arg.Trim().TrimMatchingQuotes('\"').Replace("\\\"", "\""))
        .Where(arg => !string.IsNullOrEmpty(arg))
        .ToArray();
    }
}
