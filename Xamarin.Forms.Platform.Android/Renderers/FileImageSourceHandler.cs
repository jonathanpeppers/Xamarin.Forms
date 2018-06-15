using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Android.Net;
using Android.Glide;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class FileImageSourceHandler : IImageSourceHandler, IImageViewHandler
	{
		// This is set to true when run under designer context
		internal static bool DecodeSynchronously {
			get;
			set;
		}

		public async Task<Bitmap> LoadImageAsync(ImageSource imagesource, Context context, CancellationToken cancelationToken = default(CancellationToken))
		{
			string file = ((FileImageSource)imagesource).File;
			Bitmap bitmap;
			if (File.Exists (file))
				bitmap = !DecodeSynchronously ? (await BitmapFactory.DecodeFileAsync (file).ConfigureAwait (false)) : BitmapFactory.DecodeFile (file);
			else
				bitmap = !DecodeSynchronously ? (await context.Resources.GetBitmapAsync (file).ConfigureAwait (false)) : context.Resources.GetBitmap (file);

			if (bitmap == null)
			{
				Log.Warning(nameof(FileImageSourceHandler), "Could not find image or image file was invalid: {0}", imagesource);
			}

			return bitmap;
		}

		public Task LoadImageAsync(ImageSource imagesource, ImageView imageView, CancellationToken cancellationToken = default(CancellationToken))
		{
			string file = ((FileImageSource)imagesource).File;
			if (File.Exists(file))
			{
				Glide.With(imageView.Context).Load(file).Into(imageView);
			}
			else
			{
				int resource = ResourceManager.GetDrawableByName(file);
				Glide.With(imageView.Context).Load(resource).Into(imageView);
			}

			return Task.FromResult(true);
		}
	}
}