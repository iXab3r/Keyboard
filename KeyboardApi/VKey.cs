using System;

namespace KeyboardApi
{
	/// <summary>An individual keyboard VKey.</summary>
	[Serializable]
	public class VKey 
	{
		/// <summary>The shift VKey's virtual VKey code.</summary>
		public Messaging.VKeys ShiftKey { get; set; }

		/// <summary>The shift type (alt, ctrl, shift).</summary>
		public Messaging.ShiftType ShiftType { get; set; }

		/// <summary>The virtual VKey associated with it.</summary>
		public Messaging.VKeys Vk { get; set; }

		/// <summary>
		///     An internal counter used to count the number of attempts a button has tried to be pressed to exit after 4
		///     attempts.
		/// </summary>
		private int buttonCounter;

		/// <summary>Default constructor</summary>
		public VKey(Messaging.VKeys vk = Messaging.VKeys.NULL, Messaging.VKeys shiftKey = Messaging.VKeys.NULL, Messaging.ShiftType shiftType = Messaging.ShiftType.NONE)
		{
			buttonCounter = 0;
			Vk = vk;
			ShiftKey = shiftKey;
			ShiftType = shiftType;
		}

		public VKey(char c)
		{
			buttonCounter = 0;
			Vk = (Messaging.VKeys)Messaging.GetVirtualKeyCode(c);
			ShiftKey = Messaging.VKeys.NULL;
			ShiftType = Messaging.ShiftType.NONE;
		}

		/// <summary>Constructor if you already have a whole VKey.  Good for making a dereferenced copy.</summary>
		/// <param name="vKey">The already built VKey.</param>
		public VKey(VKey vKey)
		{
			buttonCounter = 0;
			Vk = vKey.Vk;
			ShiftKey = vKey.ShiftKey;
			ShiftType = vKey.ShiftType;
		}

		/// <summary>Emulates a keyboard VKey press.</summary>
		/// <param name="hWnd">The handle to the window that will receive the VKey press.</param>
		/// <param name="foreground">Whether it should be a foreground VKey press or a background VKey press.</param>
		/// <returns>If the press succeeded or failed.</returns>
		public bool Press(IntPtr hWnd, bool foreground)
		{
			if (foreground)
				return PressForeground(hWnd);

			return PressBackground(hWnd);
		}

		public bool Down(IntPtr hWnd, bool foreground)
		{
			switch (ShiftType)
			{
				case Messaging.ShiftType.NONE:
					if (foreground)
					{
						if (!Messaging.ForegroundKeyDown(hWnd, this))
						{
							buttonCounter++;
							if (buttonCounter == 2)
							{
								buttonCounter = 0;
								return false;
							}
							Down(hWnd, true);
						}
					}
					else
					{
						if (!Messaging.SendMessageDown(hWnd, this, true))
						{
							buttonCounter++;
							if (buttonCounter == 2)
							{
								buttonCounter = 0;
								return false;
							}
							Down(hWnd, false);
						}
					}
					return true;
			}
			return true;
		}

		public bool Up(IntPtr hWnd, bool foreground)
		{
			switch (ShiftType)
			{
				case Messaging.ShiftType.NONE:
					if (foreground)
					{
						if (!Messaging.ForegroundKeyUp(hWnd, this))
						{
							buttonCounter++;
							if (buttonCounter == 2)
							{
								buttonCounter = 0;
								return false;
							}
							Up(hWnd, foreground);
						}
					}
					else
					{
						if (!Messaging.SendMessageUp(hWnd, this, true))
						{
							buttonCounter++;
							if (buttonCounter == 2)
							{
								buttonCounter = 0;
								return false;
							}
							Up(hWnd, foreground);
						}
					}
					return true;
			}
			return true;
		}

		public bool PressForeground()
		{
			switch (ShiftType)
			{
				case Messaging.ShiftType.NONE:
					if (!Messaging.ForegroundKeyPress(this))
					{
						buttonCounter++;
						if (buttonCounter == 2)
						{
							buttonCounter = 0;
							return false;
						}
						PressForeground();
					}
					return true;
			}
			return true;
		}

		/// <summary>Emulates a background keyboard VKey press.</summary>
		/// <param name="hWnd">The handle to the window that will receive the VKey press.</param>
		/// <returns>If the VKey press succeeded or failed.</returns>
		public bool PressBackground(IntPtr hWnd)
		{
			bool alt = false, ctrl = false, shift = false;
			switch (ShiftType)
			{
				case Messaging.ShiftType.ALT:
					alt = true;
					break;
				case Messaging.ShiftType.CTRL:
					ctrl = true;
					break;
				case Messaging.ShiftType.NONE:
					if (!Messaging.SendMessage(hWnd, this, true))
					{
						buttonCounter++;
						if (buttonCounter == 2)
						{
							buttonCounter = 0;
							return false;
						}
						PressBackground(hWnd);
					}
					return true;
				case Messaging.ShiftType.SHIFT:
					shift = true;
					break;
			}
			if (!Messaging.SendMessageAll(hWnd, this, alt, ctrl, shift))
			{
				buttonCounter++;
				if (buttonCounter == 2)
				{
					buttonCounter = 0;
					return false;
				}
				PressBackground(hWnd);
			}
			return true;
		}

		/// <summary>Emulates a foreground VKey press.</summary>
		/// <param name="hWnd">The handle to the window that will receive the VKey press.</param>
		/// <returns>Returns whether the VKey succeeded to be pressed or not.</returns>
		public bool PressForeground(IntPtr hWnd)
		{
			bool alt = false, ctrl = false, shift = false;
			switch (ShiftType)
			{
				case Messaging.ShiftType.ALT:
					alt = true;
					break;
				case Messaging.ShiftType.CTRL:
					ctrl = true;
					break;
				case Messaging.ShiftType.NONE:
					if (!Messaging.ForegroundKeyPress(hWnd, this))
					{
						buttonCounter++;
						if (buttonCounter == 2)
						{
							buttonCounter = 0;
							return false;
						}
						PressForeground(hWnd);
					}
					return true;
				case Messaging.ShiftType.SHIFT:
					shift = true;
					break;
			}
			if (!Messaging.ForegroundKeyPressAll(hWnd, this, alt, ctrl, shift))
			{
				buttonCounter++;
				if (buttonCounter == 2)
				{
					buttonCounter = 0;
					return false;
				}
				PressForeground(hWnd);
			}
			return true;
		}
		/// <summary>Override to return the VKey's string</summary>
		/// <returns>Returns the proper string.</returns>
		public override string ToString()
		{
			return string.Format("{0} {1}", ShiftType, Vk);
		}
	}
}