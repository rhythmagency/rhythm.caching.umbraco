namespace Rhythm.Caching.Umbraco
{

    // Namespaces.
    using global::Umbraco.Web;

    /// <summary>
    /// Assists with caching operations.
    /// </summary>
    public class CacheHelper
    {

        #region Properties

        /// <summary>
        /// Returns the keys to be used when caching, depending on if the site is currently
        /// in preview mode or not.
        /// </summary>
        /// <remarks>
        /// This allows for multiple caches to be used (one for live, and one for preview mode).
        /// </remarks>
        public static string[] PreviewCacheKeys => UmbracoContext.Current.InPreviewMode
            ? new[] { "Preview" }
            : new[] { "Live" };

        #endregion

    }

}