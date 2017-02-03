namespace FileHandlerActions
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
