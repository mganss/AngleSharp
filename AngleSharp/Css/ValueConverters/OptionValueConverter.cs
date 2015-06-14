﻿namespace AngleSharp.Css.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AngleSharp.Parser.Css;
    using AngleSharp.Dom.Css;

    sealed class OptionValueConverter : IValueConverter
    {
        readonly IValueConverter _converter;

        public OptionValueConverter(IValueConverter converter)
        {
            _converter = converter;
        }

        public IPropertyValue Convert(IEnumerable<CssToken> value)
        {
            return value.Any() ? _converter.Convert(value) : new OptionValue(value);
        }

        sealed class OptionValue : IPropertyValue
        {
            readonly CssValue _original;

            public OptionValue(IEnumerable<CssToken> tokens)
            {
                _original = new CssValue(tokens);
            }

            public String CssText
            {
                get { return String.Empty; }
            }

            public CssValue Original
            {
                get { return _original; }
            }

            public CssValue ExtractFor(String name)
            {
                return null;
            }
        }
    }

    sealed class OptionValueConverter<T> : IValueConverter
    {
        readonly IValueConverter _converter;
        readonly T _defaultValue;

        public OptionValueConverter(IValueConverter converter, T defaultValue)
        {
            _converter = converter;
            _defaultValue = defaultValue;
        }

        public IPropertyValue Convert(IEnumerable<CssToken> value)
        {
            return value.Any() ? _converter.Convert(value) : new OptionValue(_defaultValue, value);
        }

        sealed class OptionValue : IPropertyValue
        {
            readonly T _value;
            readonly CssValue _original;

            public OptionValue(T value, IEnumerable<CssToken> tokens)
            {
                _value = value;
                _original = new CssValue(tokens);
            }

            public String CssText
            {
                get { return String.Empty; }
            }

            public CssValue Original
            {
                get { return _original; }
            }

            public CssValue ExtractFor(String name)
            {
                return null;
            }
        }
    }
}
