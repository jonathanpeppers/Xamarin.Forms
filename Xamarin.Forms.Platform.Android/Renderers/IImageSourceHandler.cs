using System.Threading;
using System.Threading.Tasks;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public interface IImageSourceHandler : IRegisterable
	{
		Task LoadImageAsync(ImageSource imageSource, ImageView imageView, CancellationToken cancellationToken = default(CancellationToken));
	}
}