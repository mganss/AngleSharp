﻿namespace AngleSharp.Css.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using AngleSharp.Extensions;
    using AngleSharp.Parser.Css;
    using AngleSharp.Dom.Css;

    sealed class AnyValueConverter : IValueConverter
    {
        public IPropertyValue Convert(IEnumerable<CssToken> value)
        {
            return new AnyValue(value);
        }

        sealed class AnyValue : IPropertyValue
        {
            readonly CssValue _value;

            public AnyValue(IEnumerable<CssToken> tokens)
            {
                _value = new CssValue(tokens);
            }

            public String CssText
            {
                get { return _value.ToText(); }
            }

            public CssValue Original
            {
                get { return _value; }
            }

            public CssValue ExtractFor(String name)
            {
                return _value;
            }
        }
    }
}
