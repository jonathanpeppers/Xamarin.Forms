using System;
using System.Collections.Generic;
using Xamarin.Forms.Core;

namespace Xamarin.Forms.Platform.Android
{
	class AndroidAssemblyAttributesProvider : AssemblyAttributesProvider
	{
		public override DependencyAttribute[] GetDependencyAttributes() => new[]
		{
			new DependencyAttribute(typeof(Deserializer)),
			new DependencyAttribute(typeof(ResourcesProvider)),
		};

		public override HandlerAttribute[] GetHandlerAttributes() => new HandlerAttribute[]
		{
			new ExportRendererAttribute(typeof(Shell), typeof(ShellRenderer)),
			new ExportRendererAttribute(typeof(NativeViewWrapper), typeof(NativeViewWrapperRenderer)),
			new ExportCellAttribute(typeof(Cell), typeof(CellRenderer)),
			new ExportCellAttribute(typeof(EntryCell), typeof(EntryCellRenderer)),
			new ExportCellAttribute(typeof(SwitchCell), typeof(SwitchCellRenderer)),
			new ExportCellAttribute(typeof(TextCell), typeof(TextCellRenderer)),
			new ExportCellAttribute(typeof(ImageCell), typeof(ImageCellRenderer)),
			new ExportCellAttribute(typeof(ViewCell), typeof(ViewCellRenderer)),
			new ExportImageSourceHandlerAttribute(typeof(FileImageSource), typeof(FileImageSourceHandler)),
			new ExportImageSourceHandlerAttribute(typeof(StreamImageSource), typeof(StreamImagesourceHandler)),
			new ExportImageSourceHandlerAttribute(typeof(UriImageSource), typeof(ImageLoaderSourceHandler)),
			new ExportImageSourceHandlerAttribute(typeof(FontImageSource), typeof(FontImageSourceHandler)),
		};

		public override Dictionary<Type, Func<RenderWithAttribute>> GetRenderWithAttributes()
		{
			// NOTE: these are Func<T> so they don't allocate as much until called
			return new Dictionary<Type, Func<RenderWithAttribute>>
			{
				{ typeof (BoxView), () => new RenderWithAttribute(typeof (BoxRenderer)) },
				{ typeof (Entry), () => new RenderWithAttribute(typeof (EntryRenderer)) },
				{ typeof (Editor), () => new RenderWithAttribute(typeof (EditorRenderer)) },
				{ typeof (Label), () => new RenderWithAttribute(typeof (LabelRenderer)) },
				{ typeof (ImageRenderer), () => new RenderWithAttribute(typeof (ImageRenderer)) },
				{ typeof (Button), () => new RenderWithAttribute(typeof (ButtonRenderer)) },
				{ typeof (ImageButton), () => new RenderWithAttribute(typeof (ImageButtonRenderer)) },
				{ typeof (TableView), () => new RenderWithAttribute(typeof (TableViewRenderer)) },
				{ typeof (ListView), () => new RenderWithAttribute(typeof (ListViewRenderer)) },
				{ typeof (Xamarin.Forms.CollectionView), () => new RenderWithAttribute(typeof (CollectionViewRenderer)) },
				{ typeof (CarouselView), () => new RenderWithAttribute(typeof (CarouselViewRenderer)) },
				{ typeof (Slider), () => new RenderWithAttribute(typeof (SliderRenderer)) },
				{ typeof (WebView), () => new RenderWithAttribute(typeof (WebViewRenderer)) },
				{ typeof (SearchBar), () => new RenderWithAttribute(typeof (SearchBarRenderer)) },
				{ typeof (Switch), () => new RenderWithAttribute(typeof (SwitchRenderer)) },
				{ typeof (DatePicker), () => new RenderWithAttribute(typeof (DatePickerRenderer)) },
				{ typeof (TimePicker), () => new RenderWithAttribute(typeof (TimePickerRenderer)) },
				{ typeof (Picker), () => new RenderWithAttribute(typeof (PickerRenderer)) },
				{ typeof (Stepper), () => new RenderWithAttribute(typeof (StepperRenderer)) },
				{ typeof (ProgressBar), () => new RenderWithAttribute(typeof (ProgressBarRenderer)) },
				{ typeof (ScrollView), () => new RenderWithAttribute(typeof (ScrollViewRenderer)) },
				{ typeof (ActivityIndicator), () => new RenderWithAttribute(typeof (ActivityIndicatorRenderer)) },
				{ typeof (Frame), () => new RenderWithAttribute(typeof (FrameRenderer)) },
				{ typeof (IndicatorView), () => new RenderWithAttribute(typeof (IndicatorViewRenderer)) },
				//TODO: what is CheckBoxDesignerRenderer?
				{ typeof (CheckBox), () => new RenderWithAttribute(typeof (CheckBoxRenderer)) },
				{ typeof (OpenGLView), () => new RenderWithAttribute(typeof (OpenGLViewRenderer)) },
				{ typeof (TabbedPage), () => new RenderWithAttribute(typeof (TabbedRenderer)) },
				{ typeof (NavigationPage), () => new RenderWithAttribute(typeof (NavigationRenderer)) },
				{ typeof (CarouselPage), () => new RenderWithAttribute(typeof (CarouselPageRenderer)) },
				{ typeof (Page), () => new RenderWithAttribute(typeof (PageRenderer)) },
				{ typeof (MasterDetailPage), () => new RenderWithAttribute(typeof (MasterDetailRenderer)) },
				{ typeof (RefreshView), () => new RenderWithAttribute(typeof (RefreshViewRenderer)) },
				{ typeof (SwipeView), () => new RenderWithAttribute(typeof (SwipeViewRenderer)) },
			};
		}
	}
}