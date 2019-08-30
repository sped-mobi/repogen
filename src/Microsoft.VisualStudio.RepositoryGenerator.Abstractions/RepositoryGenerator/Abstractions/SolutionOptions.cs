using System.Collections.Generic;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public static class RepositoryOptions
    {
        public static Option<string> RepositoryName { get; }
            = new Option<string>(nameof(RepositoryName), "MyRepository",
                new OptionInfo("Repository Name", "The name of the repository."));

        public static Option<string> OutputDir { get; }
            = new Option<string>(nameof(OutputDir), null,
                new OptionInfo("Output Directory", "The parent directory for the repository."));

        public static Option<string> RepositoryDescription { get; }
            = new Option<string>(nameof(RepositoryDescription), null,
                new OptionInfo("Repository Description", "The description of the repository."));
    }


    public static class SolutionOptions
    {
        public static Option<string> SolutionName { get; }
            = new Option<string>(nameof(SolutionName), "MySolution",
                new OptionInfo("Solution Name", "The name of the solution."));

        public static Option<string> SolutionDir { get; }
            = new Option<string>(nameof(SolutionDir), null,
                new OptionInfo("Solution Directory", "The directory containing the solution file."));

        public static Option<IEnumerable<ProjectOptions>> Projects { get; }
            = new Option<IEnumerable<ProjectOptions>>(nameof(Projects), new List<ProjectOptions>(),
                new OptionInfo("Projects", "The projects in the solution."));
    }


    public class ProjectOptions
    {
        public string ProjectName { get; set; }

        public string ProjectDir { get; set; }

        public string RootNamespace { get; set; }

        public string TargetFramework { get; set; }

        public string ProjectType { get; set; }

        public IList<ScaffoldedFile> Files { get; set; }
    }


    //public static class ProjectOptions
    //{
    //    public static Option<string> ProjectName { get; }
    //        = new Option<string>(nameof(ProjectName), "MyProject",
    //            new OptionInfo("Project Name", "The name of the project"));

    //    public static Option<string> ProjectDir { get; }
    //        = new Option<string>(nameof(ProjectDir), null,
    //            new OptionInfo("Project Directory", "The directory containing the project file."));

    //    public static Option<string> RootNamespace { get; }
    //        = new Option<string>(nameof(RootNamespace), null,
    //            new OptionInfo("Root Namespace", "The root namespace for the project."));

    //    public static Option<string> TargetFramework { get; }
    //        = new Option<string>(nameof(TargetFramework), null,
    //            new OptionInfo("Target Framework", "The .NET Framework for the project."));
    //}


    public static class OptionFactory
    {

        public static OptionSet CreateRepositoryOptions(string repositoryName, string repositoryDescription, string repositoryDir)
        {
            var options = new OptionSet();
            options = options.WithChangedOption(RepositoryOptions.RepositoryName, repositoryName);
            options = options.WithChangedOption(RepositoryOptions.RepositoryDescription, repositoryDescription);
            options = options.WithChangedOption(RepositoryOptions.OutputDir, repositoryDir);
            return options;
        }

        public static OptionSet CreateSolutionOptions(string solutionName, string solutionDir, IEnumerable<ProjectOptions> projects)
        {
            var options = new OptionSet();
            options = options.WithChangedOption(SolutionOptions.SolutionName, solutionName);
            options = options.WithChangedOption(SolutionOptions.SolutionDir, solutionDir);
            options = options.WithChangedOption(SolutionOptions.Projects, projects);
            return options;
        }

        //public static OptionSet CreateProjectOptions(string projectName,string projectDir,string rootNamespace,string targetFramework, IEnumerable<ScaffoldedFile> files)
        //{
        //    var options = new OptionSet();
        //    options = options.WithChangedOption(ProjectOptions.ProjectName, solutionName);
        //    options = options.WithChangedOption(SolutionOptions.SolutionDir, solutionDir);
        //    return options;
        //}

        public static ProjectOptions CreateProjectOptions(
            string projectName,
            string projectDir,
            string rootNamespace,
            string targetFramework,
            string projectType,
            IEnumerable<ScaffoldedFile> files)
        {
            if (string.IsNullOrEmpty(projectName))
            {
                throw new System.ArgumentException("message", nameof(projectName));
            }

            if (string.IsNullOrEmpty(projectDir))
            {
                throw new System.ArgumentException("message", nameof(projectDir));
            }

            if (string.IsNullOrEmpty(rootNamespace))
            {
                throw new System.ArgumentException("message", nameof(rootNamespace));
            }

            if (string.IsNullOrEmpty(targetFramework))
            {
                throw new System.ArgumentException("message", nameof(targetFramework));
            }

            var options = new ProjectOptions();
            options.ProjectName = projectName;
            options.ProjectDir = projectDir;
            options.RootNamespace = rootNamespace;
            options.TargetFramework = targetFramework;
            options.ProjectType = projectType;
            options.Files = new List<ScaffoldedFile>(files);
            return options;
        }
    }
}
