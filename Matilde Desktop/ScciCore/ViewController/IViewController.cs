using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicodeSrl.ScciCore.ViewController
{

    public interface IViewController
    {

        Maschera Maschera { get; set; }

        void InitViewController(IViewController viewcontroller);

        void LoadViewController();

        IViewController SaveViewController();

    }

    public interface IViewControllerTopNonModale : IViewController
    {

        ViewControllerTopNonModale ViewController { get; set; }

    }

    public interface IViewControllerBottomNonModale : IViewController
    {

        ViewControllerBottomNonModale ViewController { get; set; }

    }

    public interface IViewControllerMultiTaskInfermieristico : IViewController
    {

        ViewControllerMultiTaskInfermieristico ViewController { get; set; }

    }

    public interface IViewControllerDiarioClinico : IViewController
    {

        ViewControllerDiarioClinico ViewController { get; set; }

    }

    public interface IViewControllerScheda : IViewController
    {

        ViewControllerScheda ViewController { get; set; }

    }

}
