using System;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    /// <summary>
    ///  A global option. An instance of this class can be used to access an option value from an OptionSet.
    /// </summary>
    /// <typeparam name="T">The type of option.</typeparam>
    public class Option<T> : IOptionWithInfo
    {
        public Option(string name, T defaultValue, OptionInfo info)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            Name = name;
            DefaultValue = defaultValue;
            Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public Option(string name, T defaultValue)
            : this(name, defaultValue, OptionInfo.Default)
        {
        }

        public Option(string name)
            : this(name, default, OptionInfo.Default)
        {

        }

        /// <summary>
        /// The name of the option.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The default value of the option.
        /// </summary>
        public T DefaultValue { get; }

        /// <summary>
        /// Optional display name and description of the option.
        /// </summary>
        public OptionInfo Info { get; }

        object IOption.DefaultValue
        {
            get
            {
                return DefaultValue;
            }
        }

        /// <summary>
        /// The type of the option default value.
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Determines whether or not this <see cref="Option{T}"/> has an option category associated with it.
        /// </summary>
        public bool HasCategory =>
            false;

        public static implicit operator OptionKey(Option<T> option)
        {
            return new OptionKey(option);
        }
    }
}
