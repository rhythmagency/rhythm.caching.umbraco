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
        /// This allows for multiple caches to be used (one for live, one for preview mode,
        /// and one for the back office).
        /// </remarks>
        public static string[] PreviewCacheKeys
        {
            get
            {

                // Variables.
                var key = default(string);
                var context = UmbracoContext.Current;
                var hasContext = context != null;

                // Is there an Umbraco context?
                if (hasContext)
                {
                    key = context.InPreviewMode
                        ? "Preview"
                        : "Live";
                }
                else
                {
                    key = "Back Office";
                }

                // Return the key (wrapped in an array).
                return new[] { key };

            }
        }

        #endregion

    }

}