using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.StyleSheets
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
	sealed class StylePropertyAttribute : Attribute
	{
		public string CssPropertyName { get; }
		public string BindablePropertyName { get; }
		public Type TargetType { get; }
		public Type PropertyOwnerType { get; set; }
		public BindableProperty BindableProperty { get; set; }
		public bool Inherited { get; set; } = false;


		public StylePropertyAttribute(string cssPropertyName, Type targetType, string bindablePropertyName)
		{
			CssPropertyName = cssPropertyName;
			BindablePropertyName = bindablePropertyName;
			TargetType = targetType;
		}

		StylePropertyAttribute(string cssPropertyName, Type targetType, string bindablePropertyName, bool inherited)
		{
			CssPropertyName = cssPropertyName;
			BindablePropertyName = bindablePropertyName;
			TargetType = targetType;
			Inherited = inherited;
		}

		StylePropertyAttribute(string cssPropertyName, Type targetType, string bindablePropertyName, Type propertyOwnerType)
		{
			CssPropertyName = cssPropertyName;
			BindablePropertyName = bindablePropertyName;
			TargetType = targetType;
			PropertyOwnerType = propertyOwnerType;
		}

		/// <summary>
		/// NOTE: one day this list can be moved to AssemblyAttributeProvider
		/// </summary>
		internal static StylePropertyAttribute[] GetStylePropertyAttributes() => new[]
		{
			new StylePropertyAttribute("background-color", typeof(VisualElement), nameof(VisualElement.BackgroundColorProperty)),
			new StylePropertyAttribute("background-image", typeof(Page), nameof(Page.BackgroundImageSourceProperty)),
			new StylePropertyAttribute("border-color", typeof(IBorderElement), nameof(BorderElement.BorderColorProperty)),
			new StylePropertyAttribute("border-radius", typeof(ICornerElement), nameof(CornerElement.CornerRadiusProperty)),
			new StylePropertyAttribute("border-radius", typeof(Button), nameof(Button.CornerRadiusProperty)),
			new StylePropertyAttribute("border-radius", typeof(Frame), nameof(Frame.CornerRadiusProperty)),
			new StylePropertyAttribute("border-radius", typeof(ImageButton), nameof(BorderElement.CornerRadiusProperty)),
			new StylePropertyAttribute("border-width", typeof(IBorderElement), nameof(BorderElement.BorderWidthProperty)),
			new StylePropertyAttribute("color", typeof(IColorElement), nameof(ColorElement.ColorProperty), true),
			new StylePropertyAttribute("color", typeof(ITextElement), nameof(TextElement.TextColorProperty), true),
			new StylePropertyAttribute("color", typeof(ProgressBar), nameof(ProgressBar.ProgressColorProperty)),
			new StylePropertyAttribute("color", typeof(Switch), nameof(Switch.OnColorProperty)),
			new StylePropertyAttribute("column-gap", typeof(Grid), nameof(Grid.ColumnSpacingProperty)),
			new StylePropertyAttribute("direction", typeof(VisualElement), nameof(VisualElement.FlowDirectionProperty), true),
			new StylePropertyAttribute("font-family", typeof(IFontElement), nameof(FontElement.FontFamilyProperty), true),
			new StylePropertyAttribute("font-size", typeof(IFontElement), nameof(FontElement.FontSizeProperty), true),
			new StylePropertyAttribute("font-style", typeof(IFontElement), nameof(FontElement.FontAttributesProperty), true),
			new StylePropertyAttribute("height", typeof(VisualElement), nameof(VisualElement.HeightRequestProperty)),
			new StylePropertyAttribute("margin", typeof(View), nameof(View.MarginProperty)),
			new StylePropertyAttribute("margin-left", typeof(View), nameof(View.MarginLeftProperty)),
			new StylePropertyAttribute("margin-top", typeof(View), nameof(View.MarginTopProperty)),
			new StylePropertyAttribute("margin-right", typeof(View), nameof(View.MarginRightProperty)),
			new StylePropertyAttribute("margin-bottom", typeof(View), nameof(View.MarginBottomProperty)),
			new StylePropertyAttribute("max-lines", typeof(Label), nameof(Label.MaxLinesProperty)),
			new StylePropertyAttribute("min-height", typeof(VisualElement), nameof(VisualElement.MinimumHeightRequestProperty)),
			new StylePropertyAttribute("min-width", typeof(VisualElement), nameof(VisualElement.MinimumWidthRequestProperty)),
			new StylePropertyAttribute("opacity", typeof(VisualElement), nameof(VisualElement.OpacityProperty)),
			new StylePropertyAttribute("padding", typeof(IPaddingElement), nameof(PaddingElement.PaddingProperty)),
			new StylePropertyAttribute("padding-left", typeof(IPaddingElement), nameof(PaddingElement.PaddingLeftProperty), typeof(PaddingElement)),
			new StylePropertyAttribute("padding-top", typeof(IPaddingElement), nameof(PaddingElement.PaddingTopProperty), typeof(PaddingElement)),
			new StylePropertyAttribute("padding-right", typeof(IPaddingElement), nameof(PaddingElement.PaddingRightProperty), typeof(PaddingElement)),
			new StylePropertyAttribute("padding-bottom", typeof(IPaddingElement), nameof(PaddingElement.PaddingBottomProperty), typeof(PaddingElement)),
			new StylePropertyAttribute("row-gap", typeof(Grid), nameof(Grid.RowSpacingProperty)),
			new StylePropertyAttribute("text-align", typeof(ITextAlignmentElement), nameof(TextAlignmentElement.HorizontalTextAlignmentProperty), true),
			new StylePropertyAttribute("text-decoration", typeof(IDecorableTextElement), nameof(DecorableTextElement.TextDecorationsProperty)),
			new StylePropertyAttribute("transform", typeof(VisualElement), nameof(VisualElement.TransformProperty)),
			new StylePropertyAttribute("transform-origin", typeof(VisualElement), nameof(VisualElement.TransformOriginProperty)),
			new StylePropertyAttribute("vertical-align", typeof(ITextAlignmentElement), nameof(TextAlignmentElement.VerticalTextAlignmentProperty)),
			new StylePropertyAttribute("visibility", typeof(VisualElement), nameof(VisualElement.IsVisibleProperty), true),
			new StylePropertyAttribute("width", typeof(VisualElement), nameof(VisualElement.WidthRequestProperty)),
			new StylePropertyAttribute("letter-spacing", typeof(ITextElement), nameof(TextElement.CharacterSpacingProperty), true),
			new StylePropertyAttribute("line-height", typeof(ILineHeightElement), nameof(LineHeightElement.LineHeightProperty), true),

			//flex
			new StylePropertyAttribute("align-content", typeof(FlexLayout), nameof(FlexLayout.AlignContentProperty)),
			new StylePropertyAttribute("align-items", typeof(FlexLayout), nameof(FlexLayout.AlignItemsProperty)),
			new StylePropertyAttribute("align-self", typeof(VisualElement), nameof(FlexLayout.AlignSelfProperty), typeof(FlexLayout)),
			new StylePropertyAttribute("flex-direction", typeof(FlexLayout), nameof(FlexLayout.DirectionProperty)),
			new StylePropertyAttribute("flex-basis", typeof(VisualElement), nameof(FlexLayout.BasisProperty), typeof(FlexLayout)),
			new StylePropertyAttribute("flex-grow", typeof(VisualElement), nameof(FlexLayout.GrowProperty), typeof(FlexLayout)),
			new StylePropertyAttribute("flex-shrink", typeof(VisualElement), nameof(FlexLayout.ShrinkProperty), typeof(FlexLayout)),
			new StylePropertyAttribute("flex-wrap", typeof(VisualElement), nameof(FlexLayout.WrapProperty), typeof(FlexLayout)),
			new StylePropertyAttribute("justify-content", typeof(FlexLayout), nameof(FlexLayout.JustifyContentProperty)),
			new StylePropertyAttribute("order", typeof(VisualElement), nameof(FlexLayout.OrderProperty), typeof(FlexLayout)),
			new StylePropertyAttribute("position", typeof(FlexLayout), nameof(FlexLayout.PositionProperty)),

			//xf specific
			new StylePropertyAttribute("-xf-placeholder", typeof(IPlaceholderElement), nameof(PlaceholderElement.PlaceholderProperty)),
			new StylePropertyAttribute("-xf-placeholder-color", typeof(IPlaceholderElement), nameof(PlaceholderElement.PlaceholderColorProperty)),
			new StylePropertyAttribute("-xf-max-length", typeof(InputView), nameof(InputView.MaxLengthProperty)),
			new StylePropertyAttribute("-xf-bar-background-color", typeof(IBarElement), nameof(BarElement.BarBackgroundColorProperty)),
			new StylePropertyAttribute("-xf-bar-text-color", typeof(IBarElement), nameof(BarElement.BarTextColorProperty)),
			new StylePropertyAttribute("-xf-orientation", typeof(ScrollView), nameof(ScrollView.OrientationProperty)),
			new StylePropertyAttribute("-xf-horizontal-scroll-bar-visibility", typeof(ScrollView), nameof(ScrollView.HorizontalScrollBarVisibilityProperty)),
			new StylePropertyAttribute("-xf-vertical-scroll-bar-visibility", typeof(ScrollView), nameof(ScrollView.VerticalScrollBarVisibilityProperty)),
			new StylePropertyAttribute("-xf-min-track-color", typeof(Slider), nameof(Slider.MinimumTrackColorProperty)),
			new StylePropertyAttribute("-xf-max-track-color", typeof(Slider), nameof(Slider.MaximumTrackColorProperty)),
			new StylePropertyAttribute("-xf-thumb-color", typeof(Slider), nameof(Slider.ThumbColorProperty)),
			new StylePropertyAttribute("-xf-spacing", typeof(StackLayout), nameof(StackLayout.SpacingProperty)),
			new StylePropertyAttribute("-xf-orientation", typeof(StackLayout), nameof(StackLayout.OrientationProperty)),
			new StylePropertyAttribute("-xf-visual", typeof(VisualElement), nameof(VisualElement.VisualProperty)),
			new StylePropertyAttribute("-xf-vertical-text-alignment", typeof(Label), nameof(TextAlignmentElement.VerticalTextAlignmentProperty)),
			new StylePropertyAttribute("-xf-thumb-color", typeof(Switch), nameof(Switch.ThumbColorProperty)),

			//shell
			new StylePropertyAttribute("-xf-flyout-background", typeof(Shell), nameof(Shell.FlyoutBackgroundColorProperty)),
			new StylePropertyAttribute("-xf-shell-background", typeof(Element), nameof(Shell.BackgroundColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-disabled", typeof(Element), nameof(Shell.DisabledColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-foreground", typeof(Element), nameof(Shell.ForegroundColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-tabbar-background", typeof(Element), nameof(Shell.TabBarBackgroundColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-tabbar-disabled", typeof(Element), nameof(Shell.TabBarDisabledColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-tabbar-foreground", typeof(Element), nameof(Shell.TabBarForegroundColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-tabbar-title", typeof(Element), nameof(Shell.TabBarTitleColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-tabbar-unselected", typeof(Element), nameof(Shell.TabBarUnselectedColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-title", typeof(Element), nameof(Shell.TitleColorProperty), typeof(Shell)),
			new StylePropertyAttribute("-xf-shell-unselected", typeof(Element), nameof(Shell.UnselectedColorProperty), typeof(Shell)),
		};
	}
}