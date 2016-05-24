/*
 * Markdown File Handler - Sample Code
 * Copyright (c) Microsoft Corporation
 * All rights reserved. 
 * 
 * MIT License
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the ""Software""), to deal in 
 * the Software without restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
 * Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
 
namespace MarkdownFileHandler
{
    using System.Threading.Tasks;

    public class AsyncJob
    {
        public JobStatus Status { get; private set; }

        public string Id { get { return Status.Id; } }

        private IAsyncJob Job { get; set; }

        public AsyncJob(IAsyncJob job)
        {
            this.Job = job;
            Status = new MarkdownFileHandler.JobStatus();
            Status.State = JobState.NotStarted;
            JobTracker.QueueJob(this.Status);
        }

        public async void Begin(string[] sourceUrls, string accessToken = null)
        {
            Status.State = JobState.Running;

            try
            {
                var webUrl = await Job.DoWorkAsync(sourceUrls, accessToken);

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

    public interface IAsyncJob
    {
        Task<string> DoWorkAsync(string[] sourceUrls, string accessToken);
    }
}
