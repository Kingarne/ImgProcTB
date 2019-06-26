using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImgProcTB
{
    public partial class ControlForm : Form
    {
        public Form1 Form;
        public ControlForm(Form1 form)
        {
            InitializeComponent();
            Form = form;
            propertyGrid.SelectedObject = form.tracker;
        }

        private void InitButton_Click(object sender, EventArgs e)
        {
            Form.m_init = true;
        }
    }
}
