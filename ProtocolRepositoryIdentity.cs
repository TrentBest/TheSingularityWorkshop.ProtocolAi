namespace TheSingularityWorkshop.ProtocolAi;

/// <summary>Validates a GitHub-style owner/repository identity used in provenance contracts.</summary>
internal static class ProtocolRepositoryIdentity
{
    internal static string Validate(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 256)
            throw new ArgumentException("A repository owner/name identity is required.", parameter);

        var separator = value.IndexOf('/');
        if (separator <= 0 || separator != value.LastIndexOf('/') || separator == value.Length - 1)
            throw new ArgumentException("Repository must use canonical owner/name form.", parameter);

        var owner = value.AsSpan(0, separator);
        var name = value.AsSpan(separator + 1);

        if (owner.Length > 39 || !IsAsciiAlphaNumeric(owner[0]) || !IsAsciiAlphaNumeric(owner[^1]))
            throw new ArgumentException("Repository owner must be a valid GitHub owner name.", parameter);

        foreach (var character in owner)
            if (!IsAsciiAlphaNumeric(character) && character != '-')
                throw new ArgumentException("Repository owner contains an invalid character.", parameter);

        if (name.Length > 100 || name[0] == '.' || name[^1] == '.')
            throw new ArgumentException("Repository name must be a valid GitHub repository name.", parameter);

        foreach (var character in name)
            if (!IsAsciiAlphaNumeric(character) && character is not ('-' or '_' or '.'))
                throw new ArgumentException("Repository name contains an invalid character.", parameter);

        return value;
    }

    private static bool IsAsciiAlphaNumeric(char value) =>
        value is >= 'a' and <= 'z'
            or >= 'A' and <= 'Z'
            or >= '0' and <= '9';
}
