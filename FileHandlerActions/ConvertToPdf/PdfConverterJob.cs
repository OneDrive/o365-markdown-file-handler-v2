using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandlerActions
{
    public class PdfConverterJob
    {

        public JobStatus Status { get; private set; }

        public string Id { get { return Status.Id; } }

        public PdfConverterJob()
        {
            Status = new FileHandlerActions.JobStatus();
            Status.State = JobState.NotStarted;

            JobTracker.QueueJob(this.Status);
        }

        public async void BeginConvertToPdf(string sourceUrl, string accessToken = null)
        {
            Status.State = JobState.Running;
            PdfConversion converter = new FileHandlerActions.PdfConversion();

            try
            {
                var webUrl = await converter.ConvertFileToPdfAsync(sourceUrl, accessToken);
                Status.ResultWebUrl = webUrl;
                Status.State = JobState.Complete;
            }
            catch (ConverterException ex)
            {
                Status.State = JobState.Error;
                Status.Error = ex;
            }
        }


    }
}
