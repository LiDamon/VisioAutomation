﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisioPowerTools2010
{
    public partial class FormCreateStyle : Form
    {
        public FormCreateStyle()
        {
            InitializeComponent();
        }

        public string StyleName
        {
            get { return this.textName.Text; }
        }

        public bool IncludesText
        {
            get { return this.checkBoxIncludesText.Checked; }
        }

        public bool IncludesLine
        {
            get { return this.checkBoxIncludesLIne.Checked; }
        }

        public bool IncludesFill
        {
            get { return this.checkBoxIncludesFill.Checked; }
        }

        private void FormCreateStyle_Load(object sender, EventArgs e)
        {

        }
    }
}