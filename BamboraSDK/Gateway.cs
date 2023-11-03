﻿// The MIT License (MIT)
//
// Copyright (c) 2018 Bambora, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using Bambora.NA.SDK.Data;
using Microsoft.Extensions.Logging;

/// <summary>
/// The entry-point into making payments and handling payment profiles.
///
/// This gives you access to the PaymentsAPI, ProfilesAPI, and the ReportingAPI.
///
/// Each API requires its own API key that you can obtain in the member area of your
/// Bambora account https://web.na.bambora.com/admin/sDefault.asp
///
/// You must set the MerchantID and one of the API keys before you use this class.
/// Exceptions will the thrown otherwise.
///
/// The login credentials are stored in a Configuration object.
///
/// This class is not threadsafe but designed to have its own instance per thread. If you
/// are making payments through multiple threads you want to create one Gateway object per thread.
/// </summary>
namespace Bambora.NA.SDK
{
    public class Gateway
    {
        private readonly ILogger _logger;

        private PaymentsAPI _payments;

        private ProfilesAPI _profiles;

        private ReportingAPI _reporting;

        public Gateway(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// The api version to use
        /// </summary>
        public string ApiVersion { get; set; }

        public Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = new Configuration();
                    _configuration.MerchantId = this.MerchantId;
                    _configuration.PaymentsApiPasscode = PaymentsApiKey;
                    _configuration.ReportingApiPasscode = ReportingApiKey;
                    _configuration.ProfilesApiPasscode = ProfilesApiKey;
                    _configuration.Version = ApiVersion;
                }
                return _configuration;
            }
        }

        /// <summary>
        /// The Bambora merchant ID
        /// </summary>
        public int MerchantId { get; set; }

        public PaymentsAPI Payments
        {
            get
            {
                if (_payments == null)
                    _payments = new PaymentsAPI(_logger);
                _payments.Configuration = Configuration;
                if (WebCommandExecuter != null)
                    _payments.WebCommandExecuter = WebCommandExecuter;
                return _payments;
            }
        }

        /// <summary>
        /// The API Key (Passcode) for accessing the payments API.
        /// </summary>
        public string PaymentsApiKey
        {
            set
            {
                Configuration.PaymentsApiPasscode = value;
            }
            get
            {
                return Configuration.PaymentsApiPasscode;
            }
        }

        public ProfilesAPI Profiles
        {
            get
            {
                if (_profiles == null)
                    _profiles = new ProfilesAPI(_logger);
                _profiles.Configuration = Configuration;
                if (WebCommandExecuter != null)
                    _profiles.WebCommandExecuter = WebCommandExecuter;
                return _profiles;
            }
        }

        /// <summary>
        /// The API Key (Passcode) for accessing the profiles API.
        /// </summary>
        public string ProfilesApiKey
        {
            set
            {
                Configuration.ProfilesApiPasscode = value;
            }
            get
            {
                return Configuration.ProfilesApiPasscode;
            }
        }

        public ReportingAPI Reporting
        {
            get
            {
                if (_reporting == null)
                    _reporting = new ReportingAPI(_logger);
                _reporting.Configuration = Configuration;
                if (WebCommandExecuter != null)
                    _reporting.WebCommandExecuter = WebCommandExecuter;
                return _reporting;
            }
        }

        /// <summary>
        /// The API Key (Passcode) for accessing the reporting API.
        /// </summary>
        public string ReportingApiKey
        {
            set
            {
                Configuration.ReportingApiPasscode = value;
            }
            get
            {
                return Configuration.ReportingApiPasscode;
            }
        }

        public IWebCommandExecuter WebCommandExecuter { get; set; }
        private Configuration _configuration { get; set; }

        public static void ThrowIfNullArgument(object value, string name)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(name);
            }
        }
    }
}
