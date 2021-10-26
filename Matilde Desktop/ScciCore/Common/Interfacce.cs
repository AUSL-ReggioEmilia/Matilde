using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using UnicodeSrl.Scci.Enums;

namespace UnicodeSrl.ScciCore
{
    public static class Interfacce
    {

        public interface IViewUserControlBase
        {
            void ViewInit();
        }

        public interface IViewAgendaLista : IViewUserControlBase
        {
            string ViewCodAgenda { get; set; }
            string ViewDescrizioneAgenda { get; set; }
            string ViewParametriLista { get; set; }
            bool ViewPinnedFiltri { get; set; }
            bool ViewVisibleFiltri { get; set; }
            string ViewFiltroGenerico { get; set; }
        }

        public interface IViewMultiSelect : IViewUserControlBase
        {
            bool ViewShowFind { get; set; }
            bool ViewShowAll { get; set; }
            DataSet ViewDataSetSX { get; set; }
            DataSet ViewDataSetDX { get; set; }
        }

        public interface IViewPictureSelect : IViewUserControlBase
        {
            bool ViewCheckSquareImage { get; set; }
            Image ViewImage { get; set; }
            bool ViewShowSaveImage { get; set; }
            bool ViewShowToolbar { get; set; }
            ToolbarStyle ViewToolbarStyle { get; set; }
            bool ViewUseLargeImages { get; set; }
            enumZoomfactor ViewZoomFactor { get; set; }
            string ViewOpenFileDialogFilter { get; set; }
            string ViewSaveFileDialogFilter { get; set; }
        }

        public interface IViewRichTextBox : IViewUserControlBase
        {
            Font ViewFont { get; set; }
            bool ViewReadOnly { get; set; }
            String ViewRtf { get; set; }
            bool ViewShowInsertImage { get; set; }
            bool ViewShowToolbar { get; set; }
            ToolbarStyle ViewToolbarStyle { get; set; }
            String ViewText { get; set; }
            bool ViewUseLargeImages { get; set; }
        }

        public interface IViewUserControlMiddle
        {
            void Aggiorna();
            void Carica();
            void Ferma();
        }

        public interface IViewUserControlMiddlePlugin
        {
            object EseguiComando(Dictionary<string, object> dict_pars);
        }

        public interface IViewFormlModal
        {
            string CodiceMaschera { get; set; }
            void Carica();

            Object CustomParamaters { get; set; }
        }


        public interface IViewFormMain
        {
            string CodiceMaschera { get; set; }
            bool ControlloCentraleMassimizzato { get; set; }
            int ControlloCentraleTimerRefresh { get; set; }
        }

        public interface IViewCalendario : IViewUserControlBase
        {
            Calendario Calendario { get; set; }
        }

        #region EASY CONTROLS

        public interface IEasyShortcut
        {
            Keys ShortcutKey { get; set; }
            easyStatics.easyRelativeDimensions ShortcutFontRelativeDimension { get; set; }
            easyStatics.easyShortcutPosition ShortcutPosition { get; set; }
            Color ShortcutColor { get; set; }
            void PerformActionShortcut();
        }

        public interface IEasyShortcutMultiKey
        {
            List<Keys> ShortcutKeys { get; }

            void ActionKeyDown(Keys keyCode, Keys modifiers);
        }

        public interface IEasyResizableText
        {
            easyStatics.easyRelativeDimensions TextFontRelativeDimension { get; set; }
        }

        #endregion

    }

    public class MenuToolDefinition
    {
        public string Key { get; set; }
        public string Caption { get; set; }
        public Shortcut Shortcut { get; set; }

        public MenuToolDefinition(string key, string caption, Shortcut shortcut)
        {
            this.Key = key;
            this.Caption = caption;
            this.Shortcut = shortcut;
        }
    }
}
