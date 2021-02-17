using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace SingleProject
{
	public partial class App
	{
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}
	}
}
