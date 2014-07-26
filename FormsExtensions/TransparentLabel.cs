#region Using Directives
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace FormsExtensions
{
	#if DEBUG
	public class TransparentLabel: Label
	{
		protected override CreateParams CreateParams
		{
		    get
		    {
		        CreateParams param = base.CreateParams;
		        
		        param.ExStyle |= 0x20;
		        return param;
			}
		}
		
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
			return;
		}
		
		protected override void WndProc(ref Message m)
		{
		    const int WM_NCHITTEST = 0x0084;
		    const int HTTRANSPARENT = (-1);
		
		    if (m.Msg == WM_NCHITTEST)
		        m.Result = (IntPtr)HTTRANSPARENT;
		    else
		        base.WndProc(ref m);
		    
		    return;
		}
		
		public TransparentLabel()
		{
			//SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			//SetStyle(ControlStyles.Opaque, true);
		}
	}
	#endif
}
