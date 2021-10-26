using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeSrl.ScciCore
{

    public class SelectionStrategyExtended_NoControlKey : SelectionStrategyContiguous
    {

        public SelectionStrategyExtended_NoControlKey(ISelectionManager selectionManager) : base(selectionManager) { }

        public override bool OnMouseDown(Infragistics.Shared.ISelectableItem item, ref MouseMessageInfo msginfo)
        {
            if (item.IsSelected)
                return this.SelectionManager.UnselectItem(item, false);
            else
                return this.SelectionManager.SelectItem(item, false);
        }

    }

    public class SelectionStrategyFilter : ISelectionStrategyFilter
    {

        ISelectionManager selectionManager = null;
        SelectionStrategyExtended_NoControlKey strategy = null;

        public SelectionStrategyFilter(ISelectionManager selectionManager)
        {
            this.selectionManager = selectionManager;
            this.strategy = new SelectionStrategyExtended_NoControlKey(selectionManager);
        }

        #region ISelectionStrategyFilter Members

        public ISelectionStrategy GetSelectionStrategy(Infragistics.Shared.ISelectableItem item)
        {
            if (item is UltraGridRow)
                return this.strategy;

            return null;
        }

        #endregion

    }

}
