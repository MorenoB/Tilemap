﻿using UnityEngine;
using UnityEditor;
using toinfiniityandbeyond.UI;

namespace toinfiniityandbeyond.Tilemapping {
	[CustomEditor (typeof (AutoTile))]
	public class AutoTileEditor : ScriptableTileEditor
	{
		private AutoTile autoTile;
		private bool emptySpriteWarning;

		private int [,] indexLookup = new int [,]
		{
			{ 4, 6, 14 ,12	},
			{ 5 ,7 ,15, 13	},
			{ 1 ,3 ,11, 9	},
			{ 0 ,2 ,10, 8	},
		};
		private void OnEnable()
		{
			autoTile = (AutoTile)target;
			if (autoTile.bitmaskSprites.Length != 16)
				autoTile.bitmaskSprites = new Sprite [16];
		}
		public override void OnInspectorGUI ()
		{
			EditorGUILayout.HelpBox ("Fill the grid below with tiles in the '4x4' layout. Click the help button for more info.", MessageType.Info);
			GUILayout.Label ("Sprites:", CustomStyles.leftBoldLabel);

			int size = 4;
			float sizePerButton = EditorGUIUtility.currentViewWidth / size - 15;
			emptySpriteWarning = false;
			GUILayout.BeginHorizontal ();
			for (int x = 0; x < size; x++)
			{
				GUILayout.BeginVertical ();
				for (int y = 0; y < size; y++)
				{
					GUILayout.BeginHorizontal ();

					int index = indexLookup [y, x];

					string labelText = index.ToString();

					if (!autoTile.bitmaskSprites [index])
					{
						if (index == 15) {
							GUI.color = new Color (1, 0.5f, 0.5f);
							labelText = "Tile not valid!";
						}
						else {
							GUI.color = new Color (1, 0.9f, 0.5f);
							labelText = "Empty";
						}
					} else if (!autoTile.IsElementValid (index))
					{
						GUI.color = new Color (1, 0.5f, 0.5f);
						labelText = "Enable read/write";
					}
					if (GUILayout.Button (GUIContent.none, CustomStyles.centerWhiteBoldLabel, GUILayout.Width (sizePerButton), GUILayout.Height (sizePerButton)))
					{
						EditorGUIUtility.ShowObjectPicker<Sprite> (autoTile.bitmaskSprites [index], false, "", index);
					}
					Rect r = GUILayoutUtility.GetLastRect ();

					Texture2D texture = autoTile.IsElementValid (index) ? TextureFromSprite (autoTile.bitmaskSprites [index]) : new Texture2D(16, 16);
					GUI.DrawTexture (r, texture);
					GUI.color = Color.white;

					GUIStyle labelStyle = new GUIStyle(CustomStyles.centerWhiteBoldLabel);

					GUI.Label (r, labelText, labelStyle);
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndVertical ();
			}
			GUILayout.EndHorizontal ();

			GUILayout.Label ("Settings:", CustomStyles.leftBoldLabel);
			autoTile.defaultIsFull = EditorGUILayout.Toggle ("Default is full", autoTile.defaultIsFull);

			int controlID = EditorGUIUtility.GetObjectPickerControlID ();
			if (Event.current.commandName == "ObjectSelectorUpdated")
			{
				autoTile.bitmaskSprites [controlID] = EditorGUIUtility.GetObjectPickerObject () as Sprite;
			}
			if(GUI.changed)
			{
				EditorUtility.SetDirty (this);
			}
		}
		private Texture2D TextureFromSprite(Sprite sprite)
		{
			var texture = new Texture2D ((int)sprite.rect.width, (int)sprite.rect.height, sprite.texture.format, false);
			var pixels = sprite.texture.GetPixels ((int)sprite.textureRect.x,
													(int)sprite.textureRect.y,
													(int)sprite.textureRect.width,
													(int)sprite.textureRect.height);
			texture.SetPixels (pixels);
			texture.filterMode = sprite.texture.filterMode;
			texture.wrapMode = sprite.texture.wrapMode;
			texture.Apply ();
			return texture;
		}
	}
}