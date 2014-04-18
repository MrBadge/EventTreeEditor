using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventTreeEditor
{
    public partial class Exsercises : Form
    {
        public Exsercises()
        {
            InitializeComponent();
        }

        private void Exsercises_Shown(object sender, EventArgs e)
        {
            SQLManager.FillDataSet();
        }
    }
}
