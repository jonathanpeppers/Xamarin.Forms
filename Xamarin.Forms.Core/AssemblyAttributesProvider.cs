using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms.StyleSheets;

namespace Xamarin.Forms.Core
{
	/// <summary>
	/// As an alternative to definition assembly-level custom attributes.
	/// This class provides a more performant way to declare the same data.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class AssemblyAttributesProvider
	{
		public AssemblyAttributesProvider()
		{
			Assembly = GetType().GetTypeInfo().Assembly;
		}

		internal Assembly Assembly { get; private set; }

		public virtual HandlerAttribute[] GetHandlerAttributes() => null;

		public virtual ExportEffectAttribute[] GetExportEffectAttributes() => null;

		public virtual ResolutionGroupNameAttribute GetResolutionGroupNameAttribute() => null;

		public virtual DependencyAttribute[] GetDependencyAttributes() => null;

		public virtual Dictionary<Type, Func<RenderWithAttribute>> GetRenderWithAttributes() => null;
	}
}
