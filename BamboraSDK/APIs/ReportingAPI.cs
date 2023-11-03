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
using Bambora.NA.SDK.Requests;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Bambora.NA.SDK
{
    public class ReportingAPI
    {
        private readonly ILogger _logger;
        private Configuration _configuration;
        private IWebCommandExecuter _webCommandExecuter = new WebCommandExecuter();

        public ReportingAPI(
            ILogger logger)
        {
            _logger = logger;
        }

        public Configuration Configuration
        {
            set { _configuration = value; }
        }

        public IWebCommandExecuter WebCommandExecuter
        {
            set { _webCommandExecuter = value; }
        }

        public Transaction GetTransaction(string paymentId)
        {
            string url = BamboraUrls.GetPaymentUrl
                .Replace("{v}", String.IsNullOrEmpty(_configuration.Version) ? "v1" : "v" + _configuration.Version)
                .Replace("{p}", String.IsNullOrEmpty(_configuration.Platform) ? "www" : _configuration.Platform)
                .Replace("{id}", paymentId);

            HttpsWebRequest req = new HttpsWebRequest(_logger)
            {
                MerchantId = _configuration.MerchantId,
                Passcode = _configuration.PaymentsApiPasscode,
                WebCommandExecutor = _webCommandExecuter
            };

            string response = req.ProcessTransaction(HttpMethod.Get, url);

            return JsonConvert.DeserializeObject<Transaction>(response);
        }

        /// <summary>
        /// Query for transaction data. You must specify a start date and an end date, as well as search criteria.
        /// You also specify the start row and end row for paging since the search will limit the number of returned results to 1000.
        /// </summary>
        /// <param name="reportName">Report name. </param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <param name="startRow">Start row.</param>
        /// <param name="endRow">End row.</param>
        /// <param name="criteria">Criteria.</param>
        public List<TransactionRecord> Query(DateTime startDate, DateTime endDate, int startRow, int endRow, params Criteria[] criteria)
        {
            if (endDate == null || startDate == null)
                throw new ArgumentNullException("Start Date and End Date cannot be null!");
            if (endDate < startDate)
                throw new ArgumentException("End Date cannot be less than Start Date!");
            if (endRow < startRow)
                throw new ArgumentException("End Row cannot be less than Start Row!");
            if (endRow - startRow > 1000)
                throw new ArgumentException("You cannot query more than 1000 rows at a time!");

            string url = BamboraUrls.ReportsUrl
                .Replace("{v}", String.IsNullOrEmpty(_configuration.Version) ? "v1" : "v" + _configuration.Version)
                .Replace("{p}", String.IsNullOrEmpty(_configuration.Platform) ? "www" : _configuration.Platform);

            HttpsWebRequest req = new HttpsWebRequest(_logger)
            {
                MerchantId = _configuration.MerchantId,
                Passcode = _configuration.ReportingApiPasscode,
                WebCommandExecutor = _webCommandExecuter
            };

            var query = new
            {
                name = "Search",
                start_date = startDate,
                end_date = endDate,
                start_row = startRow,
                end_row = endRow,
                criteria = criteria
            };

            string response = req.ProcessTransaction(HttpMethod.Post, url, query);
            Console.WriteLine("\n\n" + response + "\n\n");

            Records records = JsonConvert.DeserializeObject<Records>(response);

            return records.records;
        }

        private class Records
        {
            public List<TransactionRecord> records { get; set; }
        }
    }
}
