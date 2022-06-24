using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DBUp_Mysql
{
    public class AsyncHelper<TKey,TValue>
    {
        //
        //private static ReaderWriterLockSlim result_lock = new ReaderWriterLockSlim();
        private static object result_lock = new object();


        private int TaskCount;
        protected ConcurrentQueue<TKey> Data { private set; get; } = new ConcurrentQueue<TKey>();
        ////可以不需要这个变量
        //protected List<TKey> _ing_Data { private set; get; } = new List<TKey>();
        //protected List<TKey> _success_Data { private set; get; } = new List<TKey>();
        public Func<TKey, TValue> Action { get; set; }

        public Exception LastError { private set; get; } = null;

        public delegate void AsyncHander(TKey key, TValue value, int successCount = 0);
        public delegate void AsyncCountHander(int successCount);
        //public AsyncHander ErrorHander;
        public AsyncHander SuccessHander { set; get; }
        public AsyncCountHander SuccessCountHander { set; get; }
        private List<Thread> ThreadList = new List<Thread>();
        private Dictionary<TKey, TValue> Result = new Dictionary<TKey, TValue>();

        //public object ComObj;

        private int SuccessCount = 0;

        public AsyncHelper(int count, List<TKey> list, Func<TKey, TValue> fun) 
            : this(count, list)
        {
            Action = fun;
        }
        public AsyncHelper(int count, List<TKey> list) 
        {
            TaskCount = count;
            foreach (var item in list)
            {
                Data.Enqueue(item);
            }
        }

        int max_errorCount = 3;//总共发生错误的次数
        private void StartExecAction()
        {
            if (max_errorCount < 0)
                return;

            while (true)
            {
                if (max_errorCount < 0)
                    return;

                if (Data.Count == 0)
                    return;
                if (Data.TryDequeue(out TKey key) == false)
                    return;
                //_ing_Data.Add(key);
                try
                {

                    var tv = Action(key);
                    if (tv == null)
                    {
                        Data.Enqueue(key);
                        //_ing_Data.Remove(key);
                        Interlocked.Decrement(ref max_errorCount);
                        return;
                    }
                    lock (result_lock)
                    {
                        if (Result.ContainsKey(key))
                            Result[key] = tv;
                        else
                            Result.Add(key, tv);
                    }
                    //_ing_Data.Remove(key);
                    //_success_Data.Add(key);
                    Interlocked.Increment(ref SuccessCount);

                    //SuccessHander?.Invoke(key, tv, _success_Data.Count);
                    SuccessHander?.Invoke(key, tv, SuccessCount);
                }
                catch (Exception ex)
                {
                    Data.Enqueue(key);
                    //_ing_Data.Remove(key);
                    Interlocked.Decrement(ref max_errorCount);
                    if (max_errorCount < 0)
                    {
                        LastError = ex;
                        return;
                    }
                    //throw ex;
                }
            }

            return;
        }


        public void Start(bool isSync = false)
        {

            if (Action == null)
                throw new Exception("Action is null");

            ThreadList.Clear();
            int taskCount = Math.Min(TaskCount, Data.Count);
            //if (taskCount == 1)
            //{
            //    StartExecAction();
            //    return;
            //}
            for (int i = 0; i < taskCount; i++)
            {
                var t = new Thread(StartExecAction);
                t.IsBackground = true;
                ThreadList.Add(t);
                t.Start();
            }

            if (isSync)
            {
                do
                {
                    Thread.Sleep(50);
                } while (IsAllAbort() == false);
            }
            ////迭代运行写入日志记录
            //Parallel.For(0, LogCount, e =>
            //{
            //    WriteLog();
            //});
        }


        public Dictionary<TKey, TValue> Get()
        {
            return Result;
        }

        public bool IsAllAbort()
        {
            var endCount = 0;
            foreach (var workThreads in ThreadList)
            {
                if (workThreads.ThreadState == ThreadState.Aborted || workThreads.ThreadState == ThreadState.Stopped)
                {
                    endCount++;
                }
            }
            return ThreadList.Count == endCount;
        }
        //protected void Abort()
        //{
        //    foreach (var item in ThreadList)
        //    {
        //        item.Abort();
        //    }
        //}
    }
}
