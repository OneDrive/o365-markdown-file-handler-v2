using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarkdownFileHandler.Models
{
    public class AsyncActionModel
    {
        public string JobIdentifier { get; set; }

        public FileHandlerActions.JobStatus Status { get; set; }

        public string Title { get; set; }

    }
}