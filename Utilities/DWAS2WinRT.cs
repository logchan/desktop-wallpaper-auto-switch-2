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
        /// Set the lockscreen image, file MUST be in MyPictures (or its subfolder)
        /// </summary>
        /// <param name="relativePath">relative path of image</param>
        public static async void SetLockscreen(string relativePath, bool deleteAfterwards = false)
        {
            string[] path = relativePath.Split('\\');

            StorageFolder sfolder = KnownFolders.PicturesLibrary;

            // navigate to picture folder
            // note: last item of path is filename
            for(int i = 0; i < path.Length - 1; ++i)
            {
                if (path[i] == "") continue;
                sfolder = await sfolder.GetFolderAsync(path[i]);
            }

            StorageFile sfile = await sfolder.GetFileAsync(path[path.Length - 1]);
            await Windows.System.UserProfile.LockScreen.SetImageFileAsync(sfile);
            if (deleteAfterwards)
            {
                await sfile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }
    }
}
