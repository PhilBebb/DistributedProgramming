// PhilBebb
using System;
using System.Net.Sockets;

namespace RemoteImplementations.Pipelining {

    public delegate void PipeHandledEventHandle(object sender, PipeHandledEventArgs e);
    public delegate void PipeFinishedEventHandle(object sender, PipeFinishedEventArgs e);

    public abstract class Pipe {

        /// <summary>
        /// Handles the request.
        /// Fires the event(s) then starts the next pipe
        /// </summary>
        /// <param name="tcpClient">Tcp client.</param>
        public virtual void HandleRequest(TcpClient tcpClient) {
            FireEvents(tcpClient);

            if(Next != null) {
                Next.HandleRequest(tcpClient);
            }
        }

        /// <summary>
        /// Gets or sets the next client to be used in the pipe.
        /// This should by default move a user backwards in the chain on failure
        /// </summary>
        /// <value>The next pipe</value>
        public Pipe Next { get; set; }

        /// <summary>
        /// The pipe handled event.
        /// Fires when the pipe has finised
        /// </summary>
        public PipeHandledEventHandle PipeHandledEvent;
       
        /// <summary>
        /// The pipe finished event.
        /// Fires when there is no Next Pipe
        /// </summary>
        public PipeFinishedEventHandle PipeFinishedEvent;

        /// <summary>
        /// Fires the events.
        /// Will only fire the PipeFinishedEvent if there is no next pipe
        /// </summary>
        /// <param name="tcpClient">Tcp client.</param>
        internal virtual void FireEvents(TcpClient tcpClient) {
            if(PipeHandledEvent != null) {
                PipeHandledEvent(this, new PipeHandledEventArgs(tcpClient));
            }

            if(Next == null) {
                if(PipeFinishedEvent != null) {
                    PipeFinishedEvent(this, new PipeFinishedEventArgs(tcpClient));
                }
            }
        }
    }

    public class PipeHandledEventArgs : EventArgs {

        public PipeHandledEventArgs(TcpClient tcpClient) {
            Client = tcpClient;   
        }

        public TcpClient Client { get; private set; }
    }

    public class PipeFinishedEventArgs : PipeHandledEventArgs {
        public PipeFinishedEventArgs(TcpClient tcpClient) : base(tcpClient) {
        }
    }
}

