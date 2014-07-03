using System;
using System.Drawing;
using RDotNet.Graphics;
using Svg;
using Svg.DataTypes;
using Svg.Transforms;
using Color = System.Drawing.Color;
using Point = RDotNet.Graphics.Point;

namespace WebApplicationRdn.Models
{
    public class SvgCoordinateContext
    {
        public SvgUnit X { get; set; }
        public SvgUnit Y { get; set; }
        public SvgRotate Rotation { get; set; }
    }

    public class SvgPenContext
    {
        public SvgColourServer Stroke { get; set; }
        public SvgUnit StrokeWidth { get; set; }
        public SvgStrokeLineCap StrokeLineCap { get; set; }
        public SvgStrokeLineJoin StrokeLineJoin { get; set; }
        public SvgUnitCollection StrokeDashArray { get; set; }
    }

    public class SvgFontContext
    {
        public string Family { get; set; }
        public float Size { get; set; }
        public SvgFontWeight Weight { get; set; }
        public float CharacterExpansion { get; set; }
        public float LineHeight { get; set; }
    }

    public class SvgGraphicsContext
    {
        public SvgUnitType UnitType { get; set; }
        public SvgCoordinateContext Coordinate { get; set; }
        public SvgColourServer Fill { get; set; }
        public float Opacity { get; set; }
        public SvgFontContext Font { get; set; }
        public SvgPenContext Pen { get; set; }
        public SvgTextAnchor TextAnchor { get; set; }
    }

    public interface ISvgContextMapper
    {
        SvgGraphicsContext MapGraphicsContextToSvg(Point point, double rotation, GraphicsContext context);
        SvgGraphicsContext MapGraphicsContextToSvg(Point point, GraphicsContext context);
        SvgGraphicsContext MapGraphicsContextToSvg(GraphicsContext context);
        SvgCoordinateContext MapPoint(Point point, double rotation, SvgUnitType type);
        SvgCoordinateContext MapPoint(Point point, double rotation);
        SvgCoordinateContext MapPoint(Point point, SvgUnitType unitType);
        SvgCoordinateContext MapPoint(Point point);
        SvgPenContext MapPen(GraphicsContext context);
        SvgFontContext MapFont(GraphicsContext context);

        int Height { get; }
        int Width { get; }
        SvgUnitType UnitType { get; }
        Font Font { get; }
        void SetClipRegion(double x, double y, double width, double height);
    }

    public class SvgContextMapper : ISvgContextMapper
    {
        public const int DefaultHeight = 640;
        public const int DefaultWidth = 640;
        public const SvgUnitType DefaultUnitType = SvgUnitType.Pixel;
        public static readonly Font DefaultFont = new Font("Arial", 12, GraphicsUnit.Point);
        private Point _offset = new Point(0, 0);

        public int Height { get; private set; }
        public int Width { get; private set; }
        public SvgUnitType UnitType { get; private set; }
        public Font Font { get; private set; }

        public SvgContextMapper() :
            this(DefaultHeight, DefaultWidth, DefaultUnitType, DefaultFont)
        { }

        public SvgContextMapper(int height, int width, SvgUnitType unitType, Font font)
        {
            Height = height;
            Width = width;
            UnitType = unitType;
            Font = font;
        }

        public SvgGraphicsContext MapGraphicsContextToSvg(GraphicsContext context)
        {
            return MapGraphicsContextToSvg(new Point(0, 0), 0, context);
        }

        public SvgGraphicsContext MapGraphicsContextToSvg(Point point, GraphicsContext context)
        {
            return MapGraphicsContextToSvg(point, 0, context);
        }

        public SvgGraphicsContext MapGraphicsContextToSvg(Point point, double rotation, GraphicsContext context)
        {
            var svgContext = new SvgGraphicsContext
            {
                Coordinate = MapPoint(point, rotation),
                Pen = MapPen(context),
                Font = MapFont(context),
                UnitType = UnitType,
                Fill = new SvgColourServer { Colour = ConvertRDotNetColor(context.Background) },
                Opacity = (float)(1 - context.Gamma),
                TextAnchor = SvgTextAnchor.Middle
            };

            return svgContext;
        }

        public SvgPenContext MapPen(GraphicsContext context)
        {
            var pen = new SvgPenContext
            {
                Stroke = new SvgColourServer { Colour = ConvertRDotNetColor(context.Foreground) },
                StrokeWidth = new SvgUnit(UnitType, (float)context.LineWidth),
                StrokeLineCap = MapLineEnd(context.LineEnd),
                StrokeLineJoin = MapLineJoin(context.LineJoin),
                StrokeDashArray = MapLineType(context.LineType),
            };

            return pen;
        }

        public SvgCoordinateContext MapPoint(Point point)
        {
            return MapPoint(point, 0, UnitType);
        }

        public SvgCoordinateContext MapPoint(Point point, double rotation)
        {
            return MapPoint(point, rotation, UnitType);
        }

        public SvgCoordinateContext MapPoint(Point point, SvgUnitType unitType)
        {
            return MapPoint(point, 0, unitType);
        }

        public void SetClipRegion(double x, double y, double width, double height)
        {
            _offset.X = x;
            _offset.Y = y;
        }

        public SvgCoordinateContext MapPoint(Point point, double rotation, SvgUnitType unitType)
        {
            if (point.Y < 0)
            {
                point.Y = Math.Abs(point.Y) - _offset.X;
            }
            else
            {
                point.Y = Height - point.Y;
            }

            var svgStartX = new SvgUnit(unitType, (float)point.X);
            var svgStartY = new SvgUnit(unitType, (float)point.Y);
            var context = new SvgCoordinateContext
            {
                X = svgStartX,
                Y = svgStartY,
                Rotation = new SvgRotate((float)-rotation, svgStartX, svgStartY)
            };

            return context;
        }

        public SvgFontContext MapFont(GraphicsContext context)
        {
            var family = !string.IsNullOrEmpty(context.FontFamily) ? context.FontFamily : DefaultFont.FontFamily.Name;
            var font = new SvgFontContext
            {
                Family = family,
                Size = (float)context.FontSizeInPoints,
                Weight = MapFontWeight(context.FontFace),
                CharacterExpansion = (float)context.CharacterExpansion,
                LineHeight = (float)context.LineHeight
            };

            return font;
        }

        private static SvgStrokeLineCap MapLineEnd(LineEnd lineEnd)
        {
            switch (lineEnd)
            {
                case LineEnd.Butt:
                    return SvgStrokeLineCap.Butt;
                case LineEnd.Round:
                    return SvgStrokeLineCap.Round;
                default:
                    return SvgStrokeLineCap.Square;
            }
        }

        private static SvgStrokeLineJoin MapLineJoin(LineJoin lineJoin)
        {
            switch (lineJoin)
            {
                case LineJoin.Beveled:
                    return SvgStrokeLineJoin.Bevel;
                case LineJoin.Miter:
                    return SvgStrokeLineJoin.Miter;
                default:
                    return SvgStrokeLineJoin.Round;
            }
        }

        private static SvgUnitCollection MapLineType(LineType lineType)
        {
            var results = new SvgUnitCollection();

            switch (lineType)
            {
                case LineType.Dashed:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 5), new SvgUnit(SvgUnitType.User, 5) });
                    break;
                case LineType.DotDash:
                    results.AddRange(new[]
                    {
                        new SvgUnit( SvgUnitType.User, 5 ),
                        new SvgUnit( SvgUnitType.User, 5 ),
                        new SvgUnit( SvgUnitType.User, 1 ),
                        new SvgUnit( SvgUnitType.User, 5 )
                    });
                    break;
                case LineType.Dotted:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 1), new SvgUnit(SvgUnitType.User, 5) });
                    break;
                case LineType.LongDash:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 10), new SvgUnit(SvgUnitType.User, 5) });
                    break;
                case LineType.TwoDash:
                    results.AddRange(new[] { new SvgUnit(SvgUnitType.User, 1), new SvgUnit(SvgUnitType.User, 5) });
                    break;
            }

            return results;
        }

        private static SvgFontWeight MapFontWeight(FontFace face)
        {
            switch (face)
            {
                case FontFace.Bold:
                    return SvgFontWeight.bold;
                default:
                    return SvgFontWeight.normal;
            }
        }

        private static Color ConvertRDotNetColor(RDotNet.Graphics.Color color)
        {
            return Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }
    }
}
