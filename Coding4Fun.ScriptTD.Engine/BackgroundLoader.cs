using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Content;

namespace Coding4Fun.ScriptTD.Engine
{
    public delegate void LoadingDelegate(ContentManager content);

    public class BackgroundLoader
    {
        private readonly ContentManager _content;
        private readonly Queue<LoadingDelegate> _jobs;
        private readonly object _sync = new object();
        private Thread _thread;
        private bool _stop;
        private bool _running;

        public BackgroundLoader(IServiceProvider services)
        {
            _content = new ContentManager(services, "Content");
            _jobs = new Queue<LoadingDelegate>();
        }

        public void ClearJobs()
        {
            lock (_sync)
                _jobs.Clear();
        }

        public void BeginLoad(LoadingDelegate job)
        {
            lock (_sync)
            {
                _jobs.Enqueue(job);
            }

            if (_thread == null || !_thread.IsAlive)
            {
                _thread = new Thread(ThreadWorker) {Name = "BackgroundLoadThread", IsBackground = true};
            	_thread.Start();
            }
        }

    	private void ThreadWorker()
    	{
    		try
    		{
    			_running = true;
    			LoadingDelegate curJob;
    			lock (_sync)
    			{
    				curJob = _jobs.Dequeue();
    			}

    			while (curJob != null && !_stop)
    			{
    				curJob(_content);

    				lock (_sync)
    				{
    					curJob = _jobs.Count > 0 ? _jobs.Dequeue() : null;
    				}
    			}
    		}
    		finally
    		{
    			_running = false;
    		}
    	}

    	public void UnloadContent()
        {
            lock (_sync)
            {
                _stop = true;
            }

            if (_thread != null && _running)
                _thread.Join();

            _content.Unload();
            _stop = false;
        }
    }
}
