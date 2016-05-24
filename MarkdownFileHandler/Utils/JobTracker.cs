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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a limited use way of tracking asynchornous work for long running actions that span multiple HTTP requests.
    /// This is not a sufficent implementation to be used in a production service.
    /// </summary>
    public static class JobTracker
    {
        public static Dictionary<string, JobStatus> TrackedJobs = new Dictionary<string, JobStatus>();

        /// <summary>
        /// Queue a job into the job tracker and get back the unique identifier for the job
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string QueueJob(JobStatus status)
        {
            string id = Guid.NewGuid().ToString("b");
            TrackedJobs[id] = status;
            status.Id = id;

            return id;
        }


        /// <summary>
        /// Return the status of a job, based on the unique identifier provided when the job was queued.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JobStatus GetJob(string id)
        {
            JobStatus value;
            if (TrackedJobs.TryGetValue(id, out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Remove a job from the tracking queue
        /// </summary>
        /// <param name="id"></param>
        public static void Remove(string id)
        {
            TrackedJobs.Remove(id);
        }
    }


    public class JobStatus
    {
        public JobState State { get; set; }
        public Exception Error { get; set; }
        public string Id { get; internal set; }
        public string ResultWebUrl { get; internal set; }
        public Dictionary<string, string> OriginalParameters { get; set; }
    }

    public enum JobState
    {
        NotStarted,
        Running,
        Complete,
        Error
    }
}
