﻿namespace AngleSharp.Css.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using AngleSharp.Css.Values;
    using AngleSharp.Extensions;
    using AngleSharp.Parser.Css;
    using AngleSharp.Dom.Css;

    abstract class GradientConverter : IValueConverter
    {
        readonly Boolean _repeating;

        public GradientConverter(Boolean repeating)
        {
            _repeating = repeating;
        }

        public static IPropertyValue[] ToGradientStops(List<List<CssToken>> values, Int32 offset)
        {
            var stops = new IPropertyValue[values.Count - offset];

            for (int i = offset, k = 0; i < values.Count; i++, k++)
            {
                stops[k] = ToGradientStop(values[i]);

                if (stops[k] == null)
                    return null;
            }

            return stops;
        }

        public static IPropertyValue ToGradientStop(List<CssToken> value)
        {
            var color = default(IPropertyValue);
            var position = default(IPropertyValue);
            var items = value.ToItems();

            if (items.Count != 0)
            {
                position = Converters.LengthOrPercentConverter.Convert(items[items.Count - 1]);

                if (position != null)
                    items.RemoveAt(items.Count - 1);
            }

            if (items.Count != 0)
            {
                color = Converters.ColorConverter.Convert(items[items.Count - 1]);

                if (color != null)
                    items.RemoveAt(items.Count - 1);
            }

            return items.Count == 0 ? new StopValue(color, position, value) : null;
        }

        public IPropertyValue Convert(IEnumerable<CssToken> value)
        {
            var args = value.ToList();
            var initial = args.Count != 0 ? ConvertFirstArgument(args[0]) : null;
            var offset = initial != null ? 1 : 0;
            var stops = ToGradientStops(args, offset);
            return stops != null ? new GradientValue(_repeating, initial, stops, value) : null;
        }

        protected abstract IPropertyValue ConvertFirstArgument(IEnumerable<CssToken> value);

        sealed class StopValue : IPropertyValue
        {
            readonly IPropertyValue _color;
            readonly IPropertyValue _position;
            readonly CssValue _original;

            public StopValue(IPropertyValue color, IPropertyValue position, IEnumerable<CssToken> tokens)
            {
                _color = color;
                _position = position;
                _original = new CssValue(tokens);
            }

            public String CssText
            {
                get 
                {
                    if (_color == null && _position != null)
                        return _position.CssText;

                    if (_color != null && _position == null)
                        return _color.CssText;

                    return String.Concat(_color.CssText, " ", _position.CssText); 
                }
            }

            public CssValue Original
            {
                get { return _original; }
            }

            public CssValue ExtractFor(String name)
            {
                return _original;
            }
        }

        sealed class GradientValue : IPropertyValue
        {
            readonly Boolean _repeating;
            readonly IPropertyValue _initial;
            readonly IPropertyValue[] _stops;
            readonly CssValue _original;

            public GradientValue(Boolean repeating, IPropertyValue initial, IPropertyValue[] stops, IEnumerable<CssToken> tokens)
            {
                _repeating = repeating;
                _initial = initial;
                _stops = stops;
                _original = new CssValue(tokens);
            }

            public String CssText
            {
                get
                {
                    var count = _stops.Length;

                    if (_initial != null)
                        count++;

                    var args = new String[count];
                    count = 0;

                    if (_initial != null)
                        args[count++] = _initial.CssText;

                    for (int i = 0; i < _stops.Length; i++)
                        args[count++] = _stops[i].CssText;

                    return String.Join(", ", args);
                }
            }

            public CssValue Original
            {
                get { return _original; }
            }

            public CssValue ExtractFor(String name)
            {
                return _original;
            }
        }
    }

    sealed class LinearGradientConverter : GradientConverter
    {
        readonly IValueConverter _converter;

        public LinearGradientConverter(Boolean repeating)
            : base(repeating)
        {
            _converter = Converters.AngleConverter.Or(
                Converters.SideOrCornerConverter.StartsWithKeyword(Keywords.To));
        }

        protected override IPropertyValue ConvertFirstArgument(IEnumerable<CssToken> value)
        {
            return _converter.Convert(value);
        }
    }

    sealed class RadialGradientConverter : GradientConverter
    {
        readonly IValueConverter _converter;

        public RadialGradientConverter(Boolean repeating)
            : base(repeating)
        {
            var position = Converters.PointConverter.StartsWithKeyword(Keywords.At).Option(Point.Center);
            var circle = Converters.WithOrder(Converters.WithAny(Converters.Assign(Keywords.Circle, true).Option(true), Converters.LengthConverter.Option()), position);
            var ellipse = Converters.WithOrder(Converters.WithAny(Converters.Assign(Keywords.Ellipse, false).Option(false), Converters.LengthOrPercentConverter.Many(2, 2).Option()), position);
            var extents = Converters.WithOrder(Converters.WithAny(Converters.Toggle(Keywords.Circle, Keywords.Ellipse).Option(false), Map.RadialGradientSizeModes.ToConverter()), position);

            _converter = circle.Or(ellipse.Or(extents));
        }

        protected override IPropertyValue ConvertFirstArgument(IEnumerable<CssToken> value)
        {
            return _converter.Convert(value);
        }
    }
}
