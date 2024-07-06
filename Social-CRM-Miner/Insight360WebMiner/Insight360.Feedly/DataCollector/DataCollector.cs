using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insight360.Feedly.DataCollector
{
    public class DataCollector
    {
        public DataCollector() { }
        //private int Frequency;
        //ManualResetEvent stopEvent = new ManualResetEvent(false);

        //List<Thread> threads = new List<Thread>();

        //public void Start()
        //{
        //    threads.Add(new Thread(new ThreadStart(SearchThread)));
        //    foreach (Thread thread in threads)
        //    {
        //        thread.Start();
        //    }
        //}

        //public void SearchThread()
        //{
        //    DateTime dt = DateTime.Now;
        //    Util.Log.WriteOperationStarted(this.GetType(), "SearchThread", "Thread Enter Instagram");
        //    try
        //    {
        //        FeedlyClient searchClient = new FeedlyClient(stopEvent);
        //        bool exit = false;
        //        exit = stopEvent.WaitOne(1);

        //        while (!exit)
        //        {
        //            var docSources = DAL.ConfigDAL.FetchDocumentSourceConfiguration("Feedly");
        //            Frequency = DAL.ConfigDAL.GetFrequency("Feedly") * 60000;
        //            foreach (var source in docSources)
        //            {
        //                foreach (var search in source.SearchTerms)
        //                {
        //                    if (!exit)// && search.IsTermEnabled == 1)
        //                    {
        //                        dt = searchClient.FetchFeedlyFeeds(search, source);
        //                    }
        //                }
        //            }
        //            exit = stopEvent.WaitOne(Frequency);

        //            if (docSources.Count == 0)
        //                exit = stopEvent.WaitOne(5 * 60 * 1000);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Util.Log.WriteException(this.GetType(), "SearchThread FEEDLY", ex);
        //    }
        //}

        //public void Stop()
        //{
        //    stopEvent.Set();

        //    foreach (Thread thread in threads)
        //    {
        //        thread.Join();
        //    }
        //}
    }
}
