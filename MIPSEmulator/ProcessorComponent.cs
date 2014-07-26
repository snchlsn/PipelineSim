#region Using Directives
using System;
#endregion

namespace MIPSEmulator
{
	public partial class Processor
	{
		#region XML Header
		/// <summary>
		/// Base class for a hardware component of a <c>MIPSEmulator.Processor</c>.
		/// </summary>
		#endregion
		public abstract class ProcessorComponent
		{
			#region Fields
			#region XML Header
			/// <summary>
			/// The <c>MIPSEmulator.Processor</c> whereof the <c>ProcessorComponent</c> is a component.
			/// </summary>
			#endregion
			public readonly Processor Owner;
			#endregion
			
			#region Constructors
			#region XML Header
			/// <summary>
			/// Creates and initializes a new <c>ProcessorComponent</c> instance belonging to the specified
			/// <c>Processor</c>.
			/// </summary>
			/// <param name="owner">
			/// The <c>MIPSEmulator.Processor</c> whereof the new <c>ProcessorComponent</c> is a component.
			/// </param>
			#endregion
			internal ProcessorComponent(Processor owner)
			{
				Owner = owner;
			}
			#endregion
		}
	}
}
