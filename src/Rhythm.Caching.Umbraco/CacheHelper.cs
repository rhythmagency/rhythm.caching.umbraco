namespace Rhythm.Caching.Umbraco;

// Namespaces.
using global::Umbraco.Cms.Core.Web;

/// <summary>
/// Assists with caching operations.
/// </summary>
public class CacheHelper
{

    #region Public Methods

    /// <summary>
    /// Returns the keys to be used when caching, depending on if the site is currently
    /// in preview mode or not.
    /// </summary>
    /// <remarks>
    /// This allows for multiple caches to be used (one for live, one for preview mode,
    /// and one for the back office).
    /// </remarks>
    /// <param name="context">
    /// The current Umbraco context.
    /// </param>
    /// <returns>
    /// The cache keys.
    /// </returns>
    public static string[] PreviewCacheKeys(IUmbracoContext context)
    {
        // Variables.
        var key = default(string);
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

    #endregion

}