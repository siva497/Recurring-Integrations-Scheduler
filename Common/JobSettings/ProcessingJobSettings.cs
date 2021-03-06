﻿/* Copyright (c) Microsoft Corporation. All rights reserved.
   Licensed under the MIT License. */

using Quartz;
using RecurringIntegrationsScheduler.Common.Contracts;
using RecurringIntegrationsScheduler.Common.Properties;
using System;
using System.Globalization;
using System.IO;

namespace RecurringIntegrationsScheduler.Common.JobSettings
{
    /// <summary>
    /// Serialize/deserialize processing job settings
    /// </summary>
    /// <seealso cref="RecurringIntegrationsScheduler.Common.Configuration.Settings" />
    public class ProcessingJobSettings : Settings
    {
        /// <summary>
        /// Initialize and verify settings for job
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="Quartz.JobExecutionException">
        /// </exception>
        public override void Initialize(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;

            base.Initialize(context);

            var activityIdStr = dataMap.GetString(SettingsConstants.ActivityId);
            if (!Guid.TryParse(activityIdStr, out Guid activityIdGuid) || (Guid.Empty == activityIdGuid))
            {
                throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture,
                    string.Format(
                        Resources.Activity_Id_of_recurring_job_is_missing_or_is_not_a_GUID_in_job_configuration,
                        context.JobDetail.Key)));
            }

            ActivityId = activityIdGuid;

            UploadSuccessDir = dataMap.GetString(SettingsConstants.UploadSuccessDir);
            if (!string.IsNullOrEmpty(UploadSuccessDir))
            {
                try
                {
                    Directory.CreateDirectory(UploadSuccessDir);
                }
                catch (Exception ex)
                {
                    throw new JobExecutionException(
                        string.Format(CultureInfo.InvariantCulture,
                            string.Format(Resources.Upload_success_directory_does_not_exist_or_cannot_be_accessed,
                                context.JobDetail.Key)), ex);
                }
            }
            else
            {
                throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture,
                    string.Format(Resources.Upload_success_directory_is_missing_in_job_configuration,
                        context.JobDetail.Key)));
            }

            ProcessingSuccessDir = dataMap.GetString(SettingsConstants.ProcessingSuccessDir);
            if (!string.IsNullOrEmpty(ProcessingSuccessDir))
            {
                try
                {
                    Directory.CreateDirectory(ProcessingSuccessDir);
                }
                catch (Exception ex)
                {
                    throw new JobExecutionException(
                        string.Format(CultureInfo.InvariantCulture,
                            string.Format(Resources.Processing_success_directory_does_not_exist_or_cannot_be_accessed,
                                context.JobDetail.Key)), ex);
                }
            }
            else
            {
                throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture,
                    string.Format(Resources.Processing_success_directory_is_missing_in_job_configuration,
                        context.JobDetail.Key)));
            }

            ProcessingErrorsDir = dataMap.GetString(SettingsConstants.ProcessingErrorsDir);
            if (!string.IsNullOrEmpty(ProcessingErrorsDir))
            {
                try
                {
                    Directory.CreateDirectory(ProcessingErrorsDir);
                }
                catch (Exception ex)
                {
                    throw new JobExecutionException(
                        string.Format(CultureInfo.InvariantCulture,
                            string.Format(Resources.Processing_errors_directory_does_not_exist_or_cannot_be_accessed,
                                context.JobDetail.Key)), ex);
                }
            }
            else
            {
                throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture,
                    string.Format(Resources.Processing_errors_directory_is_missing_in_job_configuration,
                        context.JobDetail.Key)));
            }

            StatusFileExtension = dataMap.GetString("StatusFileExtension");
            if (string.IsNullOrEmpty(StatusFileExtension))
            {
                throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture,
                    string.Format(Resources.Extension_of_status_files_is_missing_in_job_configuration,
                        context.JobDetail.Key)));
            }
        }

        #region Members

        /// <summary>
        /// Gets or sets the activity identifier.
        /// </summary>
        /// <value>
        /// The activity identifier.
        /// </value>
        public Guid ActivityId { get; private set; }

        /// <summary>
        /// Gets the upload success dir.
        /// </summary>
        /// <value>
        /// The upload success dir.
        /// </value>
        public string UploadSuccessDir { get; private set; }

        /// <summary>
        /// Gets the processing success dir.
        /// </summary>
        /// <value>
        /// The processing success dir.
        /// </value>
        public string ProcessingSuccessDir { get; private set; }

        /// <summary>
        /// Gets the processing errors dir.
        /// </summary>
        /// <value>
        /// The processing errors dir.
        /// </value>
        public string ProcessingErrorsDir { get; private set; }

        /// <summary>
        /// Gets the status file extension.
        /// </summary>
        /// <value>
        /// The status file extension.
        /// </value>
        public string StatusFileExtension { get; private set; }

        #endregion
    }
}