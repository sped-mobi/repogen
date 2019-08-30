// -----------------------------------------------------------------------
// <copyright file="CategorizedOption.cs" company="Brad Marshall">
//     Copyright ©  Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public class CategorizedOption<T> : IOptionWithInfo
    {

        public CategorizedOption(string name, T defaultValue, OptionInfo info)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Info = info ?? throw new ArgumentNullException(nameof(info));
            Name = name;
            DefaultValue = defaultValue;
        }

        public CategorizedOption(string name, T defaultValue) : this(name, defaultValue, OptionInfo.Default)
        {
        }

        public CategorizedOption(string name) : this(name, default, OptionInfo.Default)
        {

        }

        /// <summary>
        /// Information about the option.
        /// </summary>
        public OptionInfo Info { get; }

        /// <summary>
        /// The name of the option.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The default option value.
        /// </summary>
        public T DefaultValue { get; }

        /// <summary>
        /// The type of the option value.
        /// </summary>
        public Type Type => typeof(T);


        object IOption.DefaultValue
        {
            get
            {
                return DefaultValue;
            }
        }

        public bool HasCategory =>
            true;


    }
}
