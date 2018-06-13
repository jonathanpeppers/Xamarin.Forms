using System.Threading;
using System.Threading.Tasks;
using Android.Widget;
using AUri = Android.Net.Uri;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class ImageLoaderSourceHandler : IImageSourceHandler
	{
		public Task LoadImageAsync(ImageSource imageSource, ImageView imageView, CancellationToken cancelationToken = default(CancellationToken))
		{
			var uri = ((UriImageSource)imageSource).Uri;
			imageView.SetImageURI(AUri.Parse(uri.OriginalString));
			return Task.FromResult(true);
		}
	}
}