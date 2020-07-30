using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HelloForms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BindingLabel : Label
	{
		public BindingLabel()
		{
			InitializeComponent();

			BindingContext = new { Text = "Hello" };
		}
	}
}