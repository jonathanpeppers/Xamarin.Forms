using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HelloForms
{
	public partial class MainPage : ContentPage
	{
		class ViewModel : INotifyPropertyChanged
		{
			string text;

			public string Text
			{
				get => text;
				set
				{
					text = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
				}
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public MainPage()
		{
			InitializeComponent();

			var vm = new ViewModel { Text = "FooBar" };
			for (int i = 0; i < 100; i++)
			{
				var label = new Label();
				label.BindingContext = vm;
				label.SetBinding(Label.TextProperty, "Text");
				MyLayout.Children.Add(label);
			}
		}
	}
}
