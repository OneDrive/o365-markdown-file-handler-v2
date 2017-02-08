namespace MarkdownFileHandler.Models
{
    public class SaveResults
    {
        public bool Success { get; set; }

        public string Error { get; set; }
    }

    public class ShareLinkResults : SaveResults
    {
        public string SharingUrl { get; set; }
    }
}