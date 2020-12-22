using Microsoft.Extensions.Configuration;
using NHiLo.Common.Config.Legacy;
using NHiLo.HiLo.Repository;
using System;
using System.Windows.Forms;

namespace NHiLo.Examples.CustomHiLoRepositoryApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IKeyGenerator<long> _generator;

        private void Form1_Load(object sender, EventArgs e)
        {
            HiLoRepositoryFactory.RegisterRepository("myRepository", () => new CustomFileHiLoRepository());
            var factory = new HiLoGeneratorFactory();
            _generator = factory.GetKeyGenerator("myEntity");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = _generator.GetKey().ToString();
        }
    }
}
