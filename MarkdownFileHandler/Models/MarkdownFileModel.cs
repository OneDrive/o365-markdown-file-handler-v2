using System;
using System.Web.Mvc;

namespace MarkdownFileHandler.Models
{
    public class MarkdownFileModel
    {
        private MarkdownFileModel(FileHandlerActivationParameters activationParameters)
        {
            this.ActivationParameters = activationParameters;
        }

        public FileHandlerActivationParameters ActivationParameters { get; set; }
        public MvcHtmlString HtmlFileContent { get; set; }
        public string MarkdownFileContent { get; set; }

        public string ErrorMessage { get; set; }

        public bool ReadOnly { get; set; }

        public string Filename { get; set; }


        public static MarkdownFileModel GetErrorModel(FileHandlerActivationParameters parameters, string errorMessage)
        {
            return new MarkdownFileModel(parameters) { ErrorMessage = errorMessage, ReadOnly = true };
        }

        public static MarkdownFileModel GetErrorModel(FileHandlerActivationParameters parameters, Exception ex)
        {
            return new MarkdownFileModel(parameters) { ErrorMessage = ex.Message, ReadOnly = true };
        }

        public static MarkdownFileModel GetReadOnlyModel(FileHandlerActivationParameters parameters, string filename, string markdownContent)
        {
            return new MarkdownFileModel(parameters)
            {
                MarkdownFileContent = markdownContent,
                HtmlFileContent = ConvertMarkdowntoHtml(markdownContent),
                Filename = filename,
                ReadOnly = true
            };
        }

        public static MarkdownFileModel GetWriteableModel(FileHandlerActivationParameters parameters, string filename, string markdownContent)
        {
            return new MarkdownFileModel(parameters)
            {
                MarkdownFileContent = markdownContent,
                HtmlFileContent = ConvertMarkdowntoHtml(markdownContent),
                Filename = filename,
                ReadOnly = false
            };
        }

        private static MvcHtmlString ConvertMarkdowntoHtml(string markdownContent)
        {
            return new MvcHtmlString(new MarkdownFile(markdownContent).TransformToHtml());
        }
    }
}