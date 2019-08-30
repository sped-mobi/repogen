// -----------------------------------------------------------------------
// <copyright file="IOptionWithInfo.cs" company="Brad Marshall">
//     Copyright ©  Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public interface IOptionWithInfo : IOption
    {
        OptionInfo Info { get; }
    }
}
