﻿//
//  Copyright (C) 2011 LShift Ltd.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//         http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Net.LShift.Diffa.Participants
{
  public abstract class BaseScanningParticipant : IScanningParticipantContract {
    public Stream Scan() {
      // Retrieve the query parameters for the request
      var queryParameters = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters;

      // Determine constraints and aggregation
      var constraints = DetermineConstraints(queryParameters);
      var aggregations = DetermineAggregations(queryParameters);

      try {
        var results = Query(constraints, aggregations);
        return new MemoryStream(Encoding.UTF8.GetBytes(new JArray(results.Select(r => r.ToJObject())).ToString()));
      } catch (Exception ex) {
        Console.WriteLine(ex);

        throw;
      }
    }

    /// <summary>
    /// Callback method to be implemented by subclasses to return the version information within the given constraint and aggregation bounds.
    /// </summary>
    /// <param name="constraints">the constraints to query within</param>
    /// <param name="aggregations">the aggregation factors to apply to the result</param>
    /// <returns>the version information to return to the client</returns>
    protected abstract IEnumerable<ScanResultEntry> Query(IQueryConstraint[] constraints, ICategoryFunction[] aggregations);

    /// <summary>
    /// Callback method to be implemented by subclasses allowing the request to be inspected to determine the appropriate
    /// constraints.
    /// </summary>
    /// <param name="requestParams">the parameters to the scan request</param>
    /// <returns>an array of query constraints, or IQueryContraints[0] if no constraints are specified</returns>
    protected virtual IQueryConstraint[] DetermineConstraints(NameValueCollection requestParams) {
      return new IQueryConstraint[0];
    }

    /// <summary>
    /// Callback method to be implemented by subclasses allowing the request to be inspected to determine the appropriate
    /// aggregation factors.
    /// </summary>
    /// <param name="requestParams">the parameters to the scan request</param>
    /// <returns>an array of category functions, or ICategoryFunction[0] if no aggregation factors are specified</returns>
    protected virtual ICategoryFunction[] DetermineAggregations(NameValueCollection requestParams) {
      return new ICategoryFunction[0];
    }
  }
}
