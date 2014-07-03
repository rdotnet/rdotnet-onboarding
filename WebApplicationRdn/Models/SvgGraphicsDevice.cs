using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RDotNet.Graphics;
using Svg;
using Svg.Pathing;
using Svg.Transforms;
using Point = RDotNet.Graphics.Point;
using Rectangle = RDotNet.Graphics.Rectangle;

namespace WebApplicationRdn.Models
{
    public class SvgGraphicsDevice : IGraphicsDevice
    {
        private readonly ISvgContextMapper _mapper;
        private SvgDocument _currentImage;
        private readonly List<SvgDocument> _images = new List<SvgDocument>();

        public SvgGraphicsDevice(ISvgContextMapper mapper)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            _mapper = mapper;
            Name = "SvgGraphicsDevice";
        }

        public string Name { get; private set; }

        public Rectangle GetSize(GraphicsContext context, DeviceDescription description)
        {
            return description.ClipBounds;
        }

        public bool ConfirmNewFrame(DeviceDescription description)
        {
            return true;
        }

        public void DrawCircle(Point center, double radius, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(center, context);
            AddChild(new SvgCircle
            {
                CenterX = svgContext.Coordinate.X,
                CenterY = svgContext.Coordinate.Y,
                Fill = svgContext.Fill,
                FillOpacity = svgContext.Opacity,
                Radius = new SvgUnit(svgContext.UnitType, (float)(radius)),
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void Clip(Rectangle rectangle, DeviceDescription description)
        {
            const float epsilon = 0.10f; // A sub-pixel is close enough.
            if (Math.Abs(rectangle.Width - _mapper.Width) < epsilon && Math.Abs(rectangle.Height - _mapper.Height) < epsilon) return;

            _mapper.SetClipRegion(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public Point? GetLocation(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Point source, Point destination, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(source, context);
            var end = _mapper.MapPoint(destination, 0d);
            AddChild(new SvgLine
            {
                EndX = end.X,
                EndY = end.Y,
                Fill = svgContext.Fill,
                FillOpacity = svgContext.Opacity,
                StartX = svgContext.Coordinate.X,
                StartY = svgContext.Coordinate.Y,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public MetricsInfo GetMetricInfo(int character, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(new Point(), 0, context);
            var s = new string(new[] { (char)character });
            var family = new FontFamily(svgContext.Font.Family);
            var style = GetStyle(context.FontFace);

            return new MetricsInfo
            {
                Ascent = family.GetCellAscent(style),
                Descent = family.GetCellDescent(style),
                Width = MeasureWidth(s, context, description),
            };
        }

        public void DrawPolygon(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(context);
            var collection = new SvgUnitCollection();
            collection.AddRange(points.Select(p => _mapper.MapPoint(p))
                                      .Select(p => new[] { p.X, p.Y })
                                      .SelectMany(p => p));
            AddChild(new SvgPolygon
            {
                Fill = svgContext.Fill,
                FillOpacity = svgContext.Opacity,
                Points = collection,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void DrawPolyline(IEnumerable<Point> points, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(context);
            var collection = new SvgUnitCollection();

            collection.AddRange(points.Select(p => _mapper.MapPoint(p, SvgUnitType.User))
                                      .Select(p => new[] { p.X, p.Y })
                                      .SelectMany(p => p));

            AddChild(new SvgPolyline
            {
                Fill = svgContext.Fill,
                FillOpacity = svgContext.Opacity,
                Points = collection,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeWidth = svgContext.Pen.StrokeWidth
            });
        }

        public void DrawRectangle(Rectangle rectangle, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(new Point(rectangle.Left, rectangle.Top), context);
            AddChild(new SvgRectangle
            {
                Fill = svgContext.Fill,
                FillOpacity = svgContext.Opacity,
                Height = (float)rectangle.Height,
                Stroke = svgContext.Pen.Stroke,
                StrokeDashArray = svgContext.Pen.StrokeDashArray,
                StrokeLineCap = svgContext.Pen.StrokeLineCap,
                StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                StrokeWidth = svgContext.Pen.StrokeWidth,
                Width = (float)rectangle.Width,
                X = svgContext.Coordinate.X,
                Y = svgContext.Coordinate.Y,
            });
        }

        public void DrawPath(IEnumerable<IEnumerable<Point>> points, bool winding, GraphicsContext context,
            DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(context);
            foreach (var point in points)
            {
                var vertices = point.ToList();
                var list = new SvgPathSegmentList();
                var first = vertices.First();
                list.Add(new SvgMoveToSegment(new PointF((float)first.X, (float)first.Y)));

                foreach (var vertex in vertices.Skip(1))
                {
                    list.Add(new SvgLineSegment(new PointF((float)first.X, (float)first.Y),
                                                  new PointF((float)vertex.X, (float)vertex.Y)));
                    first = vertex;
                }

                list.Add(new SvgClosePathSegment());
                AddChild(new SvgPath
                {
                    Fill = svgContext.Fill,
                    FillOpacity = svgContext.Opacity,
                    PathData = list,
                    Stroke = svgContext.Pen.Stroke,
                    StrokeDashArray = svgContext.Pen.StrokeDashArray,
                    StrokeLineCap = svgContext.Pen.StrokeLineCap,
                    StrokeLineJoin = svgContext.Pen.StrokeLineJoin,
                    StrokeWidth = svgContext.Pen.StrokeWidth
                });
            }
        }

        public void DrawRaster(Raster raster, Rectangle destination, double rotation, bool interpolated, GraphicsContext context,
            DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public Raster Capture(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public double MeasureWidth(string s, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(context);
            var text = new SvgText
            {
                FontFamily = svgContext.Font.Family,
                FontSize = svgContext.Font.Size,
                FontWeight = svgContext.Font.Weight,
                Text = s,
                TextAnchor = svgContext.TextAnchor,
                X = 0f,
                Y = 0f,
            };
            return text.Bounds.Width;
        }

        public void DrawText(string s, Point location, double rotation, double adjustment, GraphicsContext context, DeviceDescription description)
        {
            var svgContext = _mapper.MapGraphicsContextToSvg(location, rotation, context);
            AddChild(new SvgText
            {
                Fill = svgContext.Pen.Stroke,
                FontFamily = svgContext.Font.Family,
                FontSize = svgContext.Font.Size,
                FontWeight = svgContext.Font.Weight,
                Text = s,
                TextAnchor = svgContext.TextAnchor,
                Transforms = new SvgTransformCollection { svgContext.Coordinate.Rotation },
                X = svgContext.Coordinate.X,
                Y = svgContext.Coordinate.Y,
            });
        }

        public void OnActivated(DeviceDescription description)
        {
            var rectangle = new Rectangle(0, 0, _mapper.Height, _mapper.Width);
            description.Bounds = rectangle;
            description.ClipBounds = rectangle;
        }

        public void OnDeactivated(DeviceDescription description)
        { }

        public void OnNewPageRequested(GraphicsContext context, DeviceDescription description)
        {
            _currentImage = new SvgDocument
            {
                Width = new SvgUnit(SvgUnitType.Pixel, _mapper.Height),
                Height = new SvgUnit(SvgUnitType.Pixel, _mapper.Width)
            };

            _images.Add(_currentImage);
        }

        public Rectangle OnResized(DeviceDescription description)
        {
            throw new NotImplementedException();
        }

        public void OnClosed(DeviceDescription description)
        { }

        public void OnDrawStarted(DeviceDescription description)
        { }

        public void OnDrawStopped(DeviceDescription description)
        { }

        public IEnumerable<SvgDocument> GetImages()
        {
            return _images;
        }

        public void ClearImages()
        {
            _currentImage = null;
            _images.Clear();
        }

        private void AddChild(SvgElement element)
        {
            var group = _currentImage.Children.GetSvgElementOf<SvgGroup>();
            if (group == null)
            {
                group = new SvgGroup();
                _currentImage.Children.Add(group);
            }

            group.Children.Add(element);
        }

        private static FontStyle GetStyle(FontFace face)
        {
            switch (face)
            {
                case FontFace.Bold:
                    return FontStyle.Bold;

                case FontFace.Italic:
                    return FontStyle.Italic;

                case FontFace.BoldItalic:
                    return FontStyle.Bold | FontStyle.Italic;

                default:
                    return FontStyle.Regular;
            }
        }
    }
}
