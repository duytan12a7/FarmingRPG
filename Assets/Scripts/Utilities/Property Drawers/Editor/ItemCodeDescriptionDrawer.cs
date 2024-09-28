using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    // Returns the height of the property field, doubled for description display
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    // Draws the property GUI
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            EditorGUI.BeginChangeCheck();

            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2),
                "Item Description", GetItemDescription(newValue));

            // Update the property value if it has changed
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }
        }

        EditorGUI.EndProperty();
    }

    // Retrieves the item description based on the item code
    private string GetItemDescription(int itemCode)
    {
        SO_ItemList itemList = AssetDatabase.LoadAssetAtPath<SO_ItemList>("Assets/Scriptable Objects Assets/Item/so_ItemList.asset");

        if (itemList != null)
        {
            ItemDetails itemDetail = itemList.itemDetails.Find(x => x.itemCode == itemCode);

            return itemDetail != null ? itemDetail.itemDescription : string.Empty;
        }

        return string.Empty;
    }
}
