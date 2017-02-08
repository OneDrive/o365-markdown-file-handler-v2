using MarkdownDeep;

namespace MarkdownFileHandler.Models
{
    public class MarkdownFile : ApiDocs.Validation.DocFile
    {
        private readonly string markdownSource;

        public MarkdownFile(string source)
        {
            this.markdownSource = source;

            this.FullPath = "";
            this.DisplayName = "MarkdownFile.md";
            this.Parent = null;
        }
        protected override string GetContentsOfFile()
        {
            return this.markdownSource;
        }

        public string TransformToHtml()
        {
            Markdown md = new Markdown
            {
                SafeMode = false,
                ExtraMode = true
            };

            return md.Transform(this.markdownSource);
        }
    }
}