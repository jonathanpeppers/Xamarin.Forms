using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	[DebuggerDisplay("R={R}, G={G}, B={B}, A={A}, Hue={Hue}, Saturation={Saturation}, Luminosity={Luminosity}")]
	[TypeConverter(typeof(ColorTypeConverter))]
	public struct Color
	{
		readonly Mode _mode;

		enum Mode
		{
			Default,
			Rgb,
			Hsl
		}

		public static Color Default
		{
			get { return new Color(-1d, -1d, -1d, -1d, Mode.Default); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDefault
		{
			get { return _mode == Mode.Default; }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetAccent(Color value) => Accent = value;
		public static Color Accent { get; internal set; }

		readonly float _a;

		public double A
		{
			get { return _a; }
		}

		readonly float _r;

		public double R
		{
			get { return _r; }
		}

		readonly float _g;

		public double G
		{
			get { return _g; }
		}

		readonly float _b;

		public double B
		{
			get { return _b; }
		}

		readonly float _hue;

		public double Hue
		{
			get { return _hue; }
		}

		readonly float _saturation;

		public double Saturation
		{
			get { return _saturation; }
		}

		readonly float _luminosity;

		public double Luminosity
		{
			get { return _luminosity; }
		}

		public Color(double r, double g, double b, double a) : this(r, g, b, a, Mode.Rgb)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Color(double w, double x, double y, double z, Mode mode)
		{
			_mode = mode;
			switch (mode)
			{
				default:
				case Mode.Default:
					_r = _g = _b = _a = -1;
					_hue = _saturation = _luminosity = -1;
					break;
				case Mode.Rgb:
					_r = (float)w.Clamp(0, 1);
					_g = (float)x.Clamp(0, 1);
					_b = (float)y.Clamp(0, 1);
					_a = (float)z.Clamp(0, 1);
					ConvertToHsl(_r, _g, _b, mode, out _hue, out _saturation, out _luminosity);
					break;
				case Mode.Hsl:
					_hue = (float)w.Clamp(0, 1);
					_saturation = (float)x.Clamp(0, 1);
					_luminosity = (float)y.Clamp(0, 1);
					_a = (float)z.Clamp(0, 1);
					ConvertToRgb(_hue, _saturation, _luminosity, mode, out _r, out _g, out _b);
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Color(float r, float g, float b)
		{
			_r = r;
			_g = g;
			_b = b;
			_a = 1;
			_mode = Mode.Rgb;
			ConvertToHsl(_r, _g, _b, _mode, out _hue, out _saturation, out _luminosity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Color(float r, float g, float b, float a)
		{
			_r = r;
			_g = g;
			_b = b;
			_a = a;
			_mode = Mode.Rgb;
			ConvertToHsl(_r, _g, _b, _mode, out _hue, out _saturation, out _luminosity);
		}

		public Color(double r, double g, double b) : this(r, g, b, 1)
		{
		}

		public Color(double value) : this(value, value, value, 1)
		{
		}

		public Color MultiplyAlpha(double alpha)
		{
			switch (_mode)
			{
				default:
				case Mode.Default:
					throw new InvalidOperationException("Invalid on Color.Default");
				case Mode.Rgb:
					return new Color(_r, _g, _b, _a * alpha, Mode.Rgb);
				case Mode.Hsl:
					return new Color(_hue, _saturation, _luminosity, _a * alpha, Mode.Hsl);
			}
		}

		public Color AddLuminosity(double delta)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");

			return new Color(_hue, _saturation, _luminosity + delta, _a, Mode.Hsl);
		}

		public Color WithHue(double hue)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");
			return new Color(hue, _saturation, _luminosity, _a, Mode.Hsl);
		}

		public Color WithSaturation(double saturation)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");
			return new Color(_hue, saturation, _luminosity, _a, Mode.Hsl);
		}

		public Color WithLuminosity(double luminosity)
		{
			if (_mode == Mode.Default)
				throw new InvalidOperationException("Invalid on Color.Default");
			return new Color(_hue, _saturation, luminosity, _a, Mode.Hsl);
		}

		static void ConvertToRgb(float hue, float saturation, float luminosity, Mode mode, out float r, out float g, out float b)
		{
			if (mode != Mode.Hsl)
				throw new InvalidOperationException();

			if (luminosity == 0)
			{
				r = g = b = 0;
				return;
			}

			if (saturation == 0)
			{
				r = g = b = luminosity;
				return;
			}
			float temp2 = luminosity <= 0.5f ? luminosity * (1.0f + saturation) : luminosity + saturation - luminosity * saturation;
			float temp1 = 2.0f * luminosity - temp2;

			var t3 = new[] { hue + 1.0f / 3.0f, hue, hue - 1.0f / 3.0f };
			var clr = new float[] { 0, 0, 0 };
			for (var i = 0; i < 3; i++)
			{
				if (t3[i] < 0)
					t3[i] += 1.0f;
				if (t3[i] > 1)
					t3[i] -= 1.0f;
				if (6.0 * t3[i] < 1.0)
					clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0f;
				else if (2.0 * t3[i] < 1.0)
					clr[i] = temp2;
				else if (3.0 * t3[i] < 2.0)
					clr[i] = temp1 + (temp2 - temp1) * (2.0f / 3.0f - t3[i]) * 6.0f;
				else
					clr[i] = temp1;
			}

			r = clr[0];
			g = clr[1];
			b = clr[2];
		}

		static void ConvertToHsl(float r, float g, float b, Mode mode, out float h, out float s, out float l)
		{
			float v = Math.Max(r, g);
			v = Math.Max(v, b);

			float m = Math.Min(r, g);
			m = Math.Min(m, b);

			l = (m + v) / 2.0f;
			if (l <= 0.0)
			{
				h = s = l = 0;
				return;
			}
			float vm = v - m;
			s = vm;

			if (s > 0.0)
			{
				s /= l <= 0.5f ? v + m : 2.0f - v - m;
			}
			else
			{
				h = 0;
				s = 0;
				return;
			}

			float r2 = (v - r) / vm;
			float g2 = (v - g) / vm;
			float b2 = (v - b) / vm;

			if (r == v)
			{
				h = g == m ? 5.0f + b2 : 1.0f - g2;
			}
			else if (g == v)
			{
				h = b == m ? 1.0f + r2 : 3.0f - b2;
			}
			else
			{
				h = r == m ? 3.0f + g2 : 5.0f - r2;
			}
			h /= 6.0f;
		}

		public static bool operator ==(Color color1, Color color2)
		{
			return EqualsInner(color1, color2);
		}

		public static bool operator !=(Color color1, Color color2)
		{
			return !EqualsInner(color1, color2);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashcode = _r.GetHashCode();
				hashcode = (hashcode * 397) ^ _g.GetHashCode();
				hashcode = (hashcode * 397) ^ _b.GetHashCode();
				hashcode = (hashcode * 397) ^ _a.GetHashCode();
				return hashcode;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Color)
			{
				return EqualsInner(this, (Color)obj);
			}
			return base.Equals(obj);
		}

		static bool EqualsInner(Color color1, Color color2)
		{
			if (color1._mode == Mode.Default && color2._mode == Mode.Default)
				return true;
			if (color1._mode == Mode.Default || color2._mode == Mode.Default)
				return false;
			if (color1._mode == Mode.Hsl && color2._mode == Mode.Hsl)
				return color1._hue == color2._hue && color1._saturation == color2._saturation && color1._luminosity == color2._luminosity && color1._a == color2._a;
			return color1._r == color2._r && color1._g == color2._g && color1._b == color2._b && color1._a == color2._a;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[Color: A={0}, R={1}, G={2}, B={3}, Hue={4}, Saturation={5}, Luminosity={6}]", A, R, G, B, Hue, Saturation, Luminosity);
		}

		public string ToHex()
		{
			var red = (uint)(R * 255);
			var green = (uint)(G * 255);
			var blue = (uint)(B * 255);
			var alpha = (uint)(A * 255);
			return $"#{alpha:X2}{red:X2}{green:X2}{blue:X2}";
		}

		static uint ToHex (char c)
		{
			ushort x = (ushort)c;
			if (x >= '0' && x <= '9')
				return (uint)(x - '0');

			x |= 0x20;
			if (x >= 'a' && x <= 'f')
				return (uint)(x - 'a' + 10);
			return 0;
		} 

		static uint ToHexD (char c)
		{
			var j = ToHex (c);
			return (j << 4) | j;
		}

		public static Color FromHex (string hex)
		{
			// Undefined
			if (hex.Length < 3)
				return Default;
			int idx = (hex [0] == '#') ? 1 : 0;

			switch (hex.Length - idx) {
			case 3: //#rgb => ffrrggbb
				var t1 = ToHexD (hex [idx++]);
				var t2 = ToHexD (hex [idx++]);
				var t3 = ToHexD (hex [idx]);

				return FromRgb ((int)t1, (int)t2, (int)t3);

			case 4: //#argb => aarrggbb
				var f1 = ToHexD (hex [idx++]);
				var f2 = ToHexD (hex [idx++]);
				var f3 = ToHexD (hex [idx++]);
				var f4 = ToHexD (hex [idx]);
				return FromRgba ((int)f2, (int)f3, (int)f4, (int)f1);

			case 6: //#rrggbb => ffrrggbb
				return FromRgb ((int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx])));
				
			case 8: //#aarrggbb
				var a1 = ToHex (hex [idx++]) << 4 | ToHex (hex [idx++]);
				return FromRgba ((int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx++])),
						(int)(ToHex (hex [idx++]) << 4 | ToHex (hex [idx])),
						(int)a1);
				
			default: //everything else will result in unexpected results
				return Default;
			}
		}

		public static Color FromUint(uint argb)
		{
			return FromRgba((byte)((argb & 0x00ff0000) >> 0x10), (byte)((argb & 0x0000ff00) >> 0x8), (byte)(argb & 0x000000ff), (byte)((argb & 0xff000000) >> 0x18));
		}

		public static Color FromRgba(int r, int g, int b, int a)
		{
			double red = (double)r / 255;
			double green = (double)g / 255;
			double blue = (double)b / 255;
			double alpha = (double)a / 255;
			return new Color(red, green, blue, alpha, Mode.Rgb);
		}

		public static Color FromRgb(int r, int g, int b)
		{
			return FromRgba(r, g, b, 255);
		}

		public static Color FromRgba(double r, double g, double b, double a)
		{
			return new Color(r, g, b, a);
		}

		public static Color FromRgb(double r, double g, double b)
		{
			return new Color(r, g, b, 1d, Mode.Rgb);
		}

		public static Color FromHsla(double h, double s, double l, double a = 1d)
		{
			return new Color(h, s, l, a, Mode.Hsl);
		}
#if !NETSTANDARD1_0
		public static implicit operator System.Drawing.Color(Color color)
		{
			if (color.IsDefault)
				return System.Drawing.Color.Empty;
			return System.Drawing.Color.FromArgb((byte)(color._a * 255), (byte)(color._r * 255), (byte)(color._g * 255), (byte)(color._b * 255));
		}

		public static implicit operator Color(System.Drawing.Color color)
		{
			if (color.IsEmpty)
				return Color.Default;
			return FromRgba(color.R, color.G, color.B, color.A);
		}
#endif
		#region Color Definitions

		// matches colors in WPF's System.Windows.Media.Colors

		// Color.FromRgb(240, 248, 255)
		public static readonly Color AliceBlue = new Color(0.94117647409439087f, 0.97254902124404907f, 1f);
		// Color.FromRgb(250, 235, 215)
		public static readonly Color AntiqueWhite = new Color(0.98039215803146362f, 0.92156863212585449f, 0.843137264251709f);
		// Color.FromRgb(0, 255, 255)
		public static readonly Color Aqua = new Color(0f, 1f, 1f);
		// Color.FromRgb(127, 255, 212)
		public static readonly Color Aquamarine = new Color(0.49803921580314636f, 1f, 0.83137255907058716f);
		// Color.FromRgb(240, 255, 255)
		public static readonly Color Azure = new Color(0.94117647409439087f, 1f, 1f);
		// Color.FromRgb(245, 245, 220)
		public static readonly Color Beige = new Color(0.96078431606292725f, 0.96078431606292725f, 0.86274510622024536f);
		// Color.FromRgb(255, 228, 196)
		public static readonly Color Bisque = new Color(1f, 0.89411765336990356f, 0.76862746477127075f);
		// Color.FromRgb(0, 0, 0)
		public static readonly Color Black = new Color(0f, 0f, 0f);
		// Color.FromRgb(255, 235, 205)
		public static readonly Color BlanchedAlmond = new Color(1f, 0.92156863212585449f, 0.80392158031463623f);
		// Color.FromRgb(0, 0, 255)
		public static readonly Color Blue = new Color(0f, 0f, 1f);
		// Color.FromRgb(138, 43, 226)
		public static readonly Color BlueViolet = new Color(0.54117649793624878f, 0.16862745583057404f, 0.886274516582489f);
		// Color.FromRgb(165, 42, 42)
		public static readonly Color Brown = new Color(0.64705884456634521f, 0.16470588743686676f, 0.16470588743686676f);
		// Color.FromRgb(222, 184, 135)
		public static readonly Color BurlyWood = new Color(0.87058824300765991f, 0.72156864404678345f, 0.529411792755127f);
		// Color.FromRgb(95, 158, 160)
		public static readonly Color CadetBlue = new Color(0.37254902720451355f, 0.61960786581039429f, 0.62745100259780884f);
		// Color.FromRgb(127, 255, 0)
		public static readonly Color Chartreuse = new Color(0.49803921580314636f, 1f, 0f);
		// Color.FromRgb(210, 105, 30)
		public static readonly Color Chocolate = new Color(0.82352942228317261f, 0.4117647111415863f, 0.11764705926179886f);
		// Color.FromRgb(255, 127, 80)
		public static readonly Color Coral = new Color(1f, 0.49803921580314636f, 0.31372550129890442f);
		// Color.FromRgb(100, 149, 237)
		public static readonly Color CornflowerBlue = new Color(0.39215686917304993f, 0.58431375026702881f, 0.929411768913269f);
		// Color.FromRgb(255, 248, 220)
		public static readonly Color Cornsilk = new Color(1f, 0.97254902124404907f, 0.86274510622024536f);
		// Color.FromRgb(220, 20, 60)
		public static readonly Color Crimson = new Color(0.86274510622024536f, 0.0784313753247261f, 0.23529411852359772f);
		// Color.FromRgb(0, 255, 255)
		public static readonly Color Cyan = new Color(0f, 1f, 1f);
		// Color.FromRgb(0, 0, 139)
		public static readonly Color DarkBlue = new Color(0f, 0f, 0.545098066329956f);
		// Color.FromRgb(0, 139, 139)
		public static readonly Color DarkCyan = new Color(0f, 0.545098066329956f, 0.545098066329956f);
		// Color.FromRgb(184, 134, 11)
		public static readonly Color DarkGoldenrod = new Color(0.72156864404678345f, 0.52549022436141968f, 0.043137256056070328f);
		// Color.FromRgb(169, 169, 169)
		public static readonly Color DarkGray = new Color(0.66274511814117432f, 0.66274511814117432f, 0.66274511814117432f);
		// Color.FromRgb(0, 100, 0)
		public static readonly Color DarkGreen = new Color(0f, 0.39215686917304993f, 0f);
		// Color.FromRgb(189, 183, 107)
		public static readonly Color DarkKhaki = new Color(0.74117648601531982f, 0.71764707565307617f, 0.41960784792900085f);
		// Color.FromRgb(139, 0, 139)
		public static readonly Color DarkMagenta = new Color(0.545098066329956f, 0f, 0.545098066329956f);
		// Color.FromRgb(85, 107, 47)
		public static readonly Color DarkOliveGreen = new Color(0.3333333432674408f, 0.41960784792900085f, 0.18431372940540314f);
		// Color.FromRgb(255, 140, 0)
		public static readonly Color DarkOrange = new Color(1f, 0.54901963472366333f, 0f);
		// Color.FromRgb(153, 50, 204)
		public static readonly Color DarkOrchid = new Color(0.60000002384185791f, 0.19607843458652496f, 0.800000011920929f);
		// Color.FromRgb(139, 0, 0)
		public static readonly Color DarkRed = new Color(0.545098066329956f, 0f, 0f);
		// Color.FromRgb(233, 150, 122)
		public static readonly Color DarkSalmon = new Color(0.91372549533843994f, 0.58823531866073608f, 0.47843137383461f);
		// Color.FromRgb(143, 188, 143)
		public static readonly Color DarkSeaGreen = new Color(0.56078433990478516f, 0.73725491762161255f, 0.56078433990478516f);
		// Color.FromRgb(72, 61, 139)
		public static readonly Color DarkSlateBlue = new Color(0.28235295414924622f, 0.239215686917305f, 0.545098066329956f);
		// Color.FromRgb(47, 79, 79)
		public static readonly Color DarkSlateGray = new Color(0.18431372940540314f, 0.30980393290519714f, 0.30980393290519714f);
		// Color.FromRgb(0, 206, 209)
		public static readonly Color DarkTurquoise = new Color(0f, 0.80784314870834351f, 0.81960785388946533f);
		// Color.FromRgb(148, 0, 211)
		public static readonly Color DarkViolet = new Color(0.58039218187332153f, 0f, 0.82745099067687988f);
		// Color.FromRgb(255, 20, 147)
		public static readonly Color DeepPink = new Color(1f, 0.0784313753247261f, 0.57647061347961426f);
		// Color.FromRgb(0, 191, 255)
		public static readonly Color DeepSkyBlue = new Color(0f, 0.74901962280273438f, 1f);
		// Color.FromRgb(105, 105, 105)
		public static readonly Color DimGray = new Color(0.4117647111415863f, 0.4117647111415863f, 0.4117647111415863f);
		// Color.FromRgb(30, 144, 255)
		public static readonly Color DodgerBlue = new Color(0.11764705926179886f, 0.56470590829849243f, 1f);
		// Color.FromRgb(178, 34, 34)
		public static readonly Color Firebrick = new Color(0.69803923368453979f, 0.13333334028720856f, 0.13333334028720856f);
		// Color.FromRgb(255, 250, 240)
		public static readonly Color FloralWhite = new Color(1f, 0.98039215803146362f, 0.94117647409439087f);
		// Color.FromRgb(34, 139, 34)
		public static readonly Color ForestGreen = new Color(0.13333334028720856f, 0.545098066329956f, 0.13333334028720856f);
		// Color.FromRgb(255, 0, 255)
		public static readonly Color Fuchsia = new Color(1f, 0f, 1f);
		// Color.FromRgb(255, 0, 255)
		[Obsolete("Fuschia is obsolete as of version 1.3.0. Please use Fuchsia instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly Color Fuschia = new Color(1f, 0f, 1f);
		// Color.FromRgb(220, 220, 220)
		public static readonly Color Gainsboro = new Color(0.86274510622024536f, 0.86274510622024536f, 0.86274510622024536f);
		// Color.FromRgb(248, 248, 255)
		public static readonly Color GhostWhite = new Color(0.97254902124404907f, 0.97254902124404907f, 1f);
		// Color.FromRgb(255, 215, 0)
		public static readonly Color Gold = new Color(1f, 0.843137264251709f, 0f);
		// Color.FromRgb(218, 165, 32)
		public static readonly Color Goldenrod = new Color(0.85490196943283081f, 0.64705884456634521f, 0.125490203499794f);
		// Color.FromRgb(128, 128, 128)
		public static readonly Color Gray = new Color(0.501960813999176f, 0.501960813999176f, 0.501960813999176f);
		// Color.FromRgb(0, 128, 0)
		public static readonly Color Green = new Color(0f, 0.501960813999176f, 0f);
		// Color.FromRgb(173, 255, 47)
		public static readonly Color GreenYellow = new Color(0.67843139171600342f, 1f, 0.18431372940540314f);
		// Color.FromRgb(240, 255, 240)
		public static readonly Color Honeydew = new Color(0.94117647409439087f, 1f, 0.94117647409439087f);
		// Color.FromRgb(255, 105, 180)
		public static readonly Color HotPink = new Color(1f, 0.4117647111415863f, 0.70588237047195435f);
		// Color.FromRgb(205, 92, 92)
		public static readonly Color IndianRed = new Color(0.80392158031463623f, 0.36078432202339172f, 0.36078432202339172f);
		// Color.FromRgb(75, 0, 130)
		public static readonly Color Indigo = new Color(0.29411765933036804f, 0f, 0.50980395078659058f);
		// Color.FromRgb(255, 255, 240)
		public static readonly Color Ivory = new Color(1f, 1f, 0.94117647409439087f);
		// Color.FromRgb(240, 230, 140)
		public static readonly Color Khaki = new Color(0.94117647409439087f, 0.90196079015731812f, 0.54901963472366333f);
		// Color.FromRgb(230, 230, 250)
		public static readonly Color Lavender = new Color(0.90196079015731812f, 0.90196079015731812f, 0.98039215803146362f);
		// Color.FromRgb(255, 240, 245)
		public static readonly Color LavenderBlush = new Color(1f, 0.94117647409439087f, 0.96078431606292725f);
		// Color.FromRgb(124, 252, 0)
		public static readonly Color LawnGreen = new Color(0.48627451062202454f, 0.98823529481887817f, 0f);
		// Color.FromRgb(255, 250, 205)
		public static readonly Color LemonChiffon = new Color(1f, 0.98039215803146362f, 0.80392158031463623f);
		// Color.FromRgb(173, 216, 230)
		public static readonly Color LightBlue = new Color(0.67843139171600342f, 0.84705883264541626f, 0.90196079015731812f);
		// Color.FromRgb(240, 128, 128)
		public static readonly Color LightCoral = new Color(0.94117647409439087f, 0.501960813999176f, 0.501960813999176f);
		// Color.FromRgb(224, 255, 255)
		public static readonly Color LightCyan = new Color(0.87843137979507446f, 1f, 1f);
		// Color.FromRgb(250, 250, 210)
		public static readonly Color LightGoldenrodYellow = new Color(0.98039215803146362f, 0.98039215803146362f, 0.82352942228317261f);
		// Color.FromRgb(211, 211, 211)
		public static readonly Color LightGray = new Color(0.82745099067687988f, 0.82745099067687988f, 0.82745099067687988f);
		// Color.FromRgb(144, 238, 144)
		public static readonly Color LightGreen = new Color(0.56470590829849243f, 0.93333333730697632f, 0.56470590829849243f);
		// Color.FromRgb(255, 182, 193)
		public static readonly Color LightPink = new Color(1f, 0.7137255072593689f, 0.75686275959014893f);
		// Color.FromRgb(255, 160, 122)
		public static readonly Color LightSalmon = new Color(1f, 0.62745100259780884f, 0.47843137383461f);
		// Color.FromRgb(32, 178, 170)
		public static readonly Color LightSeaGreen = new Color(0.125490203499794f, 0.69803923368453979f, 0.66666668653488159f);
		// Color.FromRgb(135, 206, 250)
		public static readonly Color LightSkyBlue = new Color(0.529411792755127f, 0.80784314870834351f, 0.98039215803146362f);
		// Color.FromRgb(119, 136, 153)
		public static readonly Color LightSlateGray = new Color(0.46666666865348816f, 0.53333336114883423f, 0.60000002384185791f);
		// Color.FromRgb(176, 196, 222)
		public static readonly Color LightSteelBlue = new Color(0.69019609689712524f, 0.76862746477127075f, 0.87058824300765991f);
		// Color.FromRgb(255, 255, 224)
		public static readonly Color LightYellow = new Color(1f, 1f, 0.87843137979507446f);
		// Color.FromRgb(0, 255, 0)
		public static readonly Color Lime = new Color(0f, 1f, 0f);
		// Color.FromRgb(50, 205, 50)
		public static readonly Color LimeGreen = new Color(0.19607843458652496f, 0.80392158031463623f, 0.19607843458652496f);
		// Color.FromRgb(250, 240, 230)
		public static readonly Color Linen = new Color(0.98039215803146362f, 0.94117647409439087f, 0.90196079015731812f);
		// Color.FromRgb(255, 0, 255)
		public static readonly Color Magenta = new Color(1f, 0f, 1f);
		// Color.FromRgb(128, 0, 0)
		public static readonly Color Maroon = new Color(0.501960813999176f, 0f, 0f);
		// Color.FromRgb(102, 205, 170)
		public static readonly Color MediumAquamarine = new Color(0.40000000596046448f, 0.80392158031463623f, 0.66666668653488159f);
		// Color.FromRgb(0, 0, 205)
		public static readonly Color MediumBlue = new Color(0f, 0f, 0.80392158031463623f);
		// Color.FromRgb(186, 85, 211)
		public static readonly Color MediumOrchid = new Color(0.729411780834198f, 0.3333333432674408f, 0.82745099067687988f);
		// Color.FromRgb(147, 112, 219)
		public static readonly Color MediumPurple = new Color(0.57647061347961426f, 0.43921568989753723f, 0.85882353782653809f);
		// Color.FromRgb(60, 179, 113)
		public static readonly Color MediumSeaGreen = new Color(0.23529411852359772f, 0.70196080207824707f, 0.44313725829124451f);
		// Color.FromRgb(123, 104, 238)
		public static readonly Color MediumSlateBlue = new Color(0.48235294222831726f, 0.40784314274787903f, 0.93333333730697632f);
		// Color.FromRgb(0, 250, 154)
		public static readonly Color MediumSpringGreen = new Color(0f, 0.98039215803146362f, 0.60392159223556519f);
		// Color.FromRgb(72, 209, 204)
		public static readonly Color MediumTurquoise = new Color(0.28235295414924622f, 0.81960785388946533f, 0.800000011920929f);
		// Color.FromRgb(199, 21, 133)
		public static readonly Color MediumVioletRed = new Color(0.78039216995239258f, 0.08235294371843338f, 0.5215686559677124f);
		// Color.FromRgb(25, 25, 112)
		public static readonly Color MidnightBlue = new Color(0.098039217293262482f, 0.098039217293262482f, 0.43921568989753723f);
		// Color.FromRgb(245, 255, 250)
		public static readonly Color MintCream = new Color(0.96078431606292725f, 1f, 0.98039215803146362f);
		// Color.FromRgb(255, 228, 225)
		public static readonly Color MistyRose = new Color(1f, 0.89411765336990356f, 0.88235294818878174f);
		// Color.FromRgb(255, 228, 181)
		public static readonly Color Moccasin = new Color(1f, 0.89411765336990356f, 0.70980393886566162f);
		// Color.FromRgb(255, 222, 173)
		public static readonly Color NavajoWhite = new Color(1f, 0.87058824300765991f, 0.67843139171600342f);
		// Color.FromRgb(0, 0, 128)
		public static readonly Color Navy = new Color(0f, 0f, 0.501960813999176f);
		// Color.FromRgb(253, 245, 230)
		public static readonly Color OldLace = new Color(0.99215686321258545f, 0.96078431606292725f, 0.90196079015731812f);
		// Color.FromRgb(128, 128, 0)
		public static readonly Color Olive = new Color(0.501960813999176f, 0.501960813999176f, 0f);
		// Color.FromRgb(107, 142, 35)
		public static readonly Color OliveDrab = new Color(0.41960784792900085f, 0.55686277151107788f, 0.13725490868091583f);
		// Color.FromRgb(255, 165, 0)
		public static readonly Color Orange = new Color(1f, 0.64705884456634521f, 0f);
		// Color.FromRgb(255, 69, 0)
		public static readonly Color OrangeRed = new Color(1f, 0.27058824896812439f, 0f);
		// Color.FromRgb(218, 112, 214)
		public static readonly Color Orchid = new Color(0.85490196943283081f, 0.43921568989753723f, 0.83921569585800171f);
		// Color.FromRgb(238, 232, 170)
		public static readonly Color PaleGoldenrod = new Color(0.93333333730697632f, 0.90980392694473267f, 0.66666668653488159f);
		// Color.FromRgb(152, 251, 152)
		public static readonly Color PaleGreen = new Color(0.59607845544815063f, 0.9843137264251709f, 0.59607845544815063f);
		// Color.FromRgb(175, 238, 238)
		public static readonly Color PaleTurquoise = new Color(0.686274528503418f, 0.93333333730697632f, 0.93333333730697632f);
		// Color.FromRgb(219, 112, 147)
		public static readonly Color PaleVioletRed = new Color(0.85882353782653809f, 0.43921568989753723f, 0.57647061347961426f);
		// Color.FromRgb(255, 239, 213)
		public static readonly Color PapayaWhip = new Color(1f, 0.93725490570068359f, 0.83529412746429443f);
		// Color.FromRgb(255, 218, 185)
		public static readonly Color PeachPuff = new Color(1f, 0.85490196943283081f, 0.72549021244049072f);
		// Color.FromRgb(205, 133, 63)
		public static readonly Color Peru = new Color(0.80392158031463623f, 0.5215686559677124f, 0.24705882370471954f);
		// Color.FromRgb(255, 192, 203)
		public static readonly Color Pink = new Color(1f, 0.75294119119644165f, 0.79607844352722168f);
		// Color.FromRgb(221, 160, 221)
		public static readonly Color Plum = new Color(0.86666667461395264f, 0.62745100259780884f, 0.86666667461395264f);
		// Color.FromRgb(176, 224, 230)
		public static readonly Color PowderBlue = new Color(0.69019609689712524f, 0.87843137979507446f, 0.90196079015731812f);
		// Color.FromRgb(128, 0, 128)
		public static readonly Color Purple = new Color(0.501960813999176f, 0f, 0.501960813999176f);
		// Color.FromRgb(255, 0, 0)
		public static readonly Color Red = new Color(1f, 0f, 0f);
		// Color.FromRgb(188, 143, 143)
		public static readonly Color RosyBrown = new Color(0.73725491762161255f, 0.56078433990478516f, 0.56078433990478516f);
		// Color.FromRgb(65, 105, 225)
		public static readonly Color RoyalBlue = new Color(0.25490197539329529f, 0.4117647111415863f, 0.88235294818878174f);
		// Color.FromRgb(139, 69, 19)
		public static readonly Color SaddleBrown = new Color(0.545098066329956f, 0.27058824896812439f, 0.074509806931018829f);
		// Color.FromRgb(250, 128, 114)
		public static readonly Color Salmon = new Color(0.98039215803146362f, 0.501960813999176f, 0.44705882668495178f);
		// Color.FromRgb(244, 164, 96)
		public static readonly Color SandyBrown = new Color(0.95686274766922f, 0.64313727617263794f, 0.37647059559822083f);
		// Color.FromRgb(46, 139, 87)
		public static readonly Color SeaGreen = new Color(0.18039216101169586f, 0.545098066329956f, 0.34117648005485535f);
		// Color.FromRgb(255, 245, 238)
		public static readonly Color SeaShell = new Color(1f, 0.96078431606292725f, 0.93333333730697632f);
		// Color.FromRgb(160, 82, 45)
		public static readonly Color Sienna = new Color(0.62745100259780884f, 0.32156863808631897f, 0.17647059261798859f);
		// Color.FromRgb(192, 192, 192)
		public static readonly Color Silver = new Color(0.75294119119644165f, 0.75294119119644165f, 0.75294119119644165f);
		// Color.FromRgb(135, 206, 235)
		public static readonly Color SkyBlue = new Color(0.529411792755127f, 0.80784314870834351f, 0.92156863212585449f);
		// Color.FromRgb(106, 90, 205)
		public static readonly Color SlateBlue = new Color(0.41568627953529358f, 0.35294118523597717f, 0.80392158031463623f);
		// Color.FromRgb(112, 128, 144)
		public static readonly Color SlateGray = new Color(0.43921568989753723f, 0.501960813999176f, 0.56470590829849243f);
		// Color.FromRgb(255, 250, 250)
		public static readonly Color Snow = new Color(1f, 0.98039215803146362f, 0.98039215803146362f);
		// Color.FromRgb(0, 255, 127)
		public static readonly Color SpringGreen = new Color(0f, 1f, 0.49803921580314636f);
		// Color.FromRgb(70, 130, 180)
		public static readonly Color SteelBlue = new Color(0.27450981736183167f, 0.50980395078659058f, 0.70588237047195435f);
		// Color.FromRgb(210, 180, 140)
		public static readonly Color Tan = new Color(0.82352942228317261f, 0.70588237047195435f, 0.54901963472366333f);
		// Color.FromRgb(0, 128, 128)
		public static readonly Color Teal = new Color(0f, 0.501960813999176f, 0.501960813999176f);
		// Color.FromRgb(216, 191, 216)
		public static readonly Color Thistle = new Color(0.84705883264541626f, 0.74901962280273438f, 0.84705883264541626f);
		// Color.FromRgb(255, 99, 71)
		public static readonly Color Tomato = new Color(1f, 0.38823530077934265f, 0.27843138575553894f);
		// Color.FromRgba(255, 255, 255, 0)
		public static readonly Color Transparent = new Color(1f, 1f, 1f, 0f);
		// Color.FromRgb(64, 224, 208)
		public static readonly Color Turquoise = new Color(0.250980406999588f, 0.87843137979507446f, 0.81568628549575806f);
		// Color.FromRgb(238, 130, 238)
		public static readonly Color Violet = new Color(0.93333333730697632f, 0.50980395078659058f, 0.93333333730697632f);
		// Color.FromRgb(245, 222, 179)
		public static readonly Color Wheat = new Color(0.96078431606292725f, 0.87058824300765991f, 0.70196080207824707f);
		// Color.FromRgb(255, 255, 255)
		public static readonly Color White = new Color(1f, 1f, 1f);
		// Color.FromRgb(245, 245, 245)
		public static readonly Color WhiteSmoke = new Color(0.96078431606292725f, 0.96078431606292725f, 0.96078431606292725f);
		// Color.FromRgb(255, 255, 0)
		public static readonly Color Yellow = new Color(1f, 1f, 0f);
		// Color.FromRgb(154, 205, 50)
		public static readonly Color YellowGreen = new Color(0.60392159223556519f, 0.80392158031463623f, 0.19607843458652496f);

		#endregion
	}
}
