// -----------------------------------------------------------------------
// <copyright file="OptionInfo.cs" company="Brad Marshall">
//     Copyright ©  Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public sealed class OptionInfo
    {
        /// <summary>
        /// An <see cref="OptionInfo"/> with empty strings for its DisplayName & Description properties.
        /// </summary>
        public static readonly OptionInfo Default = new OptionInfo(string.Empty, string.Empty);


        /// <summary>
        /// Initializes a new instance of the <see cref="OptionInfo"/> class.
        /// </summary>
        /// <param name="displayName">The display name for the option.</param>
        /// <param name="description">A short description of the significance of the option.</param>
        public OptionInfo(string displayName, string description)
        {
            DisplayName = displayName;
            Description = description;
        }

        /// <summary>
        /// The display name for the option.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// A short description of the significance of the option.
        /// </summary>
        public string Description { get; }
    }
}
