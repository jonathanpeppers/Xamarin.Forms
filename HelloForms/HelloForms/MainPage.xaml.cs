using Xamarin.Forms;

namespace HelloForms
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			var layout = new StackLayout();
			for (int i = 0; i < 100; i++)
			{
				layout.Children.Add(new BindingLabel());
			}
			Content = layout;
		}
	}
}
