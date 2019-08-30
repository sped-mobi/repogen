using System;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IOption
    {
        string Name { get; }

        object DefaultValue { get; }

        Type Type { get; }

        bool HasCategory { get; }
    }
}
