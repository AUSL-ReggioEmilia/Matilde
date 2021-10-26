#region Using directives

using System;
using System.Drawing;
using Infragistics.Win;
using System.Windows.Forms;
using System.Diagnostics;

#endregion Using directives

namespace UnicodeSrl.ScciCore.CustomControls.Infra
{

    #region RichTextEditorKeyActionMappings
    public class RichTextEditorKeyActionMappings : KeyActionMappingsBase
    {
        #region Constructor
        public RichTextEditorKeyActionMappings() : base(5)
        {
        }
        #endregion Constructor

        #region LoadDefaultMappings
        public override void LoadDefaultMappings()
        {
            RichTextEditorKeyActionMapping[] defaultMappings = new RichTextEditorKeyActionMapping[]
            {
				#region OpenFileThroughDialog
																				new RichTextEditorKeyActionMapping( Keys.O ,
                                                    RichTextEditorAction.OpenFileThroughDialog,
                                                    0,
                                                    RichTextEditorStates.InEditMode,
                                                    SpecialKeys.AltShift,
                                                    SpecialKeys.Ctrl ),
				#endregion OpenFileThroughDialog

				#region SaveFileThroughDialog
																								new RichTextEditorKeyActionMapping( Keys.S ,
                                                    RichTextEditorAction.SaveFileThroughDialog,
                                                    0,
                                                    RichTextEditorStates.InEditMode,
                                                    SpecialKeys.AltShift,
                                                    SpecialKeys.Ctrl ),
				#endregion SaveFileThroughDialog

           };

            for (int i = 0; i < defaultMappings.Length; i++)
            {
                this.Add(defaultMappings[i]);
            }

        }
        #endregion LoadDefaultMappings

        #region Indexer
        public RichTextEditorKeyActionMapping this[int index]
        {
            get
            {
                this.VerifyMappingsAreLoaded();

                return (RichTextEditorKeyActionMapping)base.GetItem(index);
            }
            set
            {
                this.VerifyMappingsAreLoaded();

                base.List[index] = value;
            }
        }
        #endregion Indexer

        #region CopyTo
        public void CopyTo(RichTextEditorKeyActionMapping[] array, int index)
        {
            CopyTo((System.Array)array, index);
        }
        #endregion	CopyTo

        #region CreateActionStateMappingsCollection
        protected override KeyActionMappingsBase.ActionStateMappingsCollection CreateActionStateMappingsCollection()
        {
            return new ActionStateMappingsCollection
            (
                new ActionStateMapping[]
                {

                
					#region OpenFileThroughDialog
					new ActionStateMapping( RichTextEditorAction.OpenFileThroughDialog,
                                            (long)0,
                                            (long)RichTextEditorStates.InEditMode),
					#endregion OpenFileThroughDialog

					#region OpenFileThroughDialog
					new ActionStateMapping( RichTextEditorAction.SaveFileThroughDialog,
                                            (long)0,
                                            (long)RichTextEditorStates.InEditMode)
					#endregion OpenFileThroughDialog

				}
            );
        }
        #endregion	CreateActionStateMappingsCollection

    }
    #endregion RichTextEditorKeyActionMappings

    #region RichTextEditorKeyActionMapping class
    public class RichTextEditorKeyActionMapping : KeyActionMappingBase
    {

        #region Constructor
        public RichTextEditorKeyActionMapping(Keys keyCode,
RichTextEditorAction actionCode,
RichTextEditorStates stateDisallowed,
RichTextEditorStates stateRequired,
SpecialKeys specialKeysDisallowed,
SpecialKeys specialKeysRequired)

: base(keyCode,
actionCode,
(long)stateDisallowed,
(long)stateRequired,
specialKeysDisallowed,
specialKeysRequired)
        {
        }
        #endregion Constructor

        #region ActionCode
        public new RichTextEditorAction ActionCode
        {
            get
            {
                return (RichTextEditorAction)base.ActionCode;
            }
            set
            {
                base.ActionCode = value;
            }
        }
        #endregion ActionCode

    }
    #endregion RichTextEditorKeyActionMapping class

    #region Enumerations

    #region RichTextEditorStates enumeration
    [Flags]
    public enum RichTextEditorStates
    {
        InEditMode = 0x00000001,
    }
    #endregion	RichTextEditorStates enumeration

    #region RichTextEditorAction enumeration
    public enum RichTextEditorAction
    {
        OpenFileThroughDialog,

        SaveFileThroughDialog,

    }
    #endregion RichTextEditorAction enumeration

    #endregion Enumerations
}