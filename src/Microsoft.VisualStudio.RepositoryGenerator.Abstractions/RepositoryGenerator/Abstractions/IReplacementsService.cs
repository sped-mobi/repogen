using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IReplacementsService
    {

        IDictionary<string, string> GetReplacements(OptionSet options);

        string Replace(string code, IDictionary<string, string> replacements);
    }
}
