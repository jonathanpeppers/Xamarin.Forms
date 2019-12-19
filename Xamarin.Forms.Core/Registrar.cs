using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.Internals;
using Xamarin.Forms.StyleSheets;

namespace Xamarin.Forms
{
	[Flags]
	public enum InitializationFlags : long
	{
		DisableCss = 1 << 0
	}


	// Previewer uses reflection to bind to this method; Removal or modification of visibility will break previewer.
	internal static class Registrar
	{
		internal static void RegisterAll(Type[] attrTypes) => Internals.Registrar.RegisterAll(attrTypes);
	}
}

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class Registrar<TRegistrable> where TRegistrable : class
	{
		readonly Dictionary<Type, Dictionary<Type, (Type target, short priority)>> _handlers = new Dictionary<Type, Dictionary<Type, (Type target, short priority)>>();
		static Type _defaultVisualType = typeof(VisualMarker.DefaultVisual);
		static Type _materialVisualType = typeof(VisualMarker.MaterialVisual);

		static Type[] _defaultVisualRenderers = new[] { _defaultVisualType };

		public void Register(Type tview, Type trender, Type[] supportedVisuals, short priority)
		{
			supportedVisuals = supportedVisuals ?? _defaultVisualRenderers;
			//avoid caching null renderers
			if (trender == null)
				return;

			if (!_handlers.TryGetValue(tview, out Dictionary<Type, (Type target, short priority)> visualRenderers))
			{
				visualRenderers = new Dictionary<Type, (Type target, short priority)>();
				_handlers[tview] = visualRenderers;
			}

			for (int i = 0; i < supportedVisuals.Length; i++)
			{
				if(visualRenderers.TryGetValue(supportedVisuals[i], out (Type target, short priority) existingTargetValue))
				{
					if(existingTargetValue.priority <= priority)
						visualRenderers[supportedVisuals[i]] = (trender, priority);
				}
				else
					visualRenderers[supportedVisuals[i]] = (trender, priority);
			}
		}

		public void Register(Type tview, Type trender, Type[] supportedVisual) => Register(tview, trender, supportedVisual, 0);

		public void Register(Type tview, Type trender) => Register(tview, trender, _defaultVisualRenderers);

		internal TRegistrable GetHandler(Type type) => GetHandler(type, _defaultVisualType);

		internal TRegistrable GetHandler(Type type, Type visualType)
		{
			Type handlerType = GetHandlerType(type, visualType ?? _defaultVisualType);
			if (handlerType == null)
				return null;

			object handler = DependencyResolver.ResolveOrCreate(handlerType);

			return (TRegistrable)handler;
		}

		internal TRegistrable GetHandler(Type type, object source, IVisual visual, params object[] args)
		{
			if (args.Length == 0)
			{
				return GetHandler(type, visual?.GetType() ?? _defaultVisualType);
			}

			Type handlerType = GetHandlerType(type, visual?.GetType() ?? _defaultVisualType);
			if (handlerType == null)
				return null;

			
			return (TRegistrable)DependencyResolver.ResolveOrCreate(handlerType, source, visual?.GetType(), args);
		}

		public TOut GetHandler<TOut>(Type type) where TOut : class, TRegistrable
		{
			return GetHandler(type) as TOut;
		}

		public TOut GetHandler<TOut>(Type type, params object[] args) where TOut : class, TRegistrable
		{
			return GetHandler(type, null, null, args) as TOut;
		}

		public TOut GetHandlerForObject<TOut>(object obj) where TOut : class, TRegistrable
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			var reflectableType = obj as IReflectableType;
			var type = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : obj.GetType();

			return GetHandler(type, (obj as IVisualController)?.EffectiveVisual?.GetType()) as TOut;
		}

		public TOut GetHandlerForObject<TOut>(object obj, params object[] args) where TOut : class, TRegistrable
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			var reflectableType = obj as IReflectableType;
			var type = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : obj.GetType();

			return GetHandler(type, obj, (obj as IVisualController)?.EffectiveVisual, args) as TOut;
		}
		

		public Type GetHandlerType(Type viewType) => GetHandlerType(viewType, _defaultVisualType);

		public Type GetHandlerType(Type viewType, Type visualType)
		{
			visualType = visualType ?? _defaultVisualType;

			// 1. Do we have this specific type registered already?
			if (_handlers.TryGetValue(viewType, out Dictionary<Type, (Type target, short priority)> visualRenderers))
				if (visualRenderers.TryGetValue(visualType, out (Type target, short priority) specificTypeRenderer))
					return specificTypeRenderer.target;
				else if (visualType == _materialVisualType)
					VisualMarker.MaterialCheck();

			if (visualType != _defaultVisualType && visualRenderers != null)
				if (visualRenderers.TryGetValue(_defaultVisualType, out (Type target, short priority) specificTypeRenderer))
					return specificTypeRenderer.target;

			// 2. Do we have a RenderWith for this type or its base types? Register them now.
			RegisterRenderWithTypes(viewType, visualType);

			// 3. Do we have a custom renderer for a base type or did we just register an appropriate renderer from RenderWith?
			if (LookupHandlerType(viewType, visualType, out (Type target, short priority) baseTypeRenderer))
				return baseTypeRenderer.target;
			else
				return null;
		}

		public Type GetHandlerTypeForObject(object obj)
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			var reflectableType = obj as IReflectableType;
			var type = reflectableType != null ? reflectableType.GetTypeInfo().AsType() : obj.GetType();

			return GetHandlerType(type);
		}

		bool LookupHandlerType(Type viewType, Type visualType, out (Type target, short priority) handlerType)
		{
			visualType = visualType ?? _defaultVisualType;
			while (viewType != null && viewType != typeof(Element))
			{
				if (_handlers.TryGetValue(viewType, out Dictionary<Type, (Type target, short priority)> visualRenderers))
					if (visualRenderers.TryGetValue(visualType, out handlerType))
						return true;

				if (visualType != _defaultVisualType && visualRenderers != null)
					if (visualRenderers.TryGetValue(_defaultVisualType, out handlerType))
						return true;

				viewType = viewType.GetTypeInfo().BaseType;
			}

			handlerType = (null, 0);
			return false;
		}

		void RegisterRenderWithTypes(Type viewType, Type visualType)
		{
			visualType = visualType ?? _defaultVisualType;

			// We're going to go through each type in this viewType's inheritance chain to look for classes
			// decorated with a RenderWithAttribute. We're going to register each specific type with its
			// renderer.
			while (viewType != null && viewType != typeof(Element))
			{
				// Only go through this process if we have not registered something for this type;
				// we don't want RenderWith renderers to override ExportRenderers that are already registered.
				// Plus, there's no need to do this again if we already have a renderer registered.
				if (!_handlers.TryGetValue(viewType, out Dictionary<Type, (Type target, short priority)> visualRenderers) || 
					!(visualRenderers.ContainsKey(visualType) ||
					  visualRenderers.ContainsKey(_defaultVisualType)))
				{
					// get RenderWith attribute for just this type, do not inherit attributes from base types
					var attribute = viewType.GetTypeInfo().GetCustomAttributes<RenderWithAttribute>(false).FirstOrDefault();
					if (attribute == null)
					{
						// TODO this doesn't appear to do anything. Register just returns as a NOOP if the renderer is null
						Register(viewType, null, new[] { visualType }); // Cache this result so we don't have to do GetCustomAttributes again
					}
					else
					{
						Type specificTypeRenderer = attribute.Type;

						if (specificTypeRenderer.Name.StartsWith("_", StringComparison.Ordinal))
						{
							// TODO: Remove attribute2 once renderer names have been unified across all platforms
							var attribute2 = specificTypeRenderer.GetTypeInfo().GetCustomAttribute<RenderWithAttribute>();
							if (attribute2 != null)
							{
								for (int i = 0; i < attribute2.SupportedVisuals.Length; i++)
								{
									if (attribute2.SupportedVisuals[i] == _defaultVisualType)
										specificTypeRenderer = attribute2.Type;

									if (attribute2.SupportedVisuals[i] == visualType)
									{
										specificTypeRenderer = attribute2.Type;
										break;
									}
								}
							}

							if (specificTypeRenderer.Name.StartsWith("_", StringComparison.Ordinal))
							{
								Register(viewType, null, new[] { visualType }); // Cache this result so we don't work through this chain again

								viewType = viewType.GetTypeInfo().BaseType;
								continue;
							}
						}

						Register(viewType, specificTypeRenderer, new[] { visualType }); // Register this so we don't have to look for the RenderWithAttibute again in the future
					}
				}

				viewType = viewType.GetTypeInfo().BaseType;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class Registrar
	{
		static Registrar()
		{
			Registered = new Registrar<IRegisterable>();
		}

		internal static Dictionary<string, Type> Effects { get; } = new Dictionary<string, Type>();
		internal static Dictionary<string, IList<StylePropertyAttribute>> StyleProperties => LazyStyleProperties.Value;

		static readonly Lazy<Dictionary<string, IList<StylePropertyAttribute>>> LazyStyleProperties = new Lazy<Dictionary<string, IList<StylePropertyAttribute>>>(RegisterStylesheets);

		public static IEnumerable<Assembly> ExtraAssemblies { get; set; }

		public static Registrar<IRegisterable> Registered { get; internal set; }

		//typeof(ExportRendererAttribute);
		//typeof(ExportCellAttribute);
		//typeof(ExportImageSourceHandlerAttribute);
		public static void RegisterRenderers(HandlerAttribute[] attributes)
		{
			var length = attributes.Length;
			for (var i = 0; i < length; i++)
			{
				var attribute = attributes[i];
				if (attribute.ShouldRegister())
					Registered.Register(attribute.HandlerType, attribute.TargetType, attribute.SupportedVisuals, attribute.Priority);
			}
		}

		static Dictionary<string, IList<StylePropertyAttribute>> RegisterStylesheets()
		{
			var array = new[]
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
			var properties = new Dictionary<string, IList<StylePropertyAttribute>>();
			for (var i = 0; i < array.Length; i++)
			{
				var attribute = array[i];
				if (properties.TryGetValue(attribute.CssPropertyName, out var attrList))
					attrList.Add(attribute);
				else
					properties[attribute.CssPropertyName] = new List<StylePropertyAttribute>(1) { attribute };
			}
			return properties;
		}

		public static void RegisterEffects(string resolutionName, ExportEffectAttribute[] effectAttributes)
		{
			var exportEffectsLength = effectAttributes.Length;
			for (var i = 0; i < exportEffectsLength; i++)
			{
				var effect = effectAttributes[i];
				Effects[resolutionName + "." + effect.Id] = effect.Type;
			}
		}

		public static void RegisterAll(Type[] attrTypes)
		{
			Profile.FrameBegin();

			Assembly[] assemblies = Device.GetAssemblies();

			if (ExtraAssemblies != null)
				assemblies = assemblies.Union(ExtraAssemblies).ToArray();

			Assembly defaultRendererAssembly = Device.PlatformServices.GetType().GetTypeInfo().Assembly;
			int indexOfExecuting = Array.IndexOf(assemblies, defaultRendererAssembly);

			if (indexOfExecuting > 0)
			{
				assemblies[indexOfExecuting] = assemblies[0];
				assemblies[0] = defaultRendererAssembly;
			}

			// Don't use LINQ for performance reasons
			// Naive implementation can easily take over a second to run
			Profile.FramePartition("Reflect");
			foreach (Assembly assembly in assemblies)
			{
				Profile.FrameBegin(assembly.GetName().Name);

				foreach (Type attrType in attrTypes)
				{
					object[] attributes;
					try
					{
#if NETSTANDARD2_0
						attributes = assembly.GetCustomAttributes(attrType, true);
#else
						attributes = assembly.GetCustomAttributes(attrType).ToArray();
#endif
					}
					catch (System.IO.FileNotFoundException)
					{
						// Sometimes the previewer doesn't actually have everything required for these loads to work
						Log.Warning(nameof(Registrar), "Could not load assembly: {0} for Attibute {1} | Some renderers may not be loaded", assembly.FullName, attrType.FullName);
						continue;
					}

					var handlerAttributes = new HandlerAttribute[attributes.Length];
					Array.Copy(attributes, handlerAttributes, attributes.Length);
					RegisterRenderers(handlerAttributes);
				}

				string resolutionName = assembly.FullName;
				var resolutionNameAttribute = (ResolutionGroupNameAttribute)assembly.GetCustomAttribute(typeof(ResolutionGroupNameAttribute));
				if (resolutionNameAttribute != null)
					resolutionName = resolutionNameAttribute.ShortName;

#if NETSTANDARD2_0
				object[] effectAttributes = assembly.GetCustomAttributes(typeof(ExportEffectAttribute), true);
#else
				object[] effectAttributes = assembly.GetCustomAttributes(typeof(ExportEffectAttribute)).ToArray();
#endif
				var typedEffectAttributes = new ExportEffectAttribute[effectAttributes.Length];
				Array.Copy(effectAttributes, typedEffectAttributes, effectAttributes.Length);
				RegisterEffects(resolutionName, typedEffectAttributes);

				Profile.FrameEnd();
			}

			Profile.FramePartition("DependencyService.Initialize");
			DependencyService.Initialize(assemblies);

			Profile.FrameEnd();
		}
	}
}
