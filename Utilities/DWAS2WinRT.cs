using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows;
using Windows.Foundation;
using Windows.System;
using Windows.Storage;

namespace DWAS2.Utilities
{
    class DWAS2WinRT
    {
        /// <summary>
        /// Set the lockscreen image, file MUST be in MyPictures
        /// </summary>
        /// <param name="filename">filename of image</param>
        /// <returns>true if succeed, false otherwise</returns>
        public static async void SetLockscreen(string filename, bool deleteAfterwards = false)
        {
            StorageFolder sfolder = KnownFolders.PicturesLibrary;
            StorageFile sfile = await sfolder.GetFileAsync(filename);
            await Windows.System.UserProfile.LockScreen.SetImageFileAsync(sfile);
            if (deleteAfterwards)
            {
                await sfile.DeleteAsync();
            }
        }
    }
}
