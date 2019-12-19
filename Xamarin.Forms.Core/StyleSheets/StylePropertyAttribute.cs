using System;

namespace Xamarin.Forms.StyleSheets
{
	sealed class StylePropertyAttribute
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

		public StylePropertyAttribute(string cssPropertyName, Type targetType, string bindablePropertyName, bool inherited)
		{
			CssPropertyName = cssPropertyName;
			BindablePropertyName = bindablePropertyName;
			TargetType = targetType;
			Inherited = inherited;
		}

		public StylePropertyAttribute(string cssPropertyName, Type targetType, string bindablePropertyName, Type propertyOwnerType)
		{
			CssPropertyName = cssPropertyName;
			BindablePropertyName = bindablePropertyName;
			TargetType = targetType;
			PropertyOwnerType = propertyOwnerType;
		}
	}
}