using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Widget;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class StreamImagesourceHandler : IImageSourceHandler
	{
		public async Task LoadImageAsync(ImageSource imageSource, ImageView imageView, CancellationToken cancellationToken = default(CancellationToken))
		{
			var streamsource = imageSource as StreamImageSource;
			Bitmap bitmap = null;
			try
			{
				if (streamsource?.Stream != null)
				{
					using (Stream stream = await ((IStreamImageSource)streamsource).GetStreamAsync(cancellationToken))
						bitmap = await BitmapFactory.DecodeStreamAsync(stream);
				}

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				if (bitmap != null)
				{
					if (!imageView.IsDisposed())
						imageView.SetImageBitmap(bitmap);
				}
				else
				{
					Log.Warning(nameof(ImageLoaderSourceHandler), "Image data was invalid: {0}", streamsource);
				}
			}
			finally
			{
				bitmap?.Dispose();
			}
		}
	}
}