using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Insight360.Util
{
    public enum DataProcessingState
    {
        UnProcessed = 0,
        Processing,
        Processed,
        KeywordsNotFound,
        DocumentSourceNotFound,
        UnProcessed_DocumentUpdate=100,
        Refresh=1
    }

	public enum DataPersistenceMode
	{
		SingleDataSource ,
		MultipleDataSources
	}

    public enum MessageStatus
    {
        Draft = 0,
        Scheduled = 1,
        Approved = 2,
        Reject = 3,
        Sent=4
    }

    public enum Sentiments
    {
        Positive = 1,
        Neutral = 2,
        Negative = 3,
        Unknown = 4
    }
}
