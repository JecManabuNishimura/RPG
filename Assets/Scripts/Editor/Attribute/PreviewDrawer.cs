// PreviewDrawer.cs
using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PreviewAttribute))]
public class PreviewDrawer : PropertyDrawer
{
	private static readonly float Margin = EditorGUIUtility.standardVerticalSpacing;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// 通常のプロパティを描画
		position.height = EditorGUI.GetPropertyHeight(property, label);
		EditorGUI.PropertyField(position, property, label);

		// PreviewAttributeの取得
		var previewAttribute = (PreviewAttribute)attribute;

		// プレビュー用テクスチャの取得
		var texture = AssetPreview.GetAssetPreview(property.objectReferenceValue);
		if (texture == null) return;

		var width = previewAttribute.Height * texture.width / texture.height;

		// 右寄せにして画像を表示

		// テクスチャを表示する位置を計算
		var imageRect = new Rect
		{
			x = position.xMax - width - Margin,
			y = position.y + position.height + Margin,
			width = width,
			height = previewAttribute.Height
		};

		// 画像を表示
		GUI.DrawTexture(imageRect, texture);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		var previewAttribute = (PreviewAttribute)attribute;
		return previewAttribute.Height + EditorGUI.GetPropertyHeight(property, label) + Margin * 2;
	}
}