namespace Elmah.Blog.Identity.UI.Models
{
    /// <summary>
    /// Describes the error view model..
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Returns the request id when it isn't null'.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
    }
}
