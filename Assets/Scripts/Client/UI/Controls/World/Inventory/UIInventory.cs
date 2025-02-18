﻿using System.Collections.Generic;
using UnityEngine;

namespace Client
{
	public class UIInventory : UIControl
	{
		public RectTransform content;
		public UIInventoryButton buttonPrefab;
		public List<UIInventoryButton> inventorySlots = null;

		public override void OnStarting()
		{
		}

		public override void OnDestroying()
		{
		}

		void Update()
		{
			Character character = Character.localCharacter;
			if (character != null)
			{
				if (inventorySlots == null || inventorySlots.Count != character.InventoryController.items.Count)
				{
					AddSlots(character.InventoryController);
				}
			}
		}

		private void AddSlots(InventoryController inventory)
		{
			if (content == null || buttonPrefab == null || inventory == null)
			{
				return;
			}

			inventory.OnSlotUpdated -= OnInventorySlotUpdated;
			if (inventorySlots != null)
			{
				for (int i = 0; i < inventorySlots.Count; ++i)
				{
					Destroy(inventorySlots[i].gameObject);
				}
				inventorySlots.Clear();
			}
			inventorySlots = new List<UIInventoryButton>();

			for (int i = 0; i < inventory.items.Count; ++i)
			{
				UIInventoryButton button = Instantiate(buttonPrefab, content);
				button.index = i;
				button.referenceID = i.ToString(); // helper for UIDragObject
				button.allowedHotkeyType = HotkeyType.Inventory;
				button.hotkeyType = HotkeyType.Inventory;
				button.icon.texture = inventory.items[i] != null && inventory.items[i].stackSize > 0 ? inventory.items[i].Template.Icon : null;
				inventorySlots.Add(button); // track inventory slots for easy updating
			}
			// update our buttons when the inventory slots change
			inventory.OnSlotUpdated += OnInventorySlotUpdated;
		}

		public void OnInventorySlotUpdated(Item item, int inventoryIndex)
		{
			if (item != null && item.stackSize > 0 && inventoryIndex > -1 && inventoryIndex < inventorySlots.Count)
			{
				// update our button display
				UIInventoryButton button = inventorySlots[inventoryIndex];
				button.hotkeyType = HotkeyType.Inventory;
				if (button.icon != null) button.icon.texture = item.Template.Icon;
				//inventorySlots[i].cooldownText = character.CooldownController.IsOnCooldown();
				if (button.amountText != null) button.amountText.text = item.stackSize.ToString();
			}
			else
			{
				// the item no longer exists
				inventorySlots[inventoryIndex].Clear();
			}
		}
	}
}