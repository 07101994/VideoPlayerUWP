using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace MyVideo
{
    class rootPage : Page
    {
        private FileActivatedEventArgs _fileEventArgs = null;
        public FileActivatedEventArgs FileEvent
        {
            get { return _fileEventArgs; }
            set { _fileEventArgs = value; }
        }

        private string _rootNamespace;

        public string RootNamespace
        {
            get { return _rootNamespace; }
            set { _rootNamespace = value; }
        }

        private ProtocolActivatedEventArgs _protocolEventArgs = null;
        public ProtocolActivatedEventArgs ProtocolEvent
        {
            get { return _protocolEventArgs; }
            set { _protocolEventArgs = value; }
        }

    }
}
