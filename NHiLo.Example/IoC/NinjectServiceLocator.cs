using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using NHiLo.Example.ViewModel;

namespace NHiLo.Example.IoC
{
    public class NinjectServiceLocator
    {
        private readonly IKernel kernel;

        public NinjectServiceLocator()
        {
            kernel = new StandardKernel(new NHiLoExampleModule());
        }

        public PersonViewModel PersonViewModel
        {
            get { return kernel.Get<PersonViewModel>(); }
        }
    }
}
