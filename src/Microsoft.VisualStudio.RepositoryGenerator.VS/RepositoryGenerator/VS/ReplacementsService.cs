using System.Collections.Generic;
using Microsoft.VisualStudio.RepositoryGenerator.Abstractions;

namespace Microsoft.VisualStudio.RepositoryGenerator.VS
{
    public class ReplacementsService : IReplacementsService
    {

        public IDictionary<string, string> GetReplacements(OptionSet options)
        {
            string repositoryName = options.GetOption(RepositoryOptions.RepositoryName);
            string repositoryDescription = options.GetOption(RepositoryOptions.RepositoryDescription);
            string solutionName = options.GetOption(SolutionOptions.SolutionName);
            return new Dictionary<string, string>
            {
                ["{{name}}"] = solutionName,
                ["{{repositoryName}}"] = repositoryName,
                ["{{description}}"] = repositoryDescription
            };
        }

        public string Replace(string code, IDictionary<string, string> replacements)
        {
            string retVal = code;
            foreach (var kvp in replacements)
            {
                retVal = retVal.Replace(kvp.Key, kvp.Value);
            }
            return retVal;
        }
    }
}
