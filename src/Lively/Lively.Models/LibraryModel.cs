﻿using Lively.Common;
using Lively.Common.Helpers.MVVM;
using System;
using System.ComponentModel;
using System.IO;

namespace Lively.Models
{
    public enum LibraryItemType
    {
        [Description("Importing..")]
        processing,
        [Description("Import complete.")]
        ready,
        cmdImport,
        multiImport,
        edit
    }

    [Serializable]
    public class LibraryModel : ObservableObject, ILibraryModel
    {
        public LibraryModel(LivelyInfoModel data, string folderPath, LibraryItemType tileType = LibraryItemType.ready, bool preferPreviewGif = false)
        {
            DataType = tileType;
            WallpaperCategory = "TODO: Localize";
            LivelyInfo = new LivelyInfoModel(data);
            Title = data.Title;
            Desc = data.Desc;
            Author = data.Author;
            try
            {
                SrcWebsite = LinkHandler.SanitizeUrl(data.Contact);
            }
            catch
            {
                SrcWebsite = null;
            }

            if (data.IsAbsolutePath)
            {
                //full filepath is stored in Livelyinfo.json metadata file.
                FilePath = data.FileName;

                //This is to keep backward compatibility with older wallpaper files.
                //When I originally made the property all the paths where made absolute, not just wallpaper path.
                //But previewgif and thumb are always inside the temporary lively created folder.
                try
                {
                    //PreviewClipPath = data.Preview;
                    PreviewClipPath = Path.Combine(folderPath, Path.GetFileName(data.Preview));
                }
                catch
                {
                    PreviewClipPath = null;
                }

                try
                {
                    //ThumbnailPath = data.Thumbnail;
                    ThumbnailPath = Path.Combine(folderPath, Path.GetFileName(data.Thumbnail));
                }
                catch
                {
                    ThumbnailPath = null;
                }

                try
                {
                    LivelyPropertyPath = Path.Combine(Directory.GetParent(data.FileName).ToString(), "LivelyProperties.json");
                }
                catch
                {
                    LivelyPropertyPath = null;
                }
            }
            else
            {
                //Only relative path is stored, this will be inside "Lively Wallpaper" folder.
                if (data.Type == WallpaperType.url
                || data.Type == WallpaperType.videostream)
                {
                    //no file.
                    FilePath = data.FileName;
                }
                else
                {
                    try
                    {
                        FilePath = Path.Combine(folderPath, data.FileName);
                    }
                    catch
                    {
                        FilePath = null;
                    }

                    try
                    {
                        LivelyPropertyPath = Path.Combine(folderPath, "LivelyProperties.json");
                    }
                    catch
                    {
                        LivelyPropertyPath = null;
                    }
                }

                try
                {
                    PreviewClipPath = Path.Combine(folderPath, data.Preview);
                }
                catch
                {
                    PreviewClipPath = null;
                }

                try
                {
                    ThumbnailPath = Path.Combine(folderPath, data.Thumbnail);
                }
                catch
                {
                    ThumbnailPath = null;
                }
            }

            LivelyInfoFolderPath = folderPath;
            //Use animated gif if exists.
            ImagePath = preferPreviewGif ?
                (File.Exists(PreviewClipPath) ? PreviewClipPath : ThumbnailPath) : ThumbnailPath;

            if (data.Type == WallpaperType.video ||
                data.Type == WallpaperType.videostream ||
                data.Type == WallpaperType.gif ||
                data.Type == WallpaperType.picture)
            {
                //No user made livelyproperties file if missing, using default for video.
                if (LivelyPropertyPath == null)
                {
                    LivelyPropertyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                        "plugins", "mpv", "api", "LivelyProperties.json");
                }
            }

            ItemStartup = false;
        }

        private LivelyInfoModel _livelyInfo;
        public LivelyInfoModel LivelyInfo
        {
            get
            {
                return _livelyInfo;
            }
            set
            {
                _livelyInfo = value;
                OnPropertyChanged();
            }
        }

        private LibraryItemType _dataType;
        public LibraryItemType DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;
                OnPropertyChanged();
            }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (LivelyInfo.Type == WallpaperType.url
                || LivelyInfo.Type == WallpaperType.videostream)
                {
                    _filePath = value;
                }
                else
                {
                    _filePath = File.Exists(value) ? value : null;
                }
                OnPropertyChanged();
            }
        }

        private string _livelyInfoFolderPath;
        public string LivelyInfoFolderPath
        {
            get { return _livelyInfoFolderPath; }
            set
            {
                _livelyInfoFolderPath = value;
                OnPropertyChanged();
            }
        }

        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = string.IsNullOrWhiteSpace(value) ? "---" : (value.Length > 100 ? value.Substring(0, 100) : value);
                OnPropertyChanged();
            }
        }

        private string _author;
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = string.IsNullOrWhiteSpace(value) ? "---" : (value.Length > 100 ? value.Substring(0, 100) : value);
                OnPropertyChanged();
            }
        }

        private string _desc;
        public string Desc
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = string.IsNullOrWhiteSpace(value) ? WallpaperCategory : (value.Length > 5000 ? value.Substring(0, 5000) : value);
                OnPropertyChanged();
            }
        }

        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        private string _previewClipPath;
        public string PreviewClipPath
        {
            get
            {
                return _previewClipPath;
            }
            set
            {
                _previewClipPath = File.Exists(value) ? value : null;
                OnPropertyChanged();
            }
        }

        private string _thumbnailPath;
        public string ThumbnailPath
        {
            get
            {
                return _thumbnailPath;
            }
            set
            {
                _thumbnailPath = File.Exists(value) ? value : null;
                OnPropertyChanged();
            }
        }

        private Uri _srcWebsite;
        public Uri SrcWebsite
        {
            get
            {
                return _srcWebsite;
            }
            set
            {
                _srcWebsite = value;
                OnPropertyChanged();
            }
        }

        private string _wallpaperCategory;
        /// <summary>
        /// Localised wallpapertype text.
        /// </summary>
        public string WallpaperCategory
        {
            get
            {
                return _wallpaperCategory;
            }
            set
            {
                _wallpaperCategory = value;
                OnPropertyChanged();
            }
        }

        private string _livelyPropertyPath;
        /// <summary>
        /// LivelyProperties.json filepath if present, null otherwise.
        /// </summary>
        public string LivelyPropertyPath
        {
            get { return _livelyPropertyPath; }
            set
            {
                _livelyPropertyPath = File.Exists(value) ? value : null;
                OnPropertyChanged();
            }
        }

        private bool _itemStartup;
        public bool ItemStartup
        {
            get { return _itemStartup; }
            set
            {
                _itemStartup = value;
                OnPropertyChanged();
            }
        }
    }
}
