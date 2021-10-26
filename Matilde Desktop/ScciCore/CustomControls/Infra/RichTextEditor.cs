
#region Using directives

using System;
using System.Drawing;
using Infragistics.Win;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

#endregion Using directives

namespace UnicodeSrl.ScciCore.CustomControls.Infra
{
	#region RichTextEditor class
																public class RichTextEditor : EmbeddableEditorBase
	{
		#region Constants

		private const string NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE = "The property or method cannot be accessed when the editor in not in edit mode.";
		private const string VALUE_PROPERTY_WRONG_DATA_TYPE_EXCEPTION_MESSAGE = "The Value property can only be set to a value that is of type string.";
		private const string CURRENT_VALUE_INVALID_EXCEPTION_MESSAGE = "The current value is not valid.";
		private static readonly Size IDEAL_SIZE = new Size( 100, 96 );
		
		#endregion Constants

		#region Member variables

		private EmbeddableRichTextBox				richTextBoxEditor = null;
		private EmbeddableRichTextBox				richTextBoxRenderer = null;
		private RichTextEditorKeyActionMappings		keyActionMappings = null;

                private Bitmap                              _overflowImage = Properties.Resources.ContinuaPuntini_32;
        private Bitmap                              _overflowImageNoOverflow = null;
        private int                                 _overflowDifference = 4;
        private bool                                _overflowSkip = false;
        
        #endregion Member variables

		#region Constructors

								public RichTextEditor()
		{
		}

																																		public RichTextEditor( EmbeddableEditorOwnerBase defaultOwner ) : base( defaultOwner )
		{
		}

		#endregion Constructors

		#region Abstract property/method overrides

			#region CanFocus
										public override bool CanFocus
		{
			get{ return true; }
		}
			#endregion CanFocus

			#region Focused
								public override bool Focused
		{
			get
			{ 
								if ( ! this.IsInEditMode )
					return false;

												return this.RichTextBoxEditor.Focused; 
			}
		}
			#endregion Focused

			#region Focus
										public override bool Focus()
		{
			if ( ! this.IsInEditMode || ! this.RichTextBoxEditor.CanFocus )
				return false;

			if ( ! this.RichTextBoxEditor.Visible )
				this.RichTextBoxEditor.Show();

			return this.RichTextBoxEditor.Focus();
		}
			#endregion Focus

			#region CanEditType
												public override bool CanEditType( System.Type type )
		{
												return type == typeof(string);
		}		
			#endregion CanEditType

			#region CanRenderType
												public override bool CanRenderType( System.Type type )
		{
									return true;
		}
			#endregion CanRenderType

			#region GetEmbeddableElement
	
																												public override EmbeddableUIElementBase GetEmbeddableElement(	UIElement parentElement,
																		EmbeddableEditorOwnerBase owner,
																		object ownerContext,
																		bool includeEditElements,
																		bool reserveSpaceForEditElements,
																		bool drawOuterBorders,
																		bool isToolTip,
																		EmbeddableUIElementBase previousElement )
		{
			RichTextEditorEmbeddableUIElement prevTextElement = null;

			if ( previousElement != null &&
				 previousElement.GetType() == typeof(RichTextEditorEmbeddableUIElement) )
				 prevTextElement = previousElement as RichTextEditorEmbeddableUIElement;

			if ( null != previousElement && this.ElementBeingEdited == previousElement )
			{
				previousElement.Initialize( owner,
											this,
											ownerContext,
											includeEditElements,
											reserveSpaceForEditElements,
											drawOuterBorders,
											isToolTip );

				return previousElement;
			}

			if ( prevTextElement != null &&
				 prevTextElement.Parent == parentElement &&
				 previousElement is RichTextEditorEmbeddableUIElement )
			{
												previousElement.Initialize( owner,
											this,
											ownerContext,
											includeEditElements,
											reserveSpaceForEditElements,
											drawOuterBorders,
											isToolTip );

				return previousElement;
			}

			return new RichTextEditorEmbeddableUIElement( parentElement,
												owner,
												this,
												ownerContext,
												includeEditElements,
												reserveSpaceForEditElements,
												drawOuterBorders,
												isToolTip );
		}
		

			#endregion	GetEmbeddableElement

			#region GetEmbeddableElementType
										public override Type GetEmbeddableElementType( )
		{
			return typeof( RichTextEditorEmbeddableUIElement );
		}
			#endregion	GetEmbeddableElementType

			#region GetSize
												protected override Size GetSize( ref EditorSizeInfo sizeInfo )
		{
			return RichTextEditor.IDEAL_SIZE;
		}
			#endregion GetSize

			#region IsInputKey
												public override bool IsInputKey( Keys keyData )
		{
			return this.KeyActionMappings.IsKeyMapped( keyData, (long)this.CurrentState );
		}
			#endregion IsInputKey

			#region Clone
												public override EmbeddableEditorBase Clone(EmbeddableEditorOwnerBase defaultOwner)
		{
			RichTextEditor editor = new RichTextEditor(defaultOwner);
			editor.InitializeFrom(this);
			return editor;
		}
			#endregion	Clone

			#region CurrentEditText
																public override string CurrentEditText
		{ 
			get
			{ 
				if ( ! this.IsInEditMode )
					throw new Exception( NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );
				
				return this.RichTextBoxEditor.Text;
			} 
		}

			#endregion CurrentEditText

			#region Value
																								public override object Value
		{
			get
			{
								if ( this.IsInEditMode == false )
					throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

								if ( this.IsValid == false )
					throw new Exception( RichTextEditor.CURRENT_VALUE_INVALID_EXCEPTION_MESSAGE );

				if ( this.RichTextBoxEditor.Text.Length == 0 )
					return null;

				return this.RichTextBoxEditor.Rtf;
			}

			set
			{
								if ( this.IsInEditMode == false )
					throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );


								string stringValue = value as string;
				if ( stringValue == null )
					throw new Exception( RichTextEditor.VALUE_PROPERTY_WRONG_DATA_TYPE_EXCEPTION_MESSAGE );

												if ( this.RichTextBoxEditor.IsValidRTF(stringValue) )
					this.RichTextBoxEditor.Rtf = stringValue;
				else
					this.RichTextBoxEditor.Text = stringValue;
			}
		}
			#endregion Value

			#region IsValid
												public override bool IsValid
		{
			get
			{
								if ( ! this.IsInEditMode )
					throw new Exception( NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

																bool nullable = this.ElementBeingEdited.Owner.IsNullable( this.ElementBeingEdited.OwnerContext );
				if ( ! nullable && this.CurrentEditText.Length == 0 )
					return false;
				
												return true;
			}
		}
			#endregion IsValid

        public RichTextBoxScrollBars ScrollBars
        {
            get
            {
                try
                {
                    return this.RichTextBoxEditor.ScrollBars; 
                }
                catch (Exception)
                {
                    throw;
                }
            }
            set
            {
                try
                {
                    this.RichTextBoxEditor.ScrollBars = value;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

                    #region Overflow

        public bool OverflowSkip
        {
            get
            {
                return _overflowSkip;
            }
            set
            {
                _overflowSkip = value;
            }
        }

        public Bitmap OverflowImage
        {
            get
            {
                return _overflowImage;
            }
            set
            {
                _overflowImage = value;
            }
        }

        public Bitmap OverflowImageNoOverflow
        {
            get
            {
                return _overflowImageNoOverflow;
            }
            set
            {
                _overflowImageNoOverflow = value;
            }
        }

        public int OverflowDifference
        {
            get
            {
                return _overflowDifference;
            }
            set
            {
                _overflowDifference = value;
            }
        }

            #endregion
        
		#endregion Abstract property/method overrides

		#region Base class overrides

			#region OnBeforeEnterEditMode
												protected override void OnBeforeEnterEditMode( ref bool cancel )
		{
						base.OnBeforeEnterEditMode( ref cancel );

			if ( ! cancel )
			{
				if ( this.ElementBeingEdited != null )
				{
															string ownerValue = this.ElementBeingEdited.Owner.GetValue( this.ElementBeingEdited.OwnerContext ) as string;
					
										this.RichTextBoxEditor.Text = string.Empty;

					if ( ownerValue != null )
					{
						if ( this.RichTextBoxEditor.IsValidRTF(ownerValue) )
							this.RichTextBoxEditor.Rtf = ownerValue;
						else
							this.RichTextBoxEditor.Text = ownerValue;
					}

															RichTextEditorEmbeddableUIElement richTextEditorElement = this.ElementBeingEdited as RichTextEditorEmbeddableUIElement;
					Rectangle rect = richTextEditorElement.EditArea;
					this.RichTextBoxEditor.SetBounds( rect.Left, rect.Top, rect.Width, rect.Height );

										this.RichTextBoxEditor.EnterEditMode( this.ElementBeingEdited as RichTextEditorEmbeddableUIElement );
				}
			}
		}
			#endregion	OnBeforeEnterEditMode

			#region	OnBeforeExitEditMode		
														protected override void OnBeforeExitEditMode( ref bool cancel, bool forceExit, bool applyChanges )
		{
			base.OnBeforeExitEditMode( ref cancel, forceExit, applyChanges );

			if ( ! cancel )
			{
												this.RichTextBoxEditor.ExitEditMode();
			}

		}
			#endregion	OnBeforeExitEditMode

			#region GetDisplayValue
										protected override string GetDisplayValue()
		{
			return this.CurrentEditText;
		}
			#endregion GetDisplayValue

			#region GetEditorValue
										protected override object GetEditorValue()
		{
			return this.RichTextBoxEditor.Rtf;
		}
			#endregion GetEditorValue

		#endregion Base class overrides

		#region Internal properties

			#region RichTextBoxEditor
										internal EmbeddableRichTextBox RichTextBoxEditor
		{
			get
			{
								if ( this.richTextBoxEditor == null )
				{
					this.richTextBoxEditor = new EmbeddableRichTextBox();

										this.richTextBoxEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
				
				}

				return this.richTextBoxEditor;
			}
		}
			#endregion RichTextBoxEditor

			#region RichTextBoxRenderer
																		public EmbeddableRichTextBox RichTextBoxRenderer
		{
			get
			{
                                if ( this.richTextBoxRenderer == null )
                {
                    this.richTextBoxRenderer = new EmbeddableRichTextBox();

                                        this.richTextBoxRenderer.BorderStyle = System.Windows.Forms.BorderStyle.None;
                }

				return this.richTextBoxRenderer;
			}
		}
			#endregion RichTextBoxRenderer

			#region CurrentState
										internal RichTextEditorStates CurrentState
		{
			get
			{
				long state = 0;

				if ( this.IsInEditMode )
					state |= (long)RichTextEditorStates.InEditMode;

				return (RichTextEditorStates)state;
			}
		}
			#endregion CurrentState

		#endregion Internal properties

		#region Public properties

        public int RenderedContentHeight
        {
            get
            {
                return this.RichTextBoxRenderer.renderedRTFContentHeight;
            }
        }

			#region KeyActionMappings
										[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public RichTextEditorKeyActionMappings  KeyActionMappings
		{
			get
			{
				if ( null == this.keyActionMappings )
					this.keyActionMappings = new RichTextEditorKeyActionMappings();

				return this.keyActionMappings;
			}
		}
			#endregion KeyActionMappings

		#endregion Public properties

		#region Methods

			#region Extended functionality methods

			#region LoadFile
												public void LoadFile( string path )
		{
			if ( ! this.IsInEditMode )
				throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

			this.RichTextBoxEditor.LoadFile( path );
		}
			#endregion LoadFile

			#region SaveFile
												public void SaveFile( string path )
		{
			if ( ! this.IsInEditMode )
				throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

			this.RichTextBoxEditor.SaveFile( path );
		}
			#endregion SaveFile

			#region LoadFileThroughDialog
														public void LoadFileThroughDialog()
		{
			if ( ! this.IsInEditMode )
				throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

			OpenFileDialog dialog = new OpenFileDialog();

            string dataPath = Common.DataPath;

			if (dataPath != null)
				dialog.InitialDirectory = dataPath;

			dialog.DefaultExt = "rtf";
			dialog.FileName = "*.rtf";
			dialog.Filter = "Rtf Files (*.rtf)|*.rtf|All Files (*.*)|*.*";
			DialogResult result = dialog.ShowDialog();


			if ( result == DialogResult.OK )
			{
				this.LoadFile( dialog.FileName );
			}
		}
			#endregion LoadFileThroughDialog
	
			#region SaveFileThroughDialog
														public void SaveFileThroughDialog()
		{
			if ( ! this.IsInEditMode )
				throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = "rtf";
			dialog.FileName = "*.rtf";
			DialogResult result = dialog.ShowDialog();

			if ( result == DialogResult.OK )
			{
				this.SaveFile( dialog.FileName );
			}
		}
			#endregion SaveFileThroughDialog
	
			#endregion Extended functionality methods

			#region Protected method accessors

			#region InternalRaiseValueChanged
								internal void InternalRaiseValueChanged()
		{
			this.RaiseValueChangedEvent();
		}
			#endregion InternalRaiseValueChanged

			#region InternalRaiseKeyDown
								internal void InternalRaiseKeyDown( KeyEventArgs e )
		{
			this.RaiseKeyDownEvent( e );
		}
			#endregion InternalRaiseKeyDown

			#region InternalRaiseKeyPress
								internal void InternalRaiseKeyPress( KeyPressEventArgs e )
		{
			this.RaiseKeyPressEvent( e );
		}
			#endregion InternalRaiseKeyPress

			#region InternalRaiseKeyUp
								internal void InternalRaiseKeyUp( KeyEventArgs e )
		{
			this.RaiseKeyUpEvent( e );
		}
			#endregion InternalRaiseKeyUp

			#endregion Protected method accessors

			#region Helper routines

			#region PerformKeyAction
																internal bool PerformKeyAction( Keys keyData )
		{
												long currentState = (long)this.CurrentState;
			if ( this.KeyActionMappings.IsKeyMapped( keyData, currentState ) )
			{
				RichTextEditorKeyActionMapping keyMapping = null;

																SpecialKeys specialKeys = (SpecialKeys)0;
				
				if ( (keyData & Keys.Alt) == Keys.Alt )
				{
					specialKeys |= SpecialKeys.Alt;
					keyData &= ~Keys.Alt;
				}

				if ( (keyData & Keys.Control) == Keys.Control )
				{
					specialKeys |= SpecialKeys.Ctrl;
					keyData &= ~Keys.Control;
				}

				if ( (keyData & Keys.Shift) == Keys.Shift )
				{
					specialKeys |= SpecialKeys.Shift;
					keyData &= ~Keys.Shift;
				}


												KeyActionMappingBase[] keyMappings = this.KeyActionMappings.GetActionMappings( keyData, currentState, specialKeys );

								for ( int i = 0; i < keyMappings.Length; i ++ )
				{
					keyMapping = keyMappings[i] as RichTextEditorKeyActionMapping;

					if ( keyMapping != null )
					{
						switch( keyMapping.ActionCode )
						{
							#region OpenFileThroughDialog
							case RichTextEditorAction.OpenFileThroughDialog:
							{
								this.LoadFileThroughDialog();
							}
							break;
							#endregion OpenFileThroughDialog

							#region SaveFileThroughDialog
							case RichTextEditorAction.SaveFileThroughDialog:
							{
								this.SaveFileThroughDialog();
							}
							break;
							#endregion SaveFileThroughDialog

						}

						return true;
					}
				}

			}

			return false;
		}
			#endregion PerformKeyAction

			#endregion Helper routines

        #endregion Methods

	}
	#endregion RichTextEditor class

	#region RichTextEditorEmbeddableUIElement class
																	public class RichTextEditorEmbeddableUIElement : EmbeddableUIElementBase
	{
		#region Constructor

																										public RichTextEditorEmbeddableUIElement( UIElement parentElement,
										EmbeddableEditorOwnerBase owner,
										EmbeddableEditorBase editor,
										object ownerContext,
										bool includeEditElements,
										bool reserveSpaceForEditElements,
										bool drawOuterBorders, 
										bool isToolTip ) : base( parentElement, owner, editor, ownerContext, includeEditElements, reserveSpaceForEditElements, drawOuterBorders, isToolTip )
		{
		}

		#endregion Constructor

		#region Overrides

			#region OnMouseDown
														protected override bool OnMouseDown( MouseEventArgs e, bool adjustableArea, ref UIElement captureMouseForElement )
		{
			
						EmbeddableMouseDownEventArgs eventArgs = new EmbeddableMouseDownEventArgs(	this,
																						false,
																						e,
																						false,
																						false );
						
						bool disabled = this.Owner != null && ! this.Owner.IsEnabled( this.OwnerContext );
			bool designMode = this.Owner != null && this.Owner.DesignMode;
			if ( disabled || designMode )
				return base.OnMouseDown( e, adjustableArea, ref captureMouseForElement );

						this.RaiseMouseDownEvent( eventArgs );
			bool retVal = eventArgs.EatMessage;
			
						if ( e.Button == MouseButtons.Left )
			{
				RichTextEditor richTextEditor = this.Editor as RichTextEditor;

												if ( ! this.IsInEditMode && this.Owner.EnterEditModeOnClick( this.OwnerContext ) )
				{
					richTextEditor.EnterEditMode( this );

										if ( this.IsInEditMode &&
						 ! richTextEditor.RichTextBoxEditor.Focused )
					{
						Control owningControl = this.Owner.GetControl( this.OwnerContext );
						if ( owningControl != null )
						{
														if ( ! owningControl.Focus() )
								richTextEditor.ExitEditMode( true, true );
						}
					}
				}

																																				if ( ! eventArgs.EmbeddableElement.IsInEditMode )
					return eventArgs.EatMessage;

												return	eventArgs.EatMessage;
			}

									return retVal;

		}

														internal bool InternalOnMouseDown( MouseEventArgs e, bool adjustableArea, ref UIElement captureMouseForElement )
		{
			return this.OnMouseDown( e, adjustableArea, ref captureMouseForElement );
		}
			#endregion OnMouseDown
		
			#region PositionChildElements
								protected override void PositionChildElements()
		{
			UIElementsCollection oldElements = this.childElementsCollection;
			this.childElementsCollection = null;
			Rectangle workRect = this.RectInsideBorders;

			#region Create/extract the RichTextEditAreaUIElement

			RichTextEditAreaUIElement editAreaElement = RichTextEditorEmbeddableUIElement.ExtractExistingElement( oldElements, typeof(RichTextEditAreaUIElement), true ) as RichTextEditAreaUIElement;
			if ( editAreaElement != null )
				editAreaElement.Initialize( this );
			else
				editAreaElement = new RichTextEditAreaUIElement( this );

			#endregion Create/extract the RichTextEditAreaUIElement

			#region Image positioning

																					AppearanceData appearance = new AppearanceData();
			AppearancePropFlags requestedProps =	AppearancePropFlags.Image		|
													AppearancePropFlags.ImageHAlign |
													AppearancePropFlags.ImageVAlign;

			this.Owner.ResolveAppearance( this.OwnerContext, ref appearance, ref requestedProps );

									ImageUIElement imageElement = null;
			if ( appearance.Image != null )
			{
								ImageList imageList = this.Owner.GetImageList( this.OwnerContext );

												Image image = appearance.GetImage( imageList );

												Size imageSize = Size.Empty;
				if ( ! this.Owner.GetSizeOfImages(this.OwnerContext, out imageSize) )
					imageSize = new Size(16, 16);
				
				Rectangle imageRect = workRect;
				imageRect.Width = imageSize.Width;
				imageRect.Height = imageSize.Height;
				
				bool adjustHeight = appearance.ImageHAlign == HAlign.Center;
				bool adjustWidth = appearance.ImageVAlign == VAlign.Middle;

								switch( appearance.ImageHAlign )
				{
					case HAlign.Default:
					case HAlign.Left:
					{
						workRect.X += imageSize.Width;
						workRect.Width -= imageSize.Width;
					}
					break;

					case HAlign.Right:
					{
						workRect.Width -= imageSize.Width;
						imageRect.X = workRect.Right;
					}
					break;

					case HAlign.Center:
					{
						imageRect.X += (workRect.Width / 2);
						imageRect.X -= (imageRect.Width / 2);
					}
					break;
				}

								switch( appearance.ImageVAlign )
				{
					case VAlign.Default:
					case VAlign.Top:
					{
						if ( adjustHeight )
						{
							workRect.Y += imageSize.Height;
							workRect.Height -= imageSize.Height;
						}
					}
					break;

					case VAlign.Bottom:
					{
						imageRect.Y = workRect.Bottom - imageSize.Height;

						if ( adjustHeight )
							workRect.Height -= imageSize.Height;						
					}
					break;

					case VAlign.Middle:
					{
						imageRect.Y += (workRect.Height / 2);
						imageRect.Y -= (imageRect.Height / 2);
					}
					break;
				}

								imageElement = new ImageUIElement( this, image );
				imageElement.Rect = imageRect;
			}

			#endregion Image positioning

									editAreaElement.Rect = workRect;
			this.ChildElements.Add( editAreaElement );

									if ( imageElement != null )
				this.ChildElements.Add( imageElement );


		}
			#endregion PositionChildElements

		#endregion Overrides

		#region Internal properties

			#region RichTextEditor
								internal RichTextEditor RichTextEditor
		{
			get{ return this.Editor as RichTextEditor; }
		}
			#endregion RichTextEditor

			#region EditArea
								internal Rectangle EditArea
		{
			get
			{
				RichTextEditAreaUIElement editAreaElement = this.GetDescendant( typeof(RichTextEditAreaUIElement) ) as RichTextEditAreaUIElement;
				if ( editAreaElement == null )
					return Rectangle.Empty;

				return editAreaElement.Rect;

			}
		}
			#endregion EditArea

		#endregion Internal properties

	}
	#endregion RichTextEditorEmbeddableUIElement class

	#region RichTextEditAreaUIElement class
					public class RichTextEditAreaUIElement : UIElement
	{
		#region Member variables
		RichTextEditorEmbeddableUIElement richTextEditorEmbeddableUIElement = null;
		#endregion Member variables

		#region Constructor
										public RichTextEditAreaUIElement( RichTextEditorEmbeddableUIElement richTextEditorEmbeddableUIElement )
            : base(richTextEditorEmbeddableUIElement)
		{
			this.Initialize( richTextEditorEmbeddableUIElement );
		}

										public void Initialize( RichTextEditorEmbeddableUIElement richTextEditorEmbeddableUIElement )
		{
			this.richTextEditorEmbeddableUIElement = richTextEditorEmbeddableUIElement;
		}
		#endregion Constructor

        public int RtfContentHeight { get; set; }

        #region Overrides

        #region ClipSelf

        protected override bool ClipSelf
            {
                get
                {
                    return true;
                }
            }
            #endregion 
            #region OnMouseDown
            		    		    		    		    		    		    protected override bool OnMouseDown( MouseEventArgs e, bool adjustableArea, ref UIElement captureMouseForElement )
		    {
			    return this.richTextEditorEmbeddableUIElement.InternalOnMouseDown( e, adjustableArea, ref captureMouseForElement );
		    }
			    #endregion OnMouseDown

			#region DrawForeground
														protected override void DrawForeground(ref UIElementDrawParams drawParams)
		{
			RichTextEditorEmbeddableUIElement embeddableElement = this.richTextEditorEmbeddableUIElement;

						if ( embeddableElement.IsInEditMode )
				return;

			Rectangle rectInsideBorders = this.RectInsideBorders;
			Rectangle clipRect = this.ClipRect;

                                    EmbeddableRichTextBox rtfRenderer = new EmbeddableRichTextBox();
            rtfRenderer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            
			EmbeddableRichTextBox rtfEditor = embeddableElement.RichTextEditor.RichTextBoxEditor;

									rtfRenderer.Size = new Size( rectInsideBorders.Width, rectInsideBorders.Height );
			rtfRenderer.Font = rtfEditor.Font;

			            AppearancePropFlags requestedProps = AppearancePropFlags.BackColor;
            AppearanceData appearance = new AppearanceData();
            bool resolved = embeddableElement.Owner.ResolveAppearance(embeddableElement.OwnerContext,
                                                                                    ref appearance,
                                                                                    ref requestedProps);
            rtfRenderer.BackColor = appearance.BackColor;
            
            rtfRenderer.ForeColor = rtfEditor.ForeColor;

												            
            this.RtfContentHeight = -1; 
			object ownerValue = embeddableElement.Owner.GetValue( embeddableElement.OwnerContext );
			if ( ownerValue != null && ownerValue != DBNull.Value )
			{
				if ( rtfRenderer.IsValidRTF(ownerValue as string) == true )
					rtfRenderer.Rtf = ownerValue as string;
				else
					rtfRenderer.Text = ownerValue as string;
			}
			else
			{
				if ( embeddableElement.Owner.IsNullable(embeddableElement.OwnerContext) )
				{
					string nullText = string.Empty;
					if ( embeddableElement.Owner.GetNullText(embeddableElement.OwnerContext, out nullText) )
						rtfRenderer.Text = nullText;
					else
						rtfRenderer.Text = string.Empty;
				}
            }
            this.RtfContentHeight = rtfRenderer.renderedRTFContentHeight; 
			                        if (System.Environment.Version.Major == 1)
            {
                rtfRenderer.InternalInvokePaintBackground(drawParams.Graphics, rectInsideBorders);
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(rtfRenderer.BackColor))
                {
                    drawParams.Graphics.FillRectangle(brush, rectInsideBorders);
                }
            }

									RichTextBoxRenderer renderer = new RichTextBoxRenderer( rtfRenderer );
			Rectangle rect = rectInsideBorders;

									rect.X += 1;
			rect.Width -= 1;

                                    Point[] points = new Point[] { rect.Location };
            drawParams.Graphics.TransformPoints(System.Drawing.Drawing2D.CoordinateSpace.Page, System.Drawing.Drawing2D.CoordinateSpace.World, points);
            rect.Location = points[0];

                                    UIElement parent = drawParams.Element;
            while (parent != null)
            {
                RectangleF rectF = parent.Region.GetBounds(drawParams.Graphics);
                clipRect.Intersect(Rectangle.Ceiling(rectF));
                parent = parent.Parent;
            }

						            renderer.Render(drawParams.Graphics, rect, clipRect);

                        try
            {
                if (!embeddableElement.RichTextEditor.OverflowSkip)
                {
                    bool bAddOverflowImage = false;
                    if (embeddableElement.OwnerContext.GetType() == typeof(Infragistics.Win.UltraWinGrid.CellUIElement))
                    {
                                                                                                                        
                                                if ( (this.RtfContentHeight > 0) && (this.RtfContentHeight > clipRect.Height) )
                            bAddOverflowImage = true;
                    }

                    if (bAddOverflowImage)
                    {
                        if (embeddableElement.RichTextEditor.OverflowImage != null)
                        {
                            Point orig = clipRect.Location;
                            Point loc = new Point(
                                                    orig.X + clipRect.Width - embeddableElement.RichTextEditor.OverflowImage.Width - 2,
                                                    orig.Y + clipRect.Height - embeddableElement.RichTextEditor.OverflowImage.Height - 2
                                                   );
                            drawParams.Graphics.DrawImage(embeddableElement.RichTextEditor.OverflowImage, loc);
                        }
                    }
                    else
                    {
                        if (embeddableElement.RichTextEditor.OverflowImageNoOverflow != null)
                        {
                            Point orig = clipRect.Location;
                            Point loc = new Point(
                                                    orig.X + clipRect.Width - embeddableElement.RichTextEditor.OverflowImageNoOverflow.Width - 2,
                                                    orig.Y + clipRect.Height - embeddableElement.RichTextEditor.OverflowImageNoOverflow.Height - 2
                                                   );
                            drawParams.Graphics.DrawImage(embeddableElement.RichTextEditor.OverflowImageNoOverflow, loc);
                        }
                    }
                }
            }
            catch
            {
            }

            try
            {
                                if (rtfRenderer != null && !rtfRenderer.IsDisposed && !rtfRenderer.Disposing)
                {
                    rtfRenderer.Dispose();
                    rtfRenderer = null;
                }
            }
            catch
            {
            }
            		}
			#endregion	DrawForeground

			#region DrawBackColor
												protected override void DrawBackColor(ref UIElementDrawParams drawParams)
		{
		}
			#endregion DrawForeground

		#endregion Overrides
	}
	#endregion RichTextEditAreaUIElement class

}
