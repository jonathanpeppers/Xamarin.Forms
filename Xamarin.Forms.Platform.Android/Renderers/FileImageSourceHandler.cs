using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Net;
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class FileImageSourceHandler : IImageSourceHandler
	{
		public Task LoadImageAsync(ImageSource imageSource, ImageView imageView, CancellationToken cancellationToken = default(CancellationToken))
		{
			string name = ((FileImageSource)imageSource).File;
			int resource = ResourceManager.GetResourceByName(name);
			if (resource != 0)
			{
				imageView.SetImageResource(resource);
			}
			else
			{
				var file = new Java.IO.File(name);
				imageView.SetImageURI(Uri.FromFile(file));
			}
			return Task.FromResult(true);
		}
	}
}