
namespace Sparkle.LinkedInNET.DemoMvc5.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DataTransaction<TData> : IDisposable
    {
        private bool isDisposed;
        private Action<TData> disposeAction;

        public DataTransaction(TData data, Action<TData> disposeAction)
        {
            this.Data = data;
            this.disposeAction = disposeAction;
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.disposeAction(this.Data);
                    this.disposeAction = null;
                }

                this.isDisposed = true;
            }
        }

        public TData Data { get; set; }
    }
}
