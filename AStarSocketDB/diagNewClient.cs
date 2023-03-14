using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStarSocketDB
{
    public partial class diagNewClient : Form
    {
        public diagNewClient()
        {
            InitializeComponent();
        }

        public string ClientName { get { return txtclientname.Text; } }
    }
}
