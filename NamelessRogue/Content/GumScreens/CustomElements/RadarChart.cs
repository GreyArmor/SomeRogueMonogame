using Gum.GueDeriving;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vector2 = System.Numerics.Vector2;

//partially vibecoded
public class RadarChart : ContainerRuntime
{
	readonly string[] _labels;
	readonly List<TextRuntime> _labelTexts = new();
	readonly List<TextRuntime> _valueLabels = new();
	readonly float _radius;
	readonly float _max;
	readonly PolygonRuntime _dataShape;

	readonly TextRuntime _title;
	readonly float _inner;
	public RadarChart(string title, string[] labels, float radius = 120, float max = 100,
				  int rings = 4, float baseline = 20)
	{
		_labels = labels;
		_radius = radius;
		_max = max;
		_inner = radius * baseline / max;      // radius where value 0 sits

		// Container is a square centered on the chart
		Width = (radius + 50) * 2;
		Height = (radius + 50) * 2 + 30;
		WidthUnits = HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;

		Y += 15;

		// Grid rings: ring 0 is the zero ring, the last ring is the max ring
		for (int r = 0; r <= rings; r++)
			AddPolygon(Ring(_inner + (radius - _inner) * r / rings),
					   new Color(255, 255, 255, 70));
		// Axes + labels
		for (int i = 0; i < labels.Length; i++)
		{
			AddPolygon(new List<Vector2> { Point(i, _inner), Point(i, radius) },
					   new Color(255, 255, 255, 70));

			var label = new TextRuntime { Text = labels[i], Color = Color.White };
			label.XUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
			label.YUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
			label.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center;
			label.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;
			var p = Point(i, radius + 22);
			label.X = p.X; label.Y = p.Y;
			Children.Add(label);
			_labelTexts.Add(label);

			var valueLabel = new TextRuntime { Text = labels[i], Color = Color.White };
			valueLabel.XUnits = Gum.Converters.GeneralUnitType.Percentage;
			valueLabel.YUnits = Gum.Converters.GeneralUnitType.Percentage;
			valueLabel.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center;
			valueLabel.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Bottom;
			valueLabel.X = 50;
			valueLabel.Y = 0;
			label.Children.Add(valueLabel);
			
			_valueLabels.Add(label);
		}

		// Data polygon (updated by SetValues)
		_dataShape = AddPolygon(Ring(_inner), new Color(80, 170, 255));


		_title = new TextRuntime { Text = title, Color = Color.White };
		_title.XUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
		_title.YUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
		_title.XOrigin = RenderingLibrary.Graphics.HorizontalAlignment.Center;
		_title.YOrigin = RenderingLibrary.Graphics.VerticalAlignment.Center;
		_title.Y = -(radius + 65);     // above the top axis label (which sits near radius + 22, two lines tall)
		Children.Add(_title);
	}
	// Axis 0 points straight up, going clockwise
	Vector2 Point(int i, float r)
	{
		float a = -MathHelper.PiOver2 + MathHelper.TwoPi * i / _labels.Length;
		return new Vector2(MathF.Cos(a), MathF.Sin(a)) * r;
	}

	List<Vector2> Ring(float r)
	{
		var pts = new List<Vector2>();
		for (int i = 0; i < _labels.Length; i++) pts.Add(Point(i, r));
		return pts;
	}
	// Value -> distance from center
	float RadiusFor(float value) =>
		_inner + (_radius - _inner) * MathHelper.Clamp(value / _max, 0, 1);
	public void SetValues(float[] values)
	{
		var pts = new List<Vector2>();
		for (int i = 0; i < values.Length; i++)
		{
			pts.Add(Point(i, RadiusFor(values[i])));
			_valueLabels[i].Text = $"{values[i]:0}";
		}

		pts.Add(pts[0]);
		_dataShape.SetPoints(pts);
	}

	PolygonRuntime AddPolygon(List<Vector2> pts, Color color)
	{
		var poly = new PolygonRuntime();
		poly.XUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
		poly.YUnits = Gum.Converters.GeneralUnitType.PixelsFromMiddle;
		poly.Color = color;
		pts = new List<Vector2>(pts) { pts[0] };
		poly.SetPoints(pts);
		Children.Add(poly);
		return poly;
	}
}