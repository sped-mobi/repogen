// -----------------------------------------------------------------------
// <copyright file="OptionKey.cs" company="Brad Marshall">
//     Copyright ©  Brad Marshall. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public struct OptionKey : IEquatable<OptionKey>
    {

        public OptionKey(IOption option, string category = null)
        {
            Option = option ?? throw new ArgumentNullException(nameof(option));
            Category = category;
        }

        public IOption Option { get; }

        public string Category { get; }

        public override bool Equals(object obj)
        {
            return obj is OptionKey key &&
                   Equals(key);
        }

        public bool Equals(OptionKey other)
        {
            return Option.Equals(other.Option) && Category == other.Category;
        }

        public override int GetHashCode()
        {
            var hash = Option.GetHashCode();

            if (Category != null)
            {
                hash = unchecked(hash * (int)0xA5555529 + Category.GetHashCode());
            }

            return hash;
        }

        public static bool operator ==(OptionKey left, OptionKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OptionKey left, OptionKey right)
        {
            return !left.Equals(right);
        }
    }
}
