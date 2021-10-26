#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Infragistics.Win;
using ExtendedRichTextBox;

#endregion Using directives

namespace UnicodeSrl.ScciCore.CustomControls.Infra
{

	#region EmbeddableRichTextBox class
															public class EmbeddableRichTextBox : RichTextBoxPrintCtrl
    {
		#region Member variables

		private RichTextEditorEmbeddableUIElement		richTextEditorUIElement = null;
		private bool									suspendEventFiring = true;

		#endregion Member variables

		#region Constructor
												internal EmbeddableRichTextBox()
		{
		}
		#endregion Constructor

		#region Methods

			#region Control-class overrides
																		#region OnTextChanged
														protected override void OnTextChanged( EventArgs e )
		{
												if ( this.IsInEditMode && this.suspendEventFiring == false )
			{
				RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
				editor.InternalRaiseValueChanged();
			}

			base.OnTextChanged( e );
		}
			#endregion OnTextChanged

			#region OnKeyDown
														protected override void OnKeyDown( KeyEventArgs e )
		{
			if ( this.IsInEditMode )
			{
				RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
				editor.InternalRaiseKeyDown( e );

																																if ( e.Handled )
					return;

																				RichTextEditor richTextEditor = this.richTextEditorUIElement.Editor as RichTextEditor;
				if ( richTextEditor.PerformKeyAction(e.KeyData) )
					return;

			}

			base.OnKeyDown( e );
		}
			#endregion OnKeyDown

			#region OnKeyUp
														protected override void OnKeyUp( KeyEventArgs e )
		{
			if ( this.IsInEditMode )
			{
				RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
				editor.InternalRaiseKeyUp( e );

																																if ( e.Handled )
					return;
			}

			base.OnKeyUp( e );
		}
			#endregion OnKeyDown

			#region OnKeyPress
														protected override void OnKeyPress( KeyPressEventArgs e )
		{
			if ( this.IsInEditMode )
			{
				RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
				editor.InternalRaiseKeyPress( e );

																																if ( e.Handled )
					return;
			}

			base.OnKeyPress( e );
		}
			#endregion OnKeyPress

			#region IsInputKey
												protected override bool IsInputKey( Keys keyData )
		{
						if ( ! this.IsInEditMode )
				return base.IsInputKey( keyData );

																																													if ( base.IsInputKey( keyData ) )
				return true;

			if ( this.richTextEditorUIElement.Owner.IsKeyMapped( keyData, this.richTextEditorUIElement ) )
				return true;

			return this.richTextEditorUIElement.Editor.IsInputKey( keyData );
		}

												internal protected bool InternalIsInputKey( Keys keyData )
		{
			return this.IsInputKey( keyData );
		}

			#endregion IsInputKey

                    #region OnContentsResized

        internal int renderedRTFContentHeight { get; set; }
        protected override void OnContentsResized(ContentsResizedEventArgs e)
        {
            this.renderedRTFContentHeight = e.NewRectangle.Height;
            base.OnContentsResized(e);
        }

            #endregion
        
			#endregion Control-class overrides

			#region Helper methods

			#region EnterEditMode
												public bool EnterEditMode( RichTextEditorEmbeddableUIElement richTextEditorUIElement )
		{
						if ( richTextEditorUIElement == null )
				throw new ArgumentNullException();

															this.richTextEditorUIElement = richTextEditorUIElement;

									this.WordWrap = this.richTextEditorUIElement.Owner.WrapText( this.richTextEditorUIElement.OwnerContext );
			this.ReadOnly = this.richTextEditorUIElement.Owner.IsReadOnly( this.richTextEditorUIElement.OwnerContext );
			this.Multiline = this.richTextEditorUIElement.Owner.IsMultiLine( this.richTextEditorUIElement.OwnerContext );

						System.Windows.Forms.ScrollBars scrollbars = this.richTextEditorUIElement.Owner.GetTextBoxScrollBars( this.richTextEditorUIElement.OwnerContext );

															RichTextBoxScrollBars propertyVal = RichTextBoxScrollBars.None;
			switch ( scrollbars )
			{
				case System.Windows.Forms.ScrollBars.Both:{ propertyVal = RichTextBoxScrollBars.Both; } break;
				case System.Windows.Forms.ScrollBars.Horizontal:{ propertyVal = RichTextBoxScrollBars.Horizontal; } break;
				case System.Windows.Forms.ScrollBars.None:{ propertyVal = RichTextBoxScrollBars.None; } break;
				case System.Windows.Forms.ScrollBars.Vertical:{ propertyVal = RichTextBoxScrollBars.Vertical; } break;
			}
			this.ScrollBars = propertyVal;

						if ( this.Visible == false )
				this.Visible = true;

						Control owningControl = this.richTextEditorUIElement.Owner.GetControl( this.richTextEditorUIElement.OwnerContext );
			if ( owningControl != null )
				owningControl.Controls.Add( this );

						this.suspendEventFiring = false;

						return this.Focus();
		}
			#endregion EnterEditMode

			#region ExitEditMode
								public void ExitEditMode()
		{
						this.suspendEventFiring = true;

						this.Text = string.Empty;

						Control owningControl = this.richTextEditorUIElement.Owner.GetControl( this.richTextEditorUIElement.OwnerContext );
			if ( owningControl != null )
				owningControl.Focus();

						this.Visible = false;

						this.richTextEditorUIElement = null;
		}
			#endregion ExitEditMode

			#region IsValidRTF
												public bool IsValidRTF( string rtf )
		{
			string previousValue = this.Rtf;
			try
			{
				this.Rtf = rtf;
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				this.Rtf = previousValue;
			}
		}
			#endregion IsValidRTF

			#region InternalInvokePaintBackground
										internal void InternalInvokePaintBackground( Graphics g, Rectangle rect )
		{
			PaintEventArgs e = new PaintEventArgs( g, rect );
			this.InvokePaintBackground( this, e );
		}
			#endregion InternalInvokePaintBackground

			#endregion Helper methods

		#endregion Methods

		#region Public Properties

								
       
			#region BackColor
												public override Color BackColor
		{
			get
			{
				                if (this.IsInEditMode == false)
                {
                                        
                    return base.BackColor;
                }

																AppearancePropFlags requestedProps = AppearancePropFlags.BackColor;
				AppearanceData appearance = new AppearanceData();
				bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(	this.richTextEditorUIElement.OwnerContext,
																						ref appearance,
																						ref requestedProps );

				if ( ! resolved )
					return base.BackColor;

				Color backColor = appearance.BackColor;
				if ( backColor.A < 255 )
					backColor = Color.FromArgb( 255, backColor );

				return backColor;
			}
            set
            {
                                                if(this.IsInEditMode == false)
                    base.BackColor = value;
            }
		}
			#endregion BackColor

			#region ForeColor
												public override Color ForeColor
		{
			get
			{
								if ( this.IsInEditMode == false )
					return base.ForeColor;

																AppearancePropFlags requestedProps = AppearancePropFlags.ForeColor;
				AppearanceData appearance = new AppearanceData();
				bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(	this.richTextEditorUIElement.OwnerContext,
																						ref appearance,
																						ref requestedProps );

				if ( ! resolved )
					return base.ForeColor;

				Color foreColor = appearance.ForeColor;
				if ( foreColor.A < 255 )
					foreColor = Color.FromArgb( 255, foreColor );

				return foreColor;
			}

									set{}
		}
			#endregion ForeColor

			#region Cursor
												public override Cursor Cursor
		{
			get
			{
								if ( this.IsInEditMode == false )
					return base.Cursor;

																AppearancePropFlags requestedProps = AppearancePropFlags.Cursor;
				AppearanceData appearance = new AppearanceData();
				bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(	this.richTextEditorUIElement.OwnerContext,
																						ref appearance,
																						ref requestedProps );

				if ( ! resolved || appearance.Cursor == null )
					return base.Cursor;

				return appearance.Cursor;
			}

			set{}
		}
			#endregion Cursor

			#region Font
												public override Font Font
		{
			get
			{
								if ( this.IsInEditMode == false )
					return base.Font;

																AppearancePropFlags requestedProps = AppearancePropFlags.FontData;
				AppearanceData appearance = new AppearanceData();
				bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(	this.richTextEditorUIElement.OwnerContext,
																						ref appearance,
																						ref requestedProps );

				if ( ! resolved )
					return base.Font;

				Font createdFont = appearance.CreateFont( base.Font );
				if ( createdFont != null )
					return createdFont;
				else
					return base.Font;
			}

			set{}
		}
			#endregion Font

			#region MaxLength
										public override int MaxLength
		{
			get
			{ 
								if ( this.IsInEditMode == false )
					return base.MaxLength;

																int maxLength = base.MaxLength;
				if ( this.richTextEditorUIElement.Owner.GetMaxLength(this.richTextEditorUIElement.OwnerContext, out maxLength) )
					return maxLength;
				else
					return base.MaxLength;
			}

									set{}
		}
			#endregion MaxLength

		#endregion Public Properties

		#region Private properties

			#region IsInEditMode
								private bool IsInEditMode
		{
			get
			{ 
				return	this.richTextEditorUIElement != null &&
						this.richTextEditorUIElement.IsInEditMode;
			}
		}
			#endregion IsInEditMode

		#endregion Private properties
	}
	#endregion EmbeddableRichTextBox class

	#region RichTextBoxRenderer class
				public class RichTextBoxRenderer
	{
		#region Constants

		private const int			WM_USER = 0x400;
		private const int			EM_FORMATRANGE = WM_USER + 57;
		private const int			EM_DISPLAYBAND = WM_USER + 51;
		private const float			twipsPerInch = 1440f;

		#endregion	Constants

		#region Member Variables

		private RichTextBox			richTextBox;
		private int					currentCharacter;
		private int					nextCharacterToRender;
		private int					textLength;
		private float				pixelsPerInchX = 96;
		private float				pixelsPerInchY = 96;

		#endregion	Member Variables

		#region Constructor
										public RichTextBoxRenderer( RichTextBox richTextBox )
		{
			this.richTextBox = richTextBox;
			this.textLength = this.richTextBox.Text.Length;
		}
		#endregion	Constructor

		#region Properties

			#region RichTextBox
								public RichTextBox RichTextBox
		{
			get { return this.richTextBox; }
		}
			#endregion	RichTextBox

		#endregion	Properties

		#region Methods

	    #region Render
										        		public void Render( Graphics g, Rectangle rect, Rectangle clipRect)
		{
						if (this.richTextBox == null)
				return;

						this.pixelsPerInchX = g.DpiX;
			this.pixelsPerInchY = g.DpiY;

						this.currentCharacter = this.nextCharacterToRender = 0;
			this.textLength = this.richTextBox.Text.Length;

            int savedDcState = 0;
            IntPtr clipRegionHandle = IntPtr.Zero;
            IntPtr hdc = IntPtr.Zero;
            try
            {
                                                g.SetClip(clipRect);

                                                clipRegionHandle = g.Clip.GetHrgn(g);

                                hdc = g.GetHdc();

                                savedDcState = NativeWindowMethods.SaveDC(hdc);

                                NativeWindowMethods.SelectClipRgn(hdc, clipRegionHandle);

                                while (this.nextCharacterToRender < this.textLength)
                {
                    this.nextCharacterToRender = this.currentCharacter;
                    this.currentCharacter = this.RenderingHelper(hdc, g, rect);
                }
            }
            finally
            {
                if (hdc != IntPtr.Zero && savedDcState != 0)
                    NativeWindowMethods.RestoreDC(hdc, savedDcState);

                g.ReleaseHdc(hdc);

                g.ResetClip();

                if (clipRegionHandle != IntPtr.Zero)
                    NativeWindowMethods.DeleteObject(clipRegionHandle);
            }

		}
			#endregion Render

			#region RenderingHelper
								        private int RenderingHelper(IntPtr hdc, Graphics g, Rectangle rect)
        {
            CHARRANGE cr = new CHARRANGE();
            cr.cpMin = this.currentCharacter;
            cr.cpMax = this.textLength;


            RECT rc = new RECT();
            rc.left = rect.Left;
            rc.top = rect.Top;
            rc.right = rect.Right;
            rc.bottom = rect.Bottom;

            float scalingFactorX = this.pixelsPerInchX;
            float scalingFactorY = this.pixelsPerInchY;
            rc.left = (int)(rect.Left / scalingFactorX * twipsPerInch);
            rc.top = (int)(rect.Top / scalingFactorY * twipsPerInch);
            rc.right = (int)(rect.Right / scalingFactorX * twipsPerInch);

                                                rc.bottom = (int)(short.MaxValue / scalingFactorY * twipsPerInch);

            RECT rcPage = new RECT();

            IntPtr hwnd = this.richTextBox.Handle;
            HandleRef handleRefHwnd = new HandleRef(this.richTextBox, hwnd);
            HandleRef handleRefHdc = new HandleRef(g, hdc);

            FORMATRANGE fr = new FORMATRANGE();
            fr.hdc = hdc;
            fr.hdcTarget = hdc;
            fr.chrg = cr;
            fr.rc = rc;
            fr.rcPage = rcPage;

            IntPtr nextCharacterToRender = SendMessage(handleRefHwnd, EM_FORMATRANGE, IntPtr.Zero, ref fr);

            int cpMin = nextCharacterToRender.ToInt32();
                                                            if (cpMin < fr.chrg.cpMin)
            {
                                                cpMin = fr.chrg.cpMax + 1;
            }

            SendMessage(handleRefHwnd, EM_DISPLAYBAND, IntPtr.Zero, ref fr.rc);

            SendMessage(handleRefHwnd, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero);

            return cpMin;
        }
			#endregion	RenderingHelper

		#endregion	Methods

		#region APIs

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
		
		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, ref FORMATRANGE range);

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, ref RECT rect);

		#endregion	APIs

		#region Nested Types

			#region RECT
		[StructLayout(LayoutKind.Sequential)]
			internal struct RECT 
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}
			#endregion	RECT

			#region CHARRANGE
		[StructLayout(LayoutKind.Sequential)]
			internal struct CHARRANGE
		{
			public int cpMin;
			public int cpMax;
		}
			#endregion	CHARRANGE

			#region FORMATRANGE
		[StructLayout(LayoutKind.Sequential)]
			internal struct FORMATRANGE
		{
			public IntPtr hdc; 
			public IntPtr hdcTarget; 
			public RECT rc; 
			public RECT rcPage; 
			public CHARRANGE chrg; 
		}
			#endregion	FORMATRANGE

		#endregion	Nested Types
	}
	#endregion RichTextBoxRenderer class

}

