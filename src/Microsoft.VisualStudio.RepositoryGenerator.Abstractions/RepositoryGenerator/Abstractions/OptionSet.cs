using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.RepositoryGenerator.Abstractions
{
    public class OptionSet
    {
        private readonly object _gate = new object();
        private readonly ImmutableDictionary<OptionKey, object> _values;

        public OptionSet()
        {
            _values = ImmutableDictionary.Create<OptionKey, object>();
        }

        public OptionSet(ImmutableDictionary<OptionKey, object> values)
        {
            _values = values;
        }

        public object GetOption(OptionKey optionKey)
        {
            lock (_gate)
            {
                if (_values.TryGetValue(optionKey, out object value)) return value;
                value = optionKey.Option.DefaultValue;
                return _values.Add(optionKey, value);
            }
        }

        public T GetOption<T>(Option<T> option)
        {
            return (T)GetOption(new OptionKey(option));
        }

        public T GetOption<T>(CategorizedOption<T> option, string category)
        {
            return (T)GetOption(new OptionKey(option, category));
        }

        public OptionSet WithChangedOption(OptionKey optionAndCategory, object value)
        {
            GetOption(optionAndCategory);
            lock (_gate)
            {
                return new OptionSet(_values.SetItem(optionAndCategory, value));
            }
        }

        public OptionSet WithChangedOption<T>(Option<T> option, T value)
        {
            return WithChangedOption(new OptionKey(option), value);
        }

        public OptionSet WithChangedOption<T>(CategorizedOption<T> option, string category, T value)
        {
            return WithChangedOption(new OptionKey(option, category), value);
        }

        public IEnumerable<OptionKey> GetAccessedOptions()
        {
            return GetChangedOptions(this);
        }

        public IEnumerable<OptionKey> GetChangedOptions(OptionSet optionSet)
        {
            lock (_gate)
            {
                foreach (var kvp in _values)
                {
                    var currentValue = optionSet.GetOption(kvp.Key);
                    if (!Equals(currentValue, kvp.Value))
                    {
                        yield return kvp.Key;
                    }
                }
            }
        }
    }
}
