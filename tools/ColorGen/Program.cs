using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Forms;
using static Xamarin.Forms.Color;

namespace ColorGen
{
	static class Program
	{
		static int Main(string [] args)
		{
			if (args.Length == 0)
			{
				Console.Error.WriteLine("Please specify an output path");
				return 1;
			}

			int indent = 0;

			using (var writer = File.CreateText(args[0]))
			{
				void WriteLineWithIndent (string line)
				{
					writer.Write("".PadLeft(indent, '\t'));
					writer.WriteLine(line);
				}

				void OpenBrace()
				{
					WriteLineWithIndent("{");
					indent++;
				}

				void CloseBrace()
				{
					indent--;
					WriteLineWithIndent("}");
				}

				writer.WriteLine("// This file auto-generated, changes may be lost. See tools/ColorGen for details.");
				writer.WriteLine("using System;");
				writer.WriteLine("using System.ComponentModel;");
				writer.WriteLine();

				writer.WriteLine("namespace Xamarin.Forms");
				OpenBrace();
				WriteLineWithIndent("public partial struct Color");
				OpenBrace();

				foreach (var field in typeof(Program).GetFields())
				{
					if (field.FieldType != typeof(Color))
						continue;

					foreach (ObsoleteAttribute obsolete in field.GetCustomAttributes (typeof (ObsoleteAttribute), inherit: false))
					{
						WriteLineWithIndent($"[Obsolete(\"{obsolete.Message}\")]");
					}
					foreach (EditorBrowsableAttribute browsable in field.GetCustomAttributes(typeof(EditorBrowsableAttribute), inherit: false))
					{
						WriteLineWithIndent($"[EditorBrowsable(EditorBrowsableState.{browsable.State})]");
					}

					// See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-round-trip-r-format-specifier
					var format = "R";
					var color = (Color)field.GetValue(null);
					WriteLineWithIndent($"public static readonly Color {field.Name} = new Color ({color.R.ToString (format)}f, {color.G.ToString(format)}f, {color.B.ToString(format)}f, {color.A.ToString(format)}f, Mode.Rgb, {color.Hue.ToString(format)}f, {color.Saturation.ToString(format)}f, {color.Luminosity.ToString(format)}f);");
				}

				CloseBrace(); // public partial struct Color
				CloseBrace(); // namespace Xamarin.Forms
			}
			return 0;
		}

		// We want to generate these members in Color.g.cs
		public static readonly Color AliceBlue = FromRgb(240, 248, 255);
		public static readonly Color AntiqueWhite = FromRgb(250, 235, 215);
		public static readonly Color Aqua = FromRgb(0, 255, 255);
		public static readonly Color Aquamarine = FromRgb(127, 255, 212);
		public static readonly Color Azure = FromRgb(240, 255, 255);
		public static readonly Color Beige = FromRgb(245, 245, 220);
		public static readonly Color Bisque = FromRgb(255, 228, 196);
		public static readonly Color Black = FromRgb(0, 0, 0);
		public static readonly Color BlanchedAlmond = FromRgb(255, 235, 205);
		public static readonly Color Blue = FromRgb(0, 0, 255);
		public static readonly Color BlueViolet = FromRgb(138, 43, 226);
		public static readonly Color Brown = FromRgb(165, 42, 42);
		public static readonly Color BurlyWood = FromRgb(222, 184, 135);
		public static readonly Color CadetBlue = FromRgb(95, 158, 160);
		public static readonly Color Chartreuse = FromRgb(127, 255, 0);
		public static readonly Color Chocolate = FromRgb(210, 105, 30);
		public static readonly Color Coral = FromRgb(255, 127, 80);
		public static readonly Color CornflowerBlue = FromRgb(100, 149, 237);
		public static readonly Color Cornsilk = FromRgb(255, 248, 220);
		public static readonly Color Crimson = FromRgb(220, 20, 60);
		public static readonly Color Cyan = FromRgb(0, 255, 255);
		public static readonly Color DarkBlue = FromRgb(0, 0, 139);
		public static readonly Color DarkCyan = FromRgb(0, 139, 139);
		public static readonly Color DarkGoldenrod = FromRgb(184, 134, 11);
		public static readonly Color DarkGray = FromRgb(169, 169, 169);
		public static readonly Color DarkGreen = FromRgb(0, 100, 0);
		public static readonly Color DarkKhaki = FromRgb(189, 183, 107);
		public static readonly Color DarkMagenta = FromRgb(139, 0, 139);
		public static readonly Color DarkOliveGreen = FromRgb(85, 107, 47);
		public static readonly Color DarkOrange = FromRgb(255, 140, 0);
		public static readonly Color DarkOrchid = FromRgb(153, 50, 204);
		public static readonly Color DarkRed = FromRgb(139, 0, 0);
		public static readonly Color DarkSalmon = FromRgb(233, 150, 122);
		public static readonly Color DarkSeaGreen = FromRgb(143, 188, 143);
		public static readonly Color DarkSlateBlue = FromRgb(72, 61, 139);
		public static readonly Color DarkSlateGray = FromRgb(47, 79, 79);
		public static readonly Color DarkTurquoise = FromRgb(0, 206, 209);
		public static readonly Color DarkViolet = FromRgb(148, 0, 211);
		public static readonly Color DeepPink = FromRgb(255, 20, 147);
		public static readonly Color DeepSkyBlue = FromRgb(0, 191, 255);
		public static readonly Color DimGray = FromRgb(105, 105, 105);
		public static readonly Color DodgerBlue = FromRgb(30, 144, 255);
		public static readonly Color Firebrick = FromRgb(178, 34, 34);
		public static readonly Color FloralWhite = FromRgb(255, 250, 240);
		public static readonly Color ForestGreen = FromRgb(34, 139, 34);
		public static readonly Color Fuchsia = FromRgb(255, 0, 255);
		[Obsolete("Fuschia is obsolete as of version 1.3.0. Please use Fuchsia instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly Color Fuschia = FromRgb(255, 0, 255);
		public static readonly Color Gainsboro = FromRgb(220, 220, 220);
		public static readonly Color GhostWhite = FromRgb(248, 248, 255);
		public static readonly Color Gold = FromRgb(255, 215, 0);
		public static readonly Color Goldenrod = FromRgb(218, 165, 32);
		public static readonly Color Gray = FromRgb(128, 128, 128);
		public static readonly Color Green = FromRgb(0, 128, 0);
		public static readonly Color GreenYellow = FromRgb(173, 255, 47);
		public static readonly Color Honeydew = FromRgb(240, 255, 240);
		public static readonly Color HotPink = FromRgb(255, 105, 180);
		public static readonly Color IndianRed = FromRgb(205, 92, 92);
		public static readonly Color Indigo = FromRgb(75, 0, 130);
		public static readonly Color Ivory = FromRgb(255, 255, 240);
		public static readonly Color Khaki = FromRgb(240, 230, 140);
		public static readonly Color Lavender = FromRgb(230, 230, 250);
		public static readonly Color LavenderBlush = FromRgb(255, 240, 245);
		public static readonly Color LawnGreen = FromRgb(124, 252, 0);
		public static readonly Color LemonChiffon = FromRgb(255, 250, 205);
		public static readonly Color LightBlue = FromRgb(173, 216, 230);
		public static readonly Color LightCoral = FromRgb(240, 128, 128);
		public static readonly Color LightCyan = FromRgb(224, 255, 255);
		public static readonly Color LightGoldenrodYellow = FromRgb(250, 250, 210);
		public static readonly Color LightGray = FromRgb(211, 211, 211);
		public static readonly Color LightGreen = FromRgb(144, 238, 144);
		public static readonly Color LightPink = FromRgb(255, 182, 193);
		public static readonly Color LightSalmon = FromRgb(255, 160, 122);
		public static readonly Color LightSeaGreen = FromRgb(32, 178, 170);
		public static readonly Color LightSkyBlue = FromRgb(135, 206, 250);
		public static readonly Color LightSlateGray = FromRgb(119, 136, 153);
		public static readonly Color LightSteelBlue = FromRgb(176, 196, 222);
		public static readonly Color LightYellow = FromRgb(255, 255, 224);
		public static readonly Color Lime = FromRgb(0, 255, 0);
		public static readonly Color LimeGreen = FromRgb(50, 205, 50);
		public static readonly Color Linen = FromRgb(250, 240, 230);
		public static readonly Color Magenta = FromRgb(255, 0, 255);
		public static readonly Color Maroon = FromRgb(128, 0, 0);
		public static readonly Color MediumAquamarine = FromRgb(102, 205, 170);
		public static readonly Color MediumBlue = FromRgb(0, 0, 205);
		public static readonly Color MediumOrchid = FromRgb(186, 85, 211);
		public static readonly Color MediumPurple = FromRgb(147, 112, 219);
		public static readonly Color MediumSeaGreen = FromRgb(60, 179, 113);
		public static readonly Color MediumSlateBlue = FromRgb(123, 104, 238);
		public static readonly Color MediumSpringGreen = FromRgb(0, 250, 154);
		public static readonly Color MediumTurquoise = FromRgb(72, 209, 204);
		public static readonly Color MediumVioletRed = FromRgb(199, 21, 133);
		public static readonly Color MidnightBlue = FromRgb(25, 25, 112);
		public static readonly Color MintCream = FromRgb(245, 255, 250);
		public static readonly Color MistyRose = FromRgb(255, 228, 225);
		public static readonly Color Moccasin = FromRgb(255, 228, 181);
		public static readonly Color NavajoWhite = FromRgb(255, 222, 173);
		public static readonly Color Navy = FromRgb(0, 0, 128);
		public static readonly Color OldLace = FromRgb(253, 245, 230);
		public static readonly Color Olive = FromRgb(128, 128, 0);
		public static readonly Color OliveDrab = FromRgb(107, 142, 35);
		public static readonly Color Orange = FromRgb(255, 165, 0);
		public static readonly Color OrangeRed = FromRgb(255, 69, 0);
		public static readonly Color Orchid = FromRgb(218, 112, 214);
		public static readonly Color PaleGoldenrod = FromRgb(238, 232, 170);
		public static readonly Color PaleGreen = FromRgb(152, 251, 152);
		public static readonly Color PaleTurquoise = FromRgb(175, 238, 238);
		public static readonly Color PaleVioletRed = FromRgb(219, 112, 147);
		public static readonly Color PapayaWhip = FromRgb(255, 239, 213);
		public static readonly Color PeachPuff = FromRgb(255, 218, 185);
		public static readonly Color Peru = FromRgb(205, 133, 63);
		public static readonly Color Pink = FromRgb(255, 192, 203);
		public static readonly Color Plum = FromRgb(221, 160, 221);
		public static readonly Color PowderBlue = FromRgb(176, 224, 230);
		public static readonly Color Purple = FromRgb(128, 0, 128);
		public static readonly Color Red = FromRgb(255, 0, 0);
		public static readonly Color RosyBrown = FromRgb(188, 143, 143);
		public static readonly Color RoyalBlue = FromRgb(65, 105, 225);
		public static readonly Color SaddleBrown = FromRgb(139, 69, 19);
		public static readonly Color Salmon = FromRgb(250, 128, 114);
		public static readonly Color SandyBrown = FromRgb(244, 164, 96);
		public static readonly Color SeaGreen = FromRgb(46, 139, 87);
		public static readonly Color SeaShell = FromRgb(255, 245, 238);
		public static readonly Color Sienna = FromRgb(160, 82, 45);
		public static readonly Color Silver = FromRgb(192, 192, 192);
		public static readonly Color SkyBlue = FromRgb(135, 206, 235);
		public static readonly Color SlateBlue = FromRgb(106, 90, 205);
		public static readonly Color SlateGray = FromRgb(112, 128, 144);
		public static readonly Color Snow = FromRgb(255, 250, 250);
		public static readonly Color SpringGreen = FromRgb(0, 255, 127);
		public static readonly Color SteelBlue = FromRgb(70, 130, 180);
		public static readonly Color Tan = FromRgb(210, 180, 140);
		public static readonly Color Teal = FromRgb(0, 128, 128);
		public static readonly Color Thistle = FromRgb(216, 191, 216);
		public static readonly Color Tomato = FromRgb(255, 99, 71);
		public static readonly Color Transparent = FromRgba(255, 255, 255, 0);
		public static readonly Color Turquoise = FromRgb(64, 224, 208);
		public static readonly Color Violet = FromRgb(238, 130, 238);
		public static readonly Color Wheat = FromRgb(245, 222, 179);
		public static readonly Color White = FromRgb(255, 255, 255);
		public static readonly Color WhiteSmoke = FromRgb(245, 245, 245);
		public static readonly Color Yellow = FromRgb(255, 255, 0);
		public static readonly Color YellowGreen = FromRgb(154, 205, 50);
	}
}
