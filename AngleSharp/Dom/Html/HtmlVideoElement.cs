﻿namespace AngleSharp.Dom.Html
{
    using AngleSharp.Dom.Media;
    using AngleSharp.Extensions;
    using AngleSharp.Html;
    using AngleSharp.Services.Media;
    using System;

    /// <summary>
    /// Represents the HTML video element.
    /// </summary>
    sealed class HtmlVideoElement : HTMLMediaElement<IVideoInfo>, IHtmlVideoElement
    {
        #region Fields

        IVideoTrackList _videos;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new HTML video element.
        /// </summary>
        public HtmlVideoElement(Document owner, String prefix = null)
            : base(owner, Tags.Video, prefix)
        {
            _videos = null;
        }

        #endregion

        #region Properties

        public override IVideoTrackList VideoTracks
        {
            get { return _videos; }
        }

        /// <summary>
        /// Gets or sets the displayed width of the video element.
        /// </summary>
        public Int32 DisplayWidth
        {
            get { return GetOwnAttribute(AttributeNames.Width).ToInteger(OriginalWidth); }
            set { SetOwnAttribute(AttributeNames.Width, value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the displayed height of the video element.
        /// </summary>
        public Int32 DisplayHeight
        {
            get { return GetOwnAttribute(AttributeNames.Height).ToInteger(OriginalHeight); }
            set { SetOwnAttribute(AttributeNames.Height, value.ToString()); }
        }

        /// <summary>
        /// Gets the width of the video.
        /// </summary>
        public Int32 OriginalWidth
        {
            get { return _media != null ? _media.Width : 0; }
        }

        /// <summary>
        /// Gets the height of the video.
        /// </summary>
        public Int32 OriginalHeight
        {
            get { return _media != null ? _media.Height : 0; }
        }

        /// <summary>
        /// Gets or sets the URL to a preview image.
        /// </summary>
        public String Poster
        {
            get { return GetUrlAttribute(AttributeNames.Poster); }
            set { SetOwnAttribute(AttributeNames.Poster, value); }
        }

        #endregion
    }
}
