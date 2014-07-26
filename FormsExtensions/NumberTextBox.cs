#region Using Directives
using System;
using System.Windows.Forms;
#endregion

namespace FormsExtensions
{
	#region XML Header
	/// <summary>
	/// A <c>System.Windows.Forms.TextBox</c> that only accepts text that represents a numerical value.
	/// </summary>
	#endregion
	public class NumberTextBox: TextBox
	{
		#region Field
		private bool allowFloat, allowNegative, useBoundsToolTips;
		private dynamic lowerBound, upperBound;
		#endregion
		
		#region Properties
		#region XML Header
		/// <summary>
		/// Gets or sets a value indicating whether the current <c>NumberTextBox</c> allows floating-point numbers to
		/// be entered.
		/// </summary>
		/// <returns>
		/// <c>true</c> if floating-point numbers are allowed; <c>false</c> if not.  The default is <c>true</c>.
		/// </returns>
		/// <value>The <c>allowFloat</c> field.</value>
		#endregion
		public bool AllowFloat
		{
			get
			{
				return allowFloat;
			}
			set
			{
				allowFloat = value;
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets a value indicating whether the current <c>NumberTextBox</c> allows negative numbers to be
		/// entered.
		/// </summary>
		/// <returns>
		/// <c>true</c> if negative numbers are allowed; <c>false</c> if not.  The default is <c>true</c>.
		/// </returns>
		/// <value>The <c>allowNegative</c> field.</value>
		#endregion
		public bool AllowNegative
		{
			get
			{
				return allowNegative;
			}
			set
			{
				allowNegative = value;
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the number entered in the current <c>NumberTextBox</c>, formatted as a double-precision
		/// floating-point number.
		/// </summary>
		/// <returns>A <c>double</c> representing the number entered.</returns>
		/// <exception cref="System.OverflowException">
		/// The number entered is less than <c>System.Double.MinValue</c> or greater than
		/// <c>System.Double.MaxValue</c>.
		/// </exception>
		#endregion
		public double Double
		{
			get
			{
				if (Text.Equals(string.Empty))
					return 0;
				
				return Convert.ToDouble(Text);
			}
			set
			{
				StopFormatEnforcement();
				Text = value.ToString();
				StartFormatEnforcement();
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the number entered in the current <c>NumberTextBox</c>, formatted as a single-precision
		/// floating-point number.
		/// </summary>
		/// <returns>A <c>float</c> representing the number entered.</returns>
		/// <exception cref="System.OverflowException">
		/// The number entered is less than <c>System.Single.MinValue</c> or greater than
		/// <c>System.Single.MaxValue</c>.
		/// </exception>
		#endregion
		public float Float
		{
			get
			{
				if (Text.Equals(string.Empty))
					return 0;
				
				return Convert.ToSingle(Text);
			}
			set
			{
				StopFormatEnforcement();
				Text = value.ToString();
				StartFormatEnforcement();
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the number entered in the current <c>NumberTextBox</c>, formatted as a 32-bit integer.
		/// </summary>
		/// <returns>An <c>int</c> representing the number entered.</returns>
		/// <exception cref="System.OverflowException">
		/// The number entered is less than <c>System.Int32.MinValue</c> or greater than
		/// <c>System.Int32.MaxValue</c>.
		/// </exception>
		#endregion
		public int Int
		{
			get
			{
				if (Text.Equals(string.Empty))
					return 0;
				
				return Convert.ToInt32(Text);
			}
			set
			{
				StopFormatEnforcement();
				Text = value.ToString();
				StartFormatEnforcement();
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the lowest value that may be entered in the current <c>NumberTextBox</c>.  The default is
		/// <c>null</c>, which enforces no limit.
		/// </summary>
		/// <returns>A numerical primitive -or- <c>null</c> if no limit is enforced.</returns>
		/// <value>The <c>lowerBound</c> field.</value>
		/// <exception cref="System.ArgumentException">
		/// The assigned value is not one of the following: <c>null</c>, <c>int</c>, <c>uint</c>, <c>short</c>,
		/// <c>ushort</c>, <c>long</c>, <c>ulong</c>, <c>float</c>, or <c>double</c>.
		/// </exception>
		#endregion
		public dynamic LowerBound
		{
			get
			{
				return lowerBound;
			}
			set
			{
				if (!(value == null || value is int || value is uint || value is short || value is ushort || value is long || value is ulong || value is float || value is double))
					throw new ArgumentException("value must be a numerical primitive or null.", "value");
				
				if (value == null)
				{
					if (lowerBound != null && UpperBound == null)
						Leave -= EnforceBounds;
				}
				else if (lowerBound == null && UpperBound == null)
					Leave += EnforceBounds;
				
				lowerBound = value;
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets the greatest value that may be entered in the current <c>NumberTextBox</c>.  The default is
		/// <c>null</c>, which enforces no limit.
		/// </summary>
		/// <returns>A numerical primitive -or- <c>null</c> if no limit is enforced.</returns>
		/// <value>The <c>upperBound</c> field.</value>
		/// <exception cref="System.ArgumentException">
		/// The assigned value is not one of the following: <c>null</c>, <c>int</c>, <c>uint</c>, <c>short</c>,
		/// <c>ushort</c>, <c>long</c>, <c>ulong</c>, <c>float</c>, or <c>double</c>.
		/// </exception>
		#endregion
		public dynamic UpperBound
		{
			get
			{
				return upperBound;
			}
			set
			{
				if (!(value == null || value is int || value is uint || value is short || value is ushort || value is long || value is ulong || value is float || value is double))
					throw new ArgumentException("value must be a numerical primitive or null.", "value");
				
				if (value == null)
				{
					if (upperBound != null && LowerBound == null)
						Leave -= EnforceBounds;
				}
				else if (upperBound == null && LowerBound == null)
					Leave += EnforceBounds;
				
				upperBound = value;
			}
		}
		
		#region XML Header
		/// <summary>
		/// Gets or sets a value indicating whether violations of lower and upper bounds should be handled
		/// automatically by displaying a tooltip to the user, informing them of the limits.  The default is
		/// <c>true</c>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if violations are automatically handled with tooltips; <c>false</c> if not.
		/// </returns>
		/// <value>The <c>useBoundsToolTips</c> field.</value>
		#endregion
		public bool UseBoundsToolTips
		{
			get
			{
				return useBoundsToolTips;
			}
			set
			{
				useBoundsToolTips = value;
			}
		}
		#endregion
		
		#region Events
		#region XML Header
		/// <summary>
		/// Occurs when a value lower than the <c>LowerBound</c> property is entered.
		/// </summary>
		#endregion
		public event EventHandler LowerBoundViolated;
		
		#region XML Header
		/// <summary>
		/// Occurs when a value greater than the <c>UpperBound</c> property is entered.
		/// </summary>
		#endregion
		public event EventHandler UpperBoundViolated;
		#endregion
		
		#region Methods
		#region Event Raisers
		#region XML Header
		/// <summary>
		/// Raises the <c>LowerBoundViolated</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnLowerBoundViolated(EventArgs e)
		{
			if (LowerBoundViolated != null)
				LowerBoundViolated(this, e);
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Raises the <c>UpperBoundViolated</c> event.
		/// </summary>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void OnUpperBoundViolated(EventArgs e)
		{
			if (UpperBoundViolated != null)
				UpperBoundViolated(this, e);
			return;
		}
		#endregion
		
		#region Event Handlers
		#region XML Header
		/// <summary>
		/// Ensures that the <c>LowerBound</c> and <c>UpperBound</c> properties, if set, are not violated.
		/// </summary>
		/// <param name="sender">The <c>object</c> that rasied the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void EnforceBounds(object sender, EventArgs e)
		{
			#region Lower Bound
			if (LowerBound != null && Double < LowerBound)
			{
				StopFormatEnforcement();
				Text = LowerBound.ToString();
				StartFormatEnforcement();
				
				if (UseBoundsToolTips)
				{
					ToolTip lowerBoundToolTip = new ToolTip();
					lowerBoundToolTip.IsBalloon = true;
					lowerBoundToolTip.Show("The minimum value is " + LowerBound.ToString() + ".", this, 5, -35, 2000);
				}
				
				OnLowerBoundViolated(EventArgs.Empty);
			}
			#endregion
			
			#region Upper Bound
			else if (UpperBound != null && Double > UpperBound)
			{
				StopFormatEnforcement();
				Text = UpperBound.ToString();
				StartFormatEnforcement();
				
				if (UseBoundsToolTips)
				{
					ToolTip upperBoundToolTip = new ToolTip();
					upperBoundToolTip.IsBalloon = true;
					upperBoundToolTip.Show("The maximum value is " + UpperBound.ToString() + ".", this, 5, -35, 2000);
				}
				
				OnUpperBoundViolated(EventArgs.Empty);
			}
			#endregion
			
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Ensures that <c>Text</c> represents a valid number.
		/// </summary>
		/// <param name="sender">The <c>object</c> that raised the event.</param>
		/// <param name="e">A <c>System.EventArgs</c>.</param>
		#endregion
		private void EnforceNumberFormat(object sender, EventArgs e)
		{
			bool decimalFound = false;
			int pos = SelectionStart;
			
			for (int i = 0; i < Text.Length; ++i)
				if (!char.IsDigit(Text[i]) && !(AllowFloat && !decimalFound && Text[i] == '.') && !(AllowNegative && i == 0 && Text[i] == '-'))
				{
					Text = Text.Remove(i, 1);
					if (pos == i + 1)
						pos = 1;
				}
				else if (Text[i] == '.')
					decimalFound = true;
			
			SelectionStart = (pos < Text.Length ? pos : Text.Length);
			return;
		}
		#endregion
		
		#region XML Header
		/// <summary>
		/// Enables forced adherence to valid numerical format.
		/// </summary>
		#endregion
		protected void StartFormatEnforcement()
		{
			TextChanged += EnforceNumberFormat;
			return;
		}
		
		#region XML Header
		/// <summary>
		/// Disables forced adherence to valid numerical format.
		/// </summary>
		#endregion
		protected void StopFormatEnforcement()
		{
			TextChanged -= EnforceNumberFormat;
			return;
		}
		
		#region Constructors
		#region  XML Header
		/// <summary>
		/// Creates and initializes a new <c>NumberTextBox</c> instance.
		/// </summary>
		/// <param name="allowFloat">
		/// Indicates whether the new <c>NumberTextBox</c> should allow the user to enter floating-point numbers.
		/// The default is <c>true</c>.
		/// </param>
		/// <param name="allowNegative">
		/// Indicates whether the new <c>NumberTextBox</c> should allow the user to enter negative numbers.
		/// The default is <c>true</c>.
		/// </param>
		#endregion
		public NumberTextBox(bool allowFloat = true, bool allowNegative = true, dynamic lowerBound = null, dynamic upperBound = null, bool useBoundsToolTips = true)
		{
			#region Property Assignments
			try
			{
				LowerBound = lowerBound;
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("", "lowerBound", e);
			}
			
			try
			{
				UpperBound = upperBound;
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("", "upperBound", e);
			}
			
			AllowFloat = allowFloat;
			AllowNegative = allowNegative;
			UseBoundsToolTips = useBoundsToolTips;
			Text = "0";
			#endregion
			
			StartFormatEnforcement();
		}
		#endregion
		#endregion
	}
}
